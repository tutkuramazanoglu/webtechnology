using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Ecommerce.Models
{
    public class gmail
    {

        public static void SendMail(string body)
        {
            var fromAddress = new MailAddress("aliyildiz.tutku@gmail.com", "Deneme");
            var toAddress = new MailAddress("aliyildiz.tutku@gmail.com");
            const string subject = "Deneme";
            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "0102030405")
            })
            {
                using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}