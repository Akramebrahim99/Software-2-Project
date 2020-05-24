using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private Pharmacy db = new Pharmacy();

        public ActionResult AllClient()
        {
            return View(db.Clients.ToList());
        }

    }
}