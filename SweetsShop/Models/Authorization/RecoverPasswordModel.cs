using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetsShop.Models.Authorization
{
    public class RecoverPasswordModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Неправильно указан Email")]
        [StringLength(200, ErrorMessage = "Слишком длинный email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(50, ErrorMessage = "Слишком длинный пароль")]
        [MinLength(6, ErrorMessage = "Минимальная длина 6 символов")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Повторите пароль")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Код из сообщения на почту")]
        public int Code { get; set; }
    }
}
