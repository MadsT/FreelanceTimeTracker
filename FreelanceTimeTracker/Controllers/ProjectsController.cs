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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();

            var projects = from p in db.Projects select p;
            var thisUsersProjects = projects.Where(p => p.Client.ClientOwner.Equals(userName));

            return View(thisUsersProjects);
        }

        // GET: Projects/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null || !project.Client.ClientOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public ActionResult Create()
        {
            var usersClients = GetUsersClients();

            var projectModel = new Project();

            projectModel.Clients = GetSelectedListItems(usersClients);

            return View(projectModel);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ProjectID,ProjectName,SelectedClient")] Project project)
        {

            project.ClientID = Convert.ToInt32(project.SelectedClient);

            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null || !project.Client.ClientOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ProjectID,ProjectName,ClientID")] Project project)
        {
            var userName = User.Identity.GetUserName();

            var dbClient = db.Clients.Where(c => c.ClientID == project.ClientID).ToList()[0]; // There can only be one client with this ID.

            if (!dbClient.ClientOwner.Equals(userName))
            {
                return View(project);
            }
            project.Client = dbClient;
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                //db.Projects.Attach(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null || !project.Client.ClientOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var userName = User.Identity.GetUserName();

            Project project = db.Projects.Find(id);
            if (project.Client.ClientOwner.Equals(userName))
            {
                db.Projects.Remove(project);
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

        private ICollection<Client> GetUsersClients()
        {
            var userName = User.Identity.GetUserName();

            var clientList = db.Clients.Where(c => c.ClientOwner.Equals(userName));

            return clientList.ToList();
        }

        private IEnumerable<SelectListItem> GetSelectedListItems(IEnumerable<Client> elements)
        {
            var selectedList = new List<SelectListItem>();

            foreach(var element in elements)
            {
                selectedList.Add(new SelectListItem
                {
                    Value = element.ClientID.ToString(),
                    Text = element.ClientName
                });
            }

            return selectedList;
        }
    }
}
