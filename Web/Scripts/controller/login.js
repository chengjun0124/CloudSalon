app.controller("login", ["$scope", "locationEx", "authAPIService", "storageService", "routeService", "$rootScope", "dialogService", "$stateParams", function ($scope, locationEx, authAPIService, storageService, routeService, $rootScope, dialogService, $stateParams) {
    $rootScope.pageTitle = "登录";
    var identityCode = locationEx.queryString("identitycode");

    $scope.sendValidCode = function () {
        authAPIService.sendValidCode($scope.mobile, identityCode).then(function (resp) {
            dialogService.showMention(1, "短信发送成功");
        });
    };

    $scope.login = function () {
        authAPIService.login($scope.mobile, $scope.code, identityCode).then(function (resp) {
            storageService.set(identityCode, resp.data);

            var redirect = JSON.parse(decodeURIComponent($stateParams.redirect));
            routeService.goParams(redirect.state, redirect.params);
        });
    };

    var height = $(window).height();
    $("body").css("height", height);
}]);