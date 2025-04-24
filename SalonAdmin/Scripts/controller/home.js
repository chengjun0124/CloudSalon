app.controller("home", ["$scope", "appointmentAPIService", "storageService", "dialogService", "routeService", function ($scope, appointmentAPIService, storageService, dialogService, routeService) {
    appointmentAPIService.getTodayAppointmentCount().then(function (resp) {
        $scope.appointmentTodayCound = resp.data;
    });

    $scope.userTypeId = storageService.getUserTypeId();

    if ($scope.userTypeId == 2) {
        
        appointmentAPIService.getRecentAppointment().then(function (resp) {
            $scope.recentAppointment = resp.data;
            if ($scope.recentAppointment != null) {
                var startDate = new Date($scope.recentAppointment.appointmentDate.replace("T"," "));
                var now = new Date();
                var minutes=parseInt((startDate - now) / 60000);

                $scope.remainderHours = parseInt(minutes / 60);
                $scope.remainderMinutes = minutes = minutes % 60;
            }
        });
    }


    $scope.scan = function () {
        if (window.control != undefined)
            control.scan("aoscan");
        else
            dialogService.showMention(1, "扫码只能在集云美邦手机APP中使用");
    };

    $scope.scanResult = function (result) {
        if (/^[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+$/.test(result) == false)
            dialogService.showMention(1, "无效消费码");
        else
            routeService.goParams("check_scan", { "consumeCode": result });
    };
}]);