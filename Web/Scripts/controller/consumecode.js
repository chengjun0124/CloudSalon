app.controller("consumecode", ["$scope", "userAPIService", "$rootScope", function ($scope, userAPIService, $rootScope) {
    $rootScope.pageTitle = "我的消费码";
    $scope.user = null;

    userAPIService.getUser().then(function (resp) {
        $scope.user = resp.data;
    });

    userAPIService.generateConsumeCode().then(function (resp) {
        $("#div_consumecode").qrcode({ height: 452, width: 452, text: resp.data });
    });

}]);