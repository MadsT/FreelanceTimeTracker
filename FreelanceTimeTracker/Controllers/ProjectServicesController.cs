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
        [Authorize]
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();

            var projectServices = db.Projects
                                    .Where(p => p.Client.ClientOwner.Equals(userName))
                                    .SelectMany(p => p.ProjectServices.Select(ps => ps)).ToList();
            List<ProjectServiceViewModel> projectServiceVMList = new List<ProjectServiceViewModel>(projectServices.Count());


            foreach(ProjectService ps in projectServices)
            {
                ProjectServiceViewModel psvm = new ProjectServiceViewModel();
                psvm.ProjectId = ps.ProjectId;
                psvm.ServiceID = ps.ServiceID;
                psvm.Project = ps.Project;
                psvm.Service = ps.Service;
                psvm.HoursWorked = ps.HoursWorked;
                psvm.TotalPrice = ps.HoursWorked * psvm.Service.Price;

                projectServiceVMList.Add(psvm);
            }

            return View(projectServiceVMList);
        }

        // GET: ProjectServices/Create
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public ActionResult Edit(int? ProjectId, int? ServiceID)
        {
            if (ProjectId == null || ServiceID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectService projectService = GetProjectService(ProjectId, ServiceID);
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
        [Authorize]
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
        [Authorize]
        public ActionResult Delete(int? ProjectId, int? ServiceID)
        {
            if (ProjectId == null || ServiceID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectService projectService = GetProjectService(ProjectId, ServiceID);
            if (projectService == null)
            {
                return HttpNotFound();
            }
            return View(projectService);
        }

        // POST: ProjectServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectService projectService = db.ProjectServices.Find(id);
            db.ProjectServices.Remove(projectService);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
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

        private ProjectService GetProjectService(int? ProjectId, int? ServiceID)
        {
            if(ProjectId == null || ServiceID == null)
            {
                return null;
            }
           return db.ProjectServices.Where(ps => ps.ProjectId == ProjectId && ps.ServiceID == ServiceID).ToList()[0];
        }
    }
}
