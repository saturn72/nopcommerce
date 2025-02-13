using FluentValidation;
using KedemMarket.Admin.Models.Navbar;
using KedemMarket.Services.Navbar;
using Nop.Web.Framework.Validators;

namespace KedemMarket.Admin.Validators;
public class CreateNavbarElementPopupModelValidator : BaseNopValidator<CreateOrUpdateNavbarElementModel>
{
    public CreateNavbarElementPopupModelValidator(
        ILocalizationService localizationService,
        INavbarService navbarInfoService)
    {
        RuleFor(x => x.Caption)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.Caption.Required"));

        //does not have exist elements with the same label
        RuleFor(x => x.Caption)
            .MustAwait(async (nb, ct) =>
            {
                var nbes = await navbarInfoService.GetNavbarElementsByNavbarInfoIdAsync(nb.NavbarInfoId);
                return nbes.FirstOrDefault(d =>
                d.Id != nb.Id &&
                d.NavbarInfoId == nb.NavbarInfoId &&
                d.Caption == nb.Caption) == null;
            })
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.Caption.Unique"));

        RuleFor(x => x.Icon)
           .NotEmpty()
           .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.Icon.Required"));

        RuleFor(x => x.ActiveIcon)
                  .NotEmpty()
                  .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.ActiveIcon.Required"));

        RuleFor(x => x.Type)
           .Must(x => x.Equals(Consts.NavbarElementType.Filter, StringComparison.InvariantCultureIgnoreCase) ||
           x.Equals(Consts.NavbarElementType.Route, StringComparison.InvariantCultureIgnoreCase))
           .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.Type.Invalid"));

        RuleFor(x => x.Value)
           .NotEmpty()
           .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.Value.Required"));

        RuleFor(x => x.NavbarInfoId)
           .NotEmpty()
           .WithMessageAwait(localizationService.GetResourceAsync("Admin.NavbarElement.Fields.NavbarInfoId.Required"));

        SetDatabaseValidationRules<NavbarElement>();
    }
}