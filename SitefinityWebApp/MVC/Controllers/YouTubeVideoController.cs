using ISD.Sitefinity.Models.YouTube;
using ISD.Sitefinity.Services.YouTube;
using ISD.Sitefinity.Web.District.Mvc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace ISD.Sitefinity.Web.District.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "YouTubeVideo", Title = "You Tube Video", SectionName = "ISD")]
    public class YouTubeVideoController : Controller
    {
        public string WidgetType { get; set; }
        private YouTubeWidgetType _wigetType;
        public string NumberToShow { get; set; }
        private int _numberToShow = 5;
        public string ChannelID { get; set; }
        public string PlayListID { get; set; }
        public string VideoID { get; set; }
        public string YouTubeApiKey { get { return ConfigurationManager.AppSettings["YouTubeAPIKey"]; } }

        // GET: YouTubeVideo
        public ActionResult Index()
        {
            var model = new YouTubeVideoModel
            {
                Videos = new List<VideoItem>()
            };

            if (!string.IsNullOrEmpty(WidgetType))
            {
                _wigetType = (YouTubeWidgetType)JsonConvert.DeserializeObject<int>(WidgetType);
            }

            model.WidgetType = _wigetType;

            if (_wigetType == YouTubeWidgetType.List)
            {
                LoadListModel(model);
            }
            else if (_wigetType == YouTubeWidgetType.Single)
            {
                LoadSingleModel(model);
            }

            return View(model);
        }

        private void LoadListModel(YouTubeVideoModel model)
        {
            if (!string.IsNullOrEmpty(NumberToShow))
            {
                _numberToShow = JsonConvert.DeserializeObject<int>(NumberToShow);
            }
            if (!string.IsNullOrEmpty(PlayListID))
            {
                model.Videos = YouTubeDataService.GetPlaylistVideos(PlayListID);
            }
            else if (!string.IsNullOrEmpty(ChannelID))
            {
                model.Videos.AddRange(YouTubeDataService.GetChannelVideos(ChannelID).Take(_numberToShow).ToList());
            }
        }
        private void LoadSingleModel(YouTubeVideoModel model)
        {
            model.Videos.Add(YouTubeDataService.GetSingleVideoItem(VideoID));
        }

    }
}