app.controller("services", ["$scope", "locationEx", "serviceAPIService", "$rootScope", "dialogService", function ($scope, locationEx, serviceAPIService, $rootScope, dialogService) {
    var identityCode = locationEx.queryString("identitycode");
    var pageNumber = 1;
    $rootScope.pageTitle = "惠美丽";
    $scope.selectedType = null;
    $scope.services = null;
    $scope.dataForLoadMore = null;
    $scope.serviceTypes = null;
    

    serviceAPIService.getServiceTypes(identityCode).then(function (resp) {
        $scope.serviceTypes = resp.data;
        $scope.serviceTypes.splice(0, 0, { "serviceTypeId": null, "serviceTypeName": "全部分类" });
        $scope.selectedType = $scope.serviceTypes[0];

        getServices();

    });

    $scope.showServiceTypes = function () {
        dialogService.showServiceTypes($scope.serviceTypes, $scope.selectedType, $scope.selectType);
    };

    function getServices() {
        serviceAPIService.getServices(pageNumber, identityCode, $scope.selectedType.serviceTypeId).then(function (resp) {
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

    $scope.loadMore = function () {
        pageNumber++;
        getServices();
    };

    $scope.selectType = function (type) {
        if ($scope.selectedType == type)
            return;
        $scope.selectedType = type;
        pageNumber = 1;
        $scope.$broadcast("resetPullPixel");
        getServices();
    };

}]);



