using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetsShop.Models.Authorization
{
    public class AddressModel
    {
        [Required(ErrorMessage = "Введите город")]
        [StringLength(50)]
        public string City { get; set; }
        [Required(ErrorMessage = "Введите район")]
        [StringLength(50)]
        public string District { get; set; }
        [Required(ErrorMessage = "Введите улицу")]
        [StringLength(50)]
        public string Street { get; set; }
        [Required(ErrorMessage = "Введите дом")]
        [StringLength(50)]
        public string House { get; set; }
        [StringLength(50)]
        public string Housing { get; set; }
        [StringLength(50)]
        public string Entrance { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Введите квартиру или офис")]
        public string Flat { get; set; }
        [StringLength(50)]
        public string Floor { get; set; }
    }
}
