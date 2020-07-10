using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PicManager.Controllers
{
    public class LoginController : Controller
    {          DbPicture db = new DbPicture();
        // GET: Login
        public ActionResult LoginPage()
        {
            User user = new User();
            //获取cookie中的数据
            HttpCookie cookie = Request.Cookies.Get("UserCookie");

            //判断cookie是否空值
            if (cookie != null)
            {
                //把保存的用户名和密码赋值给对应的文本框
                //用户名
                user.UserName  = cookie.Values["UserName"].ToString();
         
                //密码
              user.PassWord = cookie.Values["UserPwd"].ToString();
             
            }
            //db.Database.CreateIfNotExists();//codeFirst创建数据库
            return View(user);
        }

        [HttpPost]
        public ActionResult LoginPage(User user,string valiCode,string IsCheck)
        {
            if (Session["ValidateCode"].ToString() != valiCode)
            {
                ModelState.AddModelError("errowMsg", "验证码错误");
                return View(user);
            }
            if (!string.IsNullOrEmpty(user.UserName)&&!string.IsNullOrEmpty(user.PassWord))
            {

           
            StringBuilder result = new StringBuilder();

            //MD5CryptoServiceProvider   是 加密服务提供程序   用来 计算字符串的 MD5 哈希值
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                //将输入字符串转换为字节数组并计算哈希。
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(user.PassWord));

                //X为     十六进制 X都是大写 x都为小写
                //2为 每次都是两位数
                //假设有两个数10和26，正常情况十六进制显示0xA、0x1A，这样看起来不整齐，为了好看，可以指定"X2"，这样显示出来就是：0x0A、0x1A。 
                //遍历哈希数据的每个字节
                //并将每个字符串格式化为十六进制字符串。
                int length = data.Length;
                for (int i = 0; i < length; i++)
                    result.Append(data[i].ToString("x2"));

            }
            string md5Pwd = result.ToString();
            var qurey = from p in db.User
                        where p.UserName == user.UserName && p.PassWord == md5Pwd
                        select p;

            if (qurey.Count() > 0)
            {
                Session["UserName"] = user.UserName;

                if (IsCheck=="on")
                {
                    HttpCookie hc = new HttpCookie("UserCookie");
                    hc["UserName"] = user.UserName;
                    hc["UserPwd"] = user.PassWord;
                    hc.Expires = DateTime.Now.AddDays(2);
                    Response.Cookies.Add(hc);
                    return RedirectToAction("UserLoginPage");
                }
                else
                {
                    HttpCookie hc = new HttpCookie("UserCookie");
                    if (hc!=null)
                    {
                        hc.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(hc);                      
                    }
                }
                return View("UserLoginPage");
            }
            }
            ModelState.AddModelError("errowMsg", "密码或用户名错误");
            return View ("LoginPage",user);

          
        }

        public ActionResult UserLoginPage(User user) {

            return View();
           
        }
        public ActionResult UserLogout() {
            Session.Abandon();
            return RedirectToAction("LoginPage");
        }


        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}