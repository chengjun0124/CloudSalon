app.controller("appointment_success", ["$scope", "$stateParams", "$rootScope", function ($scope, $stateParams, $rootScope) {
    $rootScope.pageTitle = "预约完成";

    $scope.serviceName = $stateParams.serviceName;
    $scope.beautician = $stateParams.beautician;
    $scope.date = $stateParams.date;

}]);