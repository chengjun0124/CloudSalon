app.controller("employees", ["$scope", "employeeAPIService", "dialogService", function ($scope, employeeAPIService, dialogService) {
    $scope.title = "员工";
    $scope.routeName = "home";
    var pageNumber = 1;
    $scope.dataForLoadMore = null;
    $scope.employees = null;
    
    function getEmployees() {
        employeeAPIService.getEmployees(pageNumber).then(function (resp) {
            if (pageNumber == 1)
                $scope.employees = [];

            if (resp.data.length > 0) {
                for (var i = 0; i < resp.data.length; i++)
                    $scope.employees.push(resp.data[i]);
            }

            if ($scope.employees.length == 0)//$scope.employees.length==0表示无数据
                $scope.dataForLoadMore = null;//把$scope.dataForLoadMore设置成null,页面不显示“已加载全部”
            else
                $scope.dataForLoadMore = resp.data;
        });
    }

    function getEmployeeCount() {
        employeeAPIService.getEmployeeCount().then(function (resp) {
            $scope.employeeCount = resp.data;
        });
    }

    getEmployees();
    getEmployeeCount();

    $scope.loadMore = function () {
        pageNumber++;
        getEmployees();
    };

    $scope.deleteEmployee = function ($event, employeeId, index) {
        $event.preventDefault();
        dialogService.showMention(2, "确定要删除此员工吗？", null, null, function () {
            employeeAPIService.deleteEmployee(employeeId).then(function (resp) {
                $scope.employeeCount--;
                $scope.employees.splice(index, 1);
            });
        });
    };


    $scope.workStatus = function (employee) {
        var day = new Date().getDay();
        var workingDay = "工作日";
        var offWorkingDay = "休息日";
        var returnValue;
        switch(day)
        {
            case 0:
                returnValue = employee.isDayoffSun ? offWorkingDay : workingDay;
                break;
            case 1:
                returnValue = employee.isDayoffMon ? offWorkingDay : workingDay;
                break;
            case 2:
                returnValue = employee.isDayoffTue ? offWorkingDay : workingDay;
                break;
            case 3:
                returnValue = employee.isDayoffWeb ? offWorkingDay : workingDay;
                break;
            case 4:
                returnValue = employee.isDayoffThu ? offWorkingDay : workingDay;
                break;
            case 5:
                returnValue = employee.isDayoffFri ? offWorkingDay : workingDay;
                break;
            case 6:
                returnValue = employee.isDayoffSat ? offWorkingDay : workingDay;
                break;
        }

        return returnValue;
    }
}]);