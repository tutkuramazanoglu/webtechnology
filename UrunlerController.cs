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
    public class UrunlerController : Controller
    {
        private eTicaretEntities1 db = new eTicaretEntities1();

        // GET: Urunler
        public ActionResult Index()
        {
            var UrunListe = db.Urunler.ToList();
            return View(UrunListe);
        }
        public ActionResult UrunEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UrunEkle(Urunler Model)
        {
            if (ModelState.IsValid)
            {
                db.Urunler.Add(Model);
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Hata Olustu");
            }
            return View();
        }
        // GET: Urunler/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunler.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // GET: Urunler/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Kategoriler(string kategori)
        {
            var urun = (from u in db.Urunler
                        join k in db.Kategori on u.CategoryID equals k.ID
                        where k.CategoryName == kategori
                        select new { u, });
            List<Urunler> uList = new List<Urunler>();
            foreach (var item in urun)
            {
                Urunler product = new Urunler();
                product.UrunID = item.u.UrunID;
                product.ProductName = item.u.ProductName;
                product.ProductPicture = item.u.ProductPicture;
                product.ProductPrice = item.u.ProductPrice;
                product.Currency = item.u.Currency;
                uList.Add(product);
            }
            return View(uList);


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UrunID,UrunAdı,UrunKodu,UrunFıyat,ParaBirimi,UrunBırım,Mıktar")] Urunler urunler)
        {
            if (ModelState.IsValid)
            {
                db.Urunler.Add(urunler);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(urunler);
        }

        // GET: Urunler/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunler.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // POST: Urunler/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UrunID,UrunAdı,UrunKodu,UrunFıyat,ParaBirimi,UrunBırım,Mıktar")] Urunler urunler)
        {
            if (ModelState.IsValid)
            {
                db.Entry(urunler).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(urunler);
        }

        // GET: Urunler/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urunler urunler = db.Urunler.Find(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
        }

        // POST: Urunler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Urunler urunler = db.Urunler.Find(id);
            db.Urunler.Remove(urunler);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
