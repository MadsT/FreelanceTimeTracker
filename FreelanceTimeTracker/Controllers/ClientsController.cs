using FreelanceTimeTracker.Models;
using Microsoft.AspNet.Identity;
using Moq;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace FreelanceTimeTracker.Controllers
{
    public class ClientsController : Controller
    {
        
        private IClientsRepository _repository;
        public Func<string> GetUserName;


        public ClientsController()
        {
            _repository = new ClientRepository(new ApplicationDbContext());
            GetUserName = () => User.Identity.GetUserName();
        }

        public ClientsController(IClientsRepository repository)
        {
            _repository = repository;
        }

        // GET: Clients
        [Authorize]
        public ActionResult Index()
        {
            var userName = GetUserName();
            List<Client> clients = _repository.GetClientsForUserName(userName);
            if(clients == null)
            {
                return View();
            }

            return View(clients.ToList());
        }

        // GET: Clients/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = _repository.GetClientById(id);
            if (client == null || !client.ClientOwner.Equals(userName))
            {
                return HttpNotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ClientID,ClientName,Address")] Client client)
        {
            client.ClientOwner = GetUserName();

            if (ModelState.IsValid)
            {
                _repository.AddClient(client);
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = _repository.GetClientById(id);
            if (client == null || !client.ClientOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ClientID,ClientName,Address,ClientOwner")] Client client)
        {
            var userName = GetUserName();
            if (!client.ClientOwner.Equals(userName))
            {
                return View(client);
            }
            if (ModelState.IsValid)
            {
                _repository.UpdateClient(client);
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = GetUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = _repository.GetClientById(id);
            if (client == null || !client.ClientOwner.Equals(userName))
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var userName = GetUserName();
            Client client = _repository.GetClientById(id);

            if (client.ClientOwner.Equals(userName))
            {
                _repository.DeleteClient(client);
            }
             return RedirectToAction("Index");
        }

        

        public static ClientsController CreateClientsControllerAs(string userName, IClientsRepository repository)
        {
            var mock = new Mock<ControllerContext>();
            var controller = new ClientsController(repository)
            {
                GetUserName = () => userName
            }   ;


            controller.ControllerContext = mock.Object;
            return controller;
        }
    }

    public interface IClientsRepository
    {
        List<Client> GetClientsForUserName(string userName);
        Client GetClientById(int? clientId);
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(Client client);
    }

    public class ClientRepository : IClientsRepository
    {
        private ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void DeleteClient(Client client)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }

        public Client GetClientById(int? clientId)
        {
         return _context.Clients.Find(clientId);
        }

        public List<Client> GetClientsForUserName(string userName)
        {
            var clients = from c in _context.Clients select c;

            return clients.Where(c => c.ClientOwner.Equals(userName)).ToList();
        }

        public void UpdateClient(Client client)
        {
            _context.Entry(client).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
