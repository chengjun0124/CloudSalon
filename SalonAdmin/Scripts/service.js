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

    app.factory("encodeService", [function () {
        var _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        // private method for UTF-8 encoding
        function _utf8_encode(string) {
            string = string.replace(/\r\n/g, "\n");
            var utftext = "";
            for (var n = 0; n < string.length; n++) {
                var c = string.charCodeAt(n);
                if (c < 128) {
                    utftext += String.fromCharCode(c);
                } else if ((c > 127) && (c < 2048)) {
                    utftext += String.fromCharCode((c >> 6) | 192);
                    utftext += String.fromCharCode((c & 63) | 128);
                } else {
                    utftext += String.fromCharCode((c >> 12) | 224);
                    utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                    utftext += String.fromCharCode((c & 63) | 128);
                }

            }
            return utftext;
        };
        // private method for UTF-8 decoding
        function _utf8_decode(utftext) {
            var string = "";
            var i = 0;
            var c = c1 = c2 = 0;
            while (i < utftext.length) {
                c = utftext.charCodeAt(i);
                if (c < 128) {
                    string += String.fromCharCode(c);
                    i++;
                } else if ((c > 191) && (c < 224)) {
                    c2 = utftext.charCodeAt(i + 1);
                    string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                    i += 2;
                } else {
                    c2 = utftext.charCodeAt(i + 1);
                    c3 = utftext.charCodeAt(i + 2);
                    string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                    i += 3;
                }
            }
            return string;
        };

        return {
            base64Encode: function (input) {
                var output = "";
                var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
                var i = 0;
                input = _utf8_encode(input);
                while (i < input.length) {
                    chr1 = input.charCodeAt(i++);
                    chr2 = input.charCodeAt(i++);
                    chr3 = input.charCodeAt(i++);
                    enc1 = chr1 >> 2;
                    enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                    enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                    enc4 = chr3 & 63;
                    if (isNaN(chr2)) {
                        enc3 = enc4 = 64;
                    } else if (isNaN(chr3)) {
                        enc4 = 64;
                    }
                    output = output +
                            _keyStr.charAt(enc1) + _keyStr.charAt(enc2) +
                            _keyStr.charAt(enc3) + _keyStr.charAt(enc4);
                }
                return output;
            },
            base64Decode: function (input) {
                var output = "";
                var chr1, chr2, chr3;
                var enc1, enc2, enc3, enc4;
                var i = 0;
                input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
                while (i < input.length) {
                    enc1 = _keyStr.indexOf(input.charAt(i++));
                    enc2 = _keyStr.indexOf(input.charAt(i++));
                    enc3 = _keyStr.indexOf(input.charAt(i++));
                    enc4 = _keyStr.indexOf(input.charAt(i++));
                    chr1 = (enc1 << 2) | (enc2 >> 4);
                    chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                    chr3 = ((enc3 & 3) << 6) | enc4;
                    output = output + String.fromCharCode(chr1);
                    if (enc3 != 64) {
                        output = output + String.fromCharCode(chr2);
                    }
                    if (enc4 != 64) {
                        output = output + String.fromCharCode(chr3);
                    }
                }
                output = _utf8_decode(output);
                return output;
            }
        };
    }]);

    app.factory('dialogService', ["ngDialog", "$rootScope", "routeService", function (ngDialog, $rootScope, routeService) {
        var mentionHandler;
        var uploadImageHandler;
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
                        $scope.close = function () {
                            mentionHandler.close();
                            if (routeName)
                                routeService.goParams(routeName, params);
                        };

                        $scope.confirm = function () {
                            mentionHandler.close();
                            callBack();
                            if (routeName)
                                routeService.goParams(routeName, params);
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
                        var startPosition = { x: null, y: null };
                        var offset = { x: null, y: null, w: null, h: null };
                        var imgInfo = {};
                        var imgTempInfo = {};
                        var oImgWidth, oImgHeight;

                        var img;
                        var windowWidth = $(window).width();
                        var windowHeight = $(window).height();

                        var fingerLeftIndex, fingerRightIndex, fingerUpIndex, fingerDownIndex;
                        var fingerLeftStart, fingerRightStart, fingerUpStart, fingerDownStart;
                        var divCutWidth = cutWidth;
                        var divCutHeight = cutHeight;
                        var cutterRatio = divCutWidth / divCutHeight;
                        var isMouseDown = false;

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

                            fillDivCutter();
                            refreshImg(imgInfo);

                            $(".ngdialog-content").bind("touchstart", function (e) {
                                if (e.touches.length == 1) {
                                    moveStart({
                                        x: e.touches[0].screenX,
                                        y: e.touches[0].screenY
                                    });
                                }

                                if (e.touches.length == 2) {
                                    moveStart({
                                        x: e.touches[0].screenX,
                                        y: e.touches[0].screenY
                                    },
                                    {
                                        x: e.touches[1].screenX,
                                        y: e.touches[1].screenY
                                    });
                                }
                            });
                            $(".ngdialog-content").bind("touchmove", function (e) {
                                e.preventDefault();

                                if (e.touches.length == 1) {
                                    move({
                                        x: e.touches[0].screenX,
                                        y: e.touches[0].screenY
                                    });
                                }

                                if (e.touches.length == 2) {
                                    move({
                                        x: e.touches[0].screenX,
                                        y: e.touches[0].screenY
                                    },
                                    {
                                        x: e.touches[1].screenX,
                                        y: e.touches[1].screenY
                                    });
                                }
                            });
                            $(".ngdialog-content").bind("touchend", function (e) {
                                //e.preventDefault();
                                if (e.touches.length == 1) {
                                    moveEnd({
                                        x: e.touches[0].screenX,
                                        y: e.touches[0].screenY
                                    });
                                }

                                if (e.touches.length == 0) {
                                    moveEnd();
                                }
                            });
                            $(".ngdialog-content").bind("mousedown", function (e) {
                                if (this.setCapture)
                                    this.setCapture();

                                isMouseDown = true;
                                moveStart({
                                    x: e.screenX,
                                    y: e.screenY
                                });
                            });
                            $(".ngdialog-content").bind("mousemove", function (e) {
                                if (isMouseDown) {
                                    e.preventDefault();
                                    move({
                                        x: e.screenX,
                                        y: e.screenY
                                    });
                                }
                            });
                            $(".ngdialog-content").bind("mouseup", function (e) {
                                if (this.releaseCapture)
                                    this.releaseCapture();
                                isMouseDown = false;
                                moveEnd();
                            });
                            $(".ngdialog-content").bind("mousewheel", function (e) {
                                img.css({
                                    "transition-property": "",
                                    "transition-duration": ""
                                });
                                var offset = 30;

                                if (e.originalEvent.wheelDelta > 0)//mouse wheel roll forward, enlarge image
                                    imgInfo = calculateImgInfo(imgInfo.width + offset, "width", false, true);
                                else {//mouse wheel roll back, shrink image                                    
                                    if (imgInfo.height - offset < divCutHeight || imgInfo.width - offset < divCutWidth) {
                                        if (cutterRatio < oImgWidth / oImgHeight)
                                            imgInfo = calculateImgInfo(divCutHeight, "height", false, true);
                                        else
                                            imgInfo = calculateImgInfo(divCutWidth, "width", false, true);
                                    }
                                    else
                                        imgInfo = calculateImgInfo(imgInfo.width - offset, "width", false, true);
                                }

                                refreshImg(imgInfo);
                            });

                            function moveStart(point1, point2) {
                                img.css({
                                    "transition-property": "",
                                    "transition-duration": ""
                                });

                                if (point2 == undefined) {
                                    startPosition.x = point1.x;
                                    startPosition.y = point1.y;
                                    offset.x = 0;
                                    offset.y = 0;
                                }

                                if (point2 != undefined) {
                                    imgInfo.left += offset.x;
                                    imgInfo.top += offset.y;

                                    if (point1.x < point2.x) {
                                        fingerLeftIndex = 0;
                                        fingerLeftStart = point1.x;

                                        fingerRightIndex = 1;
                                        fingerRightStart = point2.x;
                                    }
                                    else {
                                        fingerLeftIndex = 1;
                                        fingerLeftStart = point2.x;

                                        fingerRightIndex = 0;
                                        fingerRightStart = point1.x;
                                    }

                                    if (point1.y < point2.y) {
                                        fingerUpIndex = 0;
                                        fingerUpStart = point1.y

                                        fingerDownIndex = 1;
                                        fingerDownStart = point2.y
                                    }
                                    else {
                                        fingerUpIndex = 1;
                                        fingerUpStart = point2.y;

                                        fingerDownIndex = 0;
                                        fingerDownStart = point1.y;
                                    }
                                    offset.x = 0;
                                    offset.y = 0;
                                    offset.w = 0;
                                    offset.h = 0;
                                }
                            }
                            function move(point1, point2) {
                                if (point2 == undefined) {
                                    offset.x = point1.x - startPosition.x;
                                    offset.y = point1.y - startPosition.y;

                                    img.css({
                                        left: imgInfo.left + offset.x + "px",
                                        top: imgInfo.top + offset.y + "px"
                                    });
                                }

                                if (point2 != undefined) {
                                    /*双指放大缩小算法：
                                    1. 先获得左指偏移量，再加右指偏移量，就算出了宽度偏移量
                                    2. 根据上面的宽度偏移量，按比例计算出高度偏移量
                                    3. 高度偏移量：上面的高度偏移量，加上指偏移量，再加下指偏移量
                                    4. 根据上面的高度偏移量，按比例重新计算出宽度偏移量
                                    5. 高度和宽度分别除2，得到left和top的偏移量
                                    效果：始终从中间向四周扩散或收缩
                                    */

                                    var leftPoint = fingerLeftIndex == 0 ? point1 : point2;
                                    var rightPoint = fingerRightIndex == 0 ? point1 : point2;
                                    var upPoint = fingerUpIndex == 0 ? point1 : point2;
                                    var downPoint = fingerDownIndex == 0 ? point1 : point2;

                                    offset.w = fingerLeftStart - leftPoint.x;
                                    offset.w += rightPoint.x - fingerRightStart;
                                    imgTempInfo = calculateImgInfo(imgInfo.width + offset.w, "width", false, false);

                                    offset.h = fingerUpStart - upPoint.y;
                                    offset.h += downPoint.y - fingerDownStart;
                                    imgTempInfo = calculateImgInfo(imgTempInfo.height + offset.h, "height", false, false);

                                    refreshImg(imgTempInfo);
                                }
                            }
                            function moveEnd(point1) {
                                if (point1 != undefined) {
                                    //拿走了第二个手指
                                    startPosition.x = point1.x;
                                    startPosition.y = point1.y;

                                    imgInfo = imgTempInfo;

                                    offset.x = 0;
                                    offset.y = 0;
                                    offset.w = 0;
                                    offset.h = 0;
                                }

                                if (point1 == undefined) {
                                    imgInfo.left += offset.x;
                                    imgInfo.top += offset.y;

                                    offset.x = 0;
                                    offset.y = 0;

                                    if (imgInfo.width < divCutWidth || imgInfo.height < divCutHeight)
                                        fillDivCutter();
                                    else
                                        alignCutter(imgInfo);

                                    img.css({
                                        "transition-property": "left,top",
                                        "transition-duration": "0.3s"
                                    });
                                }

                                refreshImg(imgInfo);
                            }

                        };
                        function refreshImg(info) {
                            img.css({
                                width: info.width + "px",
                                left: info.left + "px",
                                height: info.height + "px",
                                top: info.top + "px"
                            });
                        }
                        function fillDivCutter() {
                            //保证裁剪器里面的图片铺满，不出现四周留空的现象。如果图片长宽都大于裁剪器，也会缩小图片，使图片的长或宽中的一条正好对准裁剪器

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
                            if (cutterRatio < oImgWidth / oImgHeight) {
                                imgInfo = calculateImgInfo(divCutHeight, "height", true, false);
                            }
                            else {
                                //反之图像的宽度设置成等于裁剪器宽度，图像的高度按比例放大或缩小
                                imgInfo = calculateImgInfo(divCutWidth, "width", true, false);
                            }
                        }
                        function alignCutter(imgInfo) {
                            //如果裁剪框内有空白，移动图片，使裁剪框内没空
                            //裁剪框左边有空
                            if (imgInfo.left > divCutLeft + divCutBorderWidth)
                                imgInfo.left = divCutLeft + divCutBorderWidth;

                            //裁剪框上边有空
                            if (imgInfo.top > divCutTop + divCutBorderWidth)
                                imgInfo.top = divCutTop + divCutBorderWidth;

                            //裁剪框右边有空
                            if (windowWidth - imgInfo.left - imgInfo.width > windowWidth - divCutWidth - divCutBorderWidth - divCutLeft) {
                                //windowWidth - divCutWidth - divCutBorderWidth - divCutLeft表示的是浏览器右边到裁剪框右边，再加上裁剪框边框宽的距离
                                imgInfo.left = windowWidth - imgInfo.width - (windowWidth - divCutWidth - divCutBorderWidth - divCutLeft);
                            }

                            //裁剪框下边有空
                            if (windowHeight - imgInfo.top - imgInfo.height > windowHeight - divCutHeight - divCutBorderWidth - divCutTop)
                                imgInfo.top = windowHeight - imgInfo.height - (windowHeight - divCutHeight - divCutBorderWidth - divCutTop);
                        }
                        function calculateImgInfo(newLength, side, isCenter, isAlignCutter) {
                            //according to new length to calculate height, width, left, top proportionally
                            var newWidth, newHeight;
                            if (side == "width") {
                                newWidth = newLength;
                                newHeight = oImgHeight * newWidth / oImgWidth;
                            }
                            else {
                                newHeight = newLength;
                                newWidth = oImgWidth * newHeight / oImgHeight;
                            }

                            var left, top;
                            if (isCenter) {
                                left = (windowWidth - newWidth) / 2;
                                top = (windowHeight - newHeight) / 2;
                            }
                            else {
                                left = imgInfo.left - (newWidth - imgInfo.width) / 2;
                                top = imgInfo.top - (newHeight - imgInfo.height) / 2;
                            }


                            var imageInfo = {
                                width: newWidth,
                                height: newHeight,
                                left: left,
                                top: top
                            };

                            if (isAlignCutter)
                                alignCutter(imageInfo);

                            return imageInfo;
                        }

                        $scope.cut = function () {
                            var canvas = document.getElementById("canvas");
                            canvas.width = cutWidth;
                            canvas.height = cutHeight;
                            var cxt = canvas.getContext("2d");

                            var ratio = imgInfo.width / oImgWidth;
                            var sourceWidth = divCutWidth / ratio;
                            var sourceHeight = divCutHeight / ratio;
                            var sourceLeft = (divCutLeft + divCutBorderWidth - imgInfo.left) / ratio;
                            var sourceTop = (divCutTop + divCutBorderWidth - imgInfo.top) / ratio;

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
                    closeByDocument: false
                });

                return loadingHandler;
            }
        };
    }]);

    app.factory("storageService", ["encodeService", function (encodeService) {
        return {
            getJWT: function () {
                return localStorage["jwt"];
            },
            setJWT: function (value) {
                localStorage["jwt"] = value;
            },
            removeJWT: function () {
                delete localStorage["jwt"];
            },
            getUserTypeId: function () {
                var claim = localStorage["jwt"].substr(0, localStorage["jwt"].indexOf("."));
                var json = encodeService.base64Decode(claim);
                json = JSON.parse(json);

                return json.UserTypeId;
            },
            getUserId: function () {
                var claim = localStorage["jwt"].substr(0, localStorage["jwt"].indexOf("."));
                var json = encodeService.base64Decode(claim);
                json = JSON.parse(json);

                return json.UserId;
            },
            getIsBeautician: function () {
                var claim = localStorage["jwt"].substr(0, localStorage["jwt"].indexOf("."));
                var json = encodeService.base64Decode(claim);
                json = JSON.parse(json);

                return json.IsBeautician;
            },
            getIdentityCode: function () {
                var claim = localStorage["jwt"].substr(0, localStorage["jwt"].indexOf("."));
                var json = encodeService.base64Decode(claim);
                json = JSON.parse(json);

                return json.IdentityCode;
            },
            getAuthSalons: function () {
                return localStorage["authSalons"];
            },
            setAuthSalons: function (value) {
                localStorage["authSalons"] = value;
            },
            removeAuthSalons: function () {
                delete localStorage["authSalons"];
            }
        };
    }]);

    app.factory('apiService', ["$http", "routeService", "ngDialog", "storageService", "dialogService", "$rootScope", function ($http, routeService, ngDialog, storageService, dialogService, $rootScope) {
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
                routeService.goParams("login");
            else
                routeService.goParams("error");
            return false;
        };


        function promiseWrapper(p) {
            this.promise = p;

            this.then = function (success, customError) {
                var p = this.promise.then(function (resp) {
                    if (!resp) return false;


                    //API return JSON which is aleady camel style(low case first letter), so need not convert there
                    //function pascalToCamel(key, val) {
                    //    var specialKeyList = ["id", "url"];

                    //    if (specialKeyList.indexOf(key.toLowerCase()) != -1 && specialKeyList.indexOf(key) == -1) {
                    //        this[key.toLowerCase()] = val;
                    //        return undefined;
                    //    }

                    //    if (key.length > 0 && key[0].search(/[A-Z]/) == 0) {
                    //        this[key.charAt(0).toLowerCase() + key.slice(1)] = val;
                    //        return undefined;
                    //    }
                    //    return val;
                    //}

                    //var strJSON = JSON.stringify(resp);
                    //resp = JSON.parse(strJSON, pascalToCamel);

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

                var token = storageService.getJWT();
                var headers = {};
                if (token != null)
                    headers.Authorization = "JWT " + token;

                var promise = $http({ url: $rootScope.APIURL + path, method: method, data: params, headers: headers });
                return new promiseWrapper(promise);
            }

        };
    }]);
})();