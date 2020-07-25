using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PicManager.Models
{
    public class PictureInfo
    {
        [Key]
        public int PicId { get; set; }
        public int UserId { get; set; }
        public string PicName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime TackDate { get; set; }
        public string TackPalce { get; set; }

    }
}