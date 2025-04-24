(function () {
    app.factory('authAPIService', ["apiService", function (apiService) {
        return {
            auth: function (userName, password) {
                return apiService.call("get", "auth/ee/" + userName + "/" + password);
            }
        };
    }]);

    app.factory('appointmentAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getAppointments: function (pageNumber) {
                return apiService.call("get", "appointment/" + pageNumber + "/" + $rootScope.PAGESIZE);
            },
            getAppointment: function (appointmentId) {
                return apiService.call("get", "appointment/detail/" + appointmentId);
            },
            changeAppointStatus: function (json) {
                return apiService.call("put", "appointment", json);
            },
            getTodayAppointmentCount: function () {
                return apiService.call("get", "appointment/todaycount");
            },
            getRecentAppointment: function () {
                return apiService.call("get", "appointment/recent");
            },
            getCompletedAppointmentCount: function (date) {
                return apiService.call("get", "appointment/count/" + date);
            },
            getCompletedAppointments: function (date, pageNumber) {
                return apiService.call("get", "appointment/" + date + "/" + pageNumber + "/" + $rootScope.PAGESIZE);
            },
            getAvaiAppointments: function (userId) {
                return apiService.call("get", "appointment/avai/user/" + userId);
            }
        };
    }]);

    app.factory('consumedServiceAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            aoCharge: function (json) {
                return apiService.call("post", "consumedservice/aocharge", json);
            },
            aoScan: function (json) {
                return apiService.call("post", "consumedservice/aoscan", json);
            },
            checkAnonym: function (json) {
                return apiService.call("post", "consumedservice/checkanonym", json);
            },
            beauticianScan: function (json) {
                return apiService.call("post", "consumedservice/beauticianscan", json);
            },
            getConsumedServices: function (date, pageNumber) {
                return apiService.call("get", "consumedservice/" + date + "/" + pageNumber + "/" + $rootScope.PAGESIZE);
            },
            getConsumedServiceCount: function (date) {
                return apiService.call("get", "consumedservice/count/" + date);
            },
            getConsumedService: function (id) {
                return apiService.call("get", "consumedservice/" + id);
            },
            deleteConsumedService: function (id) {
                return apiService.call("delete", "consumedservice/" + id);
            },
            changeConsumedService: function (json) {
                return apiService.call("put", "consumedservice/changeconsumedservice", json);
            }
        };
    }]);

    app.factory('userAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getUsers: function (pageNumber, keyword) {
                if (keyword == null || keyword.length == 0)
                    return apiService.call("get", "user/" + pageNumber + "/" + $rootScope.PAGESIZE);
                else
                    return apiService.call("get", "user/" + pageNumber + "/" + $rootScope.PAGESIZE + "/" + keyword);
            },
            getUserCount: function () {
                return apiService.call("get", "user/count");
            },
            getUser: function (userId) {
                return apiService.call("get", "user/" + userId);
            },
            updateMemo: function (json) {
                return apiService.call("put", "user/memo", json);
            },
            createUser: function (json) {
                return apiService.call("post", "user", json);
            },
            getUserId: function (consumeCode) {
                return apiService.call("get", "user/consumecode/" + consumeCode);
            }
        };
    }]);

    app.factory('purchasedServiceAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getPurchasedServices: function (pageNumber, userId) {
                return apiService.call("get", "purchasedservice/" + pageNumber + "/" + $rootScope.PAGESIZE + "/null/" + userId);
            },
            getPurchasedServiceCount: function (userId) {
                return apiService.call("get", "purchasedservice/count/" + userId);
            },
            getPurchasedService: function (purchasedServiceId) {
                return apiService.call("get", "purchasedservice/" + purchasedServiceId);
            },
            createPurchasedService: function (json) {
                return apiService.call("post", "purchasedservice", json);
            }
        };
    }]);

    app.factory('employeeAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getEmployees: function (pageNumber) {
                return apiService.call("get", "employee/" + pageNumber + "/" + $rootScope.PAGESIZE);
            },
            getEmployeeCount: function () {
                return apiService.call("get", "employee/count");
            },
            getEmployee: function (employeeId) {
                if (employeeId != null)
                    return apiService.call("get", "employee/" + employeeId);
                else
                    return apiService.call("get", "employee");
            },
            updateEmployee: function (json) {
                return apiService.call("put", "employee", json);
            },
            createEmployee: function (json) {
                return apiService.call("post", "employee", json);
            },
            deleteEmployee: function (id) {
                return apiService.call("delete", "employee/" + id);
            },
            updateProfile: function (json) {
                return apiService.call("put", "employee/profile", json);
            },
            getBeauticians: function () {
                return apiService.call("get", "employee/beautician");
            }
        };
    }]);

    app.factory('salonAPIService', ["apiService", function (apiService) {
        return {
            getSalon: function () {
                return apiService.call("get", "salon");
            },
            updateSalon: function (json) {
                return apiService.call("put", "salon", json);
            }
        };
    }]);

    app.factory('tagAPIService', ["apiService", function (apiService) {
        return {
            getServiceTypeTags: function (serviceTypeId) {
                return apiService.call("get", "tag/servicetype/" + serviceTypeId);
            }
        };
    }]);

    app.factory('serviceAPIService', ["apiService", "$rootScope", function (apiService, $rootScope) {
        return {
            getServices: function (pageNumber) {
                return apiService.call("get", "services/" + pageNumber + "/" + $rootScope.PAGESIZE);
            },
            getServiceCount: function () {
                return apiService.call("get", "service/count");
            },
            getService: function (serviceId) {
                return apiService.call("get", "service/" + serviceId);
            },
            updateService: function (json) {
                return apiService.call("put", "service", json);
            },
            createService: function (json) {
                return apiService.call("post", "service", json);
            },
            deleteService: function (id) {
                return apiService.call("delete", "service/" + id);
            },
            getServiceTypes: function () {
                return apiService.call("get", "service/servicetypes");
            },
            getAvaiServices: function (userId) {
                return apiService.call("get", "service/avai/user/" + userId);
            }
        };
    }]);

    app.factory('unavaiTimeAPIService', ["apiService", function (apiService) {
        return {
            createUnavaiTime: function (json) {
                return apiService.call("post", "unavaitime", json);
            },
            getUnavaiTimes: function () {
                return apiService.call("get", "unavaitime");
            },
            deleteUnavaiTime: function (id) {
                return apiService.call("delete", "unavaitime/" + id);
            }
        };
    }]);
    
})();