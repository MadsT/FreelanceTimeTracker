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
using Moq;

namespace FreelanceTimeTracker.Controllers
{
    public class ProjectServicesController : Controller
    {
        private IProjectServiceRepository _repository;
        public Func<string> GetUserName;


        public ProjectServicesController()
        {
            _repository = new ProjectServiceRepository(new ApplicationDbContext());
            GetUserName = () => User.Identity.GetUserName();
        }

        public ProjectServicesController(IProjectServiceRepository repository)
        {
            _repository = repository;
        }

        // GET: ProjectServices
        [Authorize]
        public ActionResult Index()
        {
            var userName = GetUserName();
            List<ProjectService> projectServices = _repository.GetProjectServicesForUserName(userName);
            if(projectServices == null)
            {
                return View();
            }
            List<ProjectServiceViewModel> projectServiceVMList = new List<ProjectServiceViewModel>(projectServices.Count);

            foreach (ProjectService ps in projectServices)
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
            var userName = GetUserName();
            ViewBag.ClientName = new SelectList(_repository.GetClientsBelongingToUserName(userName), "ClientId", "ClientName");
            ViewBag.ServiceID = new SelectList(_repository.GetServicesBelongingToUserName(userName), "ServiceiD", "ServiceName");
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
                _repository.AddProjectService(projectService);   
                return RedirectToAction("Index");
            }

            
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
            ProjectService projectService = _repository.GetProjectServiceByIds(ProjectId, ServiceID);
            if (projectService == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(_repository.GetAllProjects(), "ProjectID", "ProjectName", projectService.ProjectId);
            ViewBag.ServiceID = new SelectList(_repository.GetAllServices(), "ServiceiD", "ServiceOwner", projectService.ServiceID);

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
                _repository.UpdateProjectService(projectService);
                return RedirectToAction("Index");
            }
             ViewBag.ProjectId = new SelectList(_repository.GetAllProjects(), "ProjectID", "ProjectName", projectService.ProjectId);
            ViewBag.ServiceID = new SelectList(_repository.GetAllServices(), "ServiceiD", "ServiceOwner", projectService.ServiceID);
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
            ProjectService projectService = _repository.GetProjectServiceByIds(ProjectId, ServiceID);
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
        public ActionResult DeleteConfirmed(int? ProjectId, int? ServiceID)
        {
            ProjectService projectService = _repository.GetProjectServiceByIds(ProjectId, ServiceID);
            _repository.DeleteProjectService(projectService);
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
            var projects = _repository.GetAllProjects().Where(p => p.Client.ClientID == selectedClientId).ToList();
            
            
            var jsonProjects = JsonConvert.SerializeObject(projects, Formatting.Indented, _jsonSettings);


            return Json(jsonProjects, JsonRequestBehavior.AllowGet);
        }

        public static ProjectServicesController CreateProjectServiceControllerAs(string userName, IProjectServiceRepository repository)
        {
            var mock = new Mock<ControllerContext>();
            var controller = new ProjectServicesController(repository)
            {
                GetUserName = () => userName
            };


            controller.ControllerContext = mock.Object;
            return controller;
        }
    }

    public interface IProjectServiceRepository
    {
        List<ProjectService> GetProjectServicesForUserName(string userName);
        ProjectService GetProjectServiceByIds(int? projectId, int? serviceId);
        void AddProjectService(ProjectService projectService);
        void UpdateProjectService(ProjectService projectService);
        void DeleteProjectService(ProjectService projectService);
        List<Client> GetClientsBelongingToUserName(string userName);
        List<Service> GetServicesBelongingToUserName(string userName);
        List<Project> GetAllProjects();
        List<Service> GetAllServices();

    }

    public class ProjectServiceRepository : IProjectServiceRepository
    {
        private ApplicationDbContext _context;

        public ProjectServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public void AddProjectService(ProjectService projectService)
        {
            _context.ProjectServices.Add(projectService);
            _context.SaveChanges();
        }

        public void DeleteProjectService(ProjectService projectService)
        {
            _context.ProjectServices.Remove(projectService);
            _context.SaveChanges();
        }

        public List<Project> GetAllProjects()
        {
            return _context.Projects.ToList();
        }

        public List<Service> GetAllServices()
        {
            return _context.Services.ToList();
        }

        public List<Client> GetClientsBelongingToUserName(string userName)
        {
            var clients = from c in _context.Clients select c;

            return clients.Where(c => c.ClientOwner.Equals(userName)).ToList();
        }

        public ProjectService GetProjectServiceByIds(int? projectId, int? serviceId)
        {
            if (projectId == null || serviceId == null)
            {
                return null;
            }
            return _context.ProjectServices.Where(ps => ps.ProjectId == projectId && ps.ServiceID == serviceId).ToList()[0];
        }

        public List<ProjectService> GetProjectServicesForUserName(string userName)
        {
            return _context.Projects.Where(p => p.Client.ClientOwner.Equals(userName))
                                                .SelectMany(p => p.ProjectServices.Select(ps => ps)).ToList();
        }

        public List<Service> GetServicesBelongingToUserName(string userName)
        {
            var services = from s in _context.Services select s;

            return services.Where(s => s.ServiceOwner.Equals(userName)).ToList();
        }

        public void UpdateProjectService(ProjectService projectService)
        {
            _context.Entry(projectService).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
