using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GentleShop.Models
{
    public class Import
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "pending";

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [ForeignKey("EmployeeId")]
        public User? Employee { get; set; }

        public ICollection<ImportDetail> ImportDetails { get; set; }
            = new List<ImportDetail>();
    }
}
