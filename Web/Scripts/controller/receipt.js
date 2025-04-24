app.controller("receipt", ["$scope", "$rootScope", "consumedServiceAPIService", "$stateParams", "dialogService", function ($scope, $rootScope, consumedServiceAPIService, $stateParams, dialogService) {
    $rootScope.pageTitle = "消费单";
    $scope.consumedService = null;

    consumedServiceAPIService.getConsumedService($stateParams.consumedServiceId).then(function (resp) {
        $scope.consumedService = resp.data;
    });

    $scope.changeStatus = function (statusId) {
        consumedServiceAPIService.changeConsumedServiceStatus({ "consumedServiceId": $stateParams.consumedServiceId, "consumedServiceStatusId": statusId }).then(function (resp) {
            $scope.consumedService.consumedServiceStatusId = statusId;

            if (statusId == 2)
                dialogService.showMention(1, "消费确认成功");
            else
                dialogService.showMention(1, "消费拒绝成功");
        });
    };
}]);