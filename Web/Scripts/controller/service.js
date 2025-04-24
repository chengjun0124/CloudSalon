app.controller("service", ["$scope", "locationEx", "serviceAPIService", "$stateParams", "$rootScope", function ($scope, locationEx, serviceAPIService, $stateParams, $rootScope) {
    var identityCode = locationEx.queryString("identitycode");
    $scope.serviceId = $stateParams.serviceId
    $scope.showQRCode = false;
    $scope.isDiscount = false;    
    
    serviceAPIService.getService($scope.serviceId, identityCode).then(function (resp) {
        $scope.service = resp.data;
        $rootScope.pageTitle = $scope.service.serviceName;
        $scope.isDiscount = $scope.service.oncePriceOnSale != null || $scope.service.treatmentPriceOnSale != null;

        if ($stateParams.showQRCode == "1" && $scope.service.qrCodePicture != null)
            $scope.showQRCode = true;

        $scope.painStars = [];
        for (var i = 0; i < resp.data.pain; i++) {
            $scope.painStars.push(i);
        }
    });

}]);