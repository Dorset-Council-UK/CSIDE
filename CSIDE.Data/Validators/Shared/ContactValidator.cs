using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Validators.Shared
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public ContactValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(contact => contact.Name)
                .MaximumLength(500)
                .WithName(localizer["Name Label"]);
            RuleFor(contact => contact.Email)
                .MaximumLength(500)
                .WithName(localizer["Email Label"]);
            RuleFor(contact => contact.PrimaryContactNo)
                .MaximumLength(20)
                .WithName(localizer["Primary Contact Number Label"]);
            RuleFor(contact => contact.SecondaryContactNo)
                .MaximumLength(20)
                .WithName(localizer["Secondary Contact Number Label"]);
            RuleFor(contact => contact.OrganisationName)
                .MaximumLength(500)
                .WithName(localizer["Organisation Name Label"]);

            RuleFor(contact => contact)
                .Must(HaveAtLeastOnePropertyFilledIn)
                .WithMessage(localizer["At Least One Contact Property Validation Message"]);
        }

        private bool HaveAtLeastOnePropertyFilledIn(Contact contact)
        {

            if(string.IsNullOrEmpty(contact.Name) &&
                string.IsNullOrEmpty(contact.Email) &&
                string.IsNullOrEmpty(contact.PrimaryContactNo) &&
                string.IsNullOrEmpty(contact.SecondaryContactNo) &&
                string.IsNullOrEmpty(contact.OrganisationName))
            {
                return false;
            }
            return true;
        }
    }
}
