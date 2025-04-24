app.controller("consumedservices", ["$scope", "consumedServiceAPIService", function ($scope, consumedServiceAPIService) {
    $scope.title = "服务记录";
    $scope.routeName = "home";
    var pageNumber = 1;
    $scope.dataForLoadMore = null;
    $scope.consumedServices = null;
    //startDate是当前周的第一天，即周日
    $scope.startDate = new Date();
    $scope.startDate.setDate($scope.startDate.getDate() - $scope.startDate.getDay());
    //selectedDate默认是当天
    $scope.selectedDate = new Date();

    
    function getConsumedServiceCount() {
        consumedServiceAPIService.getConsumedServiceCount($scope.startDate.format("YYYY-MM-dd")).then(function (resp) {
            $scope.days = resp.data;
        });
    }

    function getConsumedServices() {
        consumedServiceAPIService.getConsumedServices($scope.selectedDate.format("YYYY-MM-dd"), pageNumber).then(function (resp) {
            if (pageNumber == 1)
                $scope.consumedServices = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.consumedServices.push(resp.data[i]);
            }

            if ($scope.consumedServices.length == 0)//$scope.consumedServices.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;
        });
    }

    getConsumedServiceCount();
    getConsumedServices();

    $scope.selectDate = function (date) {
        $scope.selectedDate = new Date(date);        
        $scope.$broadcast("resetPullPixel");
        pageNumber = 1;
        getConsumedServices();
    };


    $scope.selectWeek = function (n) {
        if (n > 0) {
            $scope.startDate.setDate($scope.startDate.getDate() + 7);
            $scope.selectedDate.setDate($scope.selectedDate.getDate() + 7);
        }
        else {
            $scope.startDate.setDate($scope.startDate.getDate() - 7);
            $scope.selectedDate.setDate($scope.selectedDate.getDate() - 7);
        }
        $scope.$broadcast("resetPullPixel");
        pageNumber = 1;

        getConsumedServiceCount();
        getConsumedServices();
    };

    $scope.loadMore = function () {
        pageNumber++;
        getConsumedServices();
    };
}]);