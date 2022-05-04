using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SweetsShop.Models.Authorization;

namespace SweetsShop.Models.Client
{
    public class FullOrder
    {
        [Key]
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Products { get; set; }
        public int Sum { get; set; }
        public int AddressId { get; set; }

        [ForeignKey("AddressId")]
        public virtual AddressModel AddressModel { get; set; }
    }
}
