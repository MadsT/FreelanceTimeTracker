using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FreelanceTimeTracker.Models;
using Microsoft.AspNet.Identity;

namespace FreelanceTimeTracker.Controllers
{
    public class ServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Services
        [Authorize]
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();

            var services = from s in db.Services select s;

            services = services.Where(s => s.ServiceOwner.Equals(userName));

            return View(services);
        }

        // GET: Services/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null || !service.ServiceOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // GET: Services/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ServiceiD,ServiceName,Price")] Service service)
        {
            service.ServiceOwner = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {
                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(service);
        }

        // GET: Services/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null || !service.ServiceOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ServiceiD,ServiceName,Price")] Service service)
        {
            var userName = User.Identity.GetUserName();
            if (!service.ServiceOwner.Equals(userName))
            {
                return View(service);
            }
            if (ModelState.IsValid)
            {
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(service);
        }

        // GET: Services/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null || !service.ServiceOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var userName = User.Identity.GetUserName();
            Service service = db.Services.Find(id);
            if (service.ServiceOwner.Equals(userName))
            {
                db.Services.Remove(service);
                db.SaveChanges();
            }
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
