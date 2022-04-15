using System.ComponentModel.DataAnnotations;

namespace SweetsShop.Models.Client
{
    public class Role
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }
    }
}