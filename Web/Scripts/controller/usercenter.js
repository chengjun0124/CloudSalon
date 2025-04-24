app.controller("usercenter", ["$scope", "userAPIService", "$rootScope", function ($scope, userAPIService, $rootScope) {
    $rootScope.pageTitle = "我的惠美丽";


    userAPIService.getUser().then(function (resp) {

        $scope.user = resp.data;

    });
    
}]);