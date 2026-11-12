using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GentleShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public int EmployeeId { get; set; }

        public int? VoucherId { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = "cod";

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "pending";

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "pending";

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public DateTime? ReceivedAt { get; set; }

        [ForeignKey("CustomerId")]
        public User? Customer { get; set; }

        [ForeignKey("EmployeeId")]
        public User? Employee { get; set; }

        [ForeignKey("VoucherId")]
        public Voucher? Voucher { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();
    }

}
