app.controller("receipts", ["$scope", "$rootScope", "consumedServiceAPIService", function ($scope, $rootScope, consumedServiceAPIService) {
    $rootScope.pageTitle = "消费单";
    $scope.dataForLoadMore = null;
    $scope.consumedServices = null;
    var pageNumber = 1;


    function getConsumedServices() {
        consumedServiceAPIService.getUnConfirmedConsumedServices().then(function (resp) {
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
    
    getConsumedServices();

    $scope.loadMore = function () {
        pageNumber++;
        getConsumedServices();
    };
}]);