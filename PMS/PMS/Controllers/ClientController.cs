using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class ClientController : Controller
    {
        private Pharmacy db = new Pharmacy();
        int x = 1;

        public ActionResult HomePage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            x = client.Id;
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
        public ActionResult MyProfile()
        {

            Client client = db.Clients.Find(x);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
    }
}