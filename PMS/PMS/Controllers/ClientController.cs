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

        ////////////////////////////////////// Home page and profile  //////////////////////////////////
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
        /////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Edit Profile  ////////////////////////////////////////
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
        /////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Create Order   //////////////////////////////////
        public ActionResult CreateOrder()
        {
            ViewBag.Item_id = new SelectList(db.items, "Id", "Name");
            return View();
        }


        [HttpPost]
        public ActionResult CreateOrder(Order order)
        {
            order.Client_id = (int)Session["BranchId"];
            var item = db.items.Find(order.Item_id);
            var ordercheck = (from ClientList in db.Orders
                              where ClientList.Item_id == order.Item_id && ClientList.Client_id == order.Client_id
                              select new
                              {
                                  ClientList.Id
                              });


            if (ordercheck.FirstOrDefault() == null)
            {
                if (item.Quentity >= order.Quentity)
                {
                    if (item.Discount.Equals(null))
                    {
                        order.Total_Price = item.Price * order.Quentity;
                    }
                    else
                        order.Total_Price = (item.Price * order.Quentity) - ((double)item.Discount * order.Quentity);
                    item.Quentity = item.Quentity - order.Quentity;
                    if (ModelState.IsValid)
                    {
                        db.Entry(item).State = EntityState.Modified;
                        db.Orders.Add(order);
                        db.SaveChanges();
                        return RedirectToAction("MyProfile");
                    }
                    ViewBag.Client_id = new SelectList(db.Clients, "Id", "Name", order.Client_id);
                    ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                    return View(order);
                }
                else
                {
                    ViewBag.Error = "Max Quentity = " + item.Quentity.ToString();
                    ViewBag.Client_id = new SelectList(db.Clients, "Id", "Name", order.Client_id);
                    ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                    return View(order);
                }

            }
            else
            {
                ViewBag.msg = "This client has this product in his order";
                ViewBag.Client_id = new SelectList(db.Clients, "Id", "Name", order.Client_id);
                ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                return View(order);
            }


        }
        /////////////////////////////////////////////////////////////////////////////////////////

    }
}