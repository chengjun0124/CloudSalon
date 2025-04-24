app.controller("check_anonym", ["$scope", "$stateParams", "serviceAPIService", "employeeAPIService", "consumedServiceAPIService", "dialogService", function ($scope, $stateParams, serviceAPIService, employeeAPIService, consumedServiceAPIService, dialogService) {
    $scope.title = "匿名买单";
    $scope.routeName = "service_select";
    $scope.serviceId = $stateParams.serviceId;
    $scope.service = null;
    $scope.beauticians = null;
    $scope.selectedBeautician = null;
    $scope.check = {
        payment: null,
        time: 1,
        serviceId: $scope.serviceId,
        employeeId: null,
        changeTimeReason: null
    };

    serviceAPIService.getService($scope.serviceId).then(function (resp) {
        $scope.service = resp.data;
    });

    employeeAPIService.getBeauticians().then(function (resp) {
        $scope.beauticians = resp.data;
    });

    $scope.selectBeautician = function (employee) {
        $scope.selectedBeautician = employee;
    };


    $scope.submit = function () {
        $scope.check.employeeId = $scope.selectedBeautician.employeeId;

        consumedServiceAPIService.checkAnonym($scope.check).then(function (resp) {
            dialogService.showMention(1, "匿名买单记账成功", "customers");
        });
    };

}]);