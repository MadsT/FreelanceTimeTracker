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
    public class ServicesController : Controller
    {
        private IServicesRepository _repository;
        public Func<string> GetUserName;


        public ServicesController()
        {
            _repository = new ServiceRepository(new ApplicationDbContext());
            GetUserName = () => User.Identity.GetUserName();
        }

        public ServicesController(IServicesRepository repository)
        {
            _repository = repository;
        }

        // GET: Services
        [Authorize]
        public ActionResult Index()
        {
            var userName = GetUserName();

            List<Service> services = _repository.GetServicesForUserName(userName);

            return View(services);
        }

        // GET: Services/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = _repository.GetServiceById(id);
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
            service.ServiceOwner = GetUserName();

            if (ModelState.IsValid)
            {
                _repository.AddService(service);
                return RedirectToAction("Index");
            }

            return View(service);
        }

        // GET: Services/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = _repository.GetServiceById(id);
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
        public ActionResult Edit([Bind(Include = "ServiceiD,ServiceName,Price,ServiceOwner")] Service service)
        {
            var userName = GetUserName();
            if (!service.ServiceOwner.Equals(userName))
            {
                return View(service);
            }
            if (ModelState.IsValid)
            {
                _repository.UpdateService(service);
                return RedirectToAction("Index");
            }
            return View(service);
        }

        // GET: Services/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = _repository.GetServiceById(id);
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
            var userName = GetUserName();
            Service service = _repository.GetServiceById(id);
            if (service.ServiceOwner.Equals(userName))
            {
                _repository.DeleteService(service);
            }
            return RedirectToAction("Index");
        }

        public static ServicesController CreateServicesControllerAs(string userName, IServicesRepository repository)
        {
            var mock = new Mock<ControllerContext>();
            var controller = new ServicesController(repository)
            {
                GetUserName = () => userName
            };


            controller.ControllerContext = mock.Object;
            return controller;
        }

        public interface IServicesRepository
        {
            List<Service> GetServicesForUserName(string userName);
            Service GetServiceById(int? clientId);
            void AddService(Service service);
            void UpdateService(Service service);
            void DeleteService(Service service);
        }

        public class ServiceRepository : IServicesRepository
        {
            private ApplicationDbContext _context;

            public ServiceRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public void AddService(Service service)
            {
                _context.Services.Add(service);
                _context.SaveChanges();
            }

            public void DeleteService(Service service)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }

            public List<Service> GetServicesForUserName(string userName)
            {

                var services = from s in _context.Services select s;

                return services.Where(s => s.ServiceOwner.Equals(userName)).ToList();
            }

            public Service GetServiceById(int? serviceId)
            {
                return _context.Services.Find(serviceId);
            }

            public void UpdateService(Service service)
            {
                _context.Entry(service).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}