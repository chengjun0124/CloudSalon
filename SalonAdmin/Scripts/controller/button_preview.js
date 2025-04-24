app.controller("button_preview", ["$scope",  function ($scope) {
    $scope.preSubmit = function () {
        $scope.$parent.isPreview = true;
    };

    $scope.cancel = function () {
        $scope.$parent.isPreview = false;
    };
}]);