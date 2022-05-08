using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SweetsShop.Models.Authorization;
using SweetsShop.Models.Client;
namespace SweetsShop.Models.ViewModels
{
    public class AccountVM
    {
        public AddressModel AddressModel { get; set; }
        [StringLength(12)]
        [Display(Name = "Телефон:")]
        public string Phone { get; set; }
        public string Email { get; set; }
        [Display(Name = "Адрес:")]
        public string Address { get; set; }
    }
}


