app.controller("unavaitime", ["$scope", "unavaiTimeAPIService", "dialogService", "$timeout", function ($scope, unavaiTimeAPIService, dialogService, $timeout) {
    $scope.unavaiTimes = [];
    $scope.unavaiTime = {};
    $scope.title = "不可预约时间";
    $scope.routeName = "home";
    
   
    $scope.submit = function () {
        unavaiTimeAPIService.createUnavaiTime($scope.unavaiTime).then(function (resp) {
            $scope.unavaiTime.startTime = "";
            $scope.unavaiTime.endTime = "";
            dialogService.showMention(1, "不可预约时间添加成功");

            //if do not use $timeout, use getUnavaiTimes() directly, the button in showMention does not work, need research
            $timeout(getUnavaiTimes, 200);
            //getUnavaiTimes();
        });


    };

    function getUnavaiTimes() {
        unavaiTimeAPIService.getUnavaiTimes().then(function (resp) {
            $scope.unavaiTimes = resp.data;
        });
    }

    getUnavaiTimes();

    $scope.deleteUnavaiTime = function (time) {

        unavaiTimeAPIService.deleteUnavaiTime(time.unavaiId).then(function (resp) {
            $scope.unavaiTimes.splice($scope.unavaiTimes.indexOf(time), 1);
        });
    };

}]);