app.directive('validateClick', ["$rootScope", "dialogService", function ($rootScope, dialogService) {
    return {
        scope: {
            validateClick: "&",
            panel: "@"
        },
        link: function (scope, element, attrs) {
            element.on("click", function () {
                $(".invalid_msg").text("");
                var inputs;
                var invalidMessages = [];
                var validations;
                var validation;
                var fieldName;
                var value;
                var mentionId;
                var regex;
                var validationSplit = "|||";
                var paramSplit = ":::";
                var decimal;
                var min;
                var max;
                var minLength
                var comparedValue;
                var mode;
                var forceCompare;
                var comparedFieldName;


                if (scope.panel != null && scope.panel != undefined)
                    inputs = $("#" + scope.panel + " [validation]");
                else
                    inputs = $("[validation]");

                for (var i = 0; i < inputs.length; i++) {
                    validations = $(inputs[i]).attr("validation").split(validationSplit);
                    fieldName = $(inputs[i]).attr("fieldName");
                    mentionId = $(inputs[i]).attr("mentionId");
                    value = $(inputs[i]).val();

                    for (var j = 0; j < validations.length; j++) {
                        validation = validations[j];

                        if (validation == "required") {
                            if (inputs[i].tagName.toLowerCase() == "select") {
                                if (inputs[i].selectedIndex == 0) {
                                    invalidMessages.push({
                                        message: "请选择" + fieldName,
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }
                            else if (inputs[i].tagName.toLowerCase() == "input") {
                                if (value.trim().length == 0) {
                                    invalidMessages.push({
                                        message: "请输入" + fieldName,
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }
                            else {
                                value = $(inputs[i]).attr("ng-model");
                                value = scope.$parent.$eval(value);

                                if (value == null) {
                                    invalidMessages.push({
                                        message: "请选择" + fieldName,
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }
                        }

                        if (value.length > 0) {
                            if (validation.indexOf("pattern" + paramSplit) == 0) {
                                regex = new RegExp(validation.split(paramSplit)[1]);

                                if (!regex.test(value)) {
                                    invalidMessages.push({
                                        message: fieldName + validation.split(paramSplit)[2],
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }


                            if (validation.indexOf("minlengh" + paramSplit) == 0) {
                                minLength = parseInt(validation.split(paramSplit)[1]);
                                if (value.length < minLength) {
                                    invalidMessages.push({
                                        message: fieldName + "必须大于" + minLength + "个字符",
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }

                            if (validation.indexOf("number" + paramSplit) == 0) {
                                //number:::0
                                decimal = validation.split(paramSplit)[1];

                                if (decimal == "0") {
                                    if (!/^-{0,1}\d+$/.test(value)) {
                                        invalidMessages.push({
                                            message: fieldName + "必须是整数",
                                            mentionId: mentionId
                                        });
                                        break;
                                    }
                                }
                                else {
                                    //9999999, 99能过这个regex,需要research
                                    regex = new RegExp("^\\d+(.\\d{1," + decimal + "}){0,1}$");
                                    if (!regex.test(value)) {
                                        invalidMessages.push({
                                            message: fieldName + "必须是数字，且最多包含" + decimal + "位小数",
                                            mentionId: mentionId
                                        });
                                        break;
                                    }
                                }
                            }

                            if (validation.indexOf("range" + paramSplit) == 0) {
                                //range:::1-150
                                min = validation.split(paramSplit)[1].split("-")[0];
                                max = validation.split(paramSplit)[1].split("-")[1];
                                min = parseFloat(min);
                                max = parseFloat(max);

                                if (parseFloat(value) < min || parseFloat(value) > max) {
                                    invalidMessages.push({
                                        message: fieldName + "必须在" + min + "-" + max + "之间",
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }

                            if (validation.indexOf("mobile") == 0) {
                                regex = new RegExp(/\d{11}$/);

                                if (!regex.test(value)) {
                                    invalidMessages.push({
                                        message: "手机必须为11位纯数字",
                                        mentionId: mentionId
                                    });
                                    break;
                                }
                            }
                        }

                        //以下的比较验证器放在if (value.length > 0) 外部，因为有时候为空也需比较
                        if (validation.indexOf("compare" + paramSplit) == 0) {
                            //compare:::txt_pwd:::equal:::true:::密码
                            comparedValue = validation.split(paramSplit)[1];

                            if (isNaN(comparedValue))//非数字
                                comparedValue = $("#" + validation.split(paramSplit)[1]).val();
                            else
                                comparedValue = parseFloat(comparedValue);



                            mode = validation.split(paramSplit)[2];
                            forceCompare = validation.split(paramSplit)[3] == "true" ? true : false;
                            comparedFieldName = validation.split(paramSplit)[4];

                            if (forceCompare || value.length > 0) {
                                if (mode == "equal") {
                                    if (value != comparedValue) {
                                        invalidMessages.push({
                                            message: fieldName + "和" + comparedFieldName + "必须相等",
                                            mentionId: mentionId
                                        });
                                        break;
                                    }
                                }
                                else if (mode == "<=") {
                                    if (parseFloat(value) > comparedValue) {
                                        invalidMessages.push({
                                            message: fieldName + "不能大于" + comparedFieldName,
                                            mentionId: mentionId
                                        });
                                        break;
                                    }
                                }
                            }
                        }

                        if (validation.indexOf("oneisrequired" + paramSplit) == 0) {
                            //oneisrequired:::txt_xxx:::txt_xxx2:::后衣长和xxx
                            var hasValue = false;

                            if ($(inputs[i]).attr("type") == "checkbox") {
                                hasValue = inputs[i].checked;
                            }
                            else
                                hasValue = value.trim().length > 0

                            if (!hasValue) {
                                var params = validation.split(paramSplit);
                                //k从1开始，结束在length-1, 因为数组第一个是验证器的名字oneisrequired，最后一个是要比较的其他控件对应的字段名，比如“后衣长”
                                for (var k = 1; k < params.length - 1; k++) {
                                    var ele = $("#" + params[k]);

                                    if (ele.attr("type") == "checkbox") {
                                        if (ele[0].checked) {
                                            hasValue = true;
                                            break;
                                        }
                                    }
                                    else {
                                        if (ele.val().trim().length > 0) {
                                            hasValue = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!hasValue) {
                                invalidMessages.push({
                                    message: fieldName + "和" + params[params.length - 1] + "至少填写一个",
                                    mentionId: mentionId
                                });
                                break;
                            }


                        }

                    }
                }

                //一次显示所有验证不通过的信息
                //for (var i = 0; i < invalidMessages.length; i++) {
                //    $("#" + invalidMessages[i].mentionId).text(invalidMessages[i].message);
                //}

                //一次显示第一条验证不通过的信息
                if (invalidMessages.length > 0) {
                    dialogService.showMention(1, invalidMessages[0].message);
                }

                if (invalidMessages.length == 0) {
                    scope.validateClick();
                    $rootScope.$apply();
                }
            });
        }
    };
}]);

app.directive('selector', ["$rootScope", "ngDialog", function ($rootScope, ngDialog) {
    return {
        restrict: "EA",
        scope: {
            items: "=",
            ngModel: "=",
            dialogClassName: "@",
            isMultiple: "=",
            itemValue: "@",
            itemText: "@",
            opener: "@"
        },
        link: function (scope, elem, attrs) {
            scope.$watch("ngModel", function () {
                displaySelection();
            });

            scope.$watch("items", function () {
                displaySelection();
            });

            function displaySelection() {
                if (scope.ngModel != null && scope.items != null) {
                    if (scope.isMultiple) {
                        var text = "";
                        for (var i = 0; i < scope.items.length; i++) {
                            for (var j = 0; j < scope.ngModel.length; j++) {
                                if (scope.items[i][scope.itemValue] == scope.ngModel[j])
                                    text += scope.items[i][scope.itemText] + ",";
                            }

                        }
                        text = text.substring(0, text.length - 1);
                        elem.html(text);
                    }
                    else {
                        for (var i = 0; i < scope.items.length; i++) {
                            if (scope.items[i][scope.itemValue] == scope.ngModel)
                                elem.html(scope.items[i][scope.itemText]);
                        }

                    }
                }
            }

            scope.isMultiple = attrs.isMultiple == "true" ? true : false;
            if (scope.dialogClassName == undefined)
                scope.dialogClassName = "";

            displaySelection();

            var clickHandler=function () {
                var ngDialogSelector = ngDialog.open({
                    template: $rootScope.dialogSelectorTemplate,
                    showClose: false,
                    closeByEscape: false,
                    overlay: true,
                    className: "ngdialog-theme-selector " + scope.dialogClassName,
                    closeByDocument: true,
                    controller: ["$scope", "$sce", function ($scope, $sce) {
                        //notice $scope is dialog's $scope, scope is directive's scope, they are different
                        $scope.items = scope.items;
                        $scope.isMultiple = scope.isMultiple;
                        $scope.selectedIndexes = new Array($scope.items.length);
                        $scope.itemValue = scope.itemValue;
                        $scope.itemText = scope.itemText;
                        for (var i = 0; i < $scope.selectedIndexes.length; i++) {
                            $scope.selectedIndexes[i] = false;
                        }

                        if (scope.isMultiple) {
                            for (var i = 0; i < scope.items.length; i++) {
                                for (var j = 0; j < scope.ngModel.length; j++) {
                                    if ($scope.items[i][$scope.itemValue] == scope.ngModel[j]) {
                                        $scope.selectedIndexes[i] = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else {
                            for (var i = 0; i < scope.items.length; i++) {
                                if ($scope.items[i][$scope.itemValue] == scope.ngModel) {
                                    $scope.selectedIndexes[i] = true;
                                    break;
                                }
                            }
                        }

                        $scope.select = function (item) {
                            if ($scope.isMultiple) {
                                var index = $scope.items.indexOf(item);
                                $scope.selectedIndexes[index] = !$scope.selectedIndexes[index];
                            }
                            else {
                                scope.ngModel = item[$scope.itemValue];
                                ngDialogSelector.close();
                            }
                        };

                        $scope.confirm = function () {
                            scope.ngModel = [];
                            for (var i = 0; i < $scope.items.length; i++) {
                                if ($scope.selectedIndexes[i])
                                    scope.ngModel.push($scope.items[i][$scope.itemValue]);
                            }
                            ngDialogSelector.close();
                        }
                    }]
                });
            };

            if (scope.opener == undefined)
                elem.on("click", clickHandler);
            else
                $("#" + scope.opener).on("click", clickHandler);
        }
    };
}]);

app.directive('datePickup', ["$rootScope", "ngDialog", function ($rootScope, ngDialog) {
    return {
        restrict: "EA",
        scope: {
            minDate: "@",
            maxDate: "@",
            ngModel: "=",
            dateType: "@",
            placeholder: "@",
            opener: "@"
        },
        link: function (scope, elem, attrs) {
            scope.$watch("ngModel", function () {
                if (scope.ngModel == null || scope.ngModel == undefined || scope.ngModel.length == 0)
                    elem.html(scope.placeholder);
                else
                    elem.html(scope.ngModel);
            });

            var clickHandler = function () {
                ngDialog.open({
                    template: $rootScope.dialogDatepickupTemplate,
                    showClose: false,
                    closeByEscape: false,
                    overlay: true,
                    className: "ngdialog-theme-datepickup",
                    closeByDocument: true,
                    controller: ["$scope", function ($scope) {
                        var minDate = new Date(scope.minDate);
                        var maxDate = new Date(scope.maxDate);
                        $scope.dateType = scope.dateType;
                        $scope.years = [];
                        $scope.months = [];
                        $scope.dates = [];
                        $scope.hours = [];
                        $scope.minutes = [];
                        var selectedDate;

                        if (scope.ngModel == null || scope.ngModel == undefined || scope.ngModel.length == 0)
                            selectedDate = new Date();
                        else {
                            if (scope.dateType == "date" || scope.dateType == "datetime")
                                selectedDate = new Date(scope.ngModel);
                            else
                                selectedDate = new Date("0000-1-1 " + scope.ngModel);
                        }


                        //fill $scope.years, $scope.months, $scope.dates,$scope.hours, $scope.minutes and set date select
                        for (var i = 0; i <= maxDate.getFullYear() - minDate.getFullYear() ; i++) {
                            $scope.years.push(minDate.getFullYear() + i);
                        }
                        $scope.yearOffset = (selectedDate.getFullYear() - minDate.getFullYear()) * 2;
                        fillMonths(selectedDate.getFullYear());
                        selectMonth(selectedDate.getMonth());
                        fillDates(selectedDate.getFullYear(), selectedDate.getMonth());
                        selectDate(selectedDate.getDate());

                        $scope.hours = [];
                        for (var i = 0; i < 24; i++) {
                            $scope.hours.push(i);
                        }
                        $scope.hourOffset = selectedDate.getHours() * 2;

                        $scope.minutes = [0, 30];
                        $scope.minuteOffset = selectedDate.getMinutes() == 0 ? 0 : 2;
                        //fill $scope.years, $scope.months, $scope.dates,$scope.hours, $scope.minutes and set date select


                        $scope.confirm = function () {
                            var str = "";
                            if (scope.dateType == "date" || scope.dateType == "datetime") {
                                var year = $scope.years[$scope.yearOffset / 2];
                                var month = $scope.months[$scope.monthOffset / 2];
                                var date = $scope.dates[$scope.dateOffset / 2];
                                str = year.toString() + "-" + (month + 1).toString() + "-" + date.toString();
                            }
                            if (scope.dateType == "time" || scope.dateType == "datetime") {
                                var hour = $scope.hours[$scope.hourOffset / 2];
                                var minute = $scope.minutes[$scope.minuteOffset / 2];

                                if (str.length == 0)
                                    str = hour.toStringWithPrefix(2) + ":" + minute.toStringWithPrefix(2);
                                else
                                    str = " " + hour.toStringWithPrefix(2) + ":" + minute.toStringWithPrefix(2);
                            }
                            scope.ngModel = str;
                            $scope.closeThisDialog();

                        };

                        //日期拖动事件处理函数
                        var mouseStartY;
                        var yearStartY;
                        var monthStartY;
                        var dateStartY;
                        var hourStartY;
                        var minuteStartY;
                        $scope.gearTouchStart = function (datePart) {
                            //此事件记录当前鼠标及日期的位置
                            mouseStartY = $scope.$event.targetTouches[0].screenY;

                            if (datePart == "YYYY")
                                yearStartY = $scope.yearOffset;
                            else if (datePart == "MM")
                                monthStartY = $scope.monthOffset;
                            else if (datePart == "dd")
                                dateStartY = $scope.dateOffset;
                            else if (datePart == "hh")
                                hourStartY = $scope.hourOffset;
                            else if (datePart == "mm")
                                minuteStartY = $scope.minuteOffset;
                        };
                        //手指移动
                        $scope.gearTouchMove = function (datePart) {
                            //获取当前鼠标偏移量，偏移量=鼠标当前位置 - gearTouchStart事件记录的鼠标位置
                            //把获取的偏移量引用到yearStartY，monthStartY，dateStartY上，就能实现拖动效果
                            var mouseMovedY = $scope.$event.targetTouches[0].screenY - mouseStartY;
                            //$scope.yearOffset的单位是em，mouseMovedY的单位是px，用以下算法对mouseMovedY进行调整，让滚动效果看起来自然
                            mouseMovedY = mouseMovedY * 60 / window.innerHeight;
                            var offset, maxOffset;
                            if (datePart == "YYYY") {
                                offset = yearStartY - mouseMovedY;
                                maxOffset = ($scope.years.length - 1) * 2;
                            }
                            else if (datePart == "MM") {
                                offset = monthStartY - mouseMovedY;
                                maxOffset = ($scope.months.length - 1) * 2;
                            }
                            else if (datePart == "dd") {
                                offset = dateStartY - mouseMovedY;
                                maxOffset = ($scope.dates.length - 1) * 2;
                            }
                            else if (datePart == "hh") {
                                offset = hourStartY - mouseMovedY;
                                maxOffset = ($scope.hours.length - 1) * 2;
                            }
                            else if (datePart == "mm") {
                                offset = minuteStartY - mouseMovedY;
                                maxOffset = ($scope.minutes.length - 1) * 2;
                            }

                            if (offset < 0)
                                offset = 0;
                            else if (offset > maxOffset)
                                offset = maxOffset;

                            if (datePart == "YYYY")
                                $scope.yearOffset = offset;
                            else if (datePart == "MM")
                                $scope.monthOffset = offset;
                            else if (datePart == "dd")
                                $scope.dateOffset = offset;
                            else if (datePart == "hh")
                                $scope.hourOffset = offset;
                            else if (datePart == "mm")
                                $scope.minuteOffset = offset;
                        };
                        //离开屏幕
                        $scope.gearTouchEnd = function (datePart) {
                            //精确定位日期
                            //定位至靠近的那个日期，日期刻度偏移量是2的倍数，如果余数 >= 1, 说明下一个日期的大部分接近中心，所以定位至下一个日期
                            //通过Math.floor(offset / 2) + 1，找到下一个日期位置， offset * 2计算出偏移量
                            var offset;
                            if (datePart == "YYYY")
                                offset = $scope.yearOffset;
                            else if (datePart == "MM")
                                offset = $scope.monthOffset;
                            else if (datePart == "dd")
                                offset = $scope.dateOffset;
                            else if (datePart == "hh")
                                offset = $scope.hourOffset;
                            else if (datePart == "mm")
                                offset = $scope.minuteOffset;

                            if (offset % 2 >= 1) {
                                offset = Math.floor(offset / 2) + 1;
                                offset = offset * 2;
                            }
                            else {
                                offset = Math.floor(offset / 2);
                                offset = offset * 2;
                            }

                            if (datePart == "YYYY")
                                $scope.yearOffset = offset;
                            else if (datePart == "MM")
                                $scope.monthOffset = offset;
                            else if (datePart == "dd")
                                $scope.dateOffset = offset;
                            else if (datePart == "hh")
                                $scope.hourOffset = offset;
                            else if (datePart == "mm")
                                $scope.minuteOffset = offset;
                            //精确定位日期

                            //刷新可用的月和日
                            var selectedMonth = $scope.months[$scope.monthOffset / 2];
                            fillMonths($scope.years[$scope.yearOffset / 2]);
                            selectMonth(selectedMonth);

                            var selectedDate = $scope.dates[$scope.dateOffset / 2];
                            fillDates($scope.years[$scope.yearOffset / 2], $scope.months[$scope.monthOffset / 2]);
                            selectDate(selectedDate);
                            //刷新可用的月和日
                        };

                        function fillMonths(year) {
                            var startMonth = 0, endMonth = 11;

                            //选择了min year
                            if (year == minDate.getFullYear()) {
                                //开始月份必须是minDate里的月份
                                startMonth = minDate.getMonth();
                            }
                            //选择了max year
                            if (year == maxDate.getFullYear()) {
                                //结束月份必须是maxDate里的月份
                                endMonth = maxDate.getMonth();
                            }

                            $scope.months.length = 0;
                            while (startMonth <= endMonth) {
                                $scope.months.push(startMonth);
                                startMonth++;
                            }
                        }

                        function selectMonth(month) {
                            for (var i = 0; i < $scope.months.length; i++) {
                                //选中的月份在$scope.months中存在，就继续保持选中
                                if ($scope.months[i] == month) {
                                    $scope.monthOffset = i * 2;
                                    return;
                                }
                            }

                            //选中的月份小于可用months中第一个月份,比如选中6月，但minDate的月份是10，那就让months中第一个月份被选中
                            if (month < $scope.months[0])
                                $scope.monthOffset = 0;
                            else
                                $scope.monthOffset = ($scope.months.length - 1) * 2;
                        }

                        function fillDates(year, month) {
                            var startDate = 1, endDate;
                            //选择了min year和min month
                            if (year == minDate.getFullYear() && month == minDate.getMonth())
                                startDate = minDate.getDate(); //开始日期必须是minDate里的日期

                            //选择了max year和max month
                            if (year == maxDate.getFullYear() && month == maxDate.getMonth())
                                endDate = maxDate.getDate();//结束日期等于maxDate里的日期
                            else
                                endDate = calcDays(year, month);//获取某月中的最大天数，可能的值是28,29,30,31

                            $scope.dates.length = 0;
                            while (startDate <= endDate) {
                                $scope.dates.push(startDate);
                                startDate++;
                            }
                        }

                        function selectDate(date) {
                            for (var i = 0; i < $scope.dates.length; i++) {
                                //选中的日期在$scope.dates中存在，就继续保持选中
                                if ($scope.dates[i] == date) {
                                    $scope.dateOffset = i * 2;
                                    return;
                                }
                            }

                            //选中的日期小于可用dates中第一天,比如选中6号，但minDate的日期是10，那就让dates中第一天被选中
                            if (date < $scope.dates[0])
                                $scope.dateOffset = 0;
                            else
                                $scope.dateOffset = ($scope.dates.length - 1) * 2;
                        }

                        //求月份最大天数
                        function calcDays(year, month) {
                            if (month == 1) {
                                if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0 && year % 4000 != 0))
                                    return 29;
                                else
                                    return 28;
                            }
                            else {
                                if (month == 3 || month == 5 || month == 8 || month == 10)
                                    return 30;
                                else
                                    return 31;
                            }
                        }
                        //日期拖动事件处理函数
                    }]
                });
            };

            if (scope.opener == undefined)
                elem.on("click", clickHandler);
            else
                $("#" + scope.opener).on("click", clickHandler);
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
                    //loadingHandler = dialogService.showLoading();//ngDialog展现的时候，应该不是同步地展现，目前不确定何时展现。弄清楚何时展现再实现此功能。

                    var fileReader = new FileReader();
                    fileReader.onload = function (e) {
                        
                        var img = new Image();
                        img.onload = function () {
                            //loadingHandler.close();//如果使用dialogService.showLoading()，在IE里这行代码出错，因为loadingHandler并没有完成展现，所以调用其close方法时会出错，导致IE里图片上传组件无法正常工作

                            //把img的src里的"data:xxx/xxx;base64,"替换成"data:image/jpeg;base64,"，可以确保当用户选择了图片文件，但扩展名不是.jpg,.png,.gif等非图片扩展名时，上传组件仍可工作
                            dialogService.showUploadImage(img.src.replace(/data:[^,]+,/, "data:image/jpeg;base64,"), scope.cutWidth, scope.cutHeight, scope.recieveImage);
                        };
                        img.onerror = function () {
                            //loadingHandler.close();
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

app.directive("ngTouchstart", [function () {
    return {
        controller: ["$scope", "$element", function ($scope, $element) {

            $element.bind("touchstart", onTouchStart);
            function onTouchStart(event) {
                var method = $element.attr("ng-touchstart");
                $scope.$event = event;
                $scope.$apply(method);
            }

        }]
    };
}]);

app.directive("ngTouchmove", [function () {
    return {
        controller: ["$scope", "$element", function ($scope, $element) {

            $element.bind("touchstart", onTouchStart);
            function onTouchStart(event) {
                event.preventDefault();
                $element.bind("touchmove", onTouchMove);
                $element.bind("touchend", onTouchEnd);
            }
            function onTouchMove(event) {
                var method = $element.attr("ng-touchmove");
                $scope.$event = event;
                $scope.$apply(method);
            }
            function onTouchEnd(event) {
                event.preventDefault();
                $element.unbind("touchmove", onTouchMove);
                $element.unbind("touchend", onTouchEnd);
            }

        }]
    };
}]);

app.directive("ngTouchend", [function () {
    return {
        controller: ["$scope", "$element", function ($scope, $element) {

            $element.bind("touchend", onTouchEnd);
            function onTouchEnd(event) {
                var method = $element.attr("ng-touchend");
                $scope.$event = event;
                $scope.$apply(method);
            }

        }]
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
                scope.$apply(function () { scope.ngLoad();});
            });
        }
    };
}]);

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
            var div_loadMore = $("<div style=\"display:none;\"></div>");
            if (scope.loadMoreCss == null || scope.loadMoreCss == undefined || scope.loadMoreCss.trim().length == 0)
                div_loadMore.addClass("content bgc_grey2 ta_c fc_grey");
            else
                div_loadMore.addClass(scope.loadMoreCss);
            elem.after(div_loadMore);
            var isLoadedAll = false;
            var isCallLoadEventHandler = false;
            var pullPixel = 0;//上拉下拉的拉动距离，单位是PX，上拉时此值为负数，下拉时此值为正数
            var mouseMovedY = null;
            var pullUpMin;//上拉极限，该值为负数
            //显示数据的DIV的可视高度，用于计算是否已经上拉到底部，如果到底部后，继续上拉就出现“释放加载更多”
            var visibleHeight = $(window).height() - (scope.top == null ? 0 : scope.top) - (scope.bottom == null ? 0 : scope.bottom);
            var moveTime;//自动滚动动画效果所需变量

            if (window.ontouchstart !== undefined) {//this is mobile, mobile support ontouchstart event
                scope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
                    //复原滚动条，否则其他页面无法滚动
                    $("body").css({ "height": "", "overflow": "" });
                    $("html").css({ "height": "", "overflow": "" });
                });

                //让浏览器不出现滚动条，防止滚动条滚动后，页面底部出现多余空白
                $("body").css({ "height": "100%", "overflow": "hidden" });
                $("html").css({ "height": "100%", "overflow": "hidden" });

                elem.on("touchstart", function (e) {
                    //stopRoll = true;//自动滚动动画效果所需变量
                    if (visibleHeight > elem.outerHeight() + div_loadMore.outerHeight())//当前可视范围大于显示数据的DIV的高度，所以pullUpMin = 0;
                        pullUpMin = 0;
                    else
                        pullUpMin = visibleHeight - (elem.outerHeight() + div_loadMore.outerHeight());

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
                    div_loadMore.css({
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
                        div_loadMore.css({
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

                scope.$watch("data", function () {
                    if (scope.data == null) {
                        isLoadedAll = true;
                        div_loadMore.css({ "display": "none" });
                    }
                    else if (scope.data.length < scope.pageSize) {
                        isLoadedAll = true;
                        div_loadMore.text("已加载全部");
                        div_loadMore.css({ "display": "" });
                    }
                    else {
                        isLoadedAll = false;
                        div_loadMore.text("上拉加载更多");
                        div_loadMore.css({ "display": "" });
                    }
                });
            }
            else {
                var w = $(window);
                var d = $(document);

                var handle = function () {
                    if (w.scrollTop() >= d.height() - w.outerHeight()) {//scroll is at the end of page
                        if (!isLoadedAll) {
                            div_loadMore.text("加载中。。。");
                            scope.loadMore()
                        }
                    }
                };
                w.on("scroll", handle);

                scope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
                    w.unbind("scroll", handle);
                });

                scope.$watch("data", function () {
                    if (scope.data == null) {
                        isLoadedAll = true;
                        div_loadMore.css({ "display": "none" });
                    }
                    else if (scope.data.length < scope.pageSize) {
                        isLoadedAll = true;
                        div_loadMore.text("已加载全部");
                        div_loadMore.css({ "display": "" });
                    }
                    else {
                        isLoadedAll = false;
                        div_loadMore.text("下拉加载更多");
                        div_loadMore.css({ "display": "" });
                    }
                });
            }

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
                else if (!stopRoll) {
                    //s = s + 2;
                    moveSpeed -= 1;
                    if (moveSpeed > 1)
                        setTimeout(roll, 20);
                }

                elem.css({
                    transform: "translate(0px, " + pullPixel + "px)"
                });

            }

           

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
            enter: "&",
            keys:"="
        },
        link: function (scope, element, attrs) {
            var keys = [];
            if (scope.keys == undefined) {
                keys.push(13);
            }
            else {
                keys = scope.keys;
            }

            element.keyup(function (e) {
                if (keys.indexOf(e.keyCode) != -1) {
                    scope.$apply(function () {
                        scope.enter();
                    });
                }
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
            element[0].contentWindow.document.head.innerHTML = "<style>img{max-width:100%}</style>";


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

app.directive('needPlaceholder', [function () {
    return {
        link: function (scope, element, attrs) {
            //It is possible that there are some data bind express like {{xxxx}} in the element. Aftet bind, {{xxx}} change to 20, so the lenth of {{xxx}} and 20 are different.
            //Text length would affect div's size, we should clone the element after it is binded.
            setTimeout(function () {
                var newDiv = $("<div></div>");
                newDiv.css({
                    width: element.outerWidth() + "px",
                    height: element.outerHeight() + "px"
                });

                element.after(newDiv);
            }, 100);
        }
        
    };
}]);

app.directive('slideSwitch', [function () {
    return {
        scope:{
            ngModel:"="
        },
        template: "<span class=\"switch\" ng-class=\"{'enabled':ngModel,'disabled':!ngModel}\"><span></span></span>",
        replace: true,
        link: function (scope, element, attr) {
            element.on("click", function () {
                scope.$apply(function () {
                    scope.ngModel = !scope.ngModel;
                });
            });
        }
    };
}]);

app.directive('noData', [function () {
    return {
        scope: {
            data: "="
        },
        template: "<div class=\"content ta_c bgc_grey2\" ng-if=\"data.length==0\"><img class=\"shrink_283\" src=\"imgs/none_data.png\" /></div>",
        replace: true
    };
}]);