app.controller("foot", ["$scope", "$state", function ($scope, $state) {
    $scope.isServicesActive = false;
    $scope.servicesImg = "imgs/beauty2.png";

    $scope.isMyAppointmentsActive = false;
    $scope.myAppointmentsImg = "imgs/will2.png";

    $scope.isPersonCenterActive = false;
    $scope.personCenterImg = "imgs/mine2.png";

    if ($state.current.name == "services") {
        $scope.isServicesActive = true;
        $scope.servicesImg = "imgs/beauty.png";
    }
    else if ($state.current.name == "appointments_pending") {
        $scope.isMyAppointmentsActive = true;
        $scope.myAppointmentsImg = "imgs/will.png";
    }
    else if ($state.current.name == "usercenter") {
        $scope.isPersonCenterActive = true;
        $scope.personCenterImg = "imgs/mine.png";
    }
}]);