app.controller("login", ["$scope", "authAPIService", "storageService", "routeService", "dialogService", function ($scope, authAPIService, storageService, routeService, dialogService) {
    $scope.login = function () {
        
        if (!$scope.mobile || $scope.mobile.length == 0) {
            dialogService.showMention(1, "请输入手机号码");
            return;
        }

        if (!$scope.password || $scope.password.length == 0) {
            dialogService.showMention(1, "请输入密码");
            return;
        }

        authAPIService.auth($scope.mobile, $scope.password).then(function (resp) {
            if (resp.data.length == 1) {
                storageService.setJWT(resp.data[0].jwt);
                
                if (window.control == undefined || control.addUMengAlias == undefined) {
                    dialogService.showMention(1, "通知只能在APP里接收，您目前的设备无法收到通知", "home");
                }
                else {
                    control.addUMengAlias(storageService.getUserId().toString(), "jiyunorg");
                    routeService.goParams("home");
                }
                
                
            }
            else {
                storageService.setAuthSalons(JSON.stringify(resp.data));
                routeService.goParams("salon_select");
            }
        });
    };

    $scope.focusPwd = function () {
        document.getElementById("txt_password").focus();
    };

    //var height = $(window).height();
    //$("body").css("height", height);
}]);

