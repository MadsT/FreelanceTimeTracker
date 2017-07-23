using FreelanceTimeTracker.Models;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FreelanceTimeTracker.Controllers
{
    public class ClientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Clients
        [Authorize]
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
           
            var clients = from c in db.Clients select c;

            clients = clients.Where(c => c.ClientOwner.Equals(userName));

            

            return View(clients);
        }

        // GET: Clients/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
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
            client.ClientOwner = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
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
            var userName = User.Identity.GetUserName();
            if(!client.ClientOwner.Equals(userName))
            {
                return View(client);
            }
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = User.Identity.GetUserName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
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
            var userName = User.Identity.GetUserName();
            Client client = db.Clients.Find(id);

            if (client.ClientOwner.Equals(userName))
            {
                db.Clients.Remove(client);
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
