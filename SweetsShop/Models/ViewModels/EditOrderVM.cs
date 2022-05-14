using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SweetsShop.Models.ViewModels
{
    public class EditOrderVM
    {
        public int OrderID { get; set;}
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<ProductForUserOrder> ProductsForUserOrder { get; set; }
        public string Comment { get; set; }
        public int DifferenceInSum { get; set; }
            
    }
}
