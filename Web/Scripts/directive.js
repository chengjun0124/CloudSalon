app.directive('loadMore', [function () {
    return {
        scope: {
            loadMore: "&",
            pageSize: "=",
            data: "=",
            loadMoreCss: "@",
            top: "=",//默认是0，表示显示数据的DIV到浏览器头的偏移量
            bottom: "="//默认是0，表示显示数据的DIV的底部，到浏览器底部的偏移量
        },
        link: function (scope, elem, attrs) {
            var mouseStartY;
            var div_loadMore = $("<div class=\"" + scope.loadMoreCss + "\"></div>");
            elem.append(div_loadMore);
            var isLoadedAll = false;
            var isCallLoadEventHandler = false;
            var pullPixel = 0;//上拉下拉的拉动距离，单位是PX，上拉时此值为负数，下拉时此值为正数
            var mouseMovedY = null;
            var pullUpMin;//上拉极限，该值为负数
            //显示数据的DIV的可视高度，用于计算是否已经上拉到底部，如果到底部后，继续上拉就出现“释放加载更多”
            var visibleHeight = $(window).height() - (scope.top == null ? 0 : scope.top) - (scope.bottom == null ? 0 : scope.bottom);
            var moveTime;//自动滚动动画效果所需变量


            
            scope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
                //复原滚动条，否则其他页面无法滚动
                $("body").css({ "height": "", "overflow": "" });
                $("html").css({ "height": "", "overflow": "" });
            });
            
            //让微信浏览器不出现滚动条，防止滚动条滚动后，页面底部出现多余空白
            $("body").css({ "height": "100%", "overflow": "hidden" });
            $("html").css({ "height": "100%", "overflow": "hidden" });

            elem.on("touchstart", function (e) {
                //stopRoll = true;//自动滚动动画效果所需变量
                if (visibleHeight > elem.outerHeight())//当前可视范围大于显示数据的DIV的高度，所以pullUpMin = 0;
                    pullUpMin = 0;
                else
                    pullUpMin = visibleHeight - elem.outerHeight();

                mouseStartY = e.targetTouches[0].screenY;
            });

            elem.on("touchmove", function (e) {
                e.preventDefault();
                //moveTime = new Date(); //自动滚动动画效果所需变量
                mouseMovedY = e.targetTouches[0].screenY - mouseStartY;
                if ((/android/gi).test(navigator.appVersion))
                    mouseMovedY *= 2;

                if (pullPixel + mouseMovedY > 0) {//表示已经下拉到顶部
                    mouseMovedY = 0 - pullPixel;
                }
                else if (pullPixel + mouseMovedY < pullUpMin) {//表示已经上拉到底部
                    if (!isLoadedAll) {
                        isCallLoadEventHandler = true;
                        div_loadMore.text("释放加载更多");
                    }
                    else {
                        mouseMovedY = pullUpMin - pullPixel;
                    }
                }
                else {
                    isCallLoadEventHandler = false;
                    if (!isLoadedAll)
                        div_loadMore.text("上拉加载更多");
                }

                elem.css({
                    transform: "translate(0px, " + (pullPixel + mouseMovedY) + "px)"
                });
            });

            elem.on("touchend", function (e) {
                if (mouseMovedY != null) {
                    //var endTime = new Date();//自动滚动动画效果所需变量
                    pullPixel = pullPixel + mouseMovedY;
                    if (pullPixel > 0)//表示下拉过头，即DIV顶部已出现空白，松开后复原
                        pullPixel = 0;

                    if (pullPixel < pullUpMin)//表示上拉已超过底部
                        pullPixel = pullUpMin;

                    elem.css({
                        transform: "translate(0px, " + pullPixel + "px)"
                    });

                    if (isCallLoadEventHandler) {
                        div_loadMore.text("加载中。。。");
                        scope.loadMore();
                        isCallLoadEventHandler = false;
                    }
                    mouseMovedY = null;//click事件会触发touchend，如果不清空mouseMovedY，click事件会根据mouseMovedY移动DIV
                    //自动滚动动画效果
                    //if (endTime - moveTime < 4) {
                    //    if (mouseMovedY > 100 || mouseMovedY < -100) {
                    //        stopRoll = false;
                    //        //s = 20;
                    //        moveSpeed = 50;
                    //        setTimeout(roll, 20);
                    //    }
                    //}
                }
            });

            //var s;
            var stopRoll = true;
            var moveSpeed;
            function roll() {
                //滚动需要有越滚越慢效果，调整pullPixel，s以实现越滚越慢的效果
                //把变量pullPixel 逐渐变小
                //把变量s 逐渐变大，目前s没用上，如果使用，放在setTimeout(roll, s);

                if (mouseMovedY > 0)//下拉
                    pullPixel += moveSpeed;
                else//上拉
                    pullPixel -= moveSpeed;

                if (pullPixel > 0)//表示下拉过头，即DIV顶部已出现空白
                    pullPixel = 0;
                else if (pullPixel < pullUpMin)//表示上拉已超过底部
                    pullPixel = pullUpMin;
                else if(!stopRoll){
                    //s = s + 2;
                    moveSpeed -= 1;
                    if (moveSpeed > 1)
                        setTimeout(roll, 20);
                }

                elem.css({
                    transform: "translate(0px, " + pullPixel + "px)"
                });
                
            }

            scope.$watch("data", function () {
                if (scope.data == null) {
                    isLoadedAll = true;
                    div_loadMore.text("");
                }
                else if (scope.data.length < scope.pageSize) {
                    isLoadedAll = true;
                    div_loadMore.text("已加载全部");
                }
                else {
                    isLoadedAll = false;
                    div_loadMore.text("上拉加载更多");
                }
            });

            scope.$on("resetPullPixel", function () {
                pullPixel = 0;
                elem.css({
                    transform: "translate(0px, 0px)"
                });
            });

        }
    };
}]);
app.directive('enter', [function () {
    return {
        scope: {
            enter: "&"
        },
        link: function (scope, element, attrs) {
            element.keyup(function (e) {
                if (e.keyCode == 13) {
                    scope.enter();
                }
            });
        }
    };
}]);

