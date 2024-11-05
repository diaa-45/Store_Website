using System.ComponentModel.DataAnnotations;

namespace STORE_Website.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        [Display(Name ="User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string City { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
        
}
