app.controller("services", ["$scope", "serviceAPIService", "storageService", function ($scope, serviceAPIService, storageService) {
    $scope.userTypeId = storageService.getUserTypeId();
    $scope.dataForLoadMore = null;
    var pageNumber = 1;
    $scope.services = null;

    function getServices() {
        serviceAPIService.getServices(pageNumber).then(function (resp) {
            if (pageNumber == 1)
                $scope.services = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.services.push(resp.data[i]);
            }

            if ($scope.services.length == 0)//$scope.services.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;
        });
    }

    getServices();

    serviceAPIService.getServiceCount().then(function (resp) {
        $scope.serviceCount = resp.data;
    });

    $scope.loadMore = function () {
        pageNumber++;
        getServices();
    };
}]);