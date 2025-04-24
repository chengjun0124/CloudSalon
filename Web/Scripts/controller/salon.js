app.controller("salon", ["$scope", "salonAPIService", "locationEx", "$stateParams", "$rootScope", function ($scope, salonAPIService, locationEx, $stateParams, $rootScope) {
    var identityCode = locationEx.queryString("identitycode");
    $scope.showQRCode = false;

    salonAPIService.getSalon(identityCode).then(function (resp) {
        $scope.salon = resp.data;

        $rootScope.pageTitle = $scope.salon.salonName;

        if ($stateParams.showQRCode == "1" && $scope.salon.qrCodePicture != null)
            $scope.showQRCode = true;
    });
}]);