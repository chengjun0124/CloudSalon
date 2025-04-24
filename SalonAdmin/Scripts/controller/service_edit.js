app.controller("service_edit", ["$scope", "serviceAPIService", "$stateParams", "dialogService", "$sce", "tagAPIService", function ($scope, serviceAPIService, $stateParams, dialogService, $sce, tagAPIService) {
    $scope.serviceId = $stateParams.serviceId;
    $scope.isCreate = $stateParams.serviceId == "" ? true : false;
    $scope.title = $scope.isCreate ? "新增服务" : "编辑服务";
    $scope.routeName = "services";
    $scope.serviceTypes = null;
    $scope.serviceTypeTags = [];
    $scope.inputedTag = ""
    
    $scope.pains = [
        { value: 0, text: "无疼痛" },
        { value: 1, text: "<img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/>" },
        { value: 2, text: "<img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/>" },
        { value: 3, text: "<img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/>" },
        { value: 4, text: "<img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/>" },
        { value: 5, text: "<img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/><img src=\"../imgs/star.png\" class=\"shrink_17 va_baseline\"/>" }
    ];
    $scope.service = {};
    $scope.service.effectImages = [];
    $scope.service.functionalityTags = [];

    serviceAPIService.getServiceTypes().then(function (resp) {
        $scope.serviceTypes = resp.data;
    });

    if (!$scope.isCreate) {
        serviceAPIService.getService($scope.serviceId).then(function (resp) {
            $scope.service = resp.data;
            $scope.subjectImage = $scope.service.subjectImage;
            $scope.service.subjectImage = null;

            for (var i = 0; i < $scope.service.effectImages.length; i++) {
                var index = $scope.service.effectImages[i].image.lastIndexOf("/");
                $scope.service.effectImages[i].image = $scope.service.effectImages[i].image.substr(index + 1);
            }
        });
    }

    $scope.$watch("service.serviceTypeId", function () {
        if ($scope.service.serviceTypeId != undefined) {
            tagAPIService.getServiceTypeTags($scope.service.serviceTypeId).then(function (tags) {
                $scope.serviceTypeTags = tags.data;

            });
        }
    });

    $scope.deleteTag = function (index) {
        $scope.service.functionalityTags.splice(index, 1);
    };

    $scope.addTag = function (tag) {
        if ($scope.service.functionalityTags.indexOf(tag) == -1) {
            if ($scope.service.functionalityTags.length >= 5) {
                dialogService.showMention(1, "服务功效最多放置5个标签");
            }
            else {
                $scope.service.functionalityTags.push(tag);
            }
        }
    };

    $scope.inputTag = function () {
        var tag = $scope.inputedTag.replace(/\s/g, "");
        if (tag.length > 0)
            $scope.addTag(tag);
        $scope.inputedTag = null;
    };



    $scope.submit = function () {
        if ($scope.service.subjectImage != null)
            $scope.service.subjectImage = $scope.service.subjectImage.replace(/data:[^,]+,/, "");

        if ($scope.isCreate) {
            serviceAPIService.createService($scope.service).then(function (resp) {
                dialogService.showMention(1, "服务创建成功", "services");
            });
        }
        else {
            serviceAPIService.updateService($scope.service).then(function (resp) {
                dialogService.showMention(1, "服务修改成功", "services");
            });
        }
    };

    $scope.receiveDataURL = function (dataURL) {
        $scope.service.subjectImage = dataURL;
        $scope.subjectImage = dataURL;
    };

    $scope.receiveEffectImageDataURL = function (dataURL) {
        $scope.service.effectImages.push({
            image: dataURL.replace(/data:[^,]+,/, ""),
            smallImage: dataURL
        });
    };

    $scope.deleteEffectImage = function (index) {
        dialogService.showMention(2, "确定删除此效果图？", null, null, function () {
            $scope.service.effectImages.splice(index, 1);
        });
    };
}]);
