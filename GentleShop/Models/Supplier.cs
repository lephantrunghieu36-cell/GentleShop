using System.ComponentModel.DataAnnotations;

namespace GentleShop.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
