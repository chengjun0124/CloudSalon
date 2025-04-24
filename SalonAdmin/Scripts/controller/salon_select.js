app.controller("salon_select", ["$scope", "$rootScope", "routeService", "storageService", "dialogService", function ($scope, $rootScope, routeService, storageService, dialogService) {
    $scope.title = "选择美容院";
    $scope.routeName = "login";
    $scope.authSalons = JSON.parse(storageService.getAuthSalons());    
    
    if ($scope.authSalons == null || $scope.authSalons.length == 0) {
        routeService.goParams("login");
    }
    
    $scope.selectSalon = function (jwt) {
        storageService.setJWT(jwt);
        storageService.removeAuthSalons();        
        
        if (window.control == undefined || control.addUMengAlias == undefined) {
            dialogService.showMention(1, "通知只能在APP里接收，您目前的设备无法收到通知", "home");
        }
        else {
            control.addUMengAlias(storageService.getUserId().toString(), "jiyunorg");
            routeService.goParams("home");
        }
        
    };
}]);