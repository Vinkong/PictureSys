using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PicManager.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage ="用户名不能为空")]
        [MinLength(4,ErrorMessage ="至少4个字符")] 
        [Remote("IsHaveUName", "Register", ErrorMessage ="用户名已存在", HttpMethod = "POST")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="昵称不能为空")]
        [MinLength(4,ErrorMessage ="至少4个字符")]
        [Remote("IsHaveSName", "Register", ErrorMessage = "昵称已存在",HttpMethod ="POST")]
        public string ShowUserName { get; set; }
        [Required(ErrorMessage ="密码不能为空")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[#@!~%^&*.])[a-zA-Z\d#@!~%^&*.]{8,}$",ErrorMessage = "至少8位，并包含字母、数字和特殊字符")]
        public string PassWord { get; set; }
        [Required(ErrorMessage ="确认密码不能为空")]
        [System.ComponentModel.DataAnnotations.Compare("PassWord",ErrorMessage ="两次密码输入不一致")]
        public string ConfirmPwd { get; set; }
        [Required(ErrorMessage ="邮箱不能为空")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",ErrorMessage ="邮箱格式不正确")]
        [Remote("IsExsistMail","Register",ErrorMessage ="邮箱已注册",HttpMethod ="POST")]
        public string Email { get; set; }
        public int Status { get; set; }
        public string Code { get; set; }
        public DateTime CreatTime{ get; set; }
           

    }
}