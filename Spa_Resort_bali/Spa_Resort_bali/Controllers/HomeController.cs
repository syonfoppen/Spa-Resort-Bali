using Spa_Resort_bali.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Spa_Resort_bali.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private Spa_Resort_Bali_DatabaseEntities db = new Spa_Resort_Bali_DatabaseEntities();

        public ActionResult Index()
        {
            //Create a list of lodgetypes from the database table LodgeTypes
            List<LodgeTypes> lodgeTypes = db.LodgeTypes.Where(t => t.Disabled == false).ToList();
            //create a viewbag for the list lodgetypes
            ViewBag.lodgetypes = lodgeTypes;
            return View();
        }

        // bookings is model en database
        public ActionResult Submitted(BookingModels booking)
        {
            //Create a list of lodgetypes from the database table LodgeTypes
            List<LodgeTypes> lodgeTypes = db.LodgeTypes.Where(t => t.Disabled == false).ToList();
            //create a viewbag for the list lodgetypes
            ViewBag.lodgetypes = lodgeTypes;

            //kijk of de booking checkindate nier eerder is dan de datum van vandaag
            if (booking.CheckInDate > DateTime.Now)
            {
                //kijk of de checkindate eerder is dan de checkout date
                if (booking.CheckInDate < booking.CheckOutDate)
                {
                    //kijk lodge de lodgetype bestaat
                    if (db.LodgeTypes.Where(table => table.Name == booking.LodgeTypes).Count() != 0)
                    {
                        //selecteer de lodgetype ID uit de databse
                        LodgeTypes lodgeType = db.LodgeTypes.Where(table => table.Name == booking.LodgeTypes).First();

                        //slecteer alle lodges van de geselecteerde lodtype uit de database
                        List<Lodges> lodge = db.Lodges.Where(table => table.LodgeTypeId == lodgeType.LodgeTypeId && table.Disabled == false).ToList();

                        foreach (Lodges item in lodge)
                        {
                            //kijk voor elke lodge van het geselecteerde lodge type of hij beschikbaar is
                            if (db.Bookings.Where(table => table.LodgeId == item.LodgeId && (table.CheckInDate < booking.CheckOutDate) && (booking.CheckInDate < table.CheckOutDate) && table.Canceled == false).Count() == 0)
                            {
                                //zet de geselecteerde data in een session var
                                Session["booking"] = booking;

                                //als de gebruiker al is ingelogd
                                if (User.Identity.IsAuthenticated)
                                {
                                    //TRUE: stuur de gebruiker naar de booking controller
                                    return RedirectToAction("Index", "Booking");
                                }
                                else
                                {
                                    //FALSE: stuur de gebruiker naar de register view
                                    return RedirectToAction("Register", "Account");
                                }
                            }
                        }
                        //stuur een error message als er geen lodge beschikbaar is van het geselecteerde lodge type
                        ViewBag.Message = string.Format("There is no lodge available of the type {0}", booking.LodgeTypes);
                        return View("index");
                    }
                    //stuur een error als er geen lodgetype van het geselecteerde lodge niet bestaat
                    ViewBag.Message = string.Format("The lodge type: {0} does not exisit", booking.LodgeTypes);
                    return View("index");
                }
                //sruut een error als de chekindate later is dan de checkout date
                ViewBag.Message = string.Format("The checkin date cant be later than the checkout date");
                return View("index");
            }
            //stuur een error wanneer de checkindate eerder is dan de datum van vandaag
            ViewBag.Message = string.Format("The checkin date cant be earlier than the current date");
            return View("index");
        }
    }
}