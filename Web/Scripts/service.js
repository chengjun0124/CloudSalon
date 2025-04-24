(function () {

    app.factory("locationEx", ["$location", function ($location) {

        $location.queryString = function (query) {            
            var value = $location.absUrl().match(new RegExp("[?&]" + query + "=([^#&]*)"));
            return value == null ? null : value[1];
        };

        return $location;
    }]);

    app.factory('routeService', ["$state", function ($state) {

        return {
            goParams: function (routeName, params) {
                $('html, body').scrollTop(0);

                $state.go(routeName, params, { reload: true });
            }
        };

    }]);

    app.factory('dialogService', ["ngDialog", "$rootScope", "routeService", function (ngDialog, $rootScope, routeService) {
        var uploadImageHandler;
        var mentionHandler;
        var serviceTypesHandler;
        var loadingHandler;

        return {
            showMention: function (buttonCount, message, routeName, params, callBack) {
                //if (!Array.isArray(messageList))
                //    messageList = [messageList];
                mentionHandler = ngDialog.open({
                    template: $rootScope.dialogMentionTemplate,
                    plain: false,
                    showClose: false,
                    closeByEscape: false,
                    overlay: true,
                    className: "ngdialog-theme-mention",
                    closeByDocument: false,
                    controller: ["$scope", function ($scope) {
                        $scope.$on("browserBack", function (e) {
                            e.callBack = function () {
                                mentionHandler.close();
                            };
                        });

                        $scope.close = function () {
                            mentionHandler.close();
                            if (routeName)
                                routeService.goParams(routeName, params);
                        };

                        $scope.confirm = function () {
                            mentionHandler.close();
                            callBack();
                            if (routeName)
                                routeService.goParams(routeName,params);
                        };
                    }],
                    data: { buttonCount: buttonCount, content: message }
                });
            },
            showUploadImage: function (base64Image, cutWidth, cutHeight, callBack) {
                uploadImageHandler = ngDialog.open({
                    template: $rootScope.dialogUploadImageTemplate,
                    plain: false,
                    showClose: false,
                    closeByEscape: false,
                    overlay: false,
                    className: "ngdialog-theme-upload-image",
                    closeByDocument: false,
                    data: { base64Image: base64Image },
                    controller: ["$scope", function ($scope) {
                        $scope.$on("browserBack", function (e) {
                            e.callBack = function () {
                                uploadImageHandler.close();
                            };
                            e.preventDefault();
                        });
                        var startPosition = { x: null, y: null };
                        var offset = { x: null, y: null, w: null, h: null };
                        var imgLeft, imgTop, imgHeight, imgWidth;
                        var img;
                        var windowWidth = $(window).width();
                        var windowHeight = $(window).height();
                        var oImgWidth, oImgHeight;
                        var oImgLeft, oImgTop;
                        var fingerLeftIndex, fingerRightIndex, fingerUpIndex, fingerDownIndex;
                        var fingerLeftStart, fingerRightStart, fingerUpStart, fingerDownStart;
                        var divCutWidth = cutWidth;
                        var divCutHeight = cutHeight;
                        
                        var minCutWidth = 500;
                        var minCutHeight = 600;
                        //如果裁剪框太小，小于最小裁剪框尺寸500*600，裁剪框按自身比例放大，但放大后不超过最小裁剪框尺寸
                        if (divCutWidth < minCutWidth && divCutHeight < minCutHeight) {
                            var ratio = minCutWidth / minCutHeight;

                            //ratio < divCutWidth / divCutHeight表示： 最小裁剪框尺寸比裁剪框瘦
                            if (ratio < divCutWidth / divCutHeight) {
                                //把裁剪框宽度，放大到最小裁剪框宽度
                                divCutWidth = minCutWidth;
                                divCutHeight = cutHeight * (divCutWidth / cutWidth);
                            }
                            else {
                                divCutHeight = minCutHeight;
                                divCutWidth = cutWidth * (divCutHeight / cutHeight);                                
                            }
                        }

                        var divCutBorderWidth = 1000;
                        var divCutLeft = (windowWidth - divCutWidth - (divCutBorderWidth * 2)) / 2;
                        var divCutTop = (windowHeight - divCutHeight - (divCutBorderWidth * 2)) / 2;

                        $scope.base64Image = base64Image;
                        $scope.divCutCSS = {
                            "width": divCutWidth + divCutBorderWidth * 2 + "px",
                            "height": divCutHeight + divCutBorderWidth * 2 + "px",
                            "border-width": divCutBorderWidth + "px",
                            "left": divCutLeft + "px",
                            "top": divCutTop + "px"
                        };

                        $scope.cancel = function () {
                            uploadImageHandler.close();
                        };


                        $scope.imgLoaded = function () {
                            img = $("#oimg");
                            oImgWidth = img.width();
                            oImgHeight = img.height();

                            alignDivCut();

                            $(".ngdialog-content").bind("touchstart", function (e) {
                                //e.preventDefault();
                                if (e.touches.length == 1) {
                                    startPosition.x = e.touches[0].screenX;
                                    startPosition.y = e.touches[0].screenY;
                                    offset.x = 0;
                                    offset.y = 0;
                                }

                                if (e.touches.length == 2) {

                                    imgLeft += offset.x;
                                    imgTop += offset.y;

                                    if (e.touches[0].screenX < e.touches[1].screenX) {
                                        fingerLeftIndex = 0;
                                        fingerLeftStart = e.touches[0].screenX;

                                        fingerRightIndex = 1;
                                        fingerRightStart = e.touches[1].screenX;
                                    }
                                    else {
                                        fingerLeftIndex = 1;
                                        fingerLeftStart = e.touches[1].screenX;

                                        fingerRightIndex = 0;
                                        fingerRightStart = e.touches[0].screenX;
                                    }

                                    if (e.touches[0].screenY < e.touches[1].screenY) {
                                        fingerUpIndex = 0;
                                        fingerUpStart = e.touches[0].screenY;

                                        fingerDownIndex = 1;
                                        fingerDownStart = e.touches[1].screenY;
                                    }
                                    else {
                                        fingerUpIndex = 1;
                                        fingerUpStart = e.touches[1].screenY;

                                        fingerDownIndex = 0;
                                        fingerDownStart = e.touches[0].screenY;
                                    }

                                    offset.x = 0;
                                    offset.y = 0;
                                    offset.w = 0;
                                    offset.h = 0;

                                }
                            });

                            $(".ngdialog-content").bind("touchmove", function (e) {
                                e.preventDefault();
                                if (e.touches.length == 1) {
                                    offset.x = e.touches[0].screenX - startPosition.x;
                                    offset.y = e.touches[0].screenY - startPosition.y;

                                    img.css({
                                        left: imgLeft + offset.x + "px",
                                        top: imgTop + offset.y + "px"
                                    });
                                }

                                if (e.touches.length == 2) {
                                    /*双指放大缩小算法：
                                    1. 先获得左指偏移量，再加右指偏移量，就算出了宽度偏移量
                                    2. 根据上面的宽度偏移量，按比例计算出高度偏移量
                                    3. 高度偏移量：上面的高度偏移量，加上指偏移量，再加下指偏移量
                                    4. 根据上面的高度偏移量，按比例重新计算出宽度偏移量
                                    5. 高度和宽度分别除2，得到left和top的偏移量
                                    效果：始终从中间向四周扩散或收缩
                                    */
                                    offset.w = fingerLeftStart - e.touches[fingerLeftIndex].screenX;
                                    offset.w += e.touches[fingerRightIndex].screenX - fingerRightStart;
                                    offset.h = oImgHeight * offset.w / oImgWidth;

                                    offset.h += fingerUpStart - e.touches[fingerUpIndex].screenY;
                                    offset.h += e.touches[fingerDownIndex].screenY - fingerDownStart;
                                    offset.w = oImgWidth * offset.h / oImgHeight;

                                    offset.x = -(offset.w / 2);
                                    offset.y = -(offset.h / 2);

                                    img.css({
                                        width: imgWidth + offset.w + "px",
                                        left: imgLeft + offset.x + "px",
                                        height: imgHeight + offset.h + "px",
                                        top: imgTop + offset.y + "px"
                                    });
                                }
                            });

                            $(".ngdialog-content").bind("touchend", function (e) {
                                //e.preventDefault();
                                if (e.touches.length == 1) {
                                    //拿走了第二个手指
                                    startPosition.x = e.touches[0].screenX;
                                    startPosition.y = e.touches[0].screenY;


                                    imgWidth = imgWidth + offset.w;
                                    imgHeight = imgHeight + offset.h;
                                    imgLeft = imgLeft + offset.x;
                                    imgTop = imgTop + offset.y;

                                    offset.x = 0;
                                    offset.y = 0;
                                    offset.w = 0;
                                    offset.h = 0;
                                }
                                if (e.touches.length == 0) {
                                    imgLeft += offset.x;
                                    imgTop += offset.y;

                                    offset.x = 0;
                                    offset.y = 0;

                                    if (imgWidth < divCutWidth || imgHeight < divCutHeight) {
                                        alignDivCut();
                                    }
                                    else {
                                        //拖动结束时，如果裁剪框有空白，需移动图片，直至裁剪框没空

                                        //裁剪框左边有空
                                        if (imgLeft > divCutLeft + divCutBorderWidth)
                                            imgLeft = divCutLeft + divCutBorderWidth;

                                        //裁剪框上边有空
                                        if (imgTop > divCutTop + divCutBorderWidth)
                                            imgTop = divCutTop + divCutBorderWidth;

                                        //裁剪框右边有空
                                        if (windowWidth - imgLeft - imgWidth > windowWidth - divCutWidth - divCutBorderWidth - divCutLeft) {
                                            //windowWidth - divCutWidth - divCutBorderWidth - divCutLeft表示的是浏览器右边到裁剪框右边，再加上裁剪框边框宽的距离
                                            imgLeft = windowWidth - imgWidth - (windowWidth - divCutWidth - divCutBorderWidth - divCutLeft);
                                        }

                                        //裁剪框下边有空
                                        if (windowHeight - imgTop - imgHeight > windowHeight - divCutHeight - divCutBorderWidth - divCutTop)
                                            imgTop = windowHeight - imgHeight - (windowHeight - divCutHeight - divCutBorderWidth - divCutTop);


                                        img.css({
                                            top: imgTop + "px",
                                            left: imgLeft + "px"
                                        });
                                    }
                                }
                            });
                        };

                        function alignDivCut() {
                            //保证裁剪器里面的图片铺满，不出现四周留空的现象
                            var ratio = divCutWidth / divCutHeight;

                            /*
                            如果裁剪器的宽高比小于原始图像的宽高比，即裁剪器的宽高比比原始图片的宽高比更窄（细长），那么图像的高度设置成等于裁剪器高度，图像的宽度按比例放大或缩小
                            更窄（细长）意思是：
                            —————   —————————————
                            |   |   |           |
                            |   |   |           |
                            |   |   |           |
                            |   |   |           |
                            —————   —————————————
                            裁剪器     原始图片
                            */
                            if (ratio < oImgWidth / oImgHeight) {
                                imgHeight = divCutHeight;
                                imgWidth = oImgWidth * (imgHeight / oImgHeight);
                            }
                            else {
                                //反之图像的宽度设置成等于裁剪器宽度，图像的高度按比例放大或缩小
                                imgWidth = divCutWidth;
                                imgHeight = oImgHeight * (imgWidth / oImgWidth);
                            }

                            imgTop = (windowHeight - imgHeight) / 2;
                            imgLeft = (windowWidth - imgWidth) / 2;

                            oImgLeft = imgLeft;
                            oImgTop = imgTop;

                            img.css({
                                position: "fixed",
                                width: imgWidth + "px",
                                height: imgHeight + "px",
                                top: imgTop + "px",
                                left: imgLeft + "px"
                            });
                        }

                        $scope.cut = function () {
                            var canvas = document.getElementById("canvas");
                            canvas.width = cutWidth;
                            canvas.height = cutHeight;
                            var cxt = canvas.getContext("2d");

                            var ratio = imgWidth / oImgWidth;
                            var sourceWidth = divCutWidth / ratio;
                            var sourceHeight = divCutHeight / ratio;
                            var sourceLeft = (divCutLeft + divCutBorderWidth - imgLeft) / ratio;
                            var sourceTop = (divCutTop + divCutBorderWidth - imgTop) / ratio;

                            //第一个参数和其它的是相同的，都是一个图像或者另一个 canvas 的引用。其它8个参数最好是参照右边的图解，前4个是定义图像源的切片位置和大小，后4个则是定义切片的目标显示位置和大小
                            cxt.drawImage(img[0], sourceLeft, sourceTop, sourceWidth, sourceHeight, 0, 0, cutWidth, cutHeight);

                            callBack(canvas.toDataURL("image/jpeg"));

                            uploadImageHandler.close();
                        };
                    }]
                });
            },
            showLoading: function () {
                loadingHandler = ngDialog.open({
                    template: "<div class=\"sk-circle\">" +
                                        "<div class=\"sk-circle1 sk-child\"></div>" +
                                        "<div class=\"sk-circle2 sk-child\"></div>" +
                                        "<div class=\"sk-circle3 sk-child\"></div>" +
                                        "<div class=\"sk-circle4 sk-child\"></div>" +
                                        "<div class=\"sk-circle5 sk-child\"></div>" +
                                        "<div class=\"sk-circle6 sk-child\"></div>" +
                                        "<div class=\"sk-circle7 sk-child\"></div>" +
                                        "<div class=\"sk-circle8 sk-child\"></div>" +
                                        "<div class=\"sk-circle9 sk-child\"></div>" +
                                        "<div class=\"sk-circle10 sk-child\"></div>" +
                                        "<div class=\"sk-circle11 sk-child\"></div>" +
                                        "<div class=\"sk-circle12 sk-child\"></div>" +
                                    "</div>",
                    className: "ngdialog-theme-loading",
                    plain: true,
                    showClose: false,
                    closeByEscape: false,
                    closeByDocument: false,
                    controller: ["$scope", function ($scope) {
                        $scope.$on("browserBack", function (e) {
                            e.callBack = function () {
                                loadingHandler.close();
                            };
                        });
                    }]
                });

                return loadingHandler;
            },
            showServiceTypes: function (serviceTypes, selectedType, callback) {
                serviceTypesHandler = mentionHandler = ngDialog.open({
                    template: $rootScope.dialogServiceTypesTemplate,
                    plain: false,
                    showClose: false,
                    closeByEscape: false,
                    overlay: true,
                    className: "ngdialog-theme-service-types",
                    closeByDocument: true,
                    controller: ["$scope", function ($scope) {
                        $scope.$on("browserBack", function (e) {
                            e.callBack = function () {
                                serviceTypesHandler.close();
                            };
                        });
                        $scope.serviceTypes = $scope.ngDialogData.serviceTypes;
                        $scope.selectedType = $scope.ngDialogData.selectedType;

                        $scope.excludeAll = function (item) {
                            return item.serviceTypeId != null;
                        };

                        $scope.selectType = function (type) {
                            serviceTypesHandler.close();
                            callback(type);
                        };
                    }],
                    data: { serviceTypes: serviceTypes, selectedType: selectedType }
                });
            }
        };
    }]);

    app.factory("storageService", [function () {
        return {
            set: function (key, value) {
                localStorage[key] = value;
            },
            get: function (key) {
                return localStorage[key];
            },
            setSession: function (key, value) {
                sessionStorage[key] = value;
            },
            getSession: function (key) {
                return sessionStorage[key];
            }
        };
    }]);

    app.factory('apiService', ["$http", "routeService", "ngDialog", "storageService", "locationEx", "dialogService", "$state", "$rootScope", function ($http, routeService, ngDialog, storageService, locationEx, dialogService, $state, $rootScope) {
        var callAPICount = 0;
        var loadingHandler = null;

        function defaultAPIErrorHandler(error) {
            if (error.status == 500)
                routeService.goParams("error");
            else if (error.status == 404)
                routeService.goParams("error", { msg: encodeURIComponent("您请求的数据不存在，请返回首页重新浏览") });
            else if (error.status == 400) {
                dialogService.showMention(1, error.data[0]);
            }
            else if (error.status == 401)
                routeService.goParams("login", { "redirect": encodeURIComponent("{ \"state\": \"" + $state.current.name + "\", \"params\": " + JSON.stringify($state.params) + "}") });
            else
                routeService.goParams("error");
            return false;
        }


        function promiseWrapper(p) {
            this.promise = p;

            this.then = function (success, customError) {
                var p = this.promise.then(function (resp) {
                    if (!resp) return false;

                    callAPICount--;
                    if (callAPICount == 0) {
                        loadingHandler.close();
                    }

                    var o = null;
                    if (success)
                        o = success(resp);
                    

                    if (o instanceof promiseWrapper)
                        return o.promise;
                    else
                        return o;
                },
                function (error) {
                    callAPICount = 0;
                    loadingHandler.close();

                    if (customError == null)
                        defaultAPIErrorHandler(error);
                    else {
                        var o = customError(error);

                        if (o instanceof promiseWrapper)
                            return o.promise;
                        else
                            return o;
                    }
                }
                );
                return new promiseWrapper(p);
            };
        };

        return {
            call: function (method, path, params) {
                if (callAPICount == 0) {
                    loadingHandler = dialogService.showLoading();
                }
                callAPICount++;

                var identityCode = locationEx.queryString("identitycode");
                var token = storageService.get(identityCode);
                var headers = {};
                if (token != null)
                    headers.Authorization = "JWT " + token;

                var promise = $http({ url: $rootScope.APIURL + path, method: method, data: params, headers: headers });
                return new promiseWrapper(promise);
            }

        };
    }]);
})();