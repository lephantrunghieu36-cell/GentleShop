using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GentleShop.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MinAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "active";

        [Required]
        [StringLength(255)]
        public string Type { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }

}
