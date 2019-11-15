using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Spa_Resort_bali.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Spa_Resort_bali.Controllers
{
    [Authorize(Roles = "Manager")]
    [HandleError]
    public class ManagerController : Controller
    {
        private ApplicationUserManager _userManager;
        private Spa_Resort_Bali_DatabaseEntities db = new Spa_Resort_Bali_DatabaseEntities();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Lodge Types
        public ActionResult LodgeType()
        {
            //get all the lodgetypes from the database and send them to the view
            ViewBag.LodgeTypes = db.LodgeTypes.ToList();
            return View();
        }

        public ActionResult UserOverview()
        {
            return View();
        }

        public ActionResult editlodge(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Lodges lodge = db.Lodges.Find(id);

            Addresses adres = db.Addresses.Find(lodge.AdresId);

            AddLodgeViewModels addLodge = new AddLodgeViewModels
            {
                City = adres.City,
                Streed = adres.Streed,
                PostCode = adres.PostCode,
                HouseNumber = adres.HouseNumber,
                Counrty = adres.Counrty,
                State = adres.State,
                Code = lodge.Code,
                LodgeID = lodge.LodgeId,
                AddresID = lodge.AdresId,
                LodgeTypeId = lodge.LodgeTypeId,
                Disabled = lodge.Disabled
                
            };

            return View(addLodge);
        }

        public ActionResult editlodgeapply(AddLodgeViewModels model)
        {
            ViewBag.lodges = db.Lodges.ToList();
            if (ModelState.IsValid)
            {
                Addresses oldadress = db.Addresses.Find(model.AddresID);

                oldadress.HouseNumber = model.HouseNumber;
                oldadress.PostCode = model.PostCode;
                oldadress.Streed = model.Streed;

                Lodges oldLodge = db.Lodges.Find(model.LodgeID);

                oldLodge.Disabled = model.Disabled;
                db.Entry(oldLodge).State = EntityState.Modified;
                db.Entry(oldadress).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("lodge");
        }

        public ActionResult Addlodges()
        {
            ViewBag.LodgeTypes = db.LodgeTypes.ToList();
            return View();
        }

        public ActionResult Addlodgessubmit(AddLodgeViewModels model, string LodgeTypes)
        {
            if (ModelState.IsValid)
            {
                var adress = new Addresses { PostCode = model.PostCode, Streed = model.Streed, HouseNumber = model.HouseNumber, City = "Ubud", Counrty = "Bali", State = "Bali" };

                db.Addresses.Add(adress);
                db.SaveChanges();

                Lodges lodges = new Lodges
                {
                    Code = model.Code,
                    AdresId = (from n in db.Addresses orderby n.AdresId descending select n.AdresId).First(),
                    LodgeTypeId = Convert.ToInt32(LodgeTypes)
                };
                db.Lodges.Add(lodges);
                db.SaveChanges();
            }
            return RedirectToAction("lodge");
        }

        public ActionResult Lodge()
        {
            ViewBag.lodges = db.Lodges.ToList();
            return View();
        }

        public ActionResult LodgeTypeDetails(int? id)
        {
            //look if the logetype id is set
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get the loge type details from the database
            LodgeTypes lodgeTypes = db.LodgeTypes.Find(id);

            //look if the lodgetype can be found in the database
            if (lodgeTypes == null)
            {
                return HttpNotFound();
            }

            //return the logetype details to the view
            return View(lodgeTypes);
        }

        public ActionResult AddLodgeType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLodgeTypeSubmit(LodgeTypes model, HttpPostedFileBase upload, decimal price)
        {
            if (ModelState.IsValid)
            {
                LodgeTypes lodgeTypes = new LodgeTypes
                {
                    Name = model.Name,
                    Surface = model.Surface,
                    Description = model.Description,
                    MaxPersons = model.MaxPersons
                };
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        lodgeTypes.Picture = reader.ReadBytes(upload.ContentLength);
                    }
                }

                db.LodgeTypes.Add(lodgeTypes);
                db.SaveChanges();

                LodgePrice lodgePrice = new LodgePrice
                {
                    StartingDate = DateTime.Now,
                    price = Convert.ToDecimal(price),
                    LodgeTypeId = (from n in db.LodgeTypes orderby n.LodgeTypeId descending select n.LodgeTypeId).First()
                };
                db.LodgePrice.Add(lodgePrice);
                db.SaveChanges();

                return RedirectToAction("LodgeType");
            }
            return RedirectToAction("AddLodgeType");
        }

        public ActionResult EditLodgeType(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get the loge type details from the database
            LodgeTypes lodgeTypes = db.LodgeTypes.Find(id);

            //look if the lodgetype can be found in the database
            if (lodgeTypes == null)
            {
                return HttpNotFound();
            }

            //return the logetype details to the view
            return View(lodgeTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLodgeTypeSubmit([Bind(Include = "LodgeTypeId,Name,Surface,Description,MaxPersons,Picture, price, Disabled")]LodgeTypes model, HttpPostedFileBase upload, decimal? price)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using var reader = new System.IO.BinaryReader(upload.InputStream);
                    model.Picture = reader.ReadBytes(upload.ContentLength);
                }

                if (price != null)
                {
                    LodgePrice oldlodgeprice = db.LodgePrice.Where(t => t.LodgeTypeId == model.LodgeTypeId && t.EndDate == null).First();

                    oldlodgeprice.EndDate = DateTime.Now;
                    db.Entry(oldlodgeprice).State = EntityState.Modified;

                    LodgePrice lodgePrice = new LodgePrice
                    {
                        StartingDate = DateTime.Now,
                        price = (decimal)price,
                        LodgeTypeId = model.LodgeTypeId
                    };

                    db.LodgePrice.Add(lodgePrice);
                }

                db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("LodgeType");
            }
            return RedirectToAction("EditLodgeType", model.LodgeTypeId);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeDiscounts()
        {
            List<Discounts> discounts = db.Discounts.Where(t => t.Year >= DateTime.Now.Year).ToList();
            ViewBag.discountlist = discounts;
            return View();
        }

        public ActionResult ChangeDiscountsEdit(int? year, int? month)
        {
            if (year == null && month == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Discounts discount = db.Discounts.Where(t => t.Year == year && t.Month == month).First();
            if (discount == null)
            {
                return HttpNotFound();
            }
            return View(discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeDiscountEditApply(Discounts model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ChangeDiscounts");
            }
            return RedirectToAction("ChangeDiscountsEdit", new
            {
                year = model.Year,
                month = model.Month
            }
                );
        }

        public ActionResult Adddiscount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDiscountApply(Discounts model)
        {
            if (ModelState.IsValid)
            {
                //look if the discount already exists
                if (db.Discounts.Where(t => t.Month == model.Month && t.Year == model.Year).Count() == 0)
                {
                    db.Discounts.Add(model);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.message = string.Format("A discount for {0} / {1} already exists", model.Year, model.Month);
                    return View("Adddiscount");
                }

            }
            return RedirectToAction("ChangeDiscounts");
        }

        public ActionResult Printinvoice(string message)
        {
            ViewBag.message = message;
            return View();
        }

        public ActionResult PrintinvoicesPrint(int? year, int? month)
        {
            if (year == null && month == null)
            {
                return RedirectToAction("Printinvoice", new {message = "You didnt put in any info!" });
            }
            List<LodgeTypes> lodgeTypes = db.LodgeTypes.ToList();
            if (month == null)
            {
                List<Bookings> bookings = db.Bookings.Where(t => t.CheckInDate.Year == year && t.Canceled == false).ToList();
                if (bookings.Count > 0)
                {
                    ViewBag.lodgetypes = lodgeTypes;
                    ViewBag.bookings = bookings;
                    ViewBag.year = year;
                    return View();
                }
                else
                {
                    return RedirectToAction("Printinvoice", new { message = "There are no bookings in this year!" });
                }
            }
            else if (month != null && year != null)
            {
                List<Bookings> bookings = db.Bookings.Where(t => t.CheckInDate.Year == year && t.CheckInDate.Month == month && t.Canceled == false).ToList();

                if (bookings.Count > 0)
                {
                    ViewBag.bookings = bookings;
                    ViewBag.lodgetypes = lodgeTypes;
                    ViewBag.month = month;
                    ViewBag.year = year;
                    return View();
                }
                else
                {
                    return RedirectToAction("Printinvoice", new { message = "There are no bookings in this year or month!" });
                }
            }
            return RedirectToAction("Printinvoice", new { message = "You didnt put in any info!" });
        }

        // U S E R    S T U F F
        //displays all users
        public ActionResult Users(string message = "", string messageColor = "primary")
        {
            //If a message is given, it will display it in the view
            ViewBag.profileMessage = message;
            ViewBag.profileMessageColor = messageColor;

            //the model of the view will be AspNetUsers with the Addresses and AspNetRoles included
            var aspNetUsers = db.AspNetUsers.Include(a => a.Addresses).Include(a => a.AspNetRoles);
            return View(aspNetUsers.ToList());
        }

        //displays the details of the user with the given id
        public ActionResult DetailsUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        //makes the manager able to change the properties of the user with the given id
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdressId = new SelectList(db.Addresses, "AdresId", "Streed", aspNetUsers.AdressId);
            ViewBag.RoleId = new SelectList(db.AspNetRoles, "Id", "Name", aspNetUsers.AspNetRoles.First().Id);

            return View(aspNetUsers);
        }

        //here, the things changed in the view from EditUser() will be updated to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "Id,Email,EmailConfirmed,AdressId,Firstname,Lastname,DateOfBirth")] AspNetUsers aspNetUsers, string RoleId)
        {
            if (ModelState.IsValid)
            {
                AspNetUsers oldAspNetUsers = db.AspNetUsers.Find(aspNetUsers.Id);

                //puts the data of the user with the given id in an ApplicationUser and changes it with the edited data stored in aspNetUsers
                ApplicationUser user = UserManager.FindById(aspNetUsers.Id);
                user.Id = aspNetUsers.Id;
                user.Email = aspNetUsers.Email;
                user.EmailConfirmed = aspNetUsers.EmailConfirmed;
                user.UserName = aspNetUsers.Email;
                user.AdressId = aspNetUsers.AdressId;
                user.Firstname = aspNetUsers.Firstname;
                user.Lastname = aspNetUsers.Lastname;
                user.DateOfBirth = aspNetUsers.DateOfBirth;

                //removes the old role and ads the given one
                UserManager.RemoveFromRole(aspNetUsers.Id, oldAspNetUsers.AspNetRoles.First().Name);
                UserManager.AddToRole(aspNetUsers.Id, db.AspNetRoles.Find(RoleId).Name);

                //updates the user and sends a message to the users() view
                UserManager.UpdateAsync(user);
                return RedirectToAction("Users", new { message = $"Succesfully changed {aspNetUsers.UserName}.", messageColor = "success" });
            }
            //if aspNetUsers is invalid, it will go back to users with an error message and doesn't change anything in the database
            ViewBag.AdressId = new SelectList(db.Addresses, "AdresId", "PostCode", aspNetUsers.AdressId);
            return RedirectToAction("Users", new { message = $"Failed to changed {aspNetUsers.UserName}.", messageColor = "danger" });
        }

        //idk
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}