using System;
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


            //获取cookie中的数据
            HttpCookie cookie = Request.Cookies.Get("UserCookie");

            //判断cookie是否空值
            if (cookie != null)
            {
                //把保存的用户名和密码赋值给对应的文本框
                //用户名
                var name = cookie.Values["UserName"].ToString();
                ViewBag.UserName = name;
                //密码
                var pwd = cookie.Values["UserPwd"].ToString();
                ViewBag.UserPwd = pwd;
            }
                return View();
        }
    }
}