using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FreelanceTimeTracker.Models;

namespace FreelanceTimeTracker.Controllers
{
    public class ProjectServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectServices
        public ActionResult Index()
        {
            var projectServices = db.ProjectServices.Include(p => p.Project).Include(p => p.Service);
            return View(projectServices.ToList());
        }

        // GET: ProjectServices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectService projectService = db.ProjectServices.Find(id);
            if (projectService == null)
            {
                return HttpNotFound();
            }
            return View(projectService);
        }

        // GET: ProjectServices/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectID", "ProjectName");
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceiD", "ServiceOwner");
            return View();
        }

        // POST: ProjectServices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,ServiceID,HoursWorked")] ProjectService projectService)
        {
            if (ModelState.IsValid)
            {
                db.ProjectServices.Add(projectService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectID", "ProjectName", projectService.ProjectId);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceiD", "ServiceOwner", projectService.ServiceID);
            return View(projectService);
        }

        // GET: ProjectServices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectService projectService = db.ProjectServices.Find(id);
            if (projectService == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectID", "ProjectName", projectService.ProjectId);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceiD", "ServiceOwner", projectService.ServiceID);
            return View(projectService);
        }

        // POST: ProjectServices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectId,ServiceID,HoursWorked")] ProjectService projectService)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "ProjectID", "ProjectName", projectService.ProjectId);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceiD", "ServiceOwner", projectService.ServiceID);
            return View(projectService);
        }

        // GET: ProjectServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectService projectService = db.ProjectServices.Find(id);
            if (projectService == null)
            {
                return HttpNotFound();
            }
            return View(projectService);
        }

        // POST: ProjectServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectService projectService = db.ProjectServices.Find(id);
            db.ProjectServices.Remove(projectService);
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
