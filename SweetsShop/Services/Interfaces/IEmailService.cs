using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetsShop.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendOrderToEmailAsync(string email, string WebRootPath);
        Task SendRecoverCodeToEmailAsync(string email, string WebRootPath);
        public int CodeToRecoverPassword { get; set; }
    }
}
