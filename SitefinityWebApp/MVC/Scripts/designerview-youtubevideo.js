var youTubeModule = angular.module('designer');
angular.module('designer').requires.push('sfServices');

youTubeModule.service('dataService', ['$http', 'serverContext', function ($http, serverContext) {
    this.getPlayLists = function (channelId, youTubeApiKey) {
        return new Promise(function (res, rej) {
            $.ajax({
                cache: false,
                data: $.extend({
                    key: youTubeApiKey,
                    channelId: channelId,
                    part: 'snippet,contentDetails'
                }, { maxResults: 25 }),
                dataType: 'json',
                type: 'GET',
                timeout: 5000,
                url: 'https://www.googleapis.com/youtube/v3/playlists'
            }).done(function (data) {
                var mappedPlaylists = data.items.map(function (item) {
                    return {
                        'Value': item.id.toString(),
                        'Text': item.snippet.localized.title.toString()
                    };
                });
                res(mappedPlaylists.sort(_compare));
            });
        });
    }
    this.getVideos = function (channelId, youTubeApiKey) {
        return new Promise(function (res, rej) {
            $.ajax({
                cache: false,
                data: $.extend({
                    key: youTubeApiKey,
                    channelId: channelId,
                    part: 'snippet'
                }, { maxResults: 50 }),
                dataType: 'json',
                type: 'GET',
                timeout: 5000,
                url: 'https://www.googleapis.com/youtube/v3/search'
            }).done(function (data) {
                var filteredVideos = data.items.filter(_filterNullIds);
                var mappedVideos = filteredVideos.map(function (item) {
                    
                        return {
                            'Value': item.id.videoId,
                            'Text': item.snippet.title
                        };
                                        
                });
                res(mappedVideos.sort(_compare));
            });
        });
    }
    function _filterNullIds(item) {
        if (item.id.videoId) {
            return true;
        }
        return false;
    }
    function _compare(a, b) {
        if (a.Text < b.Text)
            return -1;
        if (a.Text > b.Text)
            return 1;
        return 0;
    }
}]);


youTubeModule.controller('YouTubeVideoCtrl', ['$scope', 'dataService', 'propertyService', function ($scope, dataService, propertyService) {
    $scope.playLists = [];
    $scope.videos= [];
    $scope.widgetType = "0";
    $scope.feedback.showLoadingIndicator = true;
    var channelID;
    var youTubeApiKey;

    propertyService.get()
        .then(function (data) {
            if (data) {
                $scope.properties = propertyService.toAssociativeArray(data.Items);
                channelID = $scope.properties.ChannelID.PropertyValue;
                youTubeApiKey = $scope.properties.YouTubeApiKey.PropertyValue;

                $scope.updateDropDowns();
            }
        },
        function (data) {
            $scope.feedback.showError = true;
            if (data)
                $scope.feedback.errorMessage = data.Detail;
        })
        .finally(function () {
            $scope.feedback.showLoadingIndicator = false;

        });

    $scope.widgetTypes = [
        { 'Value': "0", 'Text': 'List' },
        { 'Value': "1", 'Text': 'Single' }
    ];

    function loadPlayListDropDown() {
        if (channelID && youTubeApiKey) {
            dataService.getPlayLists(channelID, youTubeApiKey)
                .then(function (data) {
                    $scope.$apply(function () {
                        $scope.playLists = data;
                    });
                });
        }
    }

    function loadVideoDropDown() {
        if (channelID && youTubeApiKey) {
            dataService.getVideos(channelID, youTubeApiKey)
                .then(function (data) {
                    $scope.$apply(function () {
                        $scope.videos = data;
                    });
                });
        }
    }

    $scope.updateDropDowns = function () {
        $scope.playLists = [];
        $scope.videos = [];

        if (!channelID) {
            channelID = $scope.properties.ChannelID.PropertyValue;
        }

        var widgetType = $scope.properties.WidgetType.PropertyValue;
        //widgetType: Single = 1 List = 0
        if (widgetType === '0') {
            loadPlayListDropDown();
        } else if (widgetType === '1') {
            loadVideoDropDown();
        }
    }

    if (!$scope.properties.NumberToShow.PropertyValue) {
        $scope.properties.NumberToShow.PropertyValue = 5;
    }
    $scope.$watch('properties.WidgetType.PropertyValue', function (newValue, oldValue) {
        if (newValue) {
            $scope.widgetType = newValue;
        }
    });

    $scope.$watch('widgetType', function (newValue, oldValue) {
        if (newValue) {
            $scope.properties.WidgetType.PropertyValue = JSON.stringify(newValue);
        }
    });


}]);

