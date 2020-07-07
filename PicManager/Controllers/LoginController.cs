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
        public ActionResult UserLoginPage(User user) {

            //var qurey = from p in db.User
            //           where p.UserName == user.UserName && p.PassWord == user.PassWord
            //           select  p;
            int a = 1;
           if (a > 0)
           {
             return View();           
               }

           ViewData["errowMsg"] = "密码或用户名错误";

                return  RedirectToAction("LoginPage");
        }
    }
}