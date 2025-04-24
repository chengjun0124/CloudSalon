app.controller("appointment", ["$scope", "$stateParams", "routeService", "appointmentAPIService", "$rootScope", "dialogService", function ($scope, $stateParams, routeService, appointmentAPIService, $rootScope, dialogService) {
    $rootScope.pageTitle = "预约详情";


    appointmentAPIService.getAppointment($stateParams.appointmentId).then(function (resp) {

        $scope.appointment = resp.data;

    });


    $scope.cancelAppoint = function () {
        dialogService.showMention(2, "确定要取消该预约吗？", null, null, function () {
            var json = {
                "apponintmentStatus": 5,
                "appointmentId": $stateParams.appointmentId
            };

            appointmentAPIService.changeAppointStatus(json).then(function (resp) {
                routeService.goParams("appointment_cancel_success");
            });
        });   
    };
}]);