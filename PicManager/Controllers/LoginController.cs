using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PicManager.Controllers
{
    public class LoginController : Controller
    {          DbPicture db = new DbPicture();
        // GET: Login
        public ActionResult LoginPage()
        {

            //db.Database.CreateIfNotExists();
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(User user)
        {
            var qurey = from p in db.User
                        where p.UserName == user.UserName && p.PassWord == user.PassWord
                        select p;

            if (qurey.Count() > 0)
            {
                Session["UserName"] = user.UserName;
                return RedirectToAction("UserLoginPage");
            }


            ViewData["errowMsg"] = "密码或用户名错误";
            return View ("LoginPage");

          
        }

        public ActionResult UserLoginPage(User user) {

            return View();
           
        }
        public ActionResult UserLogout() {

            return RedirectToAction("LoginPage");
        }
    }
}