using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class DetayController : Controller
    {
        private eTicaretEntities1 db = new eTicaretEntities1();
        // GET: Detay
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DetayGoster(int id)
        {
            var urun = db.Urunler.FirstOrDefault(x => x.UrunID == id);

            DateTime now = DateTime.Now;
            var uDetay = (from u in db.Urunler
                          join k in db.Kampanya
                          on u.UrunID equals k.ProductID
                          where id == u.UrunID && (k.Baslangic < DateTime.Now && k.Bitis > DateTime.Now)
                          select new { u, k, }
                       );

            if (uDetay != null)
            {
                foreach (var item in uDetay)
                {
                    urun.ProductPrice = item.k.Sale;
                }
            }

            return View(urun);
        }


    }
}