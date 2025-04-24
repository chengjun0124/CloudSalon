app.controller("memo", ["$scope", "$stateParams", "userAPIService", "dialogService", function ($scope, $stateParams, userAPIService, dialogService) {
    $scope.userId = $stateParams.userId;
    $scope.title = "备注";
    $scope.routeName = "user/" + $stateParams.userId;

    userAPIService.getUser($scope.userId).then(function (resp) {
        $scope.user = resp.data;
    });

    $scope.updateMemo = function () {
        var json = {
            "userId": $stateParams.userId,
            "memo": $scope.user.memo
        };

        userAPIService.updateMemo(json).then(function (resp) {
            dialogService.showMention(1, "更新备注成功", "user", { "userId": $stateParams.userId }, null);
        });
    };
    
}]);