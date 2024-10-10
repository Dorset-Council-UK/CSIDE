using System.ComponentModel.DataAnnotations;

namespace CSIDE.Data.Models.Authorization
{
    public class ApplicationRole
    {
        public int Id { get; set; }
        [MaxLength(200)]
        [Required]
        public string? RoleName { get; set; }
    }
}
