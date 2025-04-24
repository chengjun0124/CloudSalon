app.controller("aboutus", ["$scope", "$rootScope", function ($scope, $rootScope) {
    $rootScope.setWhiteBody = true;

    $scope.title = "关于我们";
    $scope.routeName = "setup";
}]);