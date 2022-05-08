using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
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
        public string Address { get; set; }
        public DateTime DateOrder { get; set; }
        public string? Comment { get; set; }
        public int UserId { get; set; }
        
    }
}
