using Google.Apis.Auth.OAuth2;
using Nop.Services.Common;
using Nop.Services.Customers;

namespace KM.Orders.Services.User;

public partial class FirebaseExternalUsersService : IExternalUsersService
{
    private readonly IRepository<KmUserCustomerMap> _kmUserCustomerMapRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<CustomerRole> _customerRoleRepository;
    private readonly IRepository<CustomerCustomerRoleMapping> _customerRoleMapRepository;
    private readonly IStaticCacheManager _cache;
    private readonly IUserProfileDocumentStore _userProfileStore;
    private readonly ICustomerService _customerService;
    private readonly IAddressService _addressService;

    public FirebaseExternalUsersService(
        IRepository<KmUserCustomerMap> userCustomerMapRepository,
        // IRepository<CustomerAttributeValue> customerAttributeValueRepository,
        IRepository<Customer> customerRepository,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<CustomerCustomerRoleMapping> customerRoleMapRepository,
        IUserProfileDocumentStore userProfileStore,
        ICustomerService customerService,
        IAddressService addressService,
        IStaticCacheManager cache
        )
    {
        _kmUserCustomerMapRepository = userCustomerMapRepository;
        // _customerAttributeValueRepository = customerAttributeValueRepository;
        _customerRepository = customerRepository;
        _customerRoleRepository = customerRoleRepository;
        _customerRoleMapRepository = customerRoleMapRepository;
        _addressService = addressService;
        _userProfileStore = userProfileStore;
        _customerService = customerService;
        _cache = cache;
    }

