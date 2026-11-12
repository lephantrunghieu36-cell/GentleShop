using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GentleShop.Models
{
    public class ImportDetail
    {
        [Key]
        public int Id { get; set; }

        public int ImportId { get; set; }

        public int ProductVariantId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ImportPrice { get; set; }

        [StringLength(255)]
        public string? Imei { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tax { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        public DateTime? WarrantyStart { get; set; }

        public DateTime? WarrantyEnd { get; set; }

        [ForeignKey("ImportId")]
        public Import? Import { get; set; }

        [ForeignKey("ProductVariantId")]
        public ProductVariant? ProductVariant { get; set; }
    }
}
