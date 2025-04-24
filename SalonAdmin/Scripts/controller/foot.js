app.controller("foot", ["$scope", "$state", function ($scope, $state) {
    $scope.isHomeActive = false;
    $scope.homeImg = "imgs/home.png";

    $scope.isServicesActive = false;
    $scope.servicesImg = "imgs/service.png";

    $scope.isSetupActive = false;
    $scope.setupImg = "imgs/setting.png";

    if ($state.current.name == "home") {
        $scope.isHomeActive = true;
        $scope.homeImg = "imgs/home2.png";
    }
    else if ($state.current.name == "services") {
        $scope.isServicesActive = true;
        $scope.servicesImg = "imgs/service2.png";
    }
    else if ($state.current.name == "setup") {
        $scope.isSetupActive = true;
        $scope.setupImg = "imgs/setting2.png";
    } 
}]);