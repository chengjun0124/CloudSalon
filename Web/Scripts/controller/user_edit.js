app.controller("user_edit", ["$scope", "userAPIService", "routeService", "dialogService", "$rootScope", function ($scope, userAPIService, routeService, dialogService, $rootScope) {
    var dialogHandler = null;
    var isChangedUserPicture = false;
    $rootScope.pageTitle = "修改个人信息";

    userAPIService.getUser().then(function (resp) {
        $scope.user = resp.data;
    });


    $scope.updateUser = function () {
        if (!isChangedUserPicture)
            $scope.user.picture = null;
        else
            $scope.user.picture = $scope.user.picture.replace(/data:[^,]+,/, "");

        userAPIService.updateUser($scope.user).then(function (resp) {
            routeService.goParams("usercenter");
        });
    };

    $scope.receiveDataURL = function (dataURL) {
        isChangedUserPicture = true;
        $scope.user.picture = dataURL;
    };
}]);