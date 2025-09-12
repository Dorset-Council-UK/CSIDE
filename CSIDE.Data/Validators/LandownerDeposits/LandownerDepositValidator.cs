using FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.Extensions.Localization;
using NodaTime;

namespace CSIDE.Data.Validators.LandownerDeposits
{
    public class LandownerDepositValidator : AbstractValidator<LandownerDeposit>
    {
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;

        public LandownerDepositValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;

            // This is validated within the GeometryValidator rulesets, but the NotEmpty check helps catch when no geometry is provided at all.
            RuleFor(app => app.Geom)
                .NotEmpty()
                .WithMessage(localizer["Invalid Geometry Validation Message"])
                .WithErrorCode("INVALID_GEOM");

            RuleFor(ld => ld.Location)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Landowner Deposit Location Label"]);
            RuleFor(ld => ld.ReceivedDate)
                .NotEmpty()
                .WithName(_localizer["Landowner Deposit Received Date Label"]);
            RuleFor(ld => ld.ChequeReceiptNumber)
                .MaximumLength(50)
                .WithName(_localizer["Landowner Deposit Cheque Receipt Label"]);
            RuleFor(ld => ld.ArchiveReference)
                .MaximumLength(50)
                .WithName(_localizer["Landowner Deposit Archive Reference Label"]);

            RuleFor(ld => ld.ReceivedDate)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.DateAcknowledged)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.ChequePaidInDate)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.NoticeDrafted)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.WebsiteNoticePublished)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.EmailNoticeSent)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.OnsiteNoticeErected)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.WebsiteEntryAdded)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(ld => ld.SentToArchive)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
        }
    }
}
