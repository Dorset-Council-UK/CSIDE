using System.ComponentModel.DataAnnotations;

namespace CSIDE.Data.Models.Authorization
{
    public class ApplicationUserRole
    {
        [Required]
        public required string UserId { get; set; }
        [Required]
        public int ApplicationRoleId { get; set; }
        public virtual ApplicationRole? Role { get; set; }
    }
}
