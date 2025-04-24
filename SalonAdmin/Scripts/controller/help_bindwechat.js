app.controller("help_bindwechat", ["$scope", "$rootScope", "dialogService", "storageService", function ($scope, $rootScope, dialogService, storageService) {
    $scope.title = "绑定微信公众号";
    $scope.routeName = "help_applywechat";
    //$rootScope.setWhiteBody = true;
    $scope.identityCode = storageService.getIdentityCode();
    

    $scope.copyURL = function (inputId, moduleName) {
        var input = document.getElementById(inputId);
        input.focus();
        input.selectionStart = 0;
        input.selectionEnd = input.value.length;
        document.execCommand("Copy");
        input.blur();

        dialogService.showMention(1, "复制“" + moduleName + "”网址成功");
    };
}]);