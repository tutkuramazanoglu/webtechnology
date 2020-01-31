using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;
using System.Net.Mail;
using System.Data.Entity;
using System.Threading;
using System.Globalization;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private eTicaretEntities1 db = new eTicaretEntities1();
        List<Urunler> uListe = new List<Urunler>();
        public ActionResult Index()
        {
            var urunlerim = db.Urunler.Where(x => x.UrunID > 0);
            var urunler = (from u in db.Urunler
                           join k in db.Kampanya
                             on u.UrunID equals k.ProductID
                           where u.UrunID == k.ProductID
                           orderby u.UrunID
                           select new { u, k, });

            DateTime now = DateTime.Now;

            foreach (var item in urunlerim)
            {
                foreach (var item2 in urunler)
                {
                    int baslangic = DateTime.Compare(now, Convert.ToDateTime(item2.k.Baslangic));
                    int bitis = DateTime.Compare(now, Convert.ToDateTime(item2.k.Bitis));
                    if (item.UrunID == item2.u.UrunID && baslangic >= 1 && bitis < 0)
                    {
                        item.UrunID = item2.u.UrunID;
                        item.ProductPrice = item2.k.Sale;
                    }
                }
                uListe.Add(item);
            }
            return View(uListe);
        }

        public ActionResult AramaYap(string kelime)
        {
            var item = (from p in db.Urunler where (p.ProductCode.Contains(kelime) || p.ProductName.Contains(kelime) || p.KategoriAdi.Contains(kelime)) orderby p.UrunID descending select p);
            ViewBag.aranan = kelime;
            return View(item);
        }

        public ActionResult ArızaSikayet()
        {
            return View();
        }

        public ActionResult SikayetGonder(Urunler urun, Arıza arıza)
        {
            var user = Session["KULLANICIADI"] as Kullanıcılar;
            if (user == null)
            {
                return Content("<script>alert('Lutfen Uye Girisi Yapiniz.');window.location='/Kullanıcılar/Gırıs';</script>", "text/html");
            }
            else
            {
                var product = db.Urunler.Where(x => x.ProductName == urun.ProductName).FirstOrDefault();

                if (product == null)
                {
                    return Content("<script>alert('Lutfen Urun Adını Doğru Giriniz.');window.location='/Home/ArızaSikayet';</script>", "text/html");
                }
                else
                {
                    var sikayet = (from u in db.Urunler
                                   join s in db.Sepet on product.UrunID equals s.UrunID
                                   where user.KullanıcıID == s.KullanıcıID && s.SatınAlındı == 1
                                   select new { s, }).FirstOrDefault();
                    if (sikayet == null)
                    {
                        return Content("<script>alert('Lutfen Satin Aldiginiz Bir Urun Icin Bu Formu Doldurun.');window.location='/Home/Index';</script>", "text/html");
                    }

                    else
                    {
                        arıza.ProductID = product.UrunID;
                        arıza.UserID = user.KullanıcıID;
                        db.Arıza.Add(arıza);
                        db.SaveChanges();
                        return Content("<script>alert('Mesajiniz Başarıyla Gönderildi.');window.location='/Home/Index';</script>", "text/html");
                    }
                }

            }

        }

        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(lang);
            return Redirect(returnUrl);
        }

        public ActionResult Rate(Oylama rate)
        {
            var user = Session["KULLANICIADI"] as Kullanıcılar;
            var sAlındı = (from s in db.Sepet
                           where user.KullanıcıID == s.KullanıcıID && s.SatınAlındı == 1
                           && rate.ProductID==s.UrunID
                           select new { s, }
                          ).FirstOrDefault();

            var oKullan = (from k in db.Kullanıcılar
                           join o in db.Oylama on rate.ProductID
                           equals o.ProductID where user.KullanıcıID==o.UserID
                           select new {k,o,}).FirstOrDefault();
            if (user != null)
            {
                if (sAlındı != null)
                {
                    if (oKullan == null)
                    {
                        rate.UserID = user.KullanıcıID;
                        db.Oylama.Add(rate);
                        db.SaveChanges();


                        var ortalama = (from k in db.Kullanıcılar
                                        join o in db.Oylama
                                        on k.KullanıcıID equals o.UserID
                                        where o.ProductID == rate.ProductID
                                        group o by o.ProductID into g
                                        let ortSatis = g.Average(m => m.Score)
                                        select new
                                        {
                                            SatilanUrunSayisi = g.Average(i => i.Score)
                                        }).First().ToString();

                        ortalama = ortalama.Replace("{", "").Replace("}", "").Replace("SatilanUrunSayisi", "").Replace("=", "");
                        var result = db.Urunler.SingleOrDefault(b => b.UrunID == rate.ProductID);
                        if (result != null)
                        {
                            result.Score = ortalama;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //return Content("<script>alert('Bu Urünü Daha Once Oyladiginiz Icin Tekrar Oylayamazsiniz.');window.location='/Detay/DetayGoster';</script>", "text/html", new {id=rate.Score }); 
                        return RedirectToAction("DetayGoster", "Detay", new { id = rate.ProductID });
                    }
                }

                else
                {
                    //return Content("<script>alert('Satin Almadginiz Urun Icin Oylamaya Katilamazsiniz.');window.location='/Home/Index';</script>", "text/html");       
                    return RedirectToAction("DetayGoster", "Detay", new { id = rate.ProductID });
                }
            }
            else
            {
                return Content("<script>alert('Lutfen Uye Girisi Yapiniz.');window.location='/Kullanıcılar/Gırıs';</script>", "text/html");
            }
            //return PartialView("RateHomeView", rate);
            //return View("~/Views/Detay/DetayGoster.cshtml", urun);          
            return RedirectToAction("DetayGoster", "Detay", new { id = rate.ProductID });

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



    }
}
