using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webBuy.Models;
using webBuy.Models.ViewModel;
using webBuy.Repositories;

namespace webBuy.Controllers.Admin
{
    public class AdminController : Controller
    {
        //Session authentication
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session["userProfile"] == null)
            {
                TempData["msg"] = "Session Timeout! Please Login First.";
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }
        }
        //

        UserRepository userRepository = new UserRepository();
        OrderRepository orderRepository = new OrderRepository();
        ShopRepository shopRepository = new ShopRepository();
        ComissionRepository comissionRepository = new ComissionRepository();
        WithdrawRepository withdrawRepository = new WithdrawRepository();
        ReviewRepository reviewRepository = new ReviewRepository();
        ProductRepository productRepository = new ProductRepository();
        CategoryRepository categoryRepository = new CategoryRepository();
        PaymentRepository paymentRepository = new PaymentRepository();

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

        public ActionResult GetAllProductReviews()
        {
            List<ReviewProductShopView> listReviewProductShopView = new List<ReviewProductShopView>();
            
            var allReviews = reviewRepository.GetAll().ToList();
            foreach (var item in allReviews)
            {
                ReviewProductShopView reviewProductShopView = new ReviewProductShopView();
                reviewProductShopView.ProductId= item.productId;
                reviewProductShopView.UserId = item.userId;
                reviewProductShopView.Review = item.review1;
                reviewProductShopView.Rating = (int)item.rating;

                var productDetails=productRepository.Get(item.productId);
                reviewProductShopView.ProductName = productDetails.name;

                var shopDetails=shopRepository.Get(productDetails.shopId);
                reviewProductShopView.ShopName = shopDetails.name;
                
                listReviewProductShopView.Add(reviewProductShopView);
            }

            return View("ProductReviews",listReviewProductShopView);
        }


        public ActionResult GetAllProductReviewsOrderByDesc(string order)
        {
            List<ReviewProductShopView> listReviewProductShopView = new List<ReviewProductShopView>();

            var allReviews = reviewRepository.GetAll().ToList();
            if (order == "desc")
            {
                allReviews = reviewRepository.GetProductReviewsOrderByDesc().ToList();
            }
            foreach (var item in allReviews)
            {
                ReviewProductShopView reviewProductShopView = new ReviewProductShopView();
                reviewProductShopView.ProductId = item.productId;
                reviewProductShopView.UserId = item.userId;
                reviewProductShopView.Review = item.review1;
                reviewProductShopView.Rating = (int)item.rating;

                var productDetails = productRepository.Get(item.productId);
                reviewProductShopView.ProductName = productDetails.name;

                var shopDetails = shopRepository.Get(productDetails.shopId);
                reviewProductShopView.ShopName = shopDetails.name;

                listReviewProductShopView.Add(reviewProductShopView);
            }
            return Json(listReviewProductShopView.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductDetailsWithRatingShopName(int id)
        {
            var productDetails = productRepository.Get(id);
            ReviewProductShopView reviewProductShopView = new ReviewProductShopView();
            reviewProductShopView.ProductId = id;
            reviewProductShopView.ProductName = productDetails.name;
            reviewProductShopView.UnitPrice = (double)productDetails.unitPrice;
            reviewProductShopView.Quantity = (int )productDetails.quantity;
            reviewProductShopView.ProductStatus = (int)productDetails.productStatus;
            reviewProductShopView.ProductImage = productDetails.image;
            reviewProductShopView.ProductAddedDate = productDetails.date;

            var categoryDetails = categoryRepository.Get((int)productDetails.categoryId);
            reviewProductShopView.CategoryName = categoryDetails.name;

            var shopDetails = shopRepository.Get((int)productDetails.shopId);
            reviewProductShopView.ShopName = shopDetails.name;

            return View("ProductDetailsWithRatingShopName",reviewProductShopView);
        }

        public ActionResult GetProductRating(int productId)
        {
            int[] rating = { 0, 0, 0, 0, 0 };
            var reviewDetails = reviewRepository.GetProductReviews(productId).ToList();
            foreach (var item in reviewDetails)
            {
                if (item.rating == 1)
                {
                    rating[0]++;
                }
                if (item.rating == 2)
                {
                    rating[1]++;
                }
                if (item.rating == 3)
                {
                    rating[2]++;
                }
                if (item.rating == 4)
                {
                    rating[3]++;
                }
                if (item.rating == 5)
                {
                    rating[4]++;
                }
            }
            return Json(rating.ToArray(), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult PaymentMethods()
        {
            return View(paymentRepository.GetAll().ToList()) ;
        }

        [HttpPost]
        public ActionResult PaymentMethodAdd(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "All fields Required";
                return RedirectToAction("PaymentMethods");
            }
            else
            {
                paymentRepository.Insert(payment);
                TempData["msg-type"] = "success";
                TempData["msg"] = "Payment Method Added";
                return RedirectToAction("PaymentMethods");
            }
        }

        public ActionResult PaymentMethodEdit(int id)
        {
            return View(paymentRepository.Get(id));
        }

        [HttpPost]
        public ActionResult PaymentMethodEdit(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "All fields Required";
                return RedirectToAction("PaymentMethods");
            }
            else
            {
                paymentRepository.Update(payment);
                TempData["msg-type"] = "success";
                TempData["msg"] = "Payment Method Updated";
                return RedirectToAction("PaymentMethods");
            }
        }

        public ActionResult PaymentMethodDelete(int id)
        {
            TempData["msg-type"] = "warning";
            TempData["msg"] = "'"+ paymentRepository.Get(id).paymentMethod+ "' method deleted";
            paymentRepository.Delete(id);
            return RedirectToAction("PaymentMethods");
            
        }


        public ActionResult GetCategories()
        {
            return View(categoryRepository.GetAll().ToList());
        }

        [HttpPost]
        public ActionResult CategoryAdd(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "All fields Required";
                return RedirectToAction("GetCategories");
            }
            else
            {
                categoryRepository.Insert(category);
                TempData["msg-type"] = "success";
                TempData["msg"] = "Category added";
                return RedirectToAction("GetCategories");
            }
        }

        public ActionResult CategoryEdit(int id)
        {
            return View(categoryRepository.Get(id));
        }

        [HttpPost]
        public ActionResult CategoryEdit(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "All fields Required";
                return RedirectToAction("GetCategories");
            }
            else
            {
                categoryRepository.Update(category);
                TempData["msg-type"] = "success";
                TempData["msg"] = "Category updated";
                return RedirectToAction("GetCategories");
            }
        }

        public ActionResult CategoryDelete(int id)
        {
            TempData["msg-type"] = "warning";
            TempData["msg"] = "'" + categoryRepository.Get(id).name + "' category deleted";
            categoryRepository.Delete(id);
            return RedirectToAction("GetCategories");

        }
    

        public ActionResult AllShops()
        {
            return View(shopRepository.GetAll().ToList());
        }

        [HttpPost]
        public ActionResult ShopUpdate(int id,Shop shop)
        {
            if (ModelState.IsValid)
            {
                var getShop = shopRepository.Get(id);
                getShop.setComission = shop.setComission;
                getShop.shopStatus = shop.shopStatus;
                shopRepository.Update(getShop);
                TempData["msg-type"] = "success";
                TempData["msg"] = "Shop updated";
            }
            else
            {
                TempData["msg-type"] = "danger";
                TempData["msg"] = "Please fill the fields correctly";
            }
            return RedirectToAction("AllShops");

        }


        public ActionResult ShopProducts(int id)
        {
            return View(productRepository.GetByShopId(id));
        }
        
    }

}