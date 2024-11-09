namespace STORE_Website.ViewModels
{
    public class UpdateProductViewModel
    {
        public int Id { get; set; }  // Include Id to identify the product to be updated
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}

