using FluentValidation;
using KedemMarket.Admin.Domain;
using KedemMarket.Admin.Models.Navbar;
using KedemMarket.Services.Navbar;
using Nop.Web.Framework.Validators;

namespace KedemMarket.Admin.Validators;
public class NavInfoModelValidator : BaseNopValidator<NavbarInfoModel>
{
    public NavInfoModelValidator(
        ILocalizationService localizationService,
        INavbarService navbarInfoService)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Navbar.Fields.Name.Required"));

        RuleFor(x => x.Name)
            .MustAwait(async (nb, ct) =>
            {
                var x = await navbarInfoService.GetNavbarInfoByNameAsync(nb.Name);
                return x == null;
            })
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Navbar.Fields.Name.Unique"));

        SetDatabaseValidationRules<NavbarInfo>();
    }
}