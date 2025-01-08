using FluentValidation;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.Maintenance
{
    public class SearchValidator : AbstractValidator<Search>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public SearchValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s).Must(s =>
                !string.IsNullOrEmpty(s.RouteID) ||
                !string.IsNullOrEmpty(s.AssignedToTeamId) ||
                !string.IsNullOrEmpty(s.ParishId) ||
                s.ParishIds?.Length != 0 ||
                s.JobStatusId.HasValue ||
                s.JobPriorityId.HasValue ||
                s.LogDateFrom.HasValue ||
                s.LogDateTo.HasValue ||
                s.CompletedDateFrom.HasValue ||
                s.CompletedDateTo.HasValue).WithMessage(_localizer["Search Validation At Least One Message"]);

            RuleFor(s => s.LogDateFrom).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).When(s => s.LogDateFrom.HasValue).WithMessage(_localizer["Search Validation Date Not In Future Message",localizer["Log Date From Label"]]);
            RuleFor(s => s.CompletedDateFrom).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).When(s => s.CompletedDateFrom.HasValue).WithMessage(_localizer["Search Validation Date Not In Future Message", localizer["Completed Date From Label"]]);
            RuleFor(s => s.LogDateTo).GreaterThanOrEqualTo((s) => s.LogDateFrom).When(s => s.LogDateFrom.HasValue && s.LogDateTo.HasValue);
            RuleFor(s => s.CompletedDateTo).GreaterThanOrEqualTo((s) => s.CompletedDateFrom).When(s => s.CompletedDateFrom.HasValue && s.CompletedDateTo.HasValue);
        }

    }
}

//public string? RouteID { get; set; }
//public string? AssignedToTeamId { get; set; }
//public LocalDate? LogDateFrom { get; set; }
//public LocalDate? LogDateTo { get; set; }
//public int? JobStatusId { get; set; }
//public int? JobPriorityId { get; set; }
//public LocalDate? CompletedDateFrom { get; set; }
//public LocalDate? CompletedDateTo { get; set; }