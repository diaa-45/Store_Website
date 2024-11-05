namespace STORE_Website.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        // Foreign key to the ApplicationUser
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        // Collection of items in the cart
        public List<ShoppingCartItem> Items { get; set; }
    }
}
