using PMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
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


        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Addproduct(item item)
        {
            if (item.ImageFile == null)
            {
                ViewBag.error = "*Required";
            }
            else
            {
                string FileName = Path.GetFileNameWithoutExtension(item.ImageFile.FileName);
                string Extention = Path.GetExtension(item.ImageFile.FileName);
                FileName = FileName + DateTime.Now.ToString("yymmssfff") + Extention;
                item.Image = "~/Image/" + FileName;
                FileName = Path.Combine(Server.MapPath("~/Image/"), FileName);


                if (Extention.ToLower() == ".jpg" || Extention.ToLower() == ".jpeg" || Extention.ToLower() == ".png")
                {
                    if (ModelState.IsValid)
                    {
                        item.ImageFile.SaveAs(FileName);
                        db.items.Add(item);
                        db.SaveChanges();
                        return RedirectToAction("AllProduct");
                    }
                }
                else
                {
                    ViewBag.msg = "Invaild File Type";
                }

            }
            return View(item);
        }

        public ActionResult AllProduct()
        {
            return View(db.items.ToList());
        }

    }
}