using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webBuy.Models;
using webBuy.Repositories;

namespace webBuy.Controllers.Admin
{
    public class AdminController : Controller
    {
        UserRepository userRepository = new UserRepository();
        OrderRepository orderRepository = new OrderRepository();
        ShopRepository shopRepository = new ShopRepository();
        ComissionRepository comissionRepository = new ComissionRepository();
        WithdrawRepository withdrawRepository = new WithdrawRepository();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changePassword)
        {
            if (changePassword.OldPassword == null || changePassword.NewPassword == null || changePassword.ConfirmNewPassword == null)
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "All fields need to be filled";
            }
            else
            {
                User profile = userRepository.Get((Session["userProfile"] as User).userId);
                if (profile.password != changePassword.OldPassword)
                {
                    TempData["msg-type"] = "danger";
                    TempData["msg"] = "Old password does not match";
                }
                else
                {
                    if (changePassword.NewPassword != changePassword.ConfirmNewPassword)
                    {
                        TempData["msg-type"] = "danger";
                        TempData["msg"] = "New password & confirm new password is not matching";
                    }
                    else
                    {
                        profile.password = changePassword.NewPassword;
                        userRepository.Update(profile);
                        Session["userProfile"] = profile;
                        TempData["msg-type"] = "success";
                        TempData["msg"] = "Password changed";
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult UpdateUserProfile()
        {
            User profile = userRepository.Get((Session["userProfile"] as User).userId);
            return View(profile);
        }
        [HttpPost]
        public ActionResult UpdateUserProfile(User user)
        {
            if (ModelState.IsValid)
            {
                if ((Session["userProfile"] as User).email != user.email)
                {
                    if (userRepository.VerifyEmailInDb(user.email))
                    {
                        TempData["msg-type"] = "success";
                        TempData["msg"] = "Email changed to " + "'" + user.email + "'";
                        userRepository.Update(user);
                        Session["userProfile"] = user;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["msg-type"] = "danger";
                        TempData["msg"] = "'" + user.email + "' already exists";
                        return RedirectToAction("UpdateUserProfile");
                    }
                }
                else
                {
                    TempData["msg-type"] = "success";
                    TempData["msg"] = "Profile updated";
                    userRepository.Update(user);
                    Session["userProfile"] = user;
                    return RedirectToAction("Index");
                }


            }
            else
            {
                return View("UpdateUserProfile");
            }
        }

        public ActionResult GetTodaySells()
        {
            List<double> sumTotalInWeek = new List<double>();
            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(-i).ToShortDateString();
                var listByDate = orderRepository.GetOrdersListByDate(date).ToList();
                double sumTotal = 0;
                foreach (var item in listByDate)
                {
                    sumTotal += (double)item.total;
                }
                sumTotalInWeek.Add(sumTotal);
            }

            return Json(sumTotalInWeek.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AvailableBalanceInAllShops()
        {
            double balance = 0;
            var allShopDetails = shopRepository.GetAll().ToList();
            foreach (var item in allShopDetails)
            {
                balance += (double)item.balance;
            }
            //1.00
            var balanceInFraction = string.Format("{0:f2}", balance);
            return Json(balanceInFraction, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalComission()
        {
            double balance = 0;
            var comissionDetails = comissionRepository.GetAll().ToList();
            foreach (var item in comissionDetails)
            {
                balance += (double)item.amount;
            }
            //1.00
            var balanceInFraction = string.Format("{0:f2}", balance);
            return Json(balanceInFraction, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSellerWithdrawedMoney()
        {
            double balance = 0;
            var sellerWithdrawDetails = withdrawRepository.SellerWithdraw().ToList();
            foreach (var item in sellerWithdrawDetails)
            {
                balance += (double)item.amount;
            }
            //1.00
            var balanceInFraction = string.Format("{0:f2}", balance);
            return Json(balanceInFraction, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdminWithdrawedMoney()
        {
            double balance = 0;
            var adminWithdrawDetails = withdrawRepository.AdminWithdraw().ToList();
            foreach (var item in adminWithdrawDetails)
            {
                balance += (double)item.amount;
            }
            //1.00
            var balanceInFraction = string.Format("{0:f2}", balance);
            return Json(balanceInFraction, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBannedUsersNotification()
        {
            var bannedUsers = userRepository.GetBannedUsers().ToList().Count();
            return Json(bannedUsers, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult GetBannedUsers()
        {
            var bannedUsers = userRepository.GetBannedUsers().ToList();
            return View("Users",bannedUsers);
        }
        
        public ActionResult GetAllUsers()
        {
            var AllUsers = userRepository.GetAll().ToList();
            return View("Users", AllUsers);
        }

        public ActionResult BanUser(int id)
        {
            var user = userRepository.Get(id);
            user.userStatus = 0;
            userRepository.Update(user);
            return RedirectToAction("GetBannedUsers");
        }

        public ActionResult UnbanUser(int id)
        {
            var user = userRepository.Get(id);
            user.userStatus = 1;
            userRepository.Update(user);
            return RedirectToAction("GetBannedUsers");
        }
    }

}