using CSIDE.Data.Models.Shared;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace CSIDE.Data.Models.DMMO;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DMMOApplicationExtensions
{
    public static DMMOApplicationPublicViewModel ToPublicViewModel(this DMMOApplication application, string dmmoIdPrefix)
    {
        return new DMMOApplicationPublicViewModel()
        {
            Id = application.Id,
            ReferenceNo = $"{dmmoIdPrefix}{application.Id}",
            ApplicationDate = application.ApplicationDate?.ToDateOnly(),
            ReceivedDate = application.ReceivedDate?.ToDateOnly(),
            ApplicationDetails = application.ApplicationDetails,
            LocationDescription = application.LocationDescription,
            CaseOfficer = application.CaseOfficer,
            PublicComments = application.PublicComments,
            DeterminationDate = application.DeterminationDate?.ToDateOnly(),
            Appeal = application.Appeal,
            AppealDate = application.AppealDate?.ToDateOnly(),
            DateOfDirectionOfSecState = application.DateOfDirectionOfSecState?.ToDateOnly(),
            Geom = application.Geom,
            ApplicationType = application.ApplicationType?.Name,
            CaseStatus = application.CaseStatus?.Name,
            DirectionOfSecState = application.DirectionOfSecState?.Name,
            ClaimedStatuses = [.. application.DMMOClaimedStatuses.Select(c => c.ClaimedStatus.Name)],
            Parishes = [.. application.DMMOParishes.Select(p => p.Parish.Name)],
            LinkedRoutes = [.. application.DMMOLinkedRoutes.Select(r => r.RouteId)],
            AffectedAddresses = [.. application.DMMOAddresses.Where(a => a.Address != null).Select(a => a.Address!)],
            Media = [.. application.DMMOMedia.Select(m => new MediaPublicViewModel
            {
                Title = m.Media.Title,
                UploadDate = m.Media.UploadDate!.Value.InUtc().LocalDateTime.Date.ToDateOnly(),
                DocumentType = m.MediaType?.Name,
                URL = m.Media.URL,
            })],
            Contacts = [.. application.DMMOContacts.Where(c => c.Contact.ContactType?.Name == "Applicant")
                .Select(c => new ContactPublicViewModel
                {
                    Name = c.Contact.Name,
                    Address = "",
                    Organisation = c.Contact.OrganisationName,
                    ContactType = c.Contact.ContactType?.Name
                })
            ],
            Orders = [.. application.Orders.Select(o => new OrderPublicViewModel{
                ReferenceNo = $"{dmmoIdPrefix}{application.Id}/{o.OrderId}",
                ObjectionsEndDate = o.ObjectionsEndDate?.ToDateOnly(),
                ObjectionsReceived = o.ObjectionsReceived,
                ObjectionsWithdrawn = o.ObjectionsWithdrawn,
                DeterminationProcessId = o.DeterminationProcessId,
                DeterminationProcess = o.DeterminationProcess?.Name,
                DecisionOfSecStateId = o.DecisionOfSecStateId,
                DecisionOfSecState = o.DecisionOfSecState?.Name,
                DateConfirmed = o.DateConfirmed?.ToDateOnly(),
                DateSealed = o.DateSealed?.ToDateOnly(),
                DatePublished = o.DatePublished?.ToDateOnly()
            })]
        };
    }

    public static DMMOApplicationSimplePublicViewModel ToSimplePublicViewModel(this DMMOApplication application, string dmmoIdPrefix)
    {
        return new DMMOApplicationSimplePublicViewModel()
        {
            Id = application.Id,
            ReferenceNo = $"{dmmoIdPrefix}{application.Id}",
            ApplicationDate = application.ApplicationDate?.ToDateOnly(),
            ReceivedDate = application.ReceivedDate?.ToDateOnly(),
            ApplicationDetails = application.ApplicationDetails,
            LocationDescription = application.LocationDescription,
            CaseOfficer = application.CaseOfficer,
            PublicComments = application.PublicComments,
            DeterminationDate = application.DeterminationDate?.ToDateOnly(),
            Appeal = application.Appeal,
            AppealDate = application.AppealDate?.ToDateOnly(),
            DateOfDirectionOfSecState = application.DateOfDirectionOfSecState?.ToDateOnly(),
            ApplicationType = application.ApplicationType?.Name,
            CaseStatus = application.CaseStatus?.Name,
            DirectionOfSecState = application.DirectionOfSecState?.Name,
            ClaimedStatuses = [.. application.DMMOClaimedStatuses.Select(c => c.ClaimedStatus.Name)],
            Parishes = [.. application.DMMOParishes.Select(p => p.Parish.Name)]
        };
    }
}