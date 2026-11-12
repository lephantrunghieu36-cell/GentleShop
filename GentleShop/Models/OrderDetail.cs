using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GentleShop.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductVariantId { get; set; }

        [StringLength(255)]
        public string? Imei { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        public DateTime? WarrantyStart { get; set; }

        public DateTime? WarrantyEnd { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "normal";

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("ProductVariantId")]
        public ProductVariant? ProductVariant { get; set; }
    }
}
