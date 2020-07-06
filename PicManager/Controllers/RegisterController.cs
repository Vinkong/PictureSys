using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace PicManager.Controllers
{
    public class RegisterController : Controller
    {

        public DbPicture db = new DbPicture();
        // GET: Register
        public ActionResult RegisterPage()
        {


            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(User user) {

            if (ModelState.IsValid)
            {
                string validatacode = System.Guid.NewGuid().ToString();
                user.Code = validatacode;
                user.Status = 0;
                user.CreatTime = DateTime.Now;
                db.User.Add(user);
                db.SaveChanges();
                SendEmail(validatacode, "1067945009@qq.com", user.UserName);
                return RedirectToAction("LoginPage", "Login");
            }
           
            
            return View("RegisterPage",user);
        }

        public ActionResult ActivePage(string UserName,string activeCode) {

            var qurey = from user in db.User
                        where user.UserName == UserName && user.Code == activeCode
                        select user;
            if (qurey.Count()>0)
            {
                User user = qurey.FirstOrDefault();
                user.Status = 1;
                db.SaveChanges();

                return RedirectToAction("ToLoginPage"); 
            }

            
            return View();

        }
        public ActionResult ToLoginPage() {


            return View();
        }


    /// 发送激活链接.
         /// </summary>
         public static void SendEmail(string activeCode, string mail,string UserName)
         {
            MailMessage mailMsg = new MailMessage();//两个类，别混了，要引入System.Net这个Assembly
             mailMsg.From = new MailAddress("1067945009@qq.com");//源邮件地址 ,发件人
             mailMsg.To.Add(new MailAddress(mail));//目的邮件地址。可以有多个收件人.
             mailMsg.Subject = "请激活在Picture网站中的注册链接";//发送邮件的标题 
             mailMsg.Body = "<a href='http://localhost:58716/Register/ActivePage/?UserName=" + UserName+"&activeCode=" + activeCode + "'>请单击激活注册的账户</a>";//发送邮件的内容 
             mailMsg.IsBodyHtml = true;
             SmtpClient client = new SmtpClient("smtp.qq.com");//smtp.163.com，smtp.qq.com,发件人使用的邮箱的SMTP服务器。
             client.Credentials = new NetworkCredential("1067945009@qq.com", "eviiclkuprkubefg");//指定发件人的邮箱的账号与密码.
             client.Send(mailMsg);//排队发送邮件.
         }
        
  
    }
}