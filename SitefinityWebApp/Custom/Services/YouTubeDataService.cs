using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using ISD.Sitefinity.Models.YouTube;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ISD.Sitefinity.Services.YouTube
{
    public static class YouTubeDataService
    {
        private static string _apiKey = ConfigurationManager.AppSettings["YouTubeAPIKey"];
        private static string _channelID = ConfigurationManager.AppSettings["YouTubeChannelId"];
        private static YouTubeService _youTubeService = new YouTubeService(new BaseClientService.Initializer() { ApiKey = _apiKey });

        public static List<VideoItem> GetChannelVideos(string channelID)
        {
            var searchListRequest = _youTubeService.Search.List("snippet");
            searchListRequest.ChannelId = channelID;
            searchListRequest.MaxResults = 50;
            var searchListResult = searchListRequest.Execute();

            return BuildVideoItems(searchListResult.Items);
        }
        public static List<VideoItem> GetPlaylistVideos(string playListId)
        {
            var searchListRequest = _youTubeService.PlaylistItems.List("snippet,contentDetails");
            searchListRequest.PlaylistId = playListId;
            searchListRequest.MaxResults = 50;
            var searchListResult = searchListRequest.Execute();

            return BuildVideoItems(searchListResult.Items);
        }
        private static List<VideoItem> BuildVideoItems(IList<SearchResult> searchResult)
        {
            var videoItems = searchResult.Where(x => x.Id.VideoId != null).Select(x => new VideoItem
            {
                VideoId = x.Id.VideoId,
                Title = x.Snippet.Title,
                Description = x.Snippet.Description,
                ThumbnailUrl = x.Snippet.Thumbnails.High.Url,
                PublishedAt = x.Snippet.PublishedAt
                
            }).OrderByDescending(x => x.PublishedAt).ToList();

            foreach (var item in videoItems)
            {
                TimeSpan ts = XmlConvert.ToTimeSpan(GetVideoByID(item.VideoId)?.ContentDetails?.Duration);
                item.Duration = ts.ToString(@"mm\:ss");
            }
            return videoItems;
        }
        private static List<VideoItem> BuildVideoItems(IList<PlaylistItem> searchResult)
        {
            var videoItems = searchResult.Where(x => x.Id != null).Select(x => new VideoItem
            {
                VideoId = x.Snippet.ResourceId.VideoId,
                Title = x.Snippet.Title,
                Description = x.Snippet.Description,
                ThumbnailUrl = x.Snippet.Thumbnails.High.Url,
                PublishedAt = x.Snippet.PublishedAt

            }).OrderByDescending(x => x.PublishedAt).ToList();

            foreach (var item in videoItems)
            {
                TimeSpan ts = XmlConvert.ToTimeSpan(GetVideoByID(item.VideoId)?.ContentDetails?.Duration);
                item.Duration = ts.ToString(@"mm\:ss");
            }
            return videoItems;
        }
        public static Video GetVideoByID(string videoID)
        {
            var searchListRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics");
            searchListRequest.Id = videoID;
            var searchListResult = searchListRequest.Execute();
            if (searchListResult.Items.Any())
            {
                return searchListResult.Items.FirstOrDefault();
            }
            return new Video();
        }
        public static VideoItem GetSingleVideoItem(string videoID)
        {
            var video =  GetVideoByID(videoID);

            return new VideoItem
            {
                VideoId = video.Id,
                Title = video.Snippet.Title,
                Description = video.Snippet.Description,
                ThumbnailUrl = video.Snippet.Thumbnails.High.Url,
                PublishedAt = video.Snippet.PublishedAt,
                Duration = XmlConvert.ToTimeSpan(video.ContentDetails.Duration).ToString(@"mm\:ss")
            };
            
        }





    }
}
