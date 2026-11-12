using System.ComponentModel.DataAnnotations;

namespace GentleShop.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Img { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
