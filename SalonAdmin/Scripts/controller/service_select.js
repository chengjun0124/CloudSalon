app.controller("service_select", ["$scope", "serviceAPIService", "$stateParams", "routeService", "dialogService", function ($scope, serviceAPIService, $stateParams, routeService, dialogService) {
    $scope.userId = $stateParams.userId;
    $scope.dataForLoadMore = null;
    var pageNumber = 1;
    $scope.services = null;
    $scope.title = "服务";
    if ($scope.userId == null)
        $scope.routeName = "customers";
    else
        $scope.routeName = "user/" + $scope.userId;
    $scope.selectedService = null;

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

    $scope.selectService = function (service) {
        $scope.selectedService = service;
    };


    $scope.loadMore = function () {
        pageNumber++;
        getServices();
    };

    $scope.submit = function () {
        if ($scope.userId == null)
            routeService.goParams("check_anonym", { "serviceId": $scope.selectedService.serviceId });
        else
            routeService.goParams("service_purchase", { "userId": $scope.userId, "serviceId": $scope.selectedService.serviceId });
    };
}]);