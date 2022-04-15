using System.ComponentModel.DataAnnotations;

namespace SweetsShop.Models.Authorization
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неправильно указан Email")]
        [StringLength(200, ErrorMessage = "Слишком длинный email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(50,ErrorMessage = "Слишком длинный пароль")]
        [MinLength(6,ErrorMessage = "Минимальная длина 6 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    } 
}
