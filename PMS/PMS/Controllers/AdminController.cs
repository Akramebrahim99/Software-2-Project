using PMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PMS.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private PharmacyEntities db = new PharmacyEntities();
        ////////////////////////////////////// Show All Client//////////////////////////////////
        public ActionResult AllClient()
        {
            return View(db.Clients.ToList());
        }
        ////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Add Product//////////////////////////////////
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
        ////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Show All Product//////////////////////////////////
        public ActionResult AllProduct()
        {
            return View(db.items.ToList());
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Edit Product   //////////////////////////////////
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            item item = db.items.Find(id);
            TempData["imgPath"] = item.Image;
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(item item)
        {
            if (ModelState.IsValid)
            {
                if (item.ImageFile != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(item.ImageFile.FileName);
                    string Extention = Path.GetExtension(item.ImageFile.FileName);
                    FileName = FileName + DateTime.Now.ToString("yymmssfff") + Extention;
                    item.Image = "~/Image/" + FileName;
                    FileName = Path.Combine(Server.MapPath("~/Image/"), FileName);

                    if (Extention.ToLower() == ".jpg" || Extention.ToLower() == ".jpeg" || Extention.ToLower() == ".png")
                    {
                        item.ImageFile.SaveAs(FileName);
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                        string old_image = Request.MapPath(TempData["imgPath"].ToString());
                        if (System.IO.File.Exists(old_image))
                        {
                            System.IO.File.Delete(old_image);
                        }

                        return RedirectToAction("AllProduct");

                    }
                    else
                    {
                        ViewBag.msg = "Invaild File Type";
                    }

                }
                else
                {
                    item.Image = TempData["imgPath"].ToString();
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AllProduct");
                }
            }
            return View(item);
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Delete Product   //////////////////////////////////
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            item item = db.items.Find(id);
            String currentImg = Request.MapPath(item.Image);
            db.items.Remove(item);
            db.SaveChanges();
            if (System.IO.File.Exists(currentImg))
            {
                System.IO.File.Delete(currentImg);
            }
            return RedirectToAction("AllProduct");
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Create Order   //////////////////////////////////
        public ActionResult CreateOrder()
        {
            ViewBag.Client_id = new SelectList(db.Clients, "Id", "Name");
            ViewBag.Item_id = new SelectList(db.items, "Id", "Name");
            return View();
        }


        [HttpPost]
        public ActionResult CreateOrder(Order order)
        {
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
                        order.Total_Price = item.Price*order.Quentity;
                    }
                    else
                        order.Total_Price = (item.Price*order.Quentity) - ((double)item.Discount*order.Quentity);
                    item.Quentity = item.Quentity - order.Quentity;
                    if (ModelState.IsValid)
                    {
                        db.Entry(item).State = EntityState.Modified;
                        db.Orders.Add(order);
                        db.SaveChanges();
                        return RedirectToAction("AllProduct");
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

        ////////////////////////////////////// All Order  //////////////////////////////////
        public ActionResult AllForEdit()
        {
            var orders = db.Orders.Include(o => o.Client).Include(o => o.item);
            return View(orders.ToList());
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
            var item = db.items.Find(order.Item_id);
            var item1 = db.items.Find((int)Session["olditem"]);
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
        public ActionResult AllForConfirm()
        {
            var orders = db.Orders.Include(o => o.Client).Include(o => o.item);
            return View(orders.ToList());
        }
        /////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Confirm Order  //////////////////////////////////
        public ActionResult ConfirmingReceipt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("AllForConfirm");
        }
        /////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////// All Order  //////////////////////////////////
        public ActionResult AllForCancel()
        {
            var orders = db.Orders.Include(o => o.Client).Include(o => o.item);
            return View(orders.ToList());
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

    }
}