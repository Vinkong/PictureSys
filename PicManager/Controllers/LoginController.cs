using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PicManager.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult LoginPage()
        {
            DbPicture db = new DbPicture();
            db.Database.CreateIfNotExists();
            return View();
        }
    }
}