using CSIDE.Data.Models.Shared;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace CSIDE.Data.Models.PPO
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class PPOApplicationExtensions
    {
        public static PPOApplicationPublicViewModel ToPublicViewModel(this PPOApplication application, string ppoIdPrefix)
        {
            return new PPOApplicationPublicViewModel()
            {
                Id = application.Id,
                ReferenceNo = $"{ppoIdPrefix}{application.Id}",
                ReceivedDate = application.ReceivedDate?.ToDateOnly(),
                ApplicationDetails = application.ApplicationDetails,
                LocationDescription = application.LocationDescription,
                CaseOfficer = application.CaseOfficer,
                PublicComments = application.PublicComments,
                DeterminationDate = application.DeterminationDate?.ToDateOnly(),
                DateOfDirection = application.DateOfDirection?.ToDateOnly(),
                InspectionCertification = application.InspectionCertification,
                InspectionCertificationDate = application.InspectionCertificationDate?.ToDateOnly(),
                ConfirmationPublishedDate = application.ConfirmationPublishedDate?.ToDateOnly(),
                CouncilLandAffected = application.CouncilLandAffected,
                Geom = application.Geom,
                Legislation = application.Legislation?.Name,
                CaseStatus = application.CaseStatus?.Name,
                Priority = application.Priority?.Description,
                Parishes = [.. application.PPOParishes.Select(p => p.Parish.Name)],
                ApplicationIntents = [.. application.PPOIntents.Select(i => i.Intent.Name)],
                Media = [.. application.PPOMedia.Select(m => new MediaPublicViewModel
                {
                    Title = m.Media.Title,
                    UploadDate = m.Media.UploadDate!.Value.InUtc().LocalDateTime.Date.ToDateOnly(),
                    DocumentType = m.MediaType?.Name,
                    URL = m.Media.URL,
                })],
                Contacts = [.. application.PPOContacts
                    .Where(c => c.Contact.ContactType != null && c.Contact.ContactType.Name == "Applicant")
                    .Select(c => new ContactPublicViewModel
                    {
                        Name = c.Contact.Name,
                        Address = "",
                        Organisation = c.Contact.OrganisationName,
                        ContactType = c.Contact.ContactType?.Name
                    })],
                Orders = [.. application.Orders.Select(o => new OrderPublicViewModel{
                    ReferenceNo = $"{ppoIdPrefix}{application.Id}/{o.OrderId}",
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

        public static PPOApplicationSimplePublicViewModel ToSimplePublicViewModel(this PPOApplication application, string ppoIdPrefix)
        {
            return new PPOApplicationSimplePublicViewModel()
            {
                Id = application.Id,
                ReferenceNo = $"{ppoIdPrefix}{application.Id}",
                ReceivedDate = application.ReceivedDate?.ToDateOnly(),
                ApplicationDetails = application.ApplicationDetails,
                LocationDescription = application.LocationDescription,
                CaseOfficer = application.CaseOfficer,
                PublicComments = application.PublicComments,
                DeterminationDate = application.DeterminationDate?.ToDateOnly(),
                DateOfDirection = application.DateOfDirection?.ToDateOnly(),
                InspectionCertification = application.InspectionCertification,
                InspectionCertificationDate = application.InspectionCertificationDate?.ToDateOnly(),
                ConfirmationPublishedDate = application.ConfirmationPublishedDate?.ToDateOnly(),
                CouncilLandAffected = application.CouncilLandAffected,
                Legislation = application.Legislation?.Name,
                CaseStatus = application.CaseStatus?.Name,
                Priority = application.Priority?.Description,
                Parishes = [.. application.PPOParishes.Select(p => p.Parish.Name)]
            };
        }
    }
}