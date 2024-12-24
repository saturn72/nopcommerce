using FluentValidation;
using Nop.Web.Framework.Validators;

namespace KM.Navbar.Admin.Validators;
public class NavbarElementModelValidator : BaseNopValidator<NavbarElementModel>
{
    public NavbarElementModelValidator(
        ILocalizationService localizationService,
        INavbarInfoService navbarInfoService)
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.NaNavbarElement.Fields.Label.Required"));

        //does not have exist elements with the same label
        RuleFor(x => x.Label)
            .MustAwait(async (nb, ct) =>
            {
                var nbes = await navbarInfoService.GetNavbarElementsByNavbarInfoIdAsync(nb.NavbarInfoId);
                return nbes.FirstOrDefault(d => d.Label == nb.Label) == null;
            })
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.NaNavbarElement.Fields.Label.Unique"));

        RuleFor(x => x.Icon)
           .NotEmpty()
           .WithMessageAwait(localizationService.GetResourceAsync("Admin.Navbar.Fields.Icon.Required"));

        SetDatabaseValidationRules<NavbarElement>();
    }
}