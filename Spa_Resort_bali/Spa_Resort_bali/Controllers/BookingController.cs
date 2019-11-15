using Microsoft.AspNet.Identity;
using Spa_Resort_bali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Spa_Resort_bali.Controllers
{
    [HandleError]
    [Authorize]
    public class BookingController : Controller
    {
        private Spa_Resort_Bali_DatabaseEntities db = new Spa_Resort_Bali_DatabaseEntities();

        public ActionResult Index()
        {
            //de if statement zorg er voor dat deze functie niet te gebruiken is als er niks in de sessions staat)

            if (Session["booking"] != null)
            {
                //haal de informatie van de booking uit de session
                BookingModels booking = (BookingModels)Session["booking"];

                //haal de lodge ID van de geselecteerde lodge uit de database
                LodgeTypes lodgeType = db.LodgeTypes.Where(table => table.Name == booking.LodgeTypes && table.Disabled == false).First();

                //kijk of de Lodgetype bestaat
                if (db.Lodges.Where(table => table.LodgeTypeId == lodgeType.LodgeTypeId).Count() != 0)
                {
                    //slecteer alle lodges van de geselecteerde lodtype uit de database
                    List<Lodges> lodge = db.Lodges.Where(table => table.LodgeTypeId == lodgeType.LodgeTypeId && table.Disabled == false).ToList();

                    foreach (Lodges item in lodge)
                    {
                        //kijk voor elke lodge van het geselecteerde lodge type of hij beschikbaar is
                        if (db.Bookings.Where(table => table.LodgeId == item.LodgeId && (table.CheckInDate < booking.CheckOutDate) && (booking.CheckInDate < table.CheckOutDate) && table.Canceled == false).Count() == 0)
                        {
                            //maak de booking
                            DateTime? date = booking.CheckInDate;
                            Bookings newBookings = new Bookings
                            {
                                //zet de opjecten over
                                CheckInDate = booking.CheckInDate,
                                CheckOutDate = booking.CheckOutDate,
                                Discount = (int)db.spGetDiscount(date).First(),
                                UserId = User.Identity.GetUserId(),
                                LodgeId = item.LodgeId,
                                Price = (decimal)db.spGetLodgePrice(date, item.Code).First()
                            };
                            //clear de session
                            Session.Clear();
                            db.Bookings.Add(newBookings);
                            db.SaveChanges();

                            //set een complete message in de session
                            Session["Message"] = "Your booking has been completed!";

                            //redirect to the booking view
                            return RedirectToAction("index", "bookingview"); //testing should redirect to "Booking sucseeded" page
                        }
                    }
                }
            }
            Session.Clear();
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); ;
        }
    }
}