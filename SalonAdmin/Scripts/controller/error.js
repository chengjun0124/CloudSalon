app.controller("error", ["$scope", "$stateParams", function ($scope, $stateParams) {
    if ($stateParams.msg == null || $stateParams.msg.length == 0)
        $stateParams.msg = "服务器错误，请联系管理员";


    $scope.msg = decodeURIComponent($stateParams.msg);
}]);