using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SweetsShop.Models.Authorization;

namespace SweetsShop.Models.ViewModels
{
    public class RegistrationVM
    {
        public AddressModel AddressModel { get; set; }
        [Required(ErrorMessage = "Не указан Телефон")]
        [StringLength(10)]
        public string Phone { get; set; }
        public bool SaveAddress { get; set; }
        public bool SavePhone { get; set; }
        [StringLength(150)]
        public string Comment { get; set; }
        public int Sum { get; set; }
        public List<ShoppingCart> prodList { get; set; }

    }
}
