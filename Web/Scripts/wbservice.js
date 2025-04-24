
(function () {

    app.factory("locationEx", function ($location) {

        $location.QueryString = function (query) {            
            var value = $location.absUrl().match(new RegExp("[?&]" + query + "=([^#&]*)"));            
            return value == null ? null : value[1];
        };

        return $location;
    });

    app.factory('wbService', function ($http, $state) {

        return {
            goParams: function (routeName, params) {
                $('html, body').scrollTop(0);
                $state.go(routeName, params, { reload: true });
            },
            go: function (name, params) {
                $('html, body').scrollTop(0);
                $state.go(name);
            },
            goWBPages: function (url, isOpenNew, openWindowName, openParams) {
                if (url.toLowerCase().indexOf(".aspx") > -1) {
                    url = '/bradmin/Common/ASP/xferSession.asp?redirect=' + encodeURI(url).replace(/&/g,'%26');
                }
                if (isOpenNew == true){
                    window.open(url, openWindowName, openParams);
                }
                else {
                    parseAndRedirectUrl(url, null, null);
                }

                //<iframe id="BRS_Enterprise_main" name="BRS_Enterprise_main" class="legacyFrame" title="legacyframe" src="/BRAdmin" onload="legacyFrameLoad()"></iframe>
            }

        }

    });

    app.factory('apiService', function ($http, wbService, ngDialog, $timeout) {
        var callAPICount = 0;
        var ngDialogHandle = null;
        var sessionExpiredTime = 60;   //Set 60 Minutes for Popup window and Redirect to SessionExpired page in new framework
        var sessionGoingtoExpiredTime = 5; //Set session is going to expire by 5 Minutes, ShowDialog, "Keep Working" and "Log out"
        var sessionTimeMinutes = 1000 * 60 ; //1 Minutes = 60000
        var sessionTimer = null;

        function defaultAPIErrorHandler(error) {
            if (error.status == 500)
                wbService.goParams("error", { status: error.status, errorID: error.data.ErrorID });
            else if (error.status == 400)//call any API session expired then show the legacy session expiration within the iframe
                wbService.goWBPages("/workbench/Common/SessionExpired.htm", false, null, null);
            else
                wbService.goParams("error", { status: error.status });           

            callAPICount = 0;
            ngDialogHandle.close();            
            return false;
        };

        function timeShowDialog() {
            ngDialogHandle = ngDialog.open({
                template: "wbSessionExpiredDialog",
                showClose: false,
                closeByEscape: false,
                closeByDocument: false,
                className: "ngdialog-theme-default checkSession_Dialog",
                controller: "SessionDialogCtrl"
            });
        }

        function pageRedirect() {
            //1. Popup SessionExpired when in new framework, follows what we currently have.  
            //2. If new framework session expired then show the legacy session expiration within the iframe.
            wbService.goWBPages("/workbench/Common/SessionExpired.htm", false, null, null);
            wbService.goWBPages("/bradmin/login/htm/SessionExpired.html", true, "prolongsession", openWindows.open400_250_center_nomenu_notool.value);
            //clean session have not finished
        };

        function checkExpiredSession() {
            var ExpiredTime;
            $http.get("/api/header/sessiontimeout").then(function (resp) {
                if (resp.hasOwnProperty("data")) {
                    if (resp.data != null) {
                        if (resp.data.hasOwnProperty("Data")) {
                            ExpiredTime = resp.data.Data;
                            ExpiredTime = ExpiredTime / 10000;  //transfer Ticks to millisecond
                            if (typeof (sessionTimer) != 'undefined')
                                $timeout.cancel(sessionTimer);
                            if (ExpiredTime > (sessionExpiredTime - sessionGoingtoExpiredTime) * sessionTimeMinutes && ExpiredTime < sessionExpiredTime * sessionTimeMinutes) {
                                //3. Handling session that is going to expire, ShowDialog, "Keep Working" and "Log out", reference ProlongSession.html
                                sessionTimer = $timeout(checkExpiredSession, sessionExpiredTime * sessionTimeMinutes - ExpiredTime);
                                timeShowDialog();
                            }
                            else if (ExpiredTime > sessionExpiredTime * sessionTimeMinutes) {
                                //1,2 popup session and redirect to expired window  
                                ngDialog.close();
                                pageRedirect();
                            }
                            else {
                                //refresh timer
                                sessionTimer = $timeout(checkExpiredSession, (sessionExpiredTime - sessionGoingtoExpiredTime) * sessionTimeMinutes - ExpiredTime);
                            }
                        }
                    }
                }
            })            
        }

        function setSessionTimer() {
            if (typeof (sessionTimer) != 'undefined')
                $timeout.cancel(sessionTimer);

            sessionTimer = $timeout(function () {
                checkExpiredSession();
            }, (sessionExpiredTime - sessionGoingtoExpiredTime) * sessionTimeMinutes) //55 minutes run it
        }

        function promiseWrapper(p) {
            this.promise = p;

            this.then = function (success, customError) {
                var p = this.promise.then(function (resp) {                    
                    if (!resp) return false;

                    setSessionTimer();

                    if (resp.hasOwnProperty("data")) {
                        if (resp.data != null) {
                            if (resp.data.hasOwnProperty("Data")) {
                                resp = resp.data.Data;
                            }
                        }
                    }

                    function pascalToCamel(key, val) {
                        var specialKeyList = ["id", "url"];

                        if (specialKeyList.indexOf(key.toLowerCase()) != -1 && specialKeyList.indexOf(key) == -1) {
                            this[key.toLowerCase()] = val;
                            return undefined;
                        }

                        if (key.length > 0 && key[0].search(/[A-Z]/) == 0) {
                            this[key.charAt(0).toLowerCase() + key.slice(1)] = val;
                            return undefined;
                        }
                        return val;
                    }

                    var strJSON = JSON.stringify(resp);
                    resp = JSON.parse(strJSON, pascalToCamel);

                    var o = null;
                    if (success)
                        o = success(resp);

                    callAPICount--;
                    if (callAPICount == 0) {
                        ngDialogHandle.close();
                    }
                   
                    if (o instanceof promiseWrapper)
                        return o.promise;
                    else
                        return o;
                },
                customError == null ? defaultAPIErrorHandler : function (error) {
                    var o = customError(error);
                    callAPICount--;
                    if (callAPICount == 0) {
                        ngDialogHandle.close();
                    }

                    if (o instanceof promiseWrapper)
                        return o.promise;
                    else
                        return o;
                });
                return new promiseWrapper(p);
            };
        };

        return {
            call: function (method, path, params) {
                if (callAPICount == 0) {
                    ngDialogHandle = ngDialog.open({
                        template: "<i class=\"fa fa-spinner fa-pulse fa-3x\"></i><br><span>Loading, please wait</span>",
                        appendClassName: "ngdialog-theme-loading",
                        plain: true,
                        showClose: false,
                        closeByEscape: false,
                        closeByDocument: false
                    });
                }
                callAPICount++;
                
                var promise = $http({ url: global_APIUrl + path, method: method, data: params });
                return new promiseWrapper(promise);
            }

        }
    });
    

    app.factory('gqAPIService', function (apiService) {
        return {
            getGQList: function () {
                return apiService.call("get", "GQs", null);
            },
            inactivateGQ: function (id, json) {
                return apiService.call("put", "GQs/" + id + "/Inactivate", null);
            }
        }   
    });

    app.factory('formAPIService', function (apiService) {
        return {
            getFormDetails: function (formTypeId) {
                return apiService.call("get", "v1/forms/" + formTypeId);
            },
            saveFormDetails : function (formData) {
                return apiService.call("post", "v1/forms", formData);
            },
            getFormList: function () {
                return apiService.call("get", "v1/forms");
            }
        }
    });

    app.factory('environmentAPIService', function (apiService) {
        return {
            selectEnvironment: function (json) {
                return apiService.call("put", "header/environment", json);
            },
            selectEnvironmentAndGetClientList: function (json) {
                return apiService.call("put", "header/environment/clients", json);
            },
            selectClient: function (json) {
                return apiService.call("put", "header/client", json);
            },
            getUserSession: function () {
                return apiService.call("get", "header/usersession", null);
            },
            updateSession: function () {
                return apiService.call("put", "header/sessiontimeout", null);
            }
        }
    });

    app.factory('hrStatusAPIService', function (apiService) {
        return {
            getHRStatusList: function () {
                return apiService.call("get", "hrstatus");
            }
        }
    });

    app.factory('contextMenuItemsBuilderService', function (wbService, ngDialog) {
        return {
            avalible: function (item, fieldName, value) {
                for (var p in item) {
                    if (p == fieldName) {
                        return value.indexOf(item[p]) > -1
                    }
                }
                return true;
            },
            getContextMenuItems: function (menuItems, item) {
                function replaceObjectValue(source, destination) {
                    if (typeof (destination) == "string") {
                        var str = destination;
                        if (str[0] == "$") {
                            str = str.substr(1, str.length);
                            str = source[str];
                        }
                        return str;
                    }
                    else if (typeof (destination) == "object") {
                        var newObj = Array.isArray(destination) ? [] : {};

                        for (var property in destination) {
                            newObj[property] = replaceObjectValue(source, destination[property]);
                        }
                        return newObj;
                    }
                    else {
                        return destination;
                    }
                }
                var returnedMenuItems = [];
                for (var i = 0; i < menuItems.length; i++) {

                    if (menuItems[i].getVisibleValue) {
                        if (menuItems[i].getVisibleValue(item) == false)
                            continue;
                    }
                    var fn;
                    if (menuItems[i].handleType == "route") {
                        var routeParams = replaceObjectValue(item, menuItems[i].routeParams);
                        fn = function (routeName, routeParams, isLegacy, isOpenNew, openWindowName, openParams) {
                            return function () {                                
                                if(isLegacy)
                                {
                                    url = routeName + "?clientid=" + routeParams.clientid + "&folder=" + routeParams.folder + "&folderdesc=" + routeParams.folderdesc + "&mode=" + routeParams.mode;
                                    wbService.goWBPages(url, isOpenNew, openWindowName, openParams);
                                }
                                else
                                {
                                    wbService.goParams(routeName, routeParams);
                                }
                            }
                        }(menuItems[i].routeName, routeParams, menuItems[i].isLegacy, menuItems[i].isOpenNew, menuItems[i].openWindowName, menuItems[i].openParams);
                    }
                    else if (menuItems[i].handleType == "api") {
                        var params = replaceObjectValue(item, menuItems[i].serviceFnParams);

                        fn = function (serviceFn, serviceParams, serviceSuccessFn, serviceFailFn) {
                            return function () {
                                if (serviceFailFn)
                                    serviceFn.apply(null, serviceParams).then(function (resp) { serviceSuccessFn(item, resp) }, function (error) { serviceFailFn(item, error) });
                                else
                                    serviceFn.apply(null, serviceParams).then(function (resp) { serviceSuccessFn(item, resp) });
                            };
                        }(menuItems[i].serviceFn, params, menuItems[i].serviceSuccessFn, menuItems[i].serviceFailFn);

                    }
                    else if (menuItems[i].handleType == "dialog") {
                        var ngDialogData = replaceObjectValue(item, menuItems[i].ngDialogData);

                        fn = function (templateUrl, ngDialogData, controller) {
                            return function () {
                                ngDialog.open({
                                    templateUrl: templateUrl,
                                    controller: controller,
                                    data: ngDialogData
                                });
                            }
                        }(menuItems[i].templateUrl, ngDialogData, menuItems[i].controller);
                    }

                    returnedMenuItems.push({
                        name: menuItems[i].name,
                        fn: fn
                    });
                }
                return returnedMenuItems;
            }
        }
    });

    app.factory("locationService", function () {
        return {
            locateToElement: function (obj, options, isRelocateOnResize, isFixed) {
                if (typeof obj == "string")
                    obj = $(obj);

                var left = 0, top = 0;
                var targetElement = options.targetElement;
                
                if (targetElement) {
                    if (typeof targetElement == "string")
                        targetElement = $(targetElement);

                    left = targetElement.offset().left;
                    top = targetElement.offset().top;
                }

                
                var targetElementX = options.targetElementX;
                if (targetElementX) {
                    if (typeof targetElementX == "string")
                        targetElementX = $(targetElementX);

                    left = targetElementX.offset().left;
                }
                    

                /*
                four alignment methods:
                1. LL(Left to left: left side of conetxt menu align left side of target element), this is default
                2. LR(Left to right: left side of conetxt menu align right side of target element)
                3. RL(right to left: right side of conetxt menu align left side of target element)
                4. RR(right to right: right side of conetxt menu align right side of target element)
                */
                if (options.alignX) {
                    if (options.alignX[0] == "R")
                        left = left - obj.outerWidth();
                    if (options.alignX[1] == "R")
                        left = left + (targetElementX ? targetElementX.outerWidth() : targetElement.outerWidth());
                }

                if (options.offsetX)
                    left = left + options.offsetX;


                var targetElementY = options.targetElementY;
                if (targetElementY) {
                    if (typeof targetElementY == "string")
                        targetElementY = $(targetElementY);

                    top = targetElementY.offset().top;
                }
                        

                /*
                four alignment methods:
                1. TT(top to top: top side of conetxt menu align top side of target element), this is default
                2. TB(top to bottom: top side of conetxt menu align bottom side of target element)
                3. BT(bottom to top: bottom side of conetxt menu align top side of target element)
                4. BB(bottom to bottom: bottom side of conetxt menu align bottom side of target element)
                */
                if (options.alignY) {
                    if (options.alignY[0] == "B")
                        top = top - obj.outerHeight();
                    if (options.alignY[1] == "B")
                        top = top + (targetElementY ? targetElementY.outerHeight() : targetElement.outerHeight());
                }

                if (options.offsetY)
                    top = top + options.offsetY;

                if (isFixed) {
                    top = top - $(window).scrollTop();
                    left = left - $(window).scrollLeft();
                }
                
                
                obj.css({
                    left: left + "px",
                    top: top + "px",
                });

                if (isRelocateOnResize) {
                    var that = this;
                    $(window).resize(function () {
                        that.locateToElement(obj, options, false);
                    });
                }
            }
        };
    });

})();


