app.controller("appointments_pending", ["$scope", "appointmentAPIService", "serviceAPIService", "$rootScope", "purchasedServiceAPIService", function ($scope, appointmentAPIService, serviceAPIService, $rootScope, purchasedServiceAPIService) {
    $rootScope.pageTitle = "预约";
    $scope.purchasedServices = null;
    $scope.appointments = null;
    $scope.hotServices = null;


    appointmentAPIService.getUserAppointments().then(function (resp) {
        $scope.appointments = resp.data;

        purchasedServiceAPIService.getPurchasedServices(1, 2, true).then(function (resp) {
            $scope.purchasedServices = resp.data;


            for (var i = 0; i < $scope.purchasedServices.length; i++) {
                $scope.purchasedServices[i].hasPendingAppointment = hasPendingAppointment($scope.purchasedServices[i].serviceId);
            } 
        });
    });

    function hasPendingAppointment(serviceId) {
        for (var i = 0; i < $scope.appointments.length; i++) {
            if ($scope.appointments[i].serviceId == serviceId) {
                if ($scope.appointments[i].appointmentStatusId == 1 || $scope.appointments[i].appointmentStatusId == 3)
                    return true;
            }
        }
        return false;
    }


    serviceAPIService.getHotServices(2).then(function (resp) {
        $scope.hotServices = resp.data;
    });

    

}]);