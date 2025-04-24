app.controller("customers", ["$scope", "userAPIService", "$timeout", "storageService", function ($scope, userAPIService, $timeout, storageService) {
    $scope.title = "顾客";
    $scope.routeName = "home";
    var pageNumber = 1;
    $scope.dataForLoadMore = null;
    $scope.users = null;
    $scope.userTypeId = storageService.getUserTypeId();

    function getUsers() {
        userAPIService.getUsers(pageNumber, $scope.keyword).then(function (resp) {
            if (pageNumber == 1)
                $scope.users = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.users.push(resp.data[i]);
            }

            if ($scope.users.length == 0)//$scope.users.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;
        });
    }


    userAPIService.getUserCount().then(function (resp) {
        $scope.userCount = resp.data;
    });

    getUsers();

    $scope.loadMore = function () {
        pageNumber++;
        getUsers();
    };

    var time;
    $scope.search = function () {
        if (time != null)
            $timeout.cancel(time);

        time = $timeout(function () {
            $scope.$broadcast("resetPullPixel");
            pageNumber = 1;
            getUsers();
        }, 1000);
    };
}]);