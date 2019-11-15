using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Spa_Resort_bali.Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Spa_Resort_bali.Controllers
{
    [Authorize]
    [HandleError]
    public class ProfileController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // GET: Profile
        public ActionResult Index(string message = "", string messageColor = "primary")
        {
            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.user = user;
            ViewBag.address = db.Addresses.Find(user.AdressId);
            ViewBag.profileMessage = message;
            ViewBag.profileMessageColor = messageColor;

            return View();
        }

        public ActionResult Apply(RegisterViewModel model)
        {
            string userId = User.Identity.GetUserId();

            ApplicationUser user = UserManager.FindById(userId);

            if (model.ProfilePassword != null)
            {
                if (UserManager.CheckPassword(user, model.OldPassword))
                {
                    UserManager.ChangePasswordAsync(userId, model.OldPassword, model.ProfilePassword);
                }
                else
                {
                    return RedirectToAction("Index", new { message = "The given 'old password' does not match your password.", messageColor = "danger" });
                }
            }
            user.Email = model.Email;
            user.UserName = model.Email;
            user.DateOfBirth = DateTime.Parse(model.DateOfBirth);
            UserManager.ChangePhoneNumber(userId, model.PhoneNumber, UserManager.GenerateChangePhoneNumberToken(userId, model.PhoneNumber));

            UserManager.UpdateAsync(user);

            Addresses address = db.Addresses.Find(user.AdressId);

            address.PostCode = model.Postcode;
            address.Streed = model.Streed;
            address.HouseNumber = model.HousNumber;
            address.City = model.City;
            address.State = model.State;
            address.Counrty = model.Country;

            db.Entry(address).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { message = "Succesfully changed your information.", messageColor = "success" });
        }

        // POST: /Profile/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Profile", new { message = "TwoFactorAuthentication enabled successfully.", messageColor = "success" });
        }

        //
        // POST: /Profile/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Profile", new { message = "TwoFactorAuthentication disabled successfully.", messageColor = "success" });
        }
    }
}