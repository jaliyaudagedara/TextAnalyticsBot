"use strict"

angular.module("TextAnalyticsBot", ["ngMaterial"])

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

    $scope.send = function () {
        AppService.postMessage($scope.message).then(function (result) {
            console.log(result.data);
            $scope.message.result = result.data;
        });
    };
}]);