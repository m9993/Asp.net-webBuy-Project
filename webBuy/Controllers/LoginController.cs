using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webBuy.Models;
using webBuy.Repositories;

namespace webBuy.Controllers
{
    public class LoginController : Controller
    {
        UserRepository userRepository = new UserRepository();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(User user)
        {
            var loginUser=userRepository.VerifyLogin(user.email, user.password);
            if (loginUser!= null) 
            {
                if (loginUser.userStatus==1)
                {
                    Session["userProfile"] = loginUser;
                    if (loginUser.role=="admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    if (loginUser.role == "seller")
                    {
                        return RedirectToAction("Index", "Seller");
                    }
                    if (loginUser.role == "customer")
                    {
                        return RedirectToAction("Index", "Customer");
                    }

                }
                else
                {
                    TempData["msg"] = "You have no permission to access your account! Please contact with Admin.";
                }
                
            }
            else
            {
                TempData["msg"] = "Invalid emali or password!";
            }
            return RedirectToAction("Index");


        }
    }
}