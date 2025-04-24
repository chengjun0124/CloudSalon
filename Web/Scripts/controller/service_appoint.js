app.controller("service_appoint", ["$scope", "locationEx", "serviceAPIService", "$stateParams", "appointmentAPIService", "routeService", "dialogService", "$rootScope", function ($scope, locationEx, serviceAPIService, $stateParams, appointmentAPIService, routeService, dialogService, $rootScope) {
    var identityCode = locationEx.queryString("identitycode");
    $scope.serviceId = $stateParams.serviceId;
    $scope.selectedBeautician = null;
    $scope.selectedDate = null;
    $scope.selectedTime = null;
    $scope.isSalonClose = false;
    $scope.isBeauticianDayoff = false;
    $scope.avaiAppointments = null;

    serviceAPIService.getService($scope.serviceId, identityCode).then(function (resp) {
        $scope.service = resp.data;

        $rootScope.pageTitle = $scope.service.serviceName;
    });

    appointmentAPIService.getAvaiAppointments($scope.serviceId).then(function (resp) {
        if (resp.data == null) {
            //没有可选美容师, api返回null
            return;
        }
        
        $scope.avaiAppointments = resp.data;
        //默认选中第一个技师和第一个日期
        $scope.selectedBeautician = $scope.avaiAppointments.beauticians[0];
        $scope.selectedDate = $scope.avaiAppointments.beauticians[0].avaiDates[0];
        
        //var start = new timeSpan($scope.avaiAppointments.openTime);
        //var end = new timeSpan($scope.avaiAppointments.closeTime);
        //var interval = new timeSpan($scope.avaiAppointments.interval);
        
        //$scope.times = [];
        
        //while (start.lessThan(end)) {
        //    $scope.times.push({ "time": start, "isDisabled": false });
        //    start = start.add(interval);
        //};

        
        $scope.times = [];
        var avaiTimes = $scope.avaiAppointments.beauticians[0].avaiDates[0].avaiTimes;
        for (var i = 0; i < avaiTimes.length; i++) {
            $scope.times.push({ time: new timeSpan(avaiTimes[i].time), isAvailable: true });
        }
        disableTime();
    });

    
    $scope.selectBeautician = function (beautician) {
        if ($scope.selectedBeautician == beautician)
            return;

        $scope.selectedBeautician = beautician;
        disableTime();
    };

    $scope.selectDate = function (date) {
        if ($scope.selectedDate == date)
            return;

        $scope.selectedDate = date;
        disableTime();
    };

    $scope.selectTime = function (time) {
        if (!time.isAvailable)
            return;
        $scope.selectedTime = time;
    };

    function disableTime()
    {
        if ($scope.selectedBeautician == null || $scope.selectedDate == null)
            return;

        $scope.isSalonClose = false;
        $scope.isBeauticianDayoff = false;
        for (var j = 0; j < $scope.times.length; j++) {
            $scope.times[j].isAvailable = true;
        }

        if ($scope.selectedDate.isSalonClose) {
            $scope.isSalonClose = true;
            return;
        }

        var avaiDate = null;
        for (var i = 0; i < $scope.selectedBeautician.avaiDates.length; i++) {
            avaiDate = $scope.selectedBeautician.avaiDates[i];

            if (avaiDate.date == $scope.selectedDate.date) {
                if (avaiDate.isDayoff) {
                    $scope.isBeauticianDayoff = true;
                    return;
                }

                //for (var j = 0; j < avaiDate.avaiTimes.length; j++) {
                //    for (var k = 0; k < $scope.times.length; k++) {
                //        if (new timeSpan(avaiDate.avaiTimes[j]).equal($scope.times[k].time)) {
                //            $scope.times[k].isAvailable = false;

                //            if ($scope.selectedTime == $scope.times[k])
                //                $scope.selectedTime = null;

                //            break;
                //        }
                //    }
                //}

                for (var j = 0; j < avaiDate.avaiTimes.length; j++) {

                    $scope.times[j].isAvailable = avaiDate.avaiTimes[j].isAvailable;

                    if ($scope.selectedTime == $scope.times[j] && !$scope.times[j].isAvailable)
                        $scope.selectedTime = null;
                }

                break;
            }
        }

    }


    $scope.appoint = function () {
        if ($scope.selectedBeautician == null) {
            dialogService.showMention(1, "请选择技师");
            return;
        }
        if ($scope.selectedDate == null) {
            dialogService.showMention(1, "请选择日期");
            return;
        }
        if ($scope.selectedTime == null) {
            dialogService.showMention(1, "请选择时间");
            return;
        }
        
        var json = {
            "serviceId": $scope.serviceId,
            "employeeId": $scope.selectedBeautician.employeeId,
            "appointmentDate": $scope.selectedDate.date.toDateString() + "T" + $scope.selectedTime.time.toString()//"2017-3-9 14:30:00"
        };

        
        appointmentAPIService.createAppointment(json).then(function (resp) {

            var p = {
                serviceName: $scope.service.serviceName,
                beautician: $scope.selectedBeautician.nickName,
                date: $scope.selectedDate.date.toDateString() + " " + $scope.selectedTime.time.toString()
            };
            routeService.goParams("appointment_success", p);
        });
    };

}]);