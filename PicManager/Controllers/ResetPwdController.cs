using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PicManager.Controllers
{
    public class ResetPwdController : Controller
    {
        DbPicture db = new DbPicture();

        // GET: ResetPwd
        public ActionResult ResetPwdPage()
        {
            return View();
        }

        public ActionResult SuccessMailPage() {

            return View();
        }

        public ActionResult ResetPwdUrl( string Email ,string valiCode) {

            if (Regex.IsMatch(Email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}"))
            {
                if (Session["ValidateCode"].ToString() != valiCode)
                {
                    ModelState.AddModelError("errowMsg", "验证码错误");
                    return View("ResetPwdPage");
                }
                else
                {
                    var query = from p in db.User
                                where p.Email == Email
                                select p;
                    if (query.Count() > 0)
                    {
                        //DES加密的用户名和时间
                        string  NameDES = EncryptUtils.DESEncrypt(query.FirstOrDefault().UserName, "vinkong1", "vinkong2");
                        string timers = DateTime.Now.ToString();
                        string DateStr = EncryptUtils.DESEncrypt(DateTime.Now.ToString(),"vinkong1","vinkong2");


                        SendEmail("1067945009@qq.com", NameDES, DateStr, timers);


                        return RedirectToAction("SuccessMailPage","ResetPwd");
                    }
                    else
                    {
                        ModelState.AddModelError("errowMsg", "该邮箱还没有注册");
                        return View("ResetPwdPage");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("errowMsg", "邮箱格式不正确");
                return View("ResetPwdPage");
            }  
        }

        public ActionResult ResetPwdNewPage(string  DESName,string DESTime) {


            string UserName = EncryptUtils.DESDecrypt(DESName, "vinkong1", "vinkong2");
            DateTime TimeTip =  Convert.ToDateTime( EncryptUtils.DESDecrypt(DESTime, "vinkong1", "vinkong2"));
            DateTime NowDateTime = DateTime.Now;
            int hour = (int)(NowDateTime - TimeTip).TotalMinutes;
            if (hour > 60)
            {

                return View("ResetOutTime");
            }
            ViewData["UName"] = UserName;
            return View();
        }

        public ActionResult ResetOutTime() {

            return View();

        }
        //修改密码
        public ActionResult ResetPwdDb(string UserName,string PassWord) {

            User ub = db.User.Where(u => u.UserName == UserName).FirstOrDefault();
            StringBuilder result = new StringBuilder();

            //MD5CryptoServiceProvider   是 加密服务提供程序   用来 计算字符串的 MD5 哈希值
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                //将输入字符串转换为字节数组并计算哈希。
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(PassWord));

                //X为     十六进制 X都是大写 x都为小写
                //2为 每次都是两位数
                //假设有两个数10和26，正常情况十六进制显示0xA、0x1A，这样看起来不整齐，为了好看，可以指定"X2"，这样显示出来就是：0x0A、0x1A。 
                //遍历哈希数据的每个字节
                //并将每个字符串格式化为十六进制字符串。
                int length = data.Length;
                for (int i = 0; i < length; i++)
                    result.Append(data[i].ToString("x2"));

            }
            ub.PassWord = result.ToString();
            ub.ConfirmPwd = result.ToString();
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


            return View();
        }


        /// 发送修改密码链接.
        /// </summary>
        public static void SendEmail(string mail, string Name, string TimeStr,string timers)
        {
            MailMessage mailMsg = new MailMessage();//两个类，别混了，要引入System.Net这个Assembly
            mailMsg.From = new MailAddress("1067945009@qq.com");//源邮件地址 ,发件人
            mailMsg.To.Add(new MailAddress(mail));//目的邮件地址。可以有多个收件人.
            mailMsg.Subject = "Picture找回密码邮件";//发送邮件的标题 
            mailMsg.Body = "<a href='http://vinkong.51vip.biz:19491/ResetPwd/ResetPwdNewPage/?timers=" + timers + "&DESName=" + Name + "&DESTime=" + TimeStr + "'>请点击进入修改密码界面</a>";//发送邮件的内容 
            mailMsg.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.qq.com");//smtp.163.com，smtp.qq.com,发件人使用的邮箱的SMTP服务器。
            client.Credentials = new NetworkCredential("1067945009@qq.com", "*********");//指定发件人的邮箱的账号与授权码，不是qq密码.
            client.Send(mailMsg);//排队发送邮件.
        }
    }
}