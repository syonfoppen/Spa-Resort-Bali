using Spa_Resort_bali.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Spa_Resort_bali.Controllers
{
    [Authorize(Roles = "Receptionist, Manager")]
    [HandleError]
    public class ReceptionistController : Controller
    {
        private readonly Spa_Resort_Bali_DatabaseEntities database = new Spa_Resort_Bali_DatabaseEntities();

        // GET: Receptionist
        public ActionResult Index()
        {
            List<Bookings> bookingList = database.Bookings.ToList();
            ViewBag.bookings = bookingList;
            return View();
        }
    }
}