using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Extensions
{
    public static class LandownerDepositExtensions
    {
        public static LandownerDepositPublicViewModel ToPublicViewModel(this LandownerDeposit landownerDeposit, string landownerDepositIdPrefix)
        {
            return new LandownerDepositPublicViewModel()
            {
                Id = landownerDeposit.Id,
                SecondaryId = landownerDeposit.SecondaryId,
                ReferenceNo = $"{landownerDepositIdPrefix}{landownerDeposit.Id}/{landownerDeposit.SecondaryId}",
                ReceivedDate = landownerDeposit.ReceivedDate?.ToDateOnly(),
                Location = landownerDeposit.Location,
                PrimaryContact = landownerDeposit.PrimaryContact,
                Geom = landownerDeposit.Geom,
                DepositTypes = [.. landownerDeposit.LandownerDepositTypes.Select(t => t.LandownerDepositTypeName!.Name)],
                Media = [.. landownerDeposit.LandownerDepositMedia.Select(m => new MediaPublicViewModel
                {
                    Title = m.Media.Title,
                    UploadDate = m.Media.UploadDate!.Value.InUtc().LocalDateTime.Date.ToDateOnly(),
                    DocumentType = m.MediaType?.Name,
                    URL = m.Media.URL,
                })],
                Contacts = [.. landownerDeposit.LandownerDepositContacts
                    .Where(c => c.Contact.ContactType != null && (c.Contact.ContactType.Name == "Applicant" || c.Contact.ContactType.Name == "Landowner"))
                    .Select(c => new ContactPublicViewModel
                    {
                        Name = c.Contact.Name,
                        Address = "",
                        Organisation = c.Contact.OrganisationName,
                        ContactType = c.Contact.ContactType?.Name
                    })],
                AffectedAddresses = [.. landownerDeposit.LandownerDepositAddresses.Select(a => a.Address!)],
                Parishes = [.. landownerDeposit.LandownerDepositParishes.Select(p => p.Parish.Name)],
            };
        }

        public static LandownerDepositSimplePublicViewModel ToSimplePublicViewModel(this LandownerDeposit landownerDeposit, string landownerDepositIdPrefix)
        {
            return new LandownerDepositSimplePublicViewModel()
            {
                Id = landownerDeposit.Id,
                SecondaryId = landownerDeposit.SecondaryId,
                ReferenceNo = $"{landownerDepositIdPrefix}{landownerDeposit.Id}/{landownerDeposit.SecondaryId}",
                ReceivedDate = landownerDeposit.ReceivedDate?.ToDateOnly(),
                Location = landownerDeposit.Location,
                PrimaryContact = landownerDeposit.PrimaryContact,
                DepositTypes = [.. landownerDeposit.LandownerDepositTypes.Select(t => t.LandownerDepositTypeName!.Name)],
                Parishes = [.. landownerDeposit.LandownerDepositParishes.Select(p => p.Parish.Name)],
            };
        }
    }
}