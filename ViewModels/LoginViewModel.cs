using System.ComponentModel.DataAnnotations;

namespace STORE_Website.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me ?")]
        public bool RemenberMe { get; set; }
    }
}
