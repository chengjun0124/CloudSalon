app.controller("consumedservice", ["$scope", "consumedServiceAPIService", "$stateParams", "storageService", "dialogService", "routeService", function ($scope, consumedServiceAPIService, $stateParams, storageService, dialogService, routeService) {
    $scope.title = "服务记录";
    $scope.routeName = "consumedservices";
    $scope.consumedService = null;
    $scope.userId = storageService.getUserId();
    $scope.consumedServiceId = $stateParams.consumedServiceId;
    
    consumedServiceAPIService.getConsumedService($stateParams.consumedServiceId).then(function (resp) {
        $scope.consumedService = resp.data;
    });

    $scope.delete = function () {
        dialogService.showMention(2, "确定要删除此消费单吗？", null, null, function () {
            consumedServiceAPIService.deleteConsumedService($stateParams.consumedServiceId).then(function (resp) {
                routeService.goParams("consumedservices");
            });
        });
    };
}]);