namespace STORE_Website.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        // Foreign key to the ShoppingCart
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        // Foreign key to the Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; } // Quantity of the product in the cart
    }
}