    private FirebaseAuth GetAuth()
    {
        var app = FirebaseApp.GetInstance("user-service") ?? FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.GetApplicationDefault(),
            ProjectId = "kedem-market",
        }, "user-service");

        return FirebaseAuth.GetAuth(app);
    }

    public async Task<IEnumerable<KmUserCustomerMap>> ProvisionUsersAsync(IEnumerable<string> userIds)
    {
        userIds = userIds?.Distinct();
        userIds.ThrowIfNullOrEmpty(nameof(userIds));

        var uids = userIds.Select(uid => new UidIdentifier(uid)).ToArray();
        var getUsersResponse = await GetAuth().GetUsersAsync(uids);

        var maps = new List<KmUserCustomerMap>();

        var users = getUsersResponse.Users;
        if (users.IsNullOrEmpty())
            return maps;

        var customerRoles = new Dictionary<string, CustomerRole>(StringComparer.OrdinalIgnoreCase);
        var allRoles = _customerRoleRepository.Table.Where(c => c.Active).ToList();

        foreach (var cr in allRoles)
        {
            customerRoles[cr.SystemName] = cr;
            customerRoles[cr.Name] = cr;
        }

        foreach (var user in users)
        {
            if (!user.Uid.HasValue())
                return null;

            var map = await GetOrCreateCustomerAndMap(user);
            await ProvisionUserRoles(map, user, customerRoles);
            await ProvisionUserProfile(map);
            // await ProvisionCustomAttributes(map.Customer, user, customerAttributes);

            maps.Add(map);
            var ck = BuildUserCacheKey(map.KmUserId);
            await _cache.SetAsync(ck, map);
        }
        return maps;
    }

    private async Task ProvisionUserProfile(KmUserCustomerMap map)
    {
        var userProfile = await _userProfileStore.GetByUserId(map.KmUserId);
        if (userProfile == default)
            return;

        var ba = userProfile.billingInfo;
        var names = ba.fullName.Split(' ');
        var address = new Address
        {
            Address1 = ba.address,
            City = ba.city,
            Email = ba.email,
            FirstName = names.Length >= 0 ? names[0] : null,
            LastName = names.Length > 0 ? names[1] : null,
            PhoneNumber = ba.phoneNumber,
        };

        var req = new UpdateBillingInfoRequest(map.KmUserId, address);
        await ProvisionBillingInfosAsync(new[] { req });
    }

    private CacheKey BuildUserCacheKey(string uid)
    {
        var key = string.Format("external-user:kedemmarket:uid:{0}", uid);
        var ck = new CacheKey(key, "external-user:kedemmarket:uid", "external-user:kedemmarket", "external-user");
        return _cache.PrepareKeyForShortTermCache(ck);
    }

    private async Task<KmUserCustomerMap> GetOrCreateCustomerAndMap(UserRecord user)
    {
        Customer customer;
        var map = await _kmUserCustomerMapRepository.Table.FirstOrDefaultAsync(c => c.KmUserId == user.Uid);

        if (map != null)
        {
            customer = await _customerRepository.GetByIdAsync(map.CustomerId);
            if (map.ShouldProvisionBasicClaims)
            {
                setBasicClaims();
                await _customerRepository.UpdateAsync(customer, false);
            }
            map.Customer = customer;
            return map;
        }

        //if map is null then either customer exist and not mapped, or customer not exists and not mapped
        //try to find customer by email
        customer = await _customerRepository.Table.FirstOrDefaultAsync(c => c.Email == user.Email);
        //if customer exist and not mapped, it is managed in nop dashboard
        if (customer != default)
            return await createNewMap(customer, false);

        //last option - customer not exist, create map and always copy firebase claims
        customer = new Customer();
        setBasicClaims();

        await _customerRepository.InsertAsync(customer, false);
        return await createNewMap(customer, true);

        void setBasicClaims()
        {
            customer.Active = !user.Disabled;
            customer.Email = user.Email;
            customer.Username = user.DisplayName;
            customer.SystemName = user.DisplayName;
            customer.Phone = user.PhoneNumber;
            customer.FirstName = user.DisplayName;
            customer.LastName = user.DisplayName;
        }
        async Task<KmUserCustomerMap> createNewMap(Customer customer, bool shouldProvisionBasicClaims)
        {
            var map = new KmUserCustomerMap
            {
                KmUserId = user.Uid,
                CustomerId = customer.Id,
                Customer = customer,
                ProviderId = user.ProviderId,
                TenantId = user.TenantId,
                ShouldProvisionBasicClaims = shouldProvisionBasicClaims,
                CreatedOnUtc = DateTime.UtcNow,
            };
            await _kmUserCustomerMapRepository.InsertAsync(map, false);
            return map;
        }
    }

    private async Task ProvisionUserRoles(KmUserCustomerMap map, UserRecord user, Dictionary<string, CustomerRole> customerRoles)
    {
        var hasRoles = user.CustomClaims.TryGetValue("roles", out var value) ||
            user.CustomClaims.TryGetValue("role", out value);

        var crIds = _customerRoleMapRepository.Table
        .Where(cr => cr.CustomerId == map.CustomerId)
        .Select(c => c.CustomerRoleId);
        var currentCustomerRoles = _customerRoleRepository.Table.Where(cr => crIds.Contains(cr.Id));

        if (hasRoles &&
            value is IEnumerable<string> roles &&
            roles.Any())
        {
            foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!customerRoles.TryGetValue(role, out var cr))
                    continue;

                if (currentCustomerRoles.Any(c => c.SystemName.Equals(role, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var roleMapping = new CustomerCustomerRoleMapping
                {
                    CustomerId = map.CustomerId,
                    CustomerRoleId = cr.Id,
                };
                await _customerRoleMapRepository.InsertAsync(roleMapping, false);
            }
        }

        var guestRoleId = customerRoles[NopCustomerDefaults.GuestsRoleName].Id;
        var isGuest = currentCustomerRoles.Any(c => c.Id == guestRoleId);
        if (!isGuest)
        {
            var guestRoleMapping = new CustomerCustomerRoleMapping
            {
                CustomerId = map.CustomerId,
                CustomerRoleId = guestRoleId,
            };
            await _customerRoleMapRepository.InsertAsync(guestRoleMapping, false);
        }
    }

    public async Task<KmUserCustomerMap> GetUserIdCustomerMapByUserId(string userId)
    {
        return await _kmUserCustomerMapRepository.Table.FirstOrDefaultAsync(x => x.KmUserId == userId);
    }

    public async Task ProvisionBillingInfosAsync(IEnumerable<UpdateBillingInfoRequest> requests)
    {
        foreach (var req in requests)
        {
            var maps = from m in _kmUserCustomerMapRepository.Table
                       where m.KmUserId == req.UserId
                       select m;
            var cId = maps.FirstOrDefault()?.CustomerId;
            if (cId == default || !cId.HasValue)
                continue;

            var customer = await _customerService.GetCustomerByIdAsync(cId.Value);
            if (customer == default)
                continue;

            var cba = await _customerService.GetCustomerBillingAddressAsync(customer);
            if (cba == default)
            {
                await _addressService.InsertAddressAsync(req.BillingAddress);
                customer.BillingAddressId = req.BillingAddress.Id;
                await _customerRepository.UpdateAsync(customer);
                await _customerService.InsertCustomerAddressAsync(customer, req.BillingAddress);
            }
            else
            {
                cba.FirstName = req.BillingAddress.FirstName;
                cba.LastName = req.BillingAddress.LastName;
                cba.Email = req.BillingAddress.Email;
                cba.Company = req.BillingAddress.Company;
                cba.CountryId = req.BillingAddress.CountryId;
                cba.StateProvinceId = req.BillingAddress.StateProvinceId;
                cba.County = req.BillingAddress.County;
                cba.City = req.BillingAddress.City;
                cba.Address1 = req.BillingAddress.Address1;
                cba.Address2 = req.BillingAddress.Address2;
                cba.ZipPostalCode = req.BillingAddress.ZipPostalCode;
                cba.PhoneNumber = req.BillingAddress.PhoneNumber;
                cba.FaxNumber = req.BillingAddress.FaxNumber;
                cba.CustomAttributes = req.BillingAddress.CustomAttributes;
                cba.CreatedOnUtc = req.BillingAddress.CreatedOnUtc;

                await _addressService.UpdateAddressAsync(cba);
            }
        }
    }
}