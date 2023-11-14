//using Nop.Core.Caching;
//using Nop.Core.Domain.Discounts;
//using Nop.Data;
//using Nop.Plugin.Misc.KM.Orders.Domains;
//using Nop.Plugin.Misc.KM.Orders.Models;
//using Nop.Services.Catalog;
//using Nop.Services.Discounts;
//using Nop.Services.Vendors;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

//namespace Nop.Plugin.Misc.KM.Orders.Factories
//{
//    public class KMUserCartModelFactory : IKMUserCartModelFactory
//    {
//        private readonly IProductService _productService;
//        private readonly IVendorService _vendorService;
//        private readonly IDiscountService _discountService;
//        private readonly IRepository<KMUserDiscountUsageHistory> _userDiscountUsageRepository;
//        private readonly IStaticCacheManager _staticCacheManager;

//        public KMUserCartModelFactory(
//            IProductService productService,
//            IVendorService vendorService,
//            IDiscountService discountService,
//            IRepository<KMUserDiscountUsageHistory> userDiscountUsageRepository,
//            IStaticCacheManager staticCacheManager)
//        {
//            _productService = productService;
//            _vendorService = vendorService;
//            _discountService = discountService;
//            _userDiscountUsageRepository = userDiscountUsageRepository;
//            _staticCacheManager = staticCacheManager;
//        }

//        public async Task PrepareUserCartModelAsync(UserCartModel model)
//        {
//            await ValidateDiscountsAsync(model);
//            await PrepareUserCartProductModelAsync(model);
//        }

//        public async Task PrepareUserCartProductModelAsync(UserCartModel model)
//        {
//            foreach (var item in model.Items)
//            {
//                var product = await _productService.GetProductByIdAsync(item.ProductId);

//                item.Name = product.Name;
//                item.ShortDescription = product.ShortDescription;
//                item.Sku = product.Sku;
//                item.VendorId = product.VendorId;

//                var v = await _vendorService.GetVendorByProductIdAsync(item.ProductId);
//                item.VendorName = v?.Name;

//                //allowed quantities
//                var allowedQuantities = _productService.ParseAllowedQuantities(product);
//                item.AllowedQuantities = allowedQuantities.Select(qty => new
//                {
//                    Text = qty.ToString(),
//                    Value = qty.ToString(),
//                    Selected = item.Quantity == qty
//                }).ToList();

//                //recurring info
//                if (product.IsRecurring)
//                    item.RecurringInfo = $"{product.RecurringCycleLength} {product.RecurringCyclePeriod}";


//                //rental info
//                if (!product.IsRental)
//                {
//                    item.RentalStartDateUtc = null;
//                    item.RentalEndDateUtc = null;
//                }

//            }
//            throw new NotImplementedException();
//        }

//        protected async Task ValidateDiscountsAsync(UserCartModel model)
//        {
//            foreach (var couponCode in model.CouponCodes)
//            {
//                if (couponCode.HasNoValue())
//                    continue;

//                var allCoupnCodeDiscounts = await _discountService.GetAllDiscountsAsync(couponCode: couponCode);

//                var discount = allCoupnCodeDiscounts.FirstOrDefault(d => d.RequiresCouponCode && d.CouponCode == couponCode);
//                if (discount == default)
//                    continue;

//                var res = await ValidateNotGiftcardAndDiscount(discount, model);
//                if (!res) continue;

//                res = ValidateDiscountDates(discount, model);
//                if (!res) continue;

//                ValidateDiscountLimitation(discount, model);

//                //discount requirements
//                var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, discount);

//                var requirements = await _staticCacheManager.GetAsync(key,
//                    async () => await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true));

//                //get top-level group
//                var topLevelGroup = requirements.FirstOrDefault();
//                if (topLevelGroup == null || topLevelGroup.IsGroup && !(await _discountService.GetDiscountRequirementsByParentAsync(topLevelGroup)).Any() || !topLevelGroup.InteractionType.HasValue)
//                {
//                    //there are no requirements, so discount is valid
//                    AddToAppliedDiscounts(discount, model, couponCode);
//                    continue;
//                }

//                var errors = new List<string>();
//                //requirements exist, let's check them
//                var applied = await GetValidationResultAsync(
//                    requirements,
//                    topLevelGroup.InteractionType.Value,
//                    model.StoreId,
//                    errors);
//                if (applied)
//                    AddToAppliedDiscounts(discount, model, couponCode);
//            }
//        }
//        private void AddToAppliedDiscounts(Discount discount, UserCartModel model, string couponCode)
//        {
//            (model.AppliedDiscounts ??= new List<UserCartDiscountUsageModel>()).Add(new UserCartDiscountUsageModel
//            {
//                DiscountId = discount.Id,
//                DiscountName = discount.Name,
//                CouponCode = couponCode
//            });
//        }

