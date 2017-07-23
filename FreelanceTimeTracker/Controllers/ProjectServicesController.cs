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
using Newtonsoft.Json;

namespace FreelanceTimeTracker.Controllers
{
    public class ProjectServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectServices
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();

            var projectServices = db.Projects
                                    .Where(p => p.Client.ClientOwner.Equals(userName))
                                    .SelectMany(p => p.ProjectServices.Select(ps => ps));

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
            var userName = User.Identity.GetUserName();
            ViewBag.ClientName = new SelectList(db.Clients.Where(c => c.ClientOwner.Equals(userName)), "ClientId", "ClientName");
            ViewBag.ServiceID = new SelectList(db.Services.Where(s => s.ServiceOwner.Equals(userName)), "ServiceiD", "ServiceName");
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

            //ViewBag.ProjectId = new SelectList(db.Projects, "ProjectID", "ProjectName", projectService.ProjectId);
            //ViewBag.ServiceID = new SelectList(db.Services, "ServiceiD", "ServiceOwner", projectService.ServiceID);
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


        public ActionResult PopulateProjectListInCreate(int selectedClientId)
        {
            var _jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            var projects = db.Projects.Where(p => p.Client.ClientID == selectedClientId).ToList();
            var  jsonProjects = JsonConvert.SerializeObject(projects, Formatting.Indented, _jsonSettings);


            return Json(jsonProjects, JsonRequestBehavior.AllowGet);
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
