using Google.Apis.Auth.OAuth2;

namespace KM.Orders.Services.User;

public partial class FirebaseExternalUsersService : IExternalUsersService
{
    private readonly IRepository<KmUserCustomerMap> _kmUserCustomerMapRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<CustomerRole> _customerRoleRepository;
    private readonly IRepository<CustomerCustomerRoleMapping> _customerRoleMapRepository;
    private readonly IStaticCacheManager _cache;
    private readonly IUserProfileDocumentStore _userProfileStore;

    public FirebaseExternalUsersService(
        IRepository<KmUserCustomerMap> userCustomerMapRepository,
        // IRepository<CustomerAttributeValue> customerAttributeValueRepository,
        IRepository<Customer> customerRepository,
        IRepository<CustomerRole> customerRoleRepository,
        IRepository<CustomerCustomerRoleMapping> customerRoleMapRepository,
        IUserProfileDocumentStore userProfileStore,
        IStaticCacheManager cache
        )
    {
        _kmUserCustomerMapRepository = userCustomerMapRepository;
        // _customerAttributeValueRepository = customerAttributeValueRepository;
        _customerRepository = customerRepository;
        _customerRoleRepository = customerRoleRepository;
        _customerRoleMapRepository = customerRoleMapRepository;
        _userProfileStore = userProfileStore;
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

            var map = await CreateOrUpdateCustomerAndMap(user);
            await ProvisionUserRoles(map, user, customerRoles);

            maps.Add(map);
            var ck = BuildUserCacheKey(map.KmUserId);
            await _cache.SetAsync(ck, map);
        }
        return maps;
    }

    private CacheKey BuildUserCacheKey(string uid)
    {
        var key = string.Format("external-user:kedemmarket:uid:{0}", uid);
        var ck = new CacheKey(key, "external-user:kedemmarket:uid", "external-user:kedemmarket", "external-user");
        return _cache.PrepareKeyForShortTermCache(ck);
    }

    private async Task<KmUserCustomerMap> CreateOrUpdateCustomerAndMap(UserRecord user)
    {
        Customer customer;
        var map = await _kmUserCustomerMapRepository.Table.FirstOrDefaultAsync(c => c.KmUserId == user.Uid);
        customer = await _customerRepository.GetByIdAsync(map.CustomerId);

        //map already exist and customer is not deleted- update customer and return
        if (map != null && !customer.Deleted)
        {
            if (map.ShouldProvisionBasicClaims)
            {
                setBasicClaims();
                await _customerRepository.UpdateAsync(customer, false);
            }
            map.Customer = customer;
            return map;
        }

        //map is null then either customer exist and not mapped, or customer not exists and not mapped
        //check 
        if (customer == default || customer.Deleted)
            //try to find customer by email
            customer = await _customerRepository.Table.FirstOrDefaultAsync(c => c.Email == user.Email);

        var shouldProvisionBasicClaims = customer == default || customer.Deleted;
        //customer not exist, create customer and set claims
        if (shouldProvisionBasicClaims)
        {
            customer = new Customer();
            setBasicClaims();
            await _customerRepository.InsertAsync(customer, false);
            await _customerRepository.UpdateAsync(customer, false);
        }

        return await createNewMap(customer, shouldProvisionBasicClaims);

        void setBasicClaims()
        {
            var names = user.DisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var fName = names.Length > 0 ? names[0].Trim() : user.DisplayName;
            var lName = names.Length > 1 ? user.DisplayName.Substring(names[0].Length).Trim() : user.DisplayName;

            customer.Active = !user.Disabled;
            customer.Email = user.Email;
            customer.Username = user.DisplayName;
            customer.SystemName = user.DisplayName;
            customer.Phone = user.PhoneNumber;
            customer.FirstName = fName;
            customer.LastName = lName;
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
}
