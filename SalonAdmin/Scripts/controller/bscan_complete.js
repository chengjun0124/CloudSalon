app.controller("bscan_complete", ["$scope", "$stateParams", "purchasedServiceAPIService", function ($scope, $stateParams, purchasedServiceAPIService) {
    $scope.title = "扫码成功";
    $scope.routeName = "appointments";
    $scope.purchasedServiceId = $stateParams.purchasedServiceId;
    $scope.purchasedService = null;

    purchasedServiceAPIService.getPurchasedService($scope.purchasedServiceId).then(function (resp) {
        $scope.purchasedService = resp.data;
    });

}]);