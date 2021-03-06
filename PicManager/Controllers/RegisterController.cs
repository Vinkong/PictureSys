﻿using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity.Infrastructure;
using System.Web.UI;
using System.Text.RegularExpressions;

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
                user.PassWord = result.ToString();
                user.ConfirmPwd = result.ToString();
                db.User.Add(user);
                try
                {
                    //让db 不对实体验证
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }
                catch (Exception e)
                {

                    throw;
                }

                 SendEmail(validatacode, user.Email, user.UserName);
                return RedirectToAction("LoginPage", "Login");
            }


            return View("RegisterPage", user);
        }
        public ActionResult RegisterSuccess() {

            return View();
        }

        public ActionResult ActivePage(string UserName, string activeCode) {

            var qurey = from user in db.User
                        where user.UserName == UserName && user.Code == activeCode
                        select user;
            if (qurey.Count() > 0)
            {
                User user = qurey.FirstOrDefault();
                user.Status = 1;
                try
                {
                    //让db 不对实体验证
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    db.Configuration.ValidateOnSaveEnabled = true;
                }
                catch (Exception e)
                {

                    throw;
                }

                return RedirectToAction("ToLoginPage");
            }


            return View();

        }
        public ActionResult ToLoginPage() {


            return View();
        }
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]//加上清除缓存
        public ActionResult IsHaveUName(string UserName) {

            var query = from u in db.User
                        where u.UserName == UserName
                        select u;
            if (query.Count()>0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]//加上清除缓存
        public ActionResult IsHaveSName(string ShowUserName)
        {

            var query = from u in db.User
                        where u.ShowUserName == ShowUserName
                        select u;
            if (query.Count() > 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]//加上清除缓存
        public ActionResult IsExsistMail(string Email) {

            var query = from u in db.User
                        where u.Email == Email
                        select u;
            if (query.Count() > 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //重新发送激活邮件
        public ActionResult ReEmailPage() {
            return View(); 

        }

        //处理激活数据
        public ActionResult ResendMail(string Email, string valiCode)
        {
            string validatacode = System.Guid.NewGuid().ToString();
            if (Regex.IsMatch(Email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}"))
            {
                if (Session["ValidateCode"].ToString() != valiCode)
                {
                    ModelState.AddModelError("errowMsg", "验证码错误");
                    return View("ReEmailPage");
                }
                else
                {
                    var query = from p in db.User
                                where p.Email == Email
                                select p;
                    if (query.Count() > 0)
                    {
                        User ub = query.FirstOrDefault();
                        ub.Code = validatacode;
                        try
                        {
                            //让db 不对实体验证
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.SaveChanges();
                            db.Configuration.ValidateOnSaveEnabled = true;
                        }
                        catch (Exception e)
                        {

                            throw;
                        }


                        SendEmail(validatacode, Email, query.FirstOrDefault().UserName);


                        return RedirectToAction("ResendMailSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("errowMsg", "该邮箱还没有注册");
                        return View("ReEmailPage");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("errowMsg", "邮箱格式不正确");
                return View("ReEmailPage");
            }
        }

        //发送成功激活邮件
        public ActionResult ResendMailSuccess() {

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
             mailMsg.Body = "<a href='http://vinkong.51vip.biz:19491/Register/ActivePage/?UserName=" + UserName+"&activeCode=" + activeCode + "'>请单击激活["+UserName+"]注册的账户</a>";//发送邮件的内容 
             mailMsg.IsBodyHtml = true;
             SmtpClient client = new SmtpClient("smtp.qq.com");//smtp.163.com，smtp.qq.com,发件人使用的邮箱的SMTP服务器。
             client.Credentials = new NetworkCredential("1067945009@qq.com", "zpzizkopekxqbehc");//指定发件人的邮箱的账号与授权码，不是QQ密码
             client.Send(mailMsg);//排队发送邮件.
         }
        
  
    }
}