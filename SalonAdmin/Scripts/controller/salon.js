app.controller("salon", ["$scope", "salonAPIService", "$rootScope", "storageService", function ($scope, salonAPIService, $rootScope, storageService) {
    $scope.routeName = "home";

    salonAPIService.getSalon().then(function (resp) {
        $scope.salon = resp.data;
    });

    $scope.share = function () {
        var picUrl = $scope.salon.smallPicture == null ? $rootScope.FRONTENDURL + "imgs/share.jpg" : $scope.salon.smallPicture;
        control.shareContent($scope.salon.salonName, $scope.salon.description, $rootScope.FRONTENDURL + "?identitycode=" + storageService.getIdentityCode() + "#!/salon/1", picUrl);
    };
}]);