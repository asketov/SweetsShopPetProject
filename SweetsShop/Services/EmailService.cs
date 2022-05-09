using System.IO;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SweetsShop.Models.Client;
using SweetsShop.Services.Interfaces;

namespace SweetsShop.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("SweetsShopNCH", "sweetsshopnch@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("sweetsshopnch@gmail.com", "nedu xmyj kadn unvq");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendOrderToEmailAsync(string email,string WebRootPath)
        {
            var PathToTemplate = WebRootPath + Path.DirectorySeparatorChar.ToString() + "templates"
                                 + Path.DirectorySeparatorChar.ToString() +
                                 "Order.html";
            var subject = "Новый заказ на сайте SweetsShopNCH";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            await SendEmailAsync(email, subject, HtmlBody);
        }
    }
}
