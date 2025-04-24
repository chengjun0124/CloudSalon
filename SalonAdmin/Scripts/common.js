function timeSpan(time) {
    this.hour;
    this.minute;
    this.second;

    if (/^\d{2}:\d{2}:\d{2}$/.test(time)) {
        this.hour = parseInt(time.split(":")[0]);
        this.minute = parseInt(time.split(":")[1]);
        this.second = parseInt(time.split(":")[2]);
    }
    else {
        var minutes = parseInt(time);
        this.hour = parseInt(minutes / 60);
        this.minute = minutes % 60;
        this.second = 0;
    }

    this.add = function (t) {
        var second = 0;
        var minute = 0;
        var hour = 0;

        second = this.second + t.second;
        if (second > 59) {
            minute++;
            second = second - 60;
        }

        minute = this.minute + t.minute + minute;
        if (minute > 59) {
            hour++;
            minute = minute - 60;
        }

        hour = this.hour + t.hour + hour;

        return new timeSpan(hour + ":" + minute + ":" + second);
    };

    this.lessThan = function (t) {
        if (this.hour < t.hour)
            return true;

        if (this.minute < t.minute)
            return true;

        if (this.second < t.second)
            return true;

        return false;
    };

    this.equal = function (t) {
        return this.hour == t.hour && this.minute == t.minute && this.second == t.second;
    };

    this.toString = function () {
        var h = this.hour.toStringWithPrefix(2);
        var m = this.minute.toStringWithPrefix(2);
        var s = this.second.toStringWithPrefix(2);

        return h + ":" + m + ":" + s;
    };
};

String.prototype.toDateString = function () {
    return this.split("T")[0];
};

String.prototype.toTimeString = function () {
    return this.split("T")[1];
};

String.prototype.toDateTimeString = function () {
    return this.replace("T", " ");
};

Date.prototype.format = function (f) {
    var str = f;

    str = str.replace("YYYY", this.getFullYear());
    str = str.replace("MM", (this.getMonth() + 1).toStringWithPrefix(2));
    str = str.replace("dd", (this.getDate()).toStringWithPrefix(2));
    return str;
};

Number.prototype.toStringWithPrefix = function (n) {
    return (Array(n).join('0') + this).slice(-n);
};

String.prototype.format = function (f) {
    var str = f;

    //字符串仅为时间格式 19:03:06.00025
    var matches = this.match(/^(\d{2}):(\d{2}):(\d{2})(.\d+){0,1}$/);

    if (matches != null) {
        str = str.replace("hh", matches[1]);
        str = str.replace("mm", matches[2]);
        return str;
    }

    //字符串为日期格式 2017-08-09T19:03:06.00025
    var matches = this.match(/^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(.\d+){0,1}$/);
    if (matches != null) {
        str = str.replace("YYYY", matches[1]);
        str = str.replace("MM", matches[2]);
        str = str.replace("dd", matches[3]);
        str = str.replace("hh", matches[4]);
        str = str.replace("mm", matches[5]);

        if (str.indexOf("day") > -1) {
            var day = new Date(this).getDay();

            if (day == 0)
                str = str.replace("day", "日");
            else if (day == 1)
                str = str.replace("day", "一");
            else if (day == 2)
                str = str.replace("day", "二");
            else if (day == 3)
                str = str.replace("day", "三");
            else if (day == 4)
                str = str.replace("day", "四");
            else if (day == 5)
                str = str.replace("day", "五");
            else if (day == 6)
                str = str.replace("day", "六");

        }

    }
    return str;
};

String.prototype.toMinutes = function () {
    var time = this.split(':');
    return time[0] * 60 + parseInt(time[1]);
};