app.directive("uploadImage", ["dialogService", function (dialogService) {
    //这个变量确保始终只有一个<input type="file" />被创建在页面里
    var uploadImageFile = $("<input type=\"file\" style=\"display:none\" />").appendTo("body");
    var loadingHandler;
    return {
        scope: {
            cutWidth: "=",
            cutHeight: "=",
            recieveImage: "="
        },
        link: function (scope, element, attrs) {
            element.bind("click", function (e) {
                //页面可能多次使用这个指令，但<input type=\"file\" />只有一个，所以每次点击指令时，先移除之前的onchange事件，确保onchange事件只被执行一次，并且能在下面的onload事件中传入正确的参数（scope.cutWidth, scope.cutHeight, scope.recieveImage）
                uploadImageFile.unbind("change");

                uploadImageFile.on("change", function () {
                    loadingHandler = dialogService.showLoading();

                    var fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        var img = new Image();
                        img.onload = function () {
                            loadingHandler.close();

                            //把img的src里的"data:xxx/xxx;base64,"替换成"data:image/jpeg;base64,"，可以确保当用户选择了图片文件，但扩展名不是.jpg,.png,.gif等非图片扩展名时，上传组件仍可工作
                            dialogService.showUploadImage(img.src.replace(/data:[^,]+,/, "data:image/jpeg;base64,"), scope.cutWidth, scope.cutHeight, scope.recieveImage);
                        };
                        img.onerror = function () {
                            loadingHandler.close();
                            dialogService.showMention(1, "请选择正确的图片");
                        };
                        img.src = fileReader.result;
                    };


                    fileReader.readAsDataURL(uploadImageFile[0].files[0]);

                    //清空<input type="file" />的value，否则会有以下bug：
                    //点击图片，选择图片A，无论点击取消还是确定，当再次点击图片，选择图片A，因为<input type="file" />值没变，所以不触发uploadImageFile的change事件
                    uploadImageFile.val("");

                });

                uploadImageFile.click();
            });
        }
    };
}]);


app.directive("ngLoad", [function () {
    return {
        scope: {
            ngLoad: "&"
        },
        link: function (scope, element, attrs) {
            element.bind("load", function (e) {

                //scope.ngLoad();
                scope.$apply(function () { scope.ngLoad(); });
            });
        }
    };
}]);

app.directive('htmlEditor', [function () {
    return {
        scope: {
            ngModel: "=",
            htmlEditor: "="
        },
        link: function (scope, element, attrs) {
            if (scope.htmlEditor == true) {
                element[0].contentWindow.document.designMode = "on";

                $(element[0].contentWindow.document.body).on("keyup", function () {
                    //iframe can enlarge automatically, but cannot shrink automatically
                    element.css({ "height": $(element[0].contentWindow.document).outerHeight() + "px" });
                    scope.$apply(function () {
                        scope.ngModel = element[0].contentWindow.document.body.innerHTML;
                    });
                });
            }

            $(element[0].contentWindow.document.body).css({ overflow: "hidden" });//hidden the scroll bar in the iframe

            //full display img, otherwise if img's width is more then 750px, the img will be cutted
            element[0].contentWindow.document.head.innerHTML="<style>img{max-width:100%}</style>";            


            var watchHandle = scope.$watch("ngModel", function () {
                if (scope.ngModel != undefined) {
                    element[0].contentWindow.document.body.innerHTML = scope.ngModel;
                    
                    //sometime this solution cannot full diaplay iframe(height), probably the img in the content need a little bit time to load
                    //need figure out a perfect solution
                    setTimeout(function () {
                        element.css({ "height": $(element[0].contentWindow.document).outerHeight() + "px" });
                    }, 1000);
                    watchHandle();//after initail iframe content, stop $watch("ngModel")
                }
            });
        }
    };
}]);