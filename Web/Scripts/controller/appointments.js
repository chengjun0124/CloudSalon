app.controller("appointments", ["$scope", "appointmentAPIService", "$rootScope", function ($scope, appointmentAPIService, $rootScope) {
    $rootScope.pageTitle = "预约记录";
    $scope.date = new Date();
    getAppointments();


    /*JS的setMonth方法有顺延现象: 当前月份如果没有31天,而设置了31的话,当前月份会顺延,
    简单的说: 当前是10月31日,下一天是11月1日,所以就先setMonth(11),然后再setDate(1),
    理论上来说: 现在setMonth后应该变为了11月31日,然后再setDate 之后就变为了  11月1日. 
    但是:由于有顺延现象,在设置为11月后,由于11月没有31天,所以月份就顺延了,变为了12月1日. 然后再设置setDate(1), 所以最终的结果是12月1日
    1.3解决方法
    (1)设置月份时,将日期设为1,  即 setMonth(month,1), 这样就不会顺延了(也可以在setMonth之前先调用setDate设置日期)
    (2)可以使用setFullYear()同时设置年、月、日，即setFullYear(year,month,date)*/
    $scope.nextMonth = function () {
        $scope.date.setMonth($scope.date.getMonth() + 1, 1);
        getAppointments();
    };

    $scope.previousMonth = function () {
        $scope.date.setMonth($scope.date.getMonth() - 1, 1);
        getAppointments();
    };


    function getAppointments() {
        appointmentAPIService.getUserAppointments($scope.date.format("YYYY-MM")).then(function (resp) {
            $scope.appointments = resp.data;
        });
    }

    




}]);