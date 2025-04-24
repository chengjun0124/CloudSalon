app.controller("user_add", ["$scope", "userAPIService", "dialogService", function ($scope, userAPIService, dialogService) {
    $scope.title = "新增用户";
    $scope.routeName = "customers";
    $scope.user = {
        name: null,
        mobile: null,
        memo: null
    };
    
    
    $scope.submit = function () {
        userAPIService.createUser($scope.user).then(function (resp) {
            dialogService.showMention(1, "添加用户成功", "customers");
        });
    };

}]);