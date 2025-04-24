app.controller("service", ["$scope", "serviceAPIService", "$stateParams", "routeService", "storageService", "dialogService", "$rootScope", function ($scope, serviceAPIService, $stateParams, routeService, storageService, dialogService, $rootScope) {
    $scope.serviceId = $stateParams.serviceId;
    $scope.userTypeId = storageService.getUserTypeId();
    $scope.routeName = "services";
    $scope.isDiscount = false;
    
    serviceAPIService.getService($scope.serviceId).then(function (resp) {
        $scope.service = resp.data;
        $scope.isDiscount = $scope.service.oncePriceOnSale != null || $scope.service.treatmentPriceOnSale != null;

        $scope.painStars = [];
        for (var i = 0; i < resp.data.pain; i++) {
            $scope.painStars.push(i);
        }
    });

    $scope.deleteService = function () {
        dialogService.showMention(2, "确定要删除此服务吗？", null, null, function () {
            serviceAPIService.deleteService($scope.serviceId).then(function (resp) {
                routeService.goParams("services");
            });
        });
    };

    $scope.share = function () {
        var picUrl = $scope.service.smallSubjectImage == null ? $rootScope.FRONTENDURL + "imgs/share.jpg" : $scope.service.smallSubjectImage;
        control.shareContent($scope.service.serviceName, $scope.service.functionality, $rootScope.FRONTENDURL + "?identitycode=" + storageService.getIdentityCode() + "#!/service/" + $scope.service.serviceId + "/1", picUrl);
    };
}]);