using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult HomePage()
        {
            return View();
        }
    }
}