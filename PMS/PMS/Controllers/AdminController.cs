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

    }
}