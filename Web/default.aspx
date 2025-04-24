<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Web._default" %>

<!DOCTYPE html>
<html lang="en" ng-app="app">
<head>
    <meta charset="UTF-8">
    <title ng-bind="pageTitle"></title>
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

    <link rel="stylesheet" href="css/style.css"/>
    
    <!--libs-->
    <script type="text/javascript" src="Scripts/lib/jquery-3.1.1.js"></script>
    <script type="text/javascript" src="Scripts/lib/angular.js"></script>
    <script type="text/javascript" src="Scripts/lib/angular-ui-router.js"></script>
    <script type="text/javascript" src="Scripts/lib/ngDialog.js"></script>
    <script type="text/javascript" src="Scripts/lib/jquery.qrcode.js"></script>
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
    <script type="text/javascript" src="Scripts/controller/services.js"></script>
    <script type="text/javascript" src="Scripts/controller/service.js"></script>
    <script type="text/javascript" src="Scripts/controller/service_appoint.js"></script>
    <script type="text/javascript" src="Scripts/controller/login.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointment_success.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointments_pending.js"></script>
    <script type="text/javascript" src="Scripts/controller/foot.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointment.js"></script>
    <script type="text/javascript" src="Scripts/controller/usercenter.js"></script>
    <script type="text/javascript" src="Scripts/controller/appointments.js"></script>
    <script type="text/javascript" src="Scripts/controller/user_edit.js"></script>
    <script type="text/javascript" src="Scripts/controller/error.js"></script>
    <script type="text/javascript" src="Scripts/controller/salon.js"></script>
    <script type="text/javascript" src="Scripts/controller/consumecode.js"></script>
    <script type="text/javascript" src="Scripts/controller/purchasedservices.js"></script>
    <script type="text/javascript" src="Scripts/controller/receipts.js"></script>
    <script type="text/javascript" src="Scripts/controller/receipt.js"></script>
    <script type="text/javascript" src="Scripts/controller/purchasedservice.js"></script>
    <!--controllers-->
    <%} %>
    <%else {%>
    <link rel="stylesheet" href="css/allstyle.css?d=<%=ConfigurationManager.AppSettings["AllCSSDigest"] %>"/>
	<script type="text/javascript" src="Scripts/allscript.js?d=<%=ConfigurationManager.AppSettings["AllScriptDigest"] %>"></script>
    <%}%>
    <!--global vailables-->
    <script>
        app.run(function ($rootScope) {
            $rootScope.APIURL= "<%= ConfigurationManager.AppSettings["ApiUrl"]%>";
            $rootScope.PAGESIZE= <%= ConfigurationManager.AppSettings["PageSize"]%>;
        });
        app.constant("htmlTemplates", <%= NGHtmlTemplatesJSON%>);
        app.constant("directiveInculdeHtmlTemplates", <%= NGDirectiveInculdeHtmlTemplatesJSON%>);
    </script>
    <!--global vailables-->
</head>
<body>
    <ui-view></ui-view>
</body>
</html>