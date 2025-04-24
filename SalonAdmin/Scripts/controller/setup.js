app.controller("setup", ["$scope", "employeeAPIService", "storageService", "routeService", function ($scope, employeeAPIService, storageService, routeService) {
    $scope.userTypeId = storageService.getUserTypeId();


    employeeAPIService.getEmployee().then(function (resp) {
        $scope.employee = resp.data;
    });


    $scope.logout = function () {
        if (window.control != undefined && control.removeUMengAlias != undefined)
            control.removeUMengAlias(storageService.getUserId().toString(), "jiyunorg");

        storageService.removeJWT();
        routeService.goParams("login");
    };
}]);