app.controller("appointments", ["$scope", "appointmentAPIService", function ($scope, appointmentAPIService) {
    $scope.title = "预约";
    $scope.routeName = "home";
    $scope.dataForLoadMore = null;
    var pageNumber = 1;
    $scope.appointments = null;

    function getAppointments() {
        appointmentAPIService.getAppointments(pageNumber).then(function (resp) {
            if (pageNumber == 1)
                $scope.appointments = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.appointments.push(resp.data[i]);
            }

            if ($scope.appointments.length == 0)//$scope.appointments.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;
        });
    }

    getAppointments();

    appointmentAPIService.getTodayAppointmentCount().then(function (resp) {
        $scope.appointmentTodayCount = resp.data;
    });

    $scope.loadMore = function () {
        pageNumber++;
        getAppointments();
    };
}]);