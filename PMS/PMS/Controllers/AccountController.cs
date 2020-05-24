using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class AccountController : Controller
    {
        private Pharmacy db = new Pharmacy();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            if (admin.Admin_Name == "Admin" && admin.Password == "Admin")
            {
                return RedirectToAction("AllClient", "Admin");
            }
            ViewBag.Error = "*Invalid User Name Or Password";
            return View(admin);
        }
    }
}