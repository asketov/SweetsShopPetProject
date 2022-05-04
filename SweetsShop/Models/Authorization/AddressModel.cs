using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SweetsShop.Models.Authorization
{
    public class AddressModel
    {
        [Key]
        public int Id { get; set; }
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
        public string? House { get; set; }
        [StringLength(50)]
        public string? Housing { get; set; }
        [StringLength(50)]
        public string? Entrance { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Введите квартиру или офис")]
        public string Flat { get; set; }
        [StringLength(50)]
        public string? Floor { get; set; }

        public string AddressModelToString()
        {
            string Address="";
            Address += "Город: " + this.City + ", ";
            Address += "Район: " + this.District + ", ";
            Address += "Улица: " + this.Street + ", ";
            Address += "Дом: " + this.House + ", ";
            if(this.Housing!=null) Address += "Корпус: " + this.Housing + ", ";
            if (this.Entrance != null) Address += "Подъезд: " + this.Entrance + ", ";
            if (this.Floor != null)
            {
                Address += "Кв-ва/офис: " + this.Flat + ", ";
                Address += "Этаж: " + this.Floor;
            }
            else Address += "Кв-ва/офис: " + this.Flat;
            return Address;
        }
    }
}
