using ISD.Sitefinity.Models.YouTube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISD.Sitefinity.Web.District.Mvc.Models
{
    public class YouTubeVideoModel
    {
        public List<VideoItem> Videos { get; set; }
        public YouTubeWidgetType WidgetType { get; set; }
    }
}