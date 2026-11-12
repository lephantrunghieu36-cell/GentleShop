using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GentleShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Slug { get; set; }

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public int? BrandId { get; set; }

        public string? Thumbnail { get; set; }

        public float RatingAvg { get; set; } = 0;

        public int RatingCount { get; set; } = 0;

        public int ViewCount { get; set; } = 0;

        public int SoldCount { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;

        public bool IsNew { get; set; } = false;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string? Specifications { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
