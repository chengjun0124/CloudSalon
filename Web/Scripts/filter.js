app.filter('to_trusted', ['$sce', function ($sce) {
    return function (text) {
        return $sce.trustAsHtml(text);
    };
}]);


app.filter("toMinutes", [function () {
    return function (input) {
        return input.toMinutes();
    };
}]);


app.filter("null", [function () {
    return function (input, param1) {
        return input == null ? param1 : input;
    };
}]);

app.filter("toCNWeek", [function () {
    return function (input) {
        if (input.toLocaleLowerCase() == "mon")
            return "一";
        if (input.toLocaleLowerCase() == "tue")
            return "二";
        if (input.toLocaleLowerCase() == "wed")
            return "三";
        if (input.toLocaleLowerCase() == "thu")
            return "四";
        if (input.toLocaleLowerCase() == "fri")
            return "五";
        if (input.toLocaleLowerCase() == "sat")
            return "六";
        if (input.toLocaleLowerCase() == "sun")
            return "日";
    };
}]);

app.filter("todate", [function () {
    return function (input) {
        var date = new Date(input);
        var today = new Date();

        if (date.getYear() == today.getYear() && date.getMonth() == today.getMonth() && date.getDate() == today.getDate())
            return "今日";
        if (date.getYear() == today.getYear() && date.getMonth() == today.getMonth() && date.getDate() == today.getDate() + 1)
            return "明日";

        return date.getDate();
    };
}]);

app.filter("toAppointmentStatus", [function () {
    return function (input) {

        if (input == 1)
            return "等待确认";

        if (input == 2)
            return "预约失败";

        if (input == 3)
            return "预约成功";

        if (input == 4)
            return "完成服务";

        if (input == 5)
            return "用户取消";

        if (input == 6)
            return "技师取消";

    };
}]);

app.filter("asteriskMobile",[ function () {
    return function (input) {
        var arr = "";

        for (var i = 0; i < input.length; i++) {
            if (i >= 4 && i <= 7)
                arr += "*";
            else
                arr += input[i];
        }
        return arr;
    };
}]);

app.filter("toPurchaseMode", [function () {
    return function (input) {
        if (input.mode == 0)
            return "单次";
        else if (input.mode == 1)
            return "疗程";
    };
}]);

app.filter("toServiceTime", [function () {
    return function (input) {
        if (input == null)
            return "无限制";

        if (input.time == null)
            return "无限制"
        else
            return input.time + "次";
    };
}]);

app.filter("toRemainTime", [function () {
    return function (input) {
        if (input == null)
            return "";

        var remainTime = input.time;
        for (var i = 0; i < input.consumedServices.length; i++) {
            remainTime -= input.consumedServices[i].time;
        }
        return remainTime;
    };
}]);

app.filter("toLastConsumedDate", [function () {
    return function (input) {
        if (input == null)
            return "未使用";

        if (input.consumedServices.length > 0)
            return input.consumedServices[0].createdDate.format("YYYY-MM-dd");
        else
            return "未使用";
    };
}]);

app.filter("toInterval", [function () {
    return function (input) {
        return input == null ? "无约束" : "间隔" + input + "天";
    };
}]);

app.filter("toChargeMode", [function () {
    return function (input) {
        if (input == 0)
            return "美容师扫码";

        if (input == 1)
            return "美管扫码";

        if (input == 2)
            return "美管手动";
    };
}]);

app.filter("toConsumeStatus", [function () {
    return function (input) {
        if (input == 1)
            return "等待用户确认";

        if (input == 2)
            return "用户确认";

        if (input == 3)
            return "服务完成";

        if (input == 4)
            return "用户拒绝";
    };
}]);