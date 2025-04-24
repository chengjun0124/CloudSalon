app.controller("purchasedservice", ["$scope", "$stateParams", "purchasedServiceAPIService", function ($scope, $stateParams, purchasedServiceAPIService) {
    $scope.purchasedServiceId = $stateParams.purchasedServiceId;
    $scope.userId = $stateParams.userId;
    $scope.title = "已购服务详情";
    $scope.routeName = "user/" + $scope.userId;
    $scope.purchasedService = null;


    purchasedServiceAPIService.getPurchasedService($scope.purchasedServiceId).then(function (resp) {
        $scope.purchasedService = resp.data;
    });
    

    $scope.getRemainTime = function () {
        var remainTime = $scope.purchasedService.time;
        for (var i = 0; i < $scope.purchasedService.consumedServices.length; i++) {
            remainTime -= $scope.purchasedService.consumedServices[i].time;
        } 
        return remainTime;
    };

    $scope.getSuggestedDate = function () {
        if ($scope.purchasedService == null)
            return "";

        if ($scope.purchasedService.consumedServices.length == 0 || !$scope.purchasedService.interval)
            return "随时";

        var d = new Date($scope.purchasedService.consumedServices[0].createdDate);
        return new Date(d.setDate(d.getDate() + $scope.purchasedService.interval+1)).format("YYYY-MM-dd");
    };
}]);