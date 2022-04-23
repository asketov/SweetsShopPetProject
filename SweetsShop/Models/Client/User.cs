using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SweetsShop.Models.Authorization;

namespace SweetsShop.Models.Client
{
    public class User
    {
        [Key] 
        public int Id { get; set; }

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        [Display(Name = "Адрес")] 
        public string? Address { get; set; }

        [Display(Name = "Телефон")] 
        public string? Phone { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")] 
        public virtual Role Role { get; set; }
        public int? AddressId { get; set; }

        [ForeignKey("AddressId")]
        public virtual AddressModel AddressModel { get; set; }
    }
}