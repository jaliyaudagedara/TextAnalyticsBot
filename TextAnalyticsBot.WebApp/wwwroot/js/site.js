"use strict"

angular.module("TextAnalyticsBot", ["ui.bootstrap", "angular-loading-bar"])

.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
})

.controller("AppController", ["$scope", function ($scope) {

}]);