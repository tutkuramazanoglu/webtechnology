using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Ecommerce.Controllers
{

    public class AdminController : Controller
    {
        private eTicaretEntities1 db = new eTicaretEntities1();
        // GET: Admin
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(string id, string pass)
        {
            var deger = db.Kullanıcılar.Where(x => x.Username == id).FirstOrDefault();
            Session["yetki"] = deger.Role;
            if (Session["yetki"] as string == "yönetici")
            {
                return RedirectToAction("Admin", "Admin");
            }
            else
            {
                return Content("<script>alert('Buraya Girmeye Yetkiniz Bulunmamaktadır.');window.location='/Home/Index';</script>", "text/html");
            }
        }
        public ActionResult Admin()
        {
            List<object> model = new List<object>();
            model.Add(db.Urunler.OrderByDescending(i => i.UrunID).Take(10).ToList());
            model.Add(db.Kullanıcılar.OrderByDescending(i => i.KullanıcıID).Take(10).ToList());
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Urunler urun, HttpPostedFileBase file)
        {
            var yenıUrun = db.Urunler.FirstOrDefault(x => x.ProductName == urun.ProductName);
            if (yenıUrun == null)
            {
                if (file != null && file.ContentLength > 0)
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/resimler"), Path.GetFileName(file.FileName));
                        string dbPath = "/resimler/" + file.FileName;
                        file.SaveAs(path);
                        urun.ProductPicture = dbPath;

                        Kategori kategori = db.Kategori.Where(x => x.ID == urun.CategoryID).First();
                        urun.KategoriAdi = kategori.CategoryName;
                        db.Urunler.Add(urun);
                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
            }
            else
            {
                return Content("<script>alert('Böyle Bir Urun Bulunmaktadır.');window.location='/Admin/Admin';</script>", "text/html");
            }

            return View();
        }

        public ActionResult Create()
        {

            return View();
        }

        public ActionResult Edit(int id)
        {
            var duzenlenen = db.Urunler.Where(x => x.UrunID == id).FirstOrDefault();
            return View(duzenlenen);
        }

        [HttpPost]
        public ActionResult Edit(Urunler urun)
        {
            db.Entry(urun).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Admin");
        }

        public ActionResult Sil(int id)
        {
            return View(db.Urunler.Find(id));
        }

        [HttpPost, ActionName("Sil")]
        public ActionResult Sil_uye(int id)
        {
            Urunler urun = db.Urunler.Find(id);
            db.Urunler.Remove(urun);
            db.SaveChanges();
            return RedirectToAction("Admin");

        }

        public ActionResult Guncelle(int id)
        {
            var duzenlenen = db.Kullanıcılar.Where(x => x.KullanıcıID == id).FirstOrDefault();
            return View(duzenlenen);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Guncelle(Kullanıcılar uye)
        {
            db.Entry(uye).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Admin");
        }

        public ActionResult Cıkıs()
        {
            Session["KULLANICIADI"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ArızaListe()
        {
            var aList = db.Arıza.ToList();
            return View(aList);
        }

        public ActionResult Rapor()
        {
            List<object> rList = new List<object>();
            
            var kullanıcı = (from k in db.Kullanıcılar
                             join s in db.Sepet
                             on k.KullanıcıID equals s.KullanıcıID
                             where s.SatınAlındı == 1
                             group s by k.Username into g
                             let toplamurun = g.Sum(m => m.Miktar)
                             orderby toplamurun descending
                             select new
                             {
                                 KullanıcıAdı = g.Key,
                                 SatilanUrunSayisi = g.Sum(i => i.Miktar)
                             }).First().ToString();
            rList.Add(kullanıcı);

            var urun = (from s in db.Sepet  join u
                            in db.Urunler on s.UrunID equals u.UrunID 
                            where s.SatınAlındı == 1
                            group s by u.ProductName into g
                            let toplamsatıs = g.Sum(t => t.Miktar)
                            orderby toplamsatıs descending
                            select new { urunAdı = g.Key, toplamsatıs}).First().ToString();
            rList.Add(urun);

            var urunSayi = (from s in db.Sepet   
                                join u in db.Urunler on s.UrunID equals u.UrunID
                                where s.SatınAlındı == 1 group s by u.ProductName into g
                                let urunadeti=g.Sum(d=>d.Miktar)
                                orderby urunadeti descending
                                select new { urunAdı=g.Key,urunadeti}).ToList();
            string sUrun = "";
            foreach (var item in urunSayi)
            {
                sUrun += "/"+item.urunAdı.ToString() + "+" + item.urunadeti.ToString() + ",";
            }
            rList.Add(sUrun);

            var ortSatis = (from u in db.Urunler 
                                join s in db.Sepet on u.UrunID equals s.UrunID
                                join k in db.Kullanıcılar on s.KullanıcıID equals k.KullanıcıID
                                group u by k.Username  into grp
                                let ortalama = grp.Sum(t => t.ProductPrice)
                                orderby ortalama descending
                                select new {kullanıcııd = grp.Key, ortalama}).ToList();
            string satıs = "";
            foreach (var item in ortSatis)
            {
                satıs += "/" + item.kullanıcııd.ToString() + "+" + item.ortalama.ToString()+ "*";
            }
            rList.Add(satıs);

            var satinAlinmayan = (from s in db.Sepet
                                       join k in db.Kullanıcılar on s.KullanıcıID equals k.KullanıcıID
                                       where s.SatınAlındı==0 group s by k.Username into g
                                       let sayi=g.Sum(t=>t.Miktar)
                                       select new { ad=g.Key, sayi}).ToList();
            string alinmayan = "";
            foreach (var item in satinAlinmayan)
            {
                alinmayan += "/" + item.ad.ToString() + "+" + item.sayi.ToString() + ",";
            }
            rList.Add(alinmayan);
            return View(rList);
           
        }

    }
}