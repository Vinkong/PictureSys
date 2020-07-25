using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PicManager.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PicManager.Controllers
{
    public class PicManagerController : Controller
    {

        DbPicture dp = new DbPicture();
        // GET: PicManager
        public ActionResult UpLoadPicPage()
        {
            return View();
        }
        public ActionResult AddPicPage()
        {
            string fileReasonVoide = "";
            string ftpServer = "172.16.9.91";
            string ftpUser = "Vinkong";
            string ftpPwd = "kfz123456";
            foreach (string key in Request.Files)  // 文件键
            {
                var postedFile = Request.Files[key];    // 获取文件键对应的文件对象
                fileReasonVoide = DateTime.Now.ToString("yyMMddHHmmss") + postedFile.FileName;
                //string filename=  Path.GetFileName(postedFile.FileName);//获取文件名有后缀
                string filenameW = Path.GetFileNameWithoutExtension(postedFile.FileName);//获取文件名没有后缀
                string extension = Path.GetExtension(postedFile.FileName);
                string filename = filenameW + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                string uri = "ftp://" + ftpServer + "/" + fileReasonVoide;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(ftpUser, ftpPwd);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UsePassive = false;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = postedFile.ContentLength;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                Stream fs = postedFile.InputStream;
                fs.Position = 0;
                try
                {
                    Stream strm = reqFTP.GetRequestStream();
                    contentLen = fs.Read(buff, 0, buffLength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
                    strm.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {

                }
             PictureInfo  pic = GetXYFromPic(@"D:\PicTure\"+ fileReasonVoide);
                HttpCookie cookie = Request.Cookies.Get("UserCookie");

                //判断cookie是否空值
                if (cookie != null)
                {
                    //把保存的用户名和密码赋值给对应的文本框
                    //用户名
                   string UName = cookie.Values["UserName"].ToString();
                  var query =  dp.User.Where(c => c.UserName == UName).FirstOrDefault();
                    pic.UserId = query.UserId;
                }
               
                if (("0001/1/1 0:00:00".Equals(pic.TackDate.ToString()))|| ("0001-01-01 00:00:00".Equals(pic.TackDate.ToString())))
                {
                    pic.TackDate = DateTime.Now;
                }
                dp.PictureInfo.Add(pic);
                if (dp.SaveChanges()<0)
                {
                    return Content("no");

                }
       
            }

            return Content("ok");
            
        }
            /// <summary>
                    /// 百度api 根据经纬度获取地理位置
                    /// </summary>
            /// <param name="lat">纬度</param>
                    /// <param name="lng">经度</param>
                    /// <returns>具体的地理位置</returns>
        public static string GetLocation(string lat, string lng)
            {
                HttpClient client = new HttpClient();
                string url = string.Format("http://api.map.baidu.com/reverse_geocoding/v3/?ak=DseeLoYERhh4mG6QVQFQ15yvzws6vPRR&callback=renderReverse&location={0},{1}&output=json&pois=1Z", lat, lng);
                string result = client.GetStringAsync(url).Result;

                var locationResult = (JObject)JsonConvert.DeserializeObject(result.Replace("renderReverse&&renderReverse", "").Replace("(", "").Replace(")", ""));

                if (locationResult == null || locationResult["result"] == null || locationResult["result"]["formatted_address"] == null)
                    return string.Empty;

                var address = Convert.ToString(locationResult["result"]["formatted_address"]);
                if (locationResult["result"]["sematic_description"] != null)
                    address += " " + Convert.ToString(locationResult["result"]["sematic_description"]);
                return address;
            }
        public  PictureInfo  GetXYFromPic(String jpgPath)
            {
            PictureInfo pt = new PictureInfo();
                string pName = System.IO.Path.GetFileName(jpgPath);
                pt.PicName = pName;
                try
                {
                    //载入图片   
                    Image objImage = Image.FromFile(jpgPath);
                    //取得所有的属性(以PropertyId做排序)   
                    var propertyItems = objImage.PropertyItems.OrderBy(x => x.Id);
                    foreach (PropertyItem objItem in propertyItems)
                    {
                        //只取Id范围为0x0000到0x001e
                        if (objItem.Id >= 0x0000 && objItem.Id <= 0x001e)
                        {
                            switch (objItem.Id)
                            {
                                case 0x0002://设置纬度
                                    if (objItem.Value.Length == 24)
                                    {
                                        //degrees(将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint)   
                                        double d = BitConverter.ToUInt32(objItem.Value, 0) * 1.0d / BitConverter.ToUInt32(objItem.Value, 4);
                                        //minutes(將byte[8]~byte[11]转成uint, 除以byte[12]~byte[15]转成的uint)   
                                        double m = BitConverter.ToUInt32(objItem.Value, 8) * 1.0d / BitConverter.ToUInt32(objItem.Value, 12);
                                        //seconds(將byte[16]~byte[19]转成uint, 除以byte[20]~byte[23]转成的uint)   
                                        double s = BitConverter.ToUInt32(objItem.Value, 16) * 1.0d / BitConverter.ToUInt32(objItem.Value, 20);
                                        double dblGPSLatitude = (((s / 60 + m) / 60) + d);
                                        pt.Latitude = dblGPSLatitude.ToString("0.00000000");

                                    }
                                    break;
                                case 0x0004: //设置经度
                                    if (objItem.Value.Length == 24)
                                    {
                                        //degrees(将byte[0]~byte[3]转成uint, 除以byte[4]~byte[7]转成的uint)   
                                        double d = BitConverter.ToUInt32(objItem.Value, 0) * 1.0d / BitConverter.ToUInt32(objItem.Value, 4);
                                        //minutes(将byte[8]~byte[11]转成uint, 除以byte[12]~byte[15]转成的uint)   
                                        double m = BitConverter.ToUInt32(objItem.Value, 8) * 1.0d / BitConverter.ToUInt32(objItem.Value, 12);
                                        //seconds(将byte[16]~byte[19]转成uint, 除以byte[20]~byte[23]转成的uint)   
                                        double s = BitConverter.ToUInt32(objItem.Value, 16) * 1.0d / BitConverter.ToUInt32(objItem.Value, 20);
                                        double dblGPSLongitude = (((s / 60 + m) / 60) + d);
                                        pt.Longitude = dblGPSLongitude.ToString("0.00000000");
                                    }
                                    break;
                            }
                        }
                        if (objItem.Id == 0x9003)//Id为0x9003表示拍照的时间,0x0132 最后更新时间
                        {
                            var propItemValue = objItem.Value;
                            var dateTimeStr = System.Text.Encoding.ASCII.GetString(propItemValue).Trim('\0');
                            var dt = DateTime.ParseExact(dateTimeStr, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                            if (!("0001/1/1 0:00:00".Equals(dt.ToString())))
                            {
                                pt.TackDate = dt;
                            }
                        pt.TackDate = DateTime.Now;
                        }


                        objImage.Dispose();

                    }
                    if (!(string.IsNullOrEmpty(pt.Latitude)) && !(string.IsNullOrEmpty(pt.Longitude)))
                    {
                        pt.TackPalce = GetLocation(pt.Latitude, pt.Longitude);
                    }
                    return pt;
                }
                catch (Exception ex)
                {
                    //MessageManager.Show(jpgPath + "该图片文件损坏");
                    //listErrorMessage.Add(jpgPath + "该照片由于照片损坏，因此无法进行导入。");
                    return pt;
                }

            }

    }
}
