app.controller("salon_edit", ["$scope", "salonAPIService", "dialogService", function ($scope, salonAPIService, dialogService) {
    $scope.salonPicture = null;
    $scope.qrPicture = null;
    $scope.salon = {};
    $scope.title = "编辑店铺";
    $scope.routeName = "salon";


    salonAPIService.getSalon().then(function (resp) {
        $scope.salon = resp.data;
        $scope.salonPicture = resp.data.picture;
        $scope.qrPicture = resp.data.qrCodePicture;
        $scope.salon.picture = null;
        $scope.salon.qrCodePicture = null;

        $scope.salon.openTime = $scope.salon.openTime.format("hh:mm");
        $scope.salon.closeTime = $scope.salon.closeTime.format("hh:mm");
    });


    $scope.submit = function () {
        if ($scope.salon.picture != null)
            $scope.salon.picture = $scope.salon.picture.replace(/data:[^,]+,/, "");
        if ($scope.salon.qrCodePicture != null)
            $scope.salon.qrCodePicture = $scope.salon.qrCodePicture.replace(/data:[^,]+,/, "");

        salonAPIService.updateSalon($scope.salon).then(function (resp) {
            dialogService.showMention(1, "修改美容院成功", "salon");
        });
    };

    $scope.receiveQRDataURL = function (dataURL) {
        $scope.salon.qrCodePicture = dataURL;
        $scope.qrPicture = dataURL;
    };

    $scope.receiveDataURL = function (dataURL) {
        $scope.salon.picture = dataURL;
        $scope.salonPicture = dataURL;
    };
}]);