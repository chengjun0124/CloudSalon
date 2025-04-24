/**
 * Created by Administrator on 2017/3/31 0031.
 */
//选择哪一天
$(".clock_day_list ul li").each(function(){
    var _this=$(this);
    _this.click(function(){
        _this.parent().find("li").attr("class","");
        _this.addClass("week_current");
    })
});