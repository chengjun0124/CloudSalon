app.controller("service_purchase", ["$scope", "serviceAPIService", "$stateParams", "dialogService", "purchasedServiceAPIService", "dialogService", function ($scope, serviceAPIService, $stateParams, dialogService, purchasedServiceAPIService, dialogService) {
    $scope.userId = $stateParams.userId;
    $scope.serviceId = $stateParams.serviceId;
    $scope.title = "购买服务";
    $scope.routeName = "service_select/" + $scope.userId;
    $scope.service = null;
    $scope.purchase = {
        payment: null,
        time: null,
        mode: null,
        serviceId: $scope.serviceId,
        userId: $scope.userId
    };
    $scope.isPreview = false;

    serviceAPIService.getService($scope.serviceId).then(function (resp) {
        $scope.service = resp.data;

        $scope.painStars = [];
        for (var i = 0; i < resp.data.pain; i++) {
            $scope.painStars.push(i);
        }
    });

    $scope.selectMode = function (mode) {
        $scope.purchase.mode = mode;

        if (mode == 0) {
            $scope.purchase.payment = $scope.service.oncePrice;
            $scope.purchase.time = 1;
        }
        else if (mode == 1) {
            $scope.purchase.payment = $scope.service.treatmentPrice;

            if ($scope.service.treatmentPrice)
                $scope.purchase.time = $scope.service.treatmentTime;
            else
                $scope.purchase.time = null;
        }
    };

    $scope.submit = function () {
        purchasedServiceAPIService.createPurchasedService($scope.purchase).then(function (resp) {
            dialogService.showMention(1, "购买服务成功", "user", { "userId": $scope.userId });
        });
    };
}]);