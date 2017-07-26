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
using Moq;

namespace FreelanceTimeTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private IProjectsRepository _repository;
        public Func<string> GetUserName;


        public ProjectsController()
        {
            _repository = new ProjectRepository(new ApplicationDbContext());
            GetUserName = () => User.Identity.GetUserName();
        }

        public ProjectsController(IProjectsRepository repository)
        {
            _repository = repository;
        }

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            var userName = GetUserName();

            List<Project> thisUsersProjects = _repository.GetProjectsForUserName(userName);

            return View(thisUsersProjects);
        }

        // GET: Projects/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = _repository.GetProjectById(id);
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
            var userName = GetUserName();
            var usersClients = _repository.GetClientsForUserName(userName);

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
                _repository.AddProject(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = _repository.GetProjectById(id);
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
            var userName = GetUserName();
            var dbClient = _repository.GetProjectById(project.ProjectID).Client;


            //var dbClient = db.Clients.Where(c => c.ClientID == project.ClientID).ToList()[0]; // There can only be one client with this ID.

            if (!dbClient.ClientOwner.Equals(userName))
            {
                return View(project);
            }
            project.Client = dbClient;
            if (ModelState.IsValid)
            {
                _repository.UpdateProject(project);
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = _repository.GetProjectById(id);
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
            var userName = GetUserName();

            Project project = _repository.GetProjectById(id);
            if (project.Client.ClientOwner.Equals(userName))
            {
                _repository.DeleteProject(project);
            }

            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetSelectedListItems(List<Client> elements)
        {
            var selectedList = new List<SelectListItem>();

            foreach (var element in elements)
            {
                selectedList.Add(new SelectListItem
                {
                    Value = element.ClientID.ToString(),
                    Text = element.ClientName
                });
            }

            return selectedList;
        }

        public static ProjectsController CreateProjectsControllerAs(string userName, IProjectsRepository repository)
        {
            var mock = new Mock<ControllerContext>();
            var controller = new ProjectsController(repository)
            {
                GetUserName = () => userName
            };


            controller.ControllerContext = mock.Object;
            return controller;
        }
    }

    public interface IProjectsRepository
    {
        List<Project> GetProjectsForUserName(string userName);
        Project GetProjectById(int? clientId);
        void AddProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(Project project);
        List<Client> GetClientsForUserName(string userName);
    }

    public class ProjectRepository : IProjectsRepository
    {
        private ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        public void DeleteProject(Project project)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
        }

        public Project GetProjectById(int? projectId)
        {
            return _context.Projects.Find(projectId);
        }

        public List<Project> GetProjectsForUserName(string userName)
        {
            var projects = from p in _context.Projects select p;
            return projects.Where(p => p.Client.ClientOwner.Equals(userName)).ToList();
        }

        public void UpdateProject(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public List<Client> GetClientsForUserName(string userName)
        {
            var clients = from c in _context.Clients select c;

            return clients.Where(c => c.ClientOwner.Equals(userName)).ToList();
        }
    }
}
