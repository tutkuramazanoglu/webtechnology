using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class IletisimController : Controller
    {
        // GET: Iletisim
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(IletisimModel model)
        {
            if (ModelState.IsValid)
            {
                var body = new StringBuilder();
                body.AppendLine("Rumuz: " + model.NickName);
                body.AppendLine("İsim: " + model.FullName);
                body.AppendLine("Tel: " + model.Phone);
                body.AppendLine("Eposta: " + model.Email);
                body.AppendLine("Konu: " + model.Message);
                gmail.SendMail(body.ToString());
                ViewBag.Success = true;
            }
            return View();
        }
    }
}