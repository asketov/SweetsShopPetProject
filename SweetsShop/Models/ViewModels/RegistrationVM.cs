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
        public string Phone { get; set; }
        
    }
}
