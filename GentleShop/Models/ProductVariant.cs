using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GentleShop.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ImportPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SalePrice { get; set; }

        [StringLength(255)]
        public string? Variant { get; set; }

        [StringLength(255)]
        public string? Imei { get; set; }

        public DateTime? WarrantyStart { get; set; }

        public DateTime? WarrantyEnd { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "available";

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public ICollection<ImportDetail> ImportDetails { get; set; }
            = new List<ImportDetail>();

        public ICollection<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();
    }
}
