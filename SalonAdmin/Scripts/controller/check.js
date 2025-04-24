app.controller("check", ["$scope", "$stateParams", "purchasedServiceAPIService", "employeeAPIService", "appointmentAPIService", "consumedServiceAPIService", "dialogService", "serviceAPIService", "userAPIService", function ($scope, $stateParams, purchasedServiceAPIService, employeeAPIService, appointmentAPIService, consumedServiceAPIService, dialogService, serviceAPIService, userAPIService) {
    $scope.purchasedService = null;
    $scope.beauticians = null;
    $scope.selectedBeautician = null;
    $scope.purchasedServices = null;
    $scope.appointments = null;
    $scope.selectedAppointment = null;
    $scope.aoCharge = {
        employeeId: null,
        time: 1,
        appointmentId: null,
        changeTimeReason: null
    };
    $scope.isPreview = false;
    $scope.title;
    $scope.action;
    var userId, consumeCode, purchasedServiceId, consumedServiceId;

    if ($stateParams.userId != null) {//美管手动
        $scope.action = "aocharge";
        userId = $stateParams.userId;
        purchasedServiceId = $stateParams.purchasedServiceId;
        $scope.title = "创建消费单";
        $scope.routeName = "purchasedservice/" + purchasedServiceId + "/" + userId;        

        purchasedServiceAPIService.getPurchasedService(purchasedServiceId).then(function (resp) {
            $scope.purchasedService = resp.data;
        });

        getBeauticians();
        getAvaiAppointments();
    }
    else if ($stateParams.consumeCode != null) {//美管扫码
        $scope.action = "aoscan";
        consumeCode = $stateParams.consumeCode;
        $scope.title = "创建消费单";
        $scope.routeName = "home";

        userAPIService.getUserId(consumeCode).then(function (resp) {
            userId = resp.data;

            getAvaiServices();
            getAvaiAppointments();
            getBeauticians();
        }, function (error) {
            dialogService.showMention(1, error.data[0], "home");
        });
    }
    else if ($stateParams.consumedServiceId != null) { //修改消费单
        $scope.action = "update";
        consumedServiceId = $stateParams.consumedServiceId;
        $scope.routeName = "consumedservices";
        $scope.title = "修改消费单";

        consumedServiceAPIService.getConsumedService(consumedServiceId).then(function (resp) {
            $scope.aoCharge.time = resp.data.time;
            $scope.aoCharge.changeTimeReason = resp.data.changeTimeReason;
            userId = resp.data.userId;

            getBeauticians(resp.data.employeeId);
            getAvaiAppointments(resp.data.appointmentId);
            getAvaiServices(resp.data.serviceId);
        });
    }

    $scope.selectBeautician = function (employee) {
        $scope.selectedBeautician = employee;
    };

    $scope.selectAppointment = function (appointment) {
        if ($scope.selectedAppointment == appointment)
            $scope.selectedAppointment = null;
        else
            $scope.selectedAppointment = appointment;
    };

    $scope.selectPurchasedService = function (ps) {
        $scope.purchasedService = ps;
    };

    $scope.submit = function () {
        $scope.aoCharge.employeeId = $scope.selectedBeautician.employeeId;
        if ($scope.selectedAppointment != null)
            $scope.aoCharge.appointmentId = $scope.selectedAppointment.appointmentId;

        
        if ($scope.action == "aocharge") {//美管手动
            $scope.aoCharge.purchasedServiceId = purchasedServiceId;

            consumedServiceAPIService.aoCharge($scope.aoCharge).then(function (resp) {
                dialogService.showMention(1, "店方完成单方面扣费，请务必敦促用户当面完成确认，否则本次扣费不成功！", "purchasedservice", { "purchasedServiceId": purchasedServiceId, "userId": userId });
            });
        }
        else if ($scope.action == "aoscan")  {//美管扫码
            $scope.aoCharge.serviceId = $scope.purchasedService.serviceId;
            $scope.aoCharge.consumeCode = consumeCode;            

            consumedServiceAPIService.aoScan($scope.aoCharge).then(function (resp) {
                dialogService.showMention(1, "店方完成单方面扣费，请务必敦促用户当面完成确认，否则本次扣费不成功！", "home");
            });
        }
        else if ($scope.action == "update") {//修改消费单
            $scope.aoCharge.serviceId = $scope.purchasedService.serviceId;
            $scope.aoCharge.consumedServiceId = consumedServiceId;

            consumedServiceAPIService.changeConsumedService($scope.aoCharge).then(function (resp) {
                dialogService.showMention(1, "修改消费单成功，请务必敦促用户当面完成确认，否则本次扣费不成功！");
            });
        }
    };

    function getAvaiAppointments(appointmentId) {
        appointmentAPIService.getAvaiAppointments(userId).then(function (resp) {
            $scope.appointments = resp.data;

            for (var i = 0; i < $scope.appointments.length; i++) {
                if ($scope.appointments[i].appointmentId == appointmentId) {
                    $scope.selectedAppointment = $scope.appointments[i];
                    break;
                }
            }
        });
    }

    function getAvaiServices(serviceId) {
        serviceAPIService.getAvaiServices(userId).then(function (resp) {
            $scope.purchasedServices = resp.data;

            for (var i = 0; i < $scope.purchasedServices.length; i++) {
                if ($scope.purchasedServices[i].serviceId == serviceId) {
                    $scope.purchasedService = $scope.purchasedServices[i];
                    break;
                }
            }
        });
    }

    function getBeauticians(employeeId) {
        employeeAPIService.getBeauticians().then(function (resp) {
            $scope.beauticians = resp.data;

            for (var i = 0; i < $scope.beauticians.length; i++) {
                if ($scope.beauticians[i].employeeId == employeeId) {
                    $scope.selectedBeautician = $scope.beauticians[i];
                    break;
                }
            }
        });
    }
}]);