//        protected async Task<bool> GetValidationResultAsync(
//            IEnumerable<DiscountRequirement> requirements,
//            RequirementGroupInteractionType groupInteractionType,
//            int storeId,
//            IList<string> errors)
//        {
//            var result = false;

//            foreach (var requirement in requirements)
//            {
//                if (requirement.IsGroup)
//                {
//                    var childRequirements = await _discountService.GetDiscountRequirementsByParentAsync(requirement);
//                    //get child requirements for the group
//                    var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
//                    result = await GetValidationResultAsync(childRequirements, interactionType, storeId, errors);
//                }

//                /********************************************************/
//                /* enables discounts plugins - disabled at this point */
//                /********************************************************/

//                //else
//                //{
//                //    //or try to get validation result for the requirement
//                //    var requirementRulePlugin = await _discountPluginManager
//                //        .LoadPluginBySystemNameAsync(requirement.DiscountRequirementRuleSystemName, null, storeId);
//                //    if (requirementRulePlugin == null)
//                //        continue;


//                //    var ruleResult = await requirementRulePlugin.CheckRequirementAsync(new DiscountRequirementValidationRequest
//                //    {
//                //        DiscountRequirementId = requirement.Id,
//                //        Customer = customer,
//                //        Store = store
//                //    });

//                //    //add validation error
//                //    if (!ruleResult.IsValid)
//                //    {
//                //        errors.Add("Shopping Cart discount cannot be used");
//                //        if (!string.IsNullOrEmpty(ruleResult.UserError))
//                //            errors.Add(ruleResult.UserError);
//                //        return false;
//                //    }

//                //    result = ruleResult.IsValid;
//                //}

//                //all requirements must be met, so return false
//                if (!result && groupInteractionType == RequirementGroupInteractionType.And)
//                    return false;

//                //any of requirements must be met, so return true
//                if (result && groupInteractionType == RequirementGroupInteractionType.Or)
//                    return true;
//            }

//            return result;
//        }

//        protected bool ValidateDiscountDates(Discount discount, UserCartModel model)
//        {

//            //check date range
//            var now = DateTime.UtcNow;
//            if (discount.StartDateUtc.HasValue)
//            {
//                var startDate = DateTime.SpecifyKind(discount.StartDateUtc.Value, DateTimeKind.Utc);
//                if (startDate.CompareTo(now) > 0)
//                {
//                    model.Errors.Add("Shopping cart discount not started yet");
//                    return false;
//                }
//            }

//            if (discount.EndDateUtc.HasValue)
//            {
//                var endDate = DateTime.SpecifyKind(discount.EndDateUtc.Value, DateTimeKind.Utc);
//                if (endDate.CompareTo(now) < 0)
//                {
//                    model.Errors.Add("Shopping cart discount expired");
//                    return false;
//                }
//            }

//            return true;
//        }

//        protected async Task<bool> ValidateNotGiftcardAndDiscount(Discount discount, UserCartModel model)
//        {
//            var cartProductIds = model.Items.Select(ci => ci.ProductId).ToArray();
//            //Do not allow discounts applied to order subtotal or total when a customer has gift cards in the cart.
//            //Otherwise, this customer can purchase gift cards with discount and get more than paid ("free money").
//            if ((discount.DiscountType == DiscountType.AssignedToOrderSubTotal ||
//                discount.DiscountType == DiscountType.AssignedToOrderTotal) &&
//                await _productService.HasAnyGiftCardProductAsync(cartProductIds))
//            {
//                model.Errors.Add("Shopping Cart discounts cannot be used with giftcards");
//                return false;
//            }
//            return true;
//        }

//        protected void ValidateDiscountLimitation(Discount discount, UserCartModel model)
//        {

//            //discount limitation
//            switch (discount.DiscountLimitation)
//            {
//                case DiscountLimitationType.NTimesOnly:
//                    {

//                        var usedTimes = from d in _userDiscountUsageRepository.Table
//                                        where d.DiscountId == discount.Id
//                                        select d;

//                        var usedTimesCount = usedTimes.Count();

//                        if (usedTimesCount >= discount.LimitationTimes)
//                            model.Errors.Add("Shopping cart discount exceeded allowed usages");
//                    }

//                    break;
//                case DiscountLimitationType.NTimesPerCustomer:
//                    {
//                        var usedTimes = from d in _userDiscountUsageRepository.Table
//                                        where d.DiscountId == discount.Id && d.KmUserId == model.UserId
//                                        select d;

//                        var usedTimesCount = usedTimes.Count();

//                        if (usedTimesCount >= discount.LimitationTimes)
//                            model.Errors.Add("Shopping cart discount exceeded allowed usages for customer");
//                    }
//                    break;
//                case DiscountLimitationType.Unlimited:
//                default:
//                    break;
//            }
//        }
//    }
//}
