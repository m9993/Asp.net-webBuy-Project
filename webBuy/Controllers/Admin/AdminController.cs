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
            if (changePassword.OldPassword==null || changePassword.NewPassword == null || changePassword.ConfirmNewPassword == null)
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "All fields need to be filled";
            }
            else
            {
                User profile=userRepository.Get((Session["userProfile"] as User).userId);
                if (profile.password!= changePassword.OldPassword)
                {
                    TempData["msg-type"] = "danger";
                    TempData["msg"] = "Old password does not match";
                }
                else
                {
                    if (changePassword.NewPassword!= changePassword.ConfirmNewPassword)
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
                if ((Session["userProfile"] as User).email!=user.email)
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

    }
}