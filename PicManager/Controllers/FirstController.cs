﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PicManager.Controllers
{
    public class FirstController : Controller
    {
        // GET: First
        public ActionResult IndexPage()
        {
                return View();
        }

        //壁纸
        public ActionResult ButiPage() {

            return View();
        }
    }
}