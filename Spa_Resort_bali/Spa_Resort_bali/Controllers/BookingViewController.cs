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
    [HandleError]
    [Authorize]
    public class BookingViewController : Controller
    {
        private ApplicationUserManager _userManager;

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

        private Spa_Resort_Bali_DatabaseEntities db = new Spa_Resort_Bali_DatabaseEntities();

        // GET: BookingView
        public ActionResult Index(string message)
        {
            string userid = User.Identity.GetUserId();
            List<Bookings> bookings = db.Bookings.Where(d => d.UserId == userid).ToList();

            ViewBag.bookings = bookings;
            ViewBag.Message = Session["Message"];
            Session.Clear();

            return View();
        }

        // GET: BookingDetails
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.GetUserId() == booking.UserId)
            {
                return View(booking);
            }
            else if (User.IsInRole("Receptionist"))
            {
                return View(booking);
            }
            else if (User.IsInRole("Manager"))
            {
                return View(booking);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.GetUserId() != booking.UserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (booking.CheckInDate.AddDays(-7) < DateTime.Now)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            booking.Canceled = true;

            db.Entry(booking).State = EntityState.Modified;
            if (!User.IsInRole("Receptionist") && !User.IsInRole("Manager"))
            {
                UserManager.SendEmail(booking.UserId, string.Format("Booking #{0} has been Canceled", booking.BookingId),
   string.Format("Dear {0} {1}, We want to tell you that your booking with booking number #{2} has been Canceled by you.", booking.AspNetUsers.Firstname, booking.AspNetUsers.Lastname, booking.BookingId));
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Manager")]
        public ActionResult PrintBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bookings booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }
    }
}