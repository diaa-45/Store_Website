namespace STORE_Website.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string  Name { get; set; }
        public string? Description { get; set; }

        List<Product> Products { get; set; }
    }
}
