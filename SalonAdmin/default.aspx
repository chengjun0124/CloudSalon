<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SalonAdmin._default" %>

<!DOCTYPE html>
<html lang="en" ng-app="app">
<head>
    <meta charset="UTF-8">
    <title>美容院管理系统</title>
    <meta name="viewport" content="width=750,user-scalable=no">
    <meta name="format-detection" content="telephone=no,address=no,email=no">
    <meta name="mobileOptimized" content="width">
    <meta name="handheldFriendly" content="true">
    <meta name="apple-mobile-web-app-capable" content="yes">

    <%if(HttpContext.Current.IsDebuggingEnabled) {%>
    <!--libs-->
    <link href="css/ngDialog.css" rel="stylesheet" />
    <link href="css/ngDialog-theme-default.css" rel="stylesheet" />  
    <!--libs-->


    <link rel="stylesheet" href="css/manage_style.css"/>


    <!--libs-->
    <script type="text/javascript" src="Scripts/lib/jquery-3.1.1.js"></script>
    <script type="text/javascript" src="Scripts/lib/angular.js"></script>
    <script type="text/javascript" src="Scripts/lib/angular-ui-router.js"></script>
    <script type="text/javascript" src="Scripts/lib/ngDialog.js"></script>
    <!--libs-->


    <!--common js-->
    <script type="text/javascript" src="Scripts/route.js"></script>
    <script type="text/javascript" src="Scripts/service.js"></script>
    <script type="text/javascript" src="Scripts/api.js"></script>
    <script type="text/javascript" src="Scripts/filter.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/directive.js"></script>
    <!--common js-->
    <!--controllers-->
    <script type="text/javascript" src="Scripts/controller/login.js"></script>
    <script type="text/javascript" src="Scripts/controller/home.js"></script>
    <script type="text/javascript" src="Scripts/controller/foot.js"></script>
    <script type="text/javascript" src="Scripts/controller/error.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointments.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointment.js"></script>
    <script type="text/javascript" src="Scripts/controller/customers.js"></script>
    <script type="text/javascript" src="Scripts/controller/employees.js"></script>
    <script type="text/javascript" src="Scripts/controller/employee_edit.js"></script>
    <script type="text/javascript" src="Scripts/controller/salon.js"></script>
    <script type="text/javascript" src="Scripts/controller/salon_edit.js"></script>
    <script type="text/javascript" src="Scripts/controller/services.js"></script>
    <script type="text/javascript" src="Scripts/controller/service.js"></script>
    <script type="text/javascript" src="Scripts/controller/service_edit.js"></script>
    <script type="text/javascript" src="Scripts/controller/unavaitime.js"></script>
    <script type="text/javascript" src="Scripts/controller/setup.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointments_completed.js"></script>
    <script type="text/javascript" src="Scripts/controller/help.js"></script>
    <script type="text/javascript" src="Scripts/controller/help_applywechat.js"></script>
    <script type="text/javascript" src="Scripts/controller/help_bindwechat.js"></script>
    <script type="text/javascript" src="Scripts/controller/help_saloninfo.js"></script>
    <script type="text/javascript" src="Scripts/controller/aboutus.js"></script>
    <script type="text/javascript" src="Scripts/controller/salon_select.js"></script>
    <script type="text/javascript" src="Scripts/controller/profile.js"></script>
    <script type="text/javascript" src="Scripts/controller/user.js"></script>
    <script type="text/javascript" src="Scripts/controller/memo.js"></script>
    <script type="text/javascript" src="Scripts/controller/purchasedservice.js"></script>
    <script type="text/javascript" src="Scripts/controller/service_select.js"></script>
    <script type="text/javascript" src="Scripts/controller/service_purchase.js"></script>
    <script type="text/javascript" src="Scripts/controller/user_add.js"></script>
    <script type="text/javascript" src="Scripts/controller/check.js"></script>
    <script type="text/javascript" src="Scripts/controller/button_preview.js"></script>
    <script type="text/javascript" src="Scripts/controller/check_anonym.js"></script>
    <script type="text/javascript" src="Scripts/controller/bscan_complete.js"></script>
    <script type="text/javascript" src="Scripts/controller/consumedservices.js"></script>
    <script type="text/javascript" src="Scripts/controller/consumedservice.js"></script>    
    <!--controllers-->
    <%} %>
    <%else {%>
    <link rel="stylesheet" href="css/allstyle.css?d=<%=ConfigurationManager.AppSettings["AllCSSDigest"] %>"/>
	<script type="text/javascript" src="Scripts/allscript.js?d=<%=ConfigurationManager.AppSettings["AllScriptDigest"] %>"></script>
    <%}%>
    <!--global availables-->
    <script>
        app.run(function ($rootScope) {
            $rootScope.APIURL= "<%= ConfigurationManager.AppSettings["ApiUrl"]%>";
            $rootScope.PAGESIZE= <%= ConfigurationManager.AppSettings["PageSize"]%>;
            $rootScope.FRONTENDURL= "<%= ConfigurationManager.AppSettings["FrondendUrl"]%>";
            
            window.jsInterface = {
                "getBackURL": function () {
                    if ($("ui-view").scope().routeName == undefined)
                        control.getBackURL(null);
                    else
                        control.getBackURL(location.origin + location.pathname + "#!/" + $("ui-view").scope().routeName);
                },
                "scanResult": function (result, type) {
                    $("ui-view").scope().scanResult(result);
                }
            };

        });
        app.constant("htmlTemplates", <%= NGHtmlTemplatesJSON%>);
        app.constant("directiveInculdeHtmlTemplates", <%= NGDirectiveInculdeHtmlTemplatesJSON%>);
    </script>
    <!--global availables-->
</head>
<body ng-class="{'bgc_white':setWhiteBody==true}">
    <ui-view></ui-view>
</body>
</html>