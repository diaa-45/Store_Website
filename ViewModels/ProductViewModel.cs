using System.ComponentModel.DataAnnotations;

namespace STORE_Website.ViewModels
{
    public class ProductViewModel
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid price.")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; } // For the uploaded image file
    }
}
