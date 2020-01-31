using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class SepetController : Controller
    {
        private eTicaretEntities1 db = new eTicaretEntities1();
        public ActionResult Index()
        {
            Kullanıcılar user = Session["KULLANICIADI"] as Kullanıcılar;
            if (user != null)
            {

                var data = (from u in db.Urunler
                            join s in db.Sepet on u.UrunID equals s.UrunID
                            where s.KullanıcıID == user.KullanıcıID && s.SatınAlındı == 0
                            select new { s, u }).ToList();
                var indirim = (from u in db.Urunler
                               join k in db.Kampanya
                               on u.UrunID equals k.ProductID
                               where k.ProductID == u.UrunID
                               && (k.Baslangic < DateTime.Now && k.Bitis > DateTime.Now)
                               select new { u, k, }
                            );
                List<Urunler> uListe = new List<Urunler>();
                if (indirim != null)
                {
                    foreach (var item in data)
                    {
                        Urunler product = new Urunler();
                        foreach (var item2 in indirim)
                        {
                            if (item.u.UrunID == item2.k.ProductID)
                            {
                                product.ProductPicture = item.u.ProductPicture;
                                product.UrunID = item.u.UrunID;
                                product.ProductName = item.u.ProductName;
                                product.ProductCode = item.u.ProductCode;
                                product.ProductPrice = item2.k.Sale;
                                product.Currency = item.u.Currency;
                                product.OrderedQuantity = item.s.Miktar;
                                uListe.Add(product);
                            }
                        }
                    }
                }

                foreach (var item in data)
                {
                    Urunler product2 = null;
                    try
                    {
                        product2 = uListe.Where(x => x.UrunID == item.u.UrunID).First();
                    }
                    catch { }
                    if (product2 == null)
                    {
                        Urunler product = new Urunler();
                        product.ProductPicture = item.u.ProductPicture;
                        product.UrunID = item.u.UrunID;
                        product.ProductName = item.u.ProductName;
                        product.ProductCode = item.u.ProductCode;
                        product.ProductPrice = item.u.ProductPrice;
                        product.Currency = item.u.Currency;
                        product.OrderedQuantity = item.s.Miktar;
                        uListe.Add(product);
                    }
                }
                return View(uListe);
            }
            else
            {
                return RedirectToAction("Gırıs", "Kullanıcılar");
            }
        }
        public ActionResult SepetEkle(Urunler urun, Sepet sepet)
        {
            if (Session["KULLANICIADI"] != null)
            {
                Sepet sepetim = null;
                Kullanıcılar user = Session["KULLANICIADI"] as Kullanıcılar;
                var yeniurun = db.Urunler.FirstOrDefault(x => x.UrunID == urun.UrunID);
                yeniurun.OrderedQuantity = urun.OrderedQuantity;
                List<Sepet> liste = (from k in db.Sepet where k.KullanıcıID == user.KullanıcıID select k).ToList();

                var indirim = (from u in db.Urunler
                               join k in db.Kampanya
                               on u.UrunID equals k.ProductID
                               where urun.UrunID == u.UrunID
                               && (k.Baslangic < DateTime.Now && k.Bitis > DateTime.Now)
                               select new { u, k, }
                               );
                try
                {
                    sepetim = liste.Where(x => (x.UrunID == urun.UrunID) && (x.SatınAlındı == 0)).First();
                }
                catch { }
                if (sepetim != null)
                {
                    sepetim.Miktar = sepetim.Miktar + Convert.ToInt32(urun.OrderedQuantity);
                    db.SaveChanges();
                }
                else
                {
                    db.SaveChanges();
                    sepet.KullanıcıID = (Session["KULLANICIADI"] as Kullanıcılar).KullanıcıID;
                    sepet.Miktar = Convert.ToInt32(yeniurun.OrderedQuantity);
                    sepet.EklenmeTarihi = DateTime.Now.ToString();
                    sepet.SatınAlındı = 0;
                    db.Sepet.Add(sepet);
                }
                db.SaveChanges();

                var data = (from u in db.Urunler
                            join s in db.Sepet on u.UrunID equals s.UrunID
                            where (s.KullanıcıID == user.KullanıcıID && s.SatınAlındı == 0)
                            select new { u, s, }).ToList();

                List<Urunler> uListe = new List<Urunler>();
                if (indirim != null)
                {
                    foreach (var item in data)
                    {
                        Urunler product = new Urunler();
                        foreach (var item1 in indirim)
                        {
                            if (item1.k.ProductID == item.s.UrunID)
                            {
                                product.ProductPrice = item1.k.Sale;
                                product.ProductName = item.u.ProductName;
                                product.UrunID = item.u.UrunID;
                                product.ProductCode = item.u.ProductCode;
                                product.OrderedQuantity = item.s.Miktar;
                                product.ProductPicture = item.u.ProductPicture;
                                product.Currency = item.u.Currency;
                                uListe.Add(product);
                            }
                        }
                    }
                }              
                    foreach (var item in data)
                    {
                    Urunler product2 = null;
                    try
                    {
                        product2 = uListe.Where(x => x.UrunID == item.u.UrunID).First();
                    }
                    catch { }
                    if (product2 == null)
                    {
                        Urunler product = new Urunler();
                        product.ProductPicture = item.u.ProductPicture;
                        product.UrunID = item.u.UrunID;
                        product.ProductName = item.u.ProductName;
                        product.ProductCode = item.u.ProductCode;
                        product.ProductPrice = item.u.ProductPrice;
                        product.Currency = item.u.Currency;
                        product.OrderedQuantity = item.s.Miktar;
                        uListe.Add(product);
                    }
                }
                return View("Index", uListe);
            }

            else
            {
                return RedirectToAction("Gırıs", "Kullanıcılar");
            }
        }
        public ActionResult SepetCıkar(int? id)
        {
            Kullanıcılar user = Session["KULLANICIADI"] as Kullanıcılar;
            var silinenurun = (from s in db.Sepet where s.UrunID == id && user.KullanıcıID == s.KullanıcıID select s).FirstOrDefault();
            if (silinenurun == null)
            {
                return RedirectToAction("Index");
            }
            if (silinenurun.Miktar == 1)
            {
                db.Sepet.Remove(silinenurun);
            }
            else
            {
                silinenurun.Miktar -= 1;

            }

            db.SaveChanges();
            var data = (from u in db.Urunler
                        join s in db.Sepet on u.UrunID equals s.UrunID
                        where s.KullanıcıID == user.KullanıcıID && s.SatınAlındı == 0
                        select new { u, s }).ToList();
            List<Urunler> sListe = new List<Urunler>();
            foreach (var item in data)
            {
                Urunler product = new Urunler();
                product.ProductName = item.u.ProductName;
                product.UrunID = item.u.UrunID;
                product.ProductCode = item.u.ProductCode;
                product.OrderedQuantity = item.s.Miktar;
                product.ProductPicture = item.u.ProductPicture;
                product.ProductPrice = item.u.ProductPrice;
                product.Currency = item.u.Currency;
                sListe.Add(product);
            }
            return View(sListe);
        }
        [HttpGet]
        public ActionResult SatınAl()
        {
            Kullanıcılar user = (Session["KULLANICIADI"] as Kullanıcılar);
            var data = (from u in db.Urunler
                        join s in db.Sepet on u.UrunID equals s.UrunID
                        where user.KullanıcıID == s.KullanıcıID && s.SatınAlındı == 0
                        select new { s, u, }).ToList();
            List<Sepet> sList = new List<Sepet>();
            foreach (var item in data)
            {
                Sepet sepet = new Sepet();
                sepet.SepetID = item.s.SepetID;
                sepet.SatınAlındı = item.s.SatınAlındı;
                sepet.Miktar = item.s.Miktar;
                sepet.KullanıcıID = item.s.KullanıcıID;
                sepet.EklenmeTarihi = item.s.EklenmeTarihi;
                sepet.UrunID = item.s.UrunID;
                sList.Add(sepet);
            }

            List<Urunler> uListe = new List<Urunler>();
            foreach (var item in data)
            {
                Urunler product = new Urunler();
                product.ProductPicture = item.u.ProductPicture;
                product.UrunID = item.u.UrunID;
                product.ProductName = item.u.ProductName;
                product.ProductCode = item.u.ProductCode;
                product.ProductPrice = item.u.ProductPrice;
                product.Currency = item.u.Currency;
                product.OrderedQuantity = item.s.Miktar;
                uListe.Add(product);
            }

            if (sList.Count == 0)
            {
                return Content("<script>alert('Sepette satın alınacak ürün bulunmamaktadır.');window.location='/Home/Index';</script>", "text/html");
            }

            Urunler tmp;
            Sepet tmpSepet;
            foreach (Sepet item in sList)
            {
                tmpSepet = db.Sepet.Find(item.SepetID);
                tmpSepet.SatınAlındı = 1;
                tmp = db.Urunler.Find(item.UrunID);
                if (tmp.Quantity > tmp.OrderedQuantity)
                {
                    tmp.Quantity = tmp.Quantity - Convert.ToInt32(tmp.OrderedQuantity);
                   
                }
                else
                {
                    return Content("<script>alert('Almak Istediğiniz Miktarda Stokta Kalmamıştır.');window.location='/Sepet/Index';</script>", "text/html");
                }


               
            }
            db.SaveChanges();

            var body = new StringBuilder();
            body.Append("Gönderenin Adı:" + user.Username);
            foreach (var item in data)
            {
                if (ModelState.IsValid)
                {
                    body.Append("\nUrun Adı:" + item.u.ProductName);
                    body.Append("\nSiparis Miktarı:" + item.s.Miktar);
                    body.Append("\nUrun Fiyatı:" + item.u.ProductPrice);
                    ViewBag.Success = true;
                }
            }

            gmail.SendMail(body.ToString());
            return Content("<script>alert('Ürünler satın alındı.');window.location='/Sepet/Index';</script>", "text/html");
        }

    }
}