var app = angular.module("app", ["ui.router", "ngDialog"]);

app.run(["$rootScope", "$state", "locationEx", "directiveInculdeHtmlTemplates", "$timeout", function ($rootScope, $state, locationEx, directiveInculdeHtmlTemplates, $timeout) {
    for (var i = 0; i < directiveInculdeHtmlTemplates.length; i++) {
        $rootScope[directiveInculdeHtmlTemplates[i].name] = directiveInculdeHtmlTemplates[i].templateUrl + "?d=" + directiveInculdeHtmlTemplates[i].digest;
    }

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        var browserBackEvent = $rootScope.$broadcast("browserBack");//下发浏览器后退按钮事件

        /*
        关闭ng-dialog的代码必须在这里去关闭，如果在接受通知的地方关闭，比如在dialogService.showMention里关闭
        $rootScope.$broadcast会抛异常，原因是关闭ng-dialog后，ng-dialog的DIV会被卸载，然而$rootScope.$broadcast会引用当前页面的所有HTML元素。
        */
        if (browserBackEvent.callBack)
            browserBackEvent.callBack();
        
        //browserBackEvent.defaultPrevented==true表示页面不需要后退，所以$state.go设置成当前state
        if (browserBackEvent.defaultPrevented) {
            $state.go(fromState, fromParams);
            event.preventDefault();

            //阻止了浏览器后退，但是浏览器的title被后退了，通过下面代码，设置浏览器的title成正确的值
            var pageTitle = $rootScope.pageTitle;
            $rootScope.pageTitle = "";
            $timeout(function () {
                $rootScope.pageTitle = pageTitle;
            }, 1);
        }
        else {
            var states = ["appointment_success", "appointments_pending", "appointment", "appointment_cancel_success", "usercenter", "appointments", "user_edit", "service_appoint", "consumecode", "purchasedservices", "receipts"];

            var identityCode = locationEx.queryString("identitycode");
            if (states.indexOf(toState.name) > -1) {
                if (!localStorage[identityCode]) {
                    $state.go("login", { "redirect": encodeURIComponent("{ \"state\": \"" + toState.name + "\", \"params\": " + JSON.stringify(toParams) + "}") });
                    event.preventDefault();
                }
            }
        }
    });
}]);

app.config(["$stateProvider", "$urlRouterProvider", "htmlTemplates", function ($stateProvider, $urlRouterProvider, htmlTemplates) {
    for (var i = 0; i < htmlTemplates.length; i++) {
        $stateProvider.state(htmlTemplates[i].state, {
            url: htmlTemplates[i].url,
            templateUrl: htmlTemplates[i].templateUrl + "?d=" + htmlTemplates[i].digest,
            controller: htmlTemplates[i].controller
        });
    }
}]);

//app.filter('to_trusted', ['$sce', function ($sce) {
//    return function (text) {
//        return $sce.trustAsHtml(text);
//    };
//}]);

//app.filter('Regex', function () {
//    return function(input, field, regex) {
//        var f, fields, i, j, out, patt;
//        if (input != null) {
//            patt = new RegExp(regex);
//            i = 0;
//            out = [];
//            while (i < input.length) {
//                fields = field.split('.').reverse();
//                j = input[i];
//                while (fields.length > 0) {
//                    f = fields.pop();
//                    j = j[f];
//                }
//                if (patt.test(j)) {
//                    out.push(input[i]);
//                }
//                i++;
//            }
//            return out;
//        }
//    };
//});

