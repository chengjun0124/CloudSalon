app.controller("appointment", ["$scope", "$stateParams", "routeService", "appointmentAPIService", "dialogService", "storageService", "consumedServiceAPIService", function ($scope, $stateParams, routeService, appointmentAPIService, dialogService, storageService, consumedServiceAPIService) {
    $scope.title = "预约";
    $scope.routeName = "appointments";
    $scope.userTypeId = storageService.getUserTypeId();
    $scope.isBeautician = storageService.getIsBeautician();
    $scope.userId = storageService.getUserId();

    appointmentAPIService.getAppointment($stateParams.appointmentId).then(function (resp) {
        $scope.appointment = resp.data;
    });

    $scope.changeStatus = function (status) {
        var msg;
        if (status == 3)
            msg = "预约已确认，如有变更，请及时联系用户";
        else if (status == 2)
            msg = "预约已拒绝";
        else if (status == 6)
            msg = "预约已取消";

        var json = {
            "apponintmentStatus": status,
            "appointmentId": $stateParams.appointmentId
        };

        appointmentAPIService.changeAppointStatus(json).then(function (resp) {
            dialogService.showMention(1, msg, "appointments");
        });
    };

    $scope.scan = function () {
        if (window.control != undefined)
            control.scan("bscan");
        else
            dialogService.showMention(1, "扫码只能在集云美邦手机APP中使用");
    };

    $scope.scanResult = function (result) {
        if (/^[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+$/.test(result) == false)
            dialogService.showMention(1, "无效消费码");
        else {
            consumedServiceAPIService.beauticianScan({ "consumeCode": result, "appointmentId": $stateParams.appointmentId }).then(function (resp) {
                routeService.goParams("bscan_complete", { "purchasedServiceId": resp.data });
            });
        }
    };
}]);