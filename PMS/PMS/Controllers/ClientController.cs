using PMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class ClientController : Controller
    {
        private PharmacyEntities db = new PharmacyEntities();
        public ActionResult HomePage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            var BranchId = id;
            Session["BranchId"] = BranchId;
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
        public ActionResult MyProfile()
        {

            Client client = db.Clients.Find((int)Session["BranchId"]);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        public ActionResult EditProfile()
        {

            Client client = db.Clients.Find((int)Session["BranchId"]);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        [HttpPost]
        public ActionResult EditProfile(Client client)
        {
            Client client5 = db.Clients.Find(client.Id);
            String username = client.User_Name.ToString();
            var client1 = (from ClientList in db.Clients
                           where ClientList.User_Name == client.User_Name
                           select new
                           {
                               ClientList.User_Name
                           });
            if ((client1.FirstOrDefault() == null || client1.FirstOrDefault().User_Name == client5.User_Name) && client.User_Name != "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(client5).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("HomePage", new { id = client.Id });
                }
            }
            else
            {
                ViewBag.error = "*invalid UserName";
            }
            return View(client);
        }
    }
}