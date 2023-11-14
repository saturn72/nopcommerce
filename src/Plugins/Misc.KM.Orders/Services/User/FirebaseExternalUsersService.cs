
namespace Nop.Plugin.Misc.KM.Orders.Services.User;

public partial class FirebaseExternalUsersService : IExternalUsersService
{
    private readonly IRepository<KmUserCustomerMap>? _userCustomerMapRepository;
    // private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;
    private readonly IRepository<Customer>? _customerRepository;
    private readonly IRepository<CustomerRole>? _customerRoleRepository;
    private readonly IRepository<CustomerCustomerRoleMapping> _customerRoleMapRepository;
    private readonly IStaticCacheManager _cache;
    // private IReadOnlyDictionary<string, CustomerAttribute> _customerAttributes;

    public FirebaseExternalUsersService(
        IRepository<KmUserCustomerMap>? userCustomerMapRepository,
        // IRepository<CustomerAttributeValue> customerAttributeValueRepository,
        IRepository<Customer>? customerRepository,
        IRepository<CustomerRole>? customerRoleRepository,
        IRepository<CustomerCustomerRoleMapping> customerRoleMapRepository,
        IStaticCacheManager cache
        )
    {
        _userCustomerMapRepository = userCustomerMapRepository;
        // _customerAttributeValueRepository = customerAttributeValueRepository;
        _customerRepository = customerRepository;
        _customerRoleRepository = customerRoleRepository;
        _customerRoleMapRepository = customerRoleMapRepository;
        _cache = cache;
    }

    private FirebaseAuth GetAuth()
    {
        if (FirebaseApp.DefaultInstance == default)
            FirebaseApp.Create();
        return FirebaseAuth.DefaultInstance;
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
            // await ProvisionCustomAttributes(map.Customer, user, customerAttributes);

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

    private async Task<KmUserCustomerMap> GetOrCreateCustomerAndMap(UserRecord user)
    {
        Customer customer;
        var map = await _userCustomerMapRepository.Table.FirstOrDefaultAsync(c => c.KmUserId == user.Uid);

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
                ShouldProvisionBasicClaims = shouldProvisionBasicClaims
            };
            await _userCustomerMapRepository.InsertAsync(map, false);
            return map;
        }
    }
    private async Task ProvisionUserRoles(KmUserCustomerMap map, UserRecord user, Dictionary<string, CustomerRole> customerRoles)
    {
        var hasRoles = user.CustomClaims.TryGetValue("roles", out var value) ||
            user.CustomClaims.TryGetValue("role", out value);

        if (hasRoles &&
            value is IEnumerable<string> roles &&
            roles.Any())
        {

            var crIds = _customerRoleMapRepository.Table
            .Where(cr => cr.CustomerId == map.CustomerId)
            .Select(c => c.CustomerRoleId);

            var currentCustomerRoles = _customerRoleRepository.Table.Where(cr => crIds.Contains(cr.Id));

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
    }

    public async Task<KmUserCustomerMap> GetUserIdCustomerMapByUserId(string userId)
    {
        return await _userCustomerMapRepository.Table.FirstOrDefaultAsync(x => x.KmUserId == userId);
    }

    // private async Task ProvisionCustomAttributes(UserRecord user, Dictionary<string, CustomerAttribute> customerAttributes)
    // {
    //     foreach (var cc in user.CustomClaims)
    //     {
    //         if (!customerAttributes.TryGetValue(cc.Key, out var cAtt))
    //             continue;

    //         var cav = new CustomerAttributeValue
    //         {
    //             CustomerAttributeId = cAtt.Id,
    //             Name = cc.Value.ToString(),
    //         };
    //         await _customerAttributeValueRepository.InsertAsync(cav, false)
    //     }
    // }
    // private async Task<IReadOnlyDictionary<string, CustomerAttribute>> ReloadAllCustomerAttributesAsync()
    // {
    //     var accs = await _customerAttributeValueRepository.GetAllCustomerAttributesAsync();

    //     var res = new Dictionary<string, CustomerAttribute>(StringComparer.OrdinalIgnoreCase);
    //     foreach (var a in accs)
    //         res[a.Name] = a;

    //     return res;
    // }
}