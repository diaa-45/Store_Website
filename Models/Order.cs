using System.ComponentModel.DataAnnotations;

namespace STORE_Website.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to the ApplicationUser
        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Order status (Pending = Cart, Completed = Purchase)
        public string? Status { get; set; }

        // List of products in the order/cart
        public List<OrderItem> OrderItems { get; set; }
    }
}
