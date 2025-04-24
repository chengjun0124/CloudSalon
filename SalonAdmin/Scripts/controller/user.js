app.controller("user", ["$scope", "userAPIService", "$stateParams", "purchasedServiceAPIService", "storageService", function ($scope, userAPIService, $stateParams, purchasedServiceAPIService, storageService) {
    $scope.title = "用户详情";
    $scope.routeName = "customers";
    $scope.userId = $stateParams.userId;
    $scope.user = null;
    var pageNumber = 1;
    $scope.purchasedServices = null;
    $scope.purchasedServiceCount = null;
    $scope.dataForLoadMore = null;
    $scope.userTypeId = storageService.getUserTypeId();
    
    userAPIService.getUser($scope.userId).then(function (resp) {
        $scope.user = resp.data;
    });


    function getPurchasedServices() {
        purchasedServiceAPIService.getPurchasedServices(pageNumber, $scope.userId).then(function (resp) {
            if (pageNumber == 1)
                $scope.purchasedServices = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.purchasedServices.push(resp.data[i]);
            }
            
            if ($scope.purchasedServices.length == 0)//$scope.purchasedServices.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;
        });   
    }

    getPurchasedServices();

    $scope.getLastConsumedDate = function (consumedServices) {
        if (consumedServices.length > 0)
            return consumedServices[0].createdDate.format("YYYY-MM-dd");
        else
            return "";
    };

    $scope.getLastConsumedEmployeeNickName = function (consumedServices) {
        if (consumedServices.length > 0)
            return consumedServices[0].employeeNickName;
        else
            return "";
    };

    $scope.getRemainTime = function (purchasedService) {
        var remainTime = purchasedService.time;
        
        for (var i = 0; i < purchasedService.consumedServices.length; i++) {
            remainTime -= purchasedService.consumedServices[i].time;
        }

        return remainTime;
    };

    

    purchasedServiceAPIService.getPurchasedServiceCount($scope.userId).then(function (resp) {
        $scope.purchasedServiceCount = resp.data;
    });


    $scope.loadMore = function () {
        pageNumber++;
        getPurchasedServices();
    };

}]);