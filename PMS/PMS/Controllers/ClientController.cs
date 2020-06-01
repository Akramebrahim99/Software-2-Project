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
                    ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                    return View(order);
                }
                else
                {
                    ViewBag.Error = "Max Quentity = " + item.Quentity.ToString();
                    ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                    return View(order);
                }

            }
            else
            {
                ViewBag.msg = "This client has this product in his order";
                ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                return View(order);
            }


        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// All Order  //////////////////////////////////
        public ActionResult AllForEdit()
        {
            int id = (int)Session["BranchId"];
            var order = db.Orders.Where(o => o.Client.Id == id);
            return View(order.ToList());
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Edit Order  //////////////////////////////////
        public ActionResult EditOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            Session["Quentity"] = order.Quentity;
            Session["olditem"] = order.Item_id;
            Session["oldclient"] = order.Client_id;
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Client_id = new SelectList(db.Clients, "Id", "Name", order.Client_id);
            ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
            return View(order);
        }

        [HttpPost]
        public ActionResult EditOrder(Order order)
        {
            order.Client_id = (int)Session["BranchId"];
            var item1 = db.items.Find((int)Session["olditem"]);
            var item = db.items.Find(order.Item_id);
            var ordercheck = (from OrderList in db.Orders
                              where OrderList.Item_id == order.Item_id && OrderList.Client_id == order.Client_id && OrderList.Quentity == order.Quentity
                              select new
                              {
                                  OrderList.Id
                              });


            if (ordercheck.FirstOrDefault() == null)
            {
                var ordercheck2 = (from OrderList in db.Orders
                                   where OrderList.Item_id == order.Item_id && OrderList.Client_id == order.Client_id
                                   select new
                                   {
                                       OrderList.Item_id,
                                       OrderList.Client_id,
                                       OrderList.Id
                                   });
                if (ordercheck2.FirstOrDefault() == null || (ordercheck2.FirstOrDefault().Item_id == (int)Session["olditem"] && ordercheck2.FirstOrDefault().Client_id == (int)Session["oldclient"]))
                {
                    if (item.Id == (int)Session["olditem"])
                    {
                        item.Quentity = item.Quentity + (int)Session["Quentity"];
                    }
                    else
                    {
                        item1.Quentity = item1.Quentity + (int)Session["Quentity"];
                        db.Entry(item1).State = EntityState.Modified;
                        db.SaveChanges();
                    }

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
                            db.Entry(order).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("AllForEdit");
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
            else
            {
                ViewBag.msg = "No thing to change";
                ViewBag.Client_id = new SelectList(db.Clients, "Id", "Name", order.Client_id);
                ViewBag.Item_id = new SelectList(db.items, "Id", "Name", order.Item_id);
                return View(order);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        
        ////////////////////////////////////// All Order  //////////////////////////////////
        public ActionResult AllForCancel()
        {
            int id = (int)Session["BranchId"];
            var order = db.Orders.Where(o => o.Client.Id == id);
            return View(order.ToList());
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Confirm Order  //////////////////////////////////
        public ActionResult CancelOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            order.Client_id = (int)Session["BranchId"];
            var item = db.items.Find(order.Item_id);
            item.Quentity = item.Quentity + order.Quentity;
            if (order == null)
            {
                return HttpNotFound();
            }
            db.Orders.Remove(order);
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AllForCancel");
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// All Order  //////////////////////////////////
        public ActionResult Allorder()
        {
            int id = (int)Session["BranchId"];
            var order = db.Orders.Where(o => o.Client.Id == id);
            return View(order.ToList());
        }
        /////////////////////////////////////////////////////////////////////////////////////////
    }
}