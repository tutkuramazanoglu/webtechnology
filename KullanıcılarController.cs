using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ecommerce.Models;

namespace Ecommerce.Controllers
{
    public class KullanıcılarController : Controller
    {
        private eTicaretEntities1 db = new eTicaretEntities1();
        ApplicationDbContext dbContext = new ApplicationDbContext();

        // GET: Kullanıcılar
        public ActionResult Gırıs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Gırıs(Kullanıcılar Model)
        {
            var kullanıcı = db.Kullanıcılar.FirstOrDefault(x => x.Username == Model.Username && x.Password == Model.Password);
            if (kullanıcı != null)
            {
                Session["KULLANICIADI"] = kullanıcı;
                Session["yetki"] = kullanıcı.Role;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı Adı veya Sifre Hatalı");
            }

           
            return View();
        }

        public ActionResult CIKIS()
        {
            Session["KULLANICIADI"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index(Kullanıcılar Model)
        {
            List<Kullanıcılar> kList = new List<Kullanıcılar>();
            Model = Session["KULLANICIADI"] as Kullanıcılar;
            kList.Add(Model);
            return View(kList);
        }

        public ActionResult UyeEkle()
        {

            return View();
        }

        [HttpPost]
        public ActionResult UyeEkle(Kullanıcılar Model)
        {
            var uyeler = db.Kullanıcılar.FirstOrDefault(x => x.IdentityNo == Model.IdentityNo);
            if (ModelState.IsValid)
            {
                if (uyeler==null)
                {
                    db.Kullanıcılar.Add(Model);
                    db.SaveChanges();
                   
                }
                else
                {
                    return Content("<script>alert('Böyle Bir Uye Zaten Bulunmaktadır.');window.location='/Kullanıcılar/Gırıs';</script>", "text/html");
                }

                }
            else
            {
                ModelState.AddModelError("", "Hata Olustu!!");
            }

            return Content("<script>alert('Uyelik Işlaminiz Başarıyla Gerçekleşti.');window.location='/Kullanıcılar/Gırıs';</script>", "text/html");
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
            return View();
        }

        public ActionResult Sil(int id)
        {
            return View(db.Kullanıcılar.Find(id));
        }

        [HttpPost, ActionName("Sil")]
        public ActionResult Sil_uye(int id)
        {
            Kullanıcılar uye = db.Kullanıcılar.Find(id);
            db.Kullanıcılar.Remove(uye);
            db.SaveChanges();
            Session["KULLANICIADI"] = null;
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KullanıcıID,KullanıcıAdı,Sifre,KullanıcıTC,Adres")] Kullanıcılar kullanıcılar)
        {
            if (ModelState.IsValid)
            {
                db.Kullanıcılar.Add(kullanıcılar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kullanıcılar);
        }

        // GET: Kullanıcılar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanıcılar kullanıcılar = db.Kullanıcılar.Find(id);
            if (kullanıcılar == null)
            {
                return HttpNotFound();
            }
            return View(kullanıcılar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KullanıcıID,KullanıcıAdı,Sifre,KullanıcıTC,Adres")] Kullanıcılar kullanıcılar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kullanıcılar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kullanıcılar);
        }


    }
}
