app.controller("purchasedservices", ["$scope", "$rootScope", "purchasedServiceAPIService", function ($scope, $rootScope, purchasedServiceAPIService) {
    $rootScope.pageTitle = "购买的服务";
    var pageNumber = 1;
    $scope.purchasedServices = null;
    $scope.purchasedServiceCount = null;
    $scope.dataForLoadMore = null;


    

    function getPurchasedServices() {
        purchasedServiceAPIService.getPurchasedServices(pageNumber, null, null).then(function (resp) {
            if (pageNumber == 1)
                $scope.purchasedServices = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.purchasedServices.push(resp.data[i]);
            }

            if ($scope.purchasedServices.length == 0)//$scope.purchasedServices.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;;
        });
    }
    
    getPurchasedServices();

    $scope.loadMore = function () {
        pageNumber++;
        getPurchasedServices();
    };

    purchasedServiceAPIService.getPurchasedServiceCount().then(function (resp) {
        $scope.purchasedServiceCount = resp.data;
    });


}]);