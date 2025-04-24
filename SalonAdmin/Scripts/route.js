var app = angular.module("app", ["ui.router", "ngDialog"]);

app.run(["$rootScope", "$state", "storageService", "directiveInculdeHtmlTemplates", function ($rootScope, $state, storageService, directiveInculdeHtmlTemplates) {
    for (var i = 0; i < directiveInculdeHtmlTemplates.length; i++) {
        $rootScope[directiveInculdeHtmlTemplates[i].name] = directiveInculdeHtmlTemplates[i].templateUrl + "?d=" + directiveInculdeHtmlTemplates[i].digest;
    }

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        //默认页面body的背景色是灰色，部分页面的body的背景色是白色,如果需要白色背景，需在指定的controller里设置$rootScope.setWhiteBody = true;
        $rootScope.setWhiteBody = false;
        var states = ["home", "appointments", "appointment", "customers", "employees", "employee_edit", "salon", "salon_edit", "services", "service", "service_edit", "unavaitime", "setup", "appointments_completed", "help", "help_applywechat", "help_bindwechat", "help_saloninfo", "aboutus", "profile", "user", "memo", "user_add"];

        if (states.indexOf(toState.name) > -1) {
            if (!storageService.getJWT()) {
                $state.go("login");
                event.preventDefault();
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
    $urlRouterProvider.otherwise("/home");
}]);
