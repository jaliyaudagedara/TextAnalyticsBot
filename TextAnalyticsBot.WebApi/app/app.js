"use strict"

angular.module("TextAnalyticsBot", ["ui.bootstrap", "angular-loading-bar"])

.service("AppService", ["$http", function ($http) {
    this.postMessage = function (message) {
        return $http({
            method: 'POST',
            url: 'api/messages',
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify(message)
        }).success(function (data) {
        }).error(function (data) {
            console.log(data);
            alert(data.Message);
        });
    };
}])

.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if(event.which === 13) {
                scope.$apply(function (){
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
})

.controller("AppController", ["$scope", "AppService", function ($scope, AppService) {
    $scope.messageTypes = [
        "Message",
        "Ping",
        "DeleteUserData",
        "BotAddedToConversation",
        "BotRemovedFromConversation",
        "UserAddedToConversation",
        "UserRemovedFromConversation",
        "EndOfConversation"
    ];

    $scope.message = {
        type: "Message",
        text: "",
        result: {}
    };

    $scope.getProgressBarType = function (value) {
        var type = "success";

        if (value < 50) {
            type = "danger";
        } else if (value == 50) {
            type = "info";
        }

        return type;
    }

    $scope.send = function () {
        AppService.postMessage($scope.message).then(function (result) {
            console.log(result.data);
            $scope.message.result = result.data;
        });
    };
}]);