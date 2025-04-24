(function () {

    app.factory('serviceAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getServices: function (pageNumber, identityCode, serviceTypeId) {
                return apiService.call("get", "service/" + pageNumber + "/" + $rootScope.PAGESIZE + "/" + identityCode + "/" + serviceTypeId);
            },
            getService: function (serviceId, identityCode) {
                return apiService.call("get", "service/" + serviceId + "/" + identityCode);
            },
            getHotServices: function (top) {
                return apiService.call("get", "service/hot/" + top);
            },
            getServiceTypes: function (identityCode) {
                return apiService.call("get", "service/hasserviceservicetypes/" + identityCode);
            }
        };
    }]);

    app.factory('authAPIService', ["apiService", function (apiService) {
        return {
            sendValidCode: function (mobile, identityCode) {
                return apiService.call("get", "auth/validcode/" + mobile + "/" + identityCode);
            },
            login: function (mobile, code, identityCode) {
                return apiService.call("get", "auth/user/" + mobile + "/" + code + "/" + identityCode);
            }
            
        };
    }]);

    app.factory('purchasedServiceAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getPurchasedServices: function (pageNumber, pageSize, isAvai) {
                if (pageSize == null)
                    return apiService.call("get", "purchasedservice/" + pageNumber + "/" + $rootScope.PAGESIZE + "/" + isAvai);
                else
                    return apiService.call("get", "purchasedservice/" + pageNumber + "/" + pageSize + "/" + isAvai);
            },
            getPurchasedServiceCount: function () {
                return apiService.call("get", "purchasedservice/count");
            },
            getPurchasedService: function (purchasedServiceId) {
                return apiService.call("get", "purchasedservice/" + purchasedServiceId);
            },
        };
    }]);

    app.factory('salonAPIService', ["salonAPIService", function (apiService) {
        return {
            getSalon: function (identityCode) {
                return apiService.call("get", "salon/" + identityCode);
            }
        };
    }]);

    app.factory('userAPIService', ["apiService", function (apiService) {
        return {
            getUser: function () {
                return apiService.call("get", "user");
            },
            updateUser: function (json) {
                return apiService.call("put", "user", json);
            },
            generateConsumeCode: function () {
                return apiService.call("get", "user/consumecode");
            }
        };
    }]);


    app.factory('consumedServiceAPIService', ["apiService", function (apiService) {
        return {
            getUnConfirmedConsumedServices: function () {
                return apiService.call("get", "consumedservice");
            },
            getConsumedService: function (id) {
                return apiService.call("get", "consumedservice/" + id);
            },
            changeConsumedServiceStatus: function (json) {
                return apiService.call("put", "consumedservice/changeconsumedservicestatus", json);
            }
        };
    }]);

    app.factory('appointmentAPIService', ["apiService", function (apiService) {
        return {
            getAvaiAppointments: function (serviceId) {
                return apiService.call("get", "appointment/" + serviceId);
            },

            createAppointment: function (json) {
                return apiService.call("post", "appointment/add", json);
            },

            getUserAppointments: function (date) {
                if (date == null)
                    return apiService.call("get", "appointment/user");
                else
                    return apiService.call("get", "appointment/user/" + date);
            },

            getAppointment: function (appointmentId) {
                return apiService.call("get", "appointment/detail/" + appointmentId);
            },

            changeAppointStatus: function (json) {
                return apiService.call("put", "appointment", json);
            }

        };
    }]);
})();


