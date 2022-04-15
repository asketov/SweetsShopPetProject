using System;
using System.Collections.Generic;
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
       public User User { get; set; }
    }
}
