app.controller("profile", ["$scope", "employeeAPIService", "dialogService", function ($scope, employeeAPIService, dialogService) {
    $scope.title = "修改个人资料";
    $scope.routeName = "setup";
    $scope.employee = {};
    
    $scope.employeeDayOffs = [];
    $scope.weekItems = [
        { text: "周一", value: 1 },
        { text: "周二", value: 2 },
        { text: "周三", value: 3 },
        { text: "周四", value: 4 },
        { text: "周五", value: 5 },
        { text: "周六", value: 6 },
        { text: "周日", value: 7 }];
    
    
    employeeAPIService.getEmployee().then(function (resp) {
        $scope.employee = resp.data;
        $scope.employee.picture = null;
        $scope.employeeDayOffs = [];
        if ($scope.employee.isDayoffMon)
            $scope.employeeDayOffs.push(1);
        if ($scope.employee.isDayoffTue)
            $scope.employeeDayOffs.push(2);
        if ($scope.employee.isDayoffWeb)
            $scope.employeeDayOffs.push(3);
        if ($scope.employee.isDayoffThu)
            $scope.employeeDayOffs.push(4);
        if ($scope.employee.isDayoffFri)
            $scope.employeeDayOffs.push(5);
        if ($scope.employee.isDayoffSat)
            $scope.employeeDayOffs.push(6);
        if ($scope.employee.isDayoffSun)
            $scope.employeeDayOffs.push(7);
    });

    $scope.receiveDataURL = function (dataURL) {
        $scope.employee.picture = dataURL;
        $scope.employee.smallPicture = dataURL;
    };

    $scope.submit = function () {
        $scope.employee.isDayoffMon = false;
        $scope.employee.isDayoffTue = false;
        $scope.employee.isDayoffWeb = false;
        $scope.employee.isDayoffThu = false;
        $scope.employee.isDayoffFri = false;
        $scope.employee.isDayoffSat = false;
        $scope.employee.isDayoffSun = false;            

        for (var i = 0; i < $scope.employeeDayOffs.length; i++) {
            if ($scope.employeeDayOffs[i] == 1) {
                $scope.employee.isDayoffMon = true;
                continue;
            }
            if ($scope.employeeDayOffs[i] == 2) {
                $scope.employee.isDayoffTue = true;
                continue;
            }
            if ($scope.employeeDayOffs[i] == 3) {
                $scope.employee.isDayoffWeb = true;
                continue;
            }
            if ($scope.employeeDayOffs[i] == 4) {
                $scope.employee.isDayoffThu = true;
                continue;
            }
            if ($scope.employeeDayOffs[i] == 5) {
                $scope.employee.isDayoffFri = true;
                continue;
            }
            if ($scope.employeeDayOffs[i] == 6) {
                $scope.employee.isDayoffSat = true;
                continue;
            }
            if ($scope.employeeDayOffs[i] == 7) {
                $scope.employee.isDayoffSun = true;
                continue;
            }
        }

        if ($scope.employee.picture != null)
            $scope.employee.picture = $scope.employee.picture.replace(/data:[^,]+,/, "");

        employeeAPIService.updateProfile($scope.employee).then(function (resp) {
            dialogService.showMention(1, "修改个人资料成功", "setup");
        });
    };
}]);