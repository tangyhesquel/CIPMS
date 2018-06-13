<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WipAndOutputNew.aspx.cs" Inherits="WipAndOutputNew" %>

<!DOCTYPE html>
<html>
<head>

    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
    <script src="js/jquery.chromatable.js" type="text/javascript"></script>
    <link href="css/mobiscroll.custom-2.5.0.min.css" rel="stylesheet" type="text/css" />
    <script src="js/mobiscroll.custom-2.5.0.min.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
    <script src="js/zebra_datepicker.js" type="text/javascript"></script>
    <link href="css/datecss/default.css" rel="stylesheet" type="text/css" />
    
    <link href="KendoUI/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="KendoUI/kendo.default.min.css" rel="stylesheet" type="text/css" />
    <link href="KendoUI/kendo.mobile.all.min.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/kendo.all.min.js" type="text/javascript"></script>
    <script src="Scripts/jszip.min.js" type="text/javascript"></script>

   <%-- <script src="Scripts/jquery.min.js" type="text/javascript"></script>
    <script src="Scripts/kendo.all.min.js" type="text/javascript"></script>
    <script src="Scripts/jszip.min.js" type="text/javascript"></script>
    <script src="Scripts/cultures/kendo.culture.zh-CN.min.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>--%>


    <title>OUTPUT AND WIP REPORT</title>
</head>
<body>
    <style type="text/css">
        th
        {
            background: #dde6f5;
            border-top: 1px solid #d6d6d6;
            border-bottom: 1px solid #d6d6d6;
        }
        tr
        {
            border-left: 1px solid #d6d6d6;
            border-right: 1px solid #d6d6d6;
        }
        .table-tr-1
        {
            background: #e9e9e9;
        }
        .table-tr-2
        {
            background: #FFBB1C;
        }
        .table-tr-3
        {
            background: #FF4500;
        }
        tr.over td
        {
            background: #bcd4ec;
        }
    </style>
    <script type="text/javascript">

        var globeltranslate; //保存json翻译的字符串
        var fty = "";
        var environment = "";

        //$("#querybutton").kendoButton();
        $("#fromdate").kendoDatePicker();
        $("#todate").kendoDatePicker();

        //语言翻译函数
        window.language; //全局变量，保存用户选择了哪个json语言文件

        function translate() {
            //alert("translate");
            $.ajax({
                type: "GET",
                url: window.language,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                cache: false,
                error: function () {
                    alert("Sorry! Pls check your network and refresh your brower! Wrong Code::001");
                },
                success: function (json) {
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#jolabel').html(globeltranslate.jotext);
                    $('#golabel').html(globeltranslate.gotext);
                    
                    $('#fromdatelabel').html(globeltranslate.fromdatelabel);
                    $('#todatelabel').html(globeltranslate.todatelabel);
                   // $('#querybutton').html(globeltranslate.querybutton);
                    $('#querybutton').val(globeltranslate.querybutton).button('refresh');
                    
                    $('#packagea').html(globeltranslate.packagea);
                    $('#matchinga').html(globeltranslate.matchinga);
                    $('#transactiona').html(globeltranslate.transactiona);
                    $('#receivea').html(globeltranslate.receivea);
                    $('#bundlereducea').html(globeltranslate.bundlereducea);
                    $('#printbarcodea').html(globeltranslate.printbarcodea);
                    $('#employeeoutputa').html(globeltranslate.employeeoutputa);
                    $('#externalreceivea').html(globeltranslate.externalreceivea);
                    $('#bundleinputa').html(globeltranslate.bundleinputa);
                    $('#reporta').html(globeltranslate.reporta);
                    $('#wipreporta').html(globeltranslate.wipreporta);
                    $('#outputreporta').html(globeltranslate.outputreporta);
                    $('#dctreporta').html(globeltranslate.dctreporta);
                    $('#docnoinquirya').html(globeltranslate.docnoinquirya);
                    $('#barcodeinquirya').html(globeltranslate.barcodeinquirya);
                    $('#wipandoutputlia').html(globeltranslate.wipandoutputlia);
                    $('#GRID_SPAN1').html(globeltranslate.GRID_SPAN1);
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                    select = globeltranslate.select;
                }
            });
            //alert("vvv");
        };
        
        function querydata()
        {
            var bydate = "false";
            if ($('#fromdate').val() != "" && $('#todate').val() != "")
                bydate = "true";
            if (($('#fromdate').val() != '' && $('#todate').val() == '') || ($('#fromdate').val() == '' && $('#todate').val() != '')) {
                alert(globeltranslate.querymessage3);
                return;
            }
            if ($('#jotext').val() == '' && $('#gotext').val() == '' && $('#fromdate').val() == '' && $('#todate').val() == '') {
                alert(globeltranslate.querymessage5);
                return;
            }
            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
 
            //var s;
            //s = "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'jo': '" + $('#jotext').val() + "', 'go': '" + $('#gotext').val() + "', 'fromdate': '" + $('#fromdate').val() + "', 'todate': '" + $('#todate').val() + "'}";
            //alert(s);

 
           $.ajax({
                    type: "POST",
                    url: "WipAndOutputNEW.aspx/Getoutput",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    async: false, //默认是true：异步，false：同步,等待执行完毕。
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'jo': '" + $('#jotext').val() + "', 'go': '" + $('#gotext').val() + "', 'fromdate': '" + $('#fromdate').val() + "', 'todate': '" + $('#todate').val() + "'}",
                    success: function (data) {
                        var result = eval(data.d);
                        //alert(result[0].Data);
                        if (result[0].SUCCESS == true) {
                            RefreshGridData(result[0].Data);
                            $.mobile.loading('hide');
                         }
                        else {
                            $.mobile.loading('hide');
                            alert(globeltranslate.errormessage + "031");
                            }
                            kendo.ui.progress($("#loading"), false);
                        },                                   
                        error: function (err) {
                            $.mobile.loading('hide');
                            alert(globeltranslate.errormessage + "031");
                        }
                    });

        };

        function init()
        {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                //语言选择
                if (window.localStorage.languageid == "1") {
                    window.language = "WipAndOutput/WipAndOutputNew-zh-CN.js";
                   } else if (window.localStorage.languageid == "2") {
                    window.language = "WipAndOutput/WipAndOutputNew-en-US.js";
                }
                translate();

                //function menu定义
                $("#my-menu").mmenu({
                    "footer": {
                        "add": true,
                        "title":globeltranslate.footertitle
                    },
                    "header": {
                        "title": globeltranslate.select,
                        "add": true,
                        "update": true
                    },
                    slidingSubmenus: false
                });
                $('#functionmenubutton').click(function () {
                    $('#my-menu').trigger('open.mm');
                });

                //用户登录之后才能进入系统模块
                if (sessionStorage.getItem("name") == null || sessionStorage.getItem("name") == "") {
                    Loginout(fty, environment);
                }
                else {
                    iprocess();
                    Accessmodulehide();
                   
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    var employeeno = sessionStorage.getItem("employeeno");
                    var factory = sessionStorage.getItem("factory");
                    var process = sessionStorage.getItem("process");
                    var productionline = sessionStorage.getItem("productionline");
                    var userbarcode = sessionStorage.getItem("userbarcode");
                    $('#jotext').focus();
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Accessfunction",
                            contentType: "application/json;charset=utf-8",
                            async: false,
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                            success: function (data) {
                                //判断用户的权限按钮
                                var result = eval(data.d);
                                //判断用户的权限模块
                                Accessmoduleshow(data.d);
                                $.mobile.loading('hide');
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "002");
                            }
                        });
                    }, 10);

                    //注销按钮：返回登录页面
                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });

                    //使用日期选择框架
                    $('#fromdate').Zebra_DatePicker();
                    $('#todate').Zebra_DatePicker();

                    //表格样式
                    var screenwidth = document.body.clientWidth;
                    var screenheight = document.body.clientHeight;
                    //$('#outputtable').chromatable({
                    //    width: screenwidth * 0.96,
                    //    height: screenheight * 0.6,
                    //    scrolling: "yes"
                    //}); $('#linetable').chromatable({
                    //    width: screenwidth * 0.96,
                    //    height: screenheight * 0.25,
                    //    scrolling: "yes"
                    //}); $('#jotable').chromatable({
                    //    width: screenwidth * 0.96,
                    //    height: screenheight * 0.6,
                    //    scrolling: "yes"
                    //});
                }
            }
        };

        function GridInit(dataSource) {
            $("#DataGrid").kendoGrid({
                toolbar: [
                    //{ template: kendo.template("<span>Group By Line</span>") },
                    { name: "excel", text: "导出到EXCEL" }
                ],
                pageable: {
                    messages: {
                        first: globeltranslate.GRID_FIRST,
                        last: globeltranslate.GRID_LAST,
                        next: globeltranslate.GRID_NEXT,
                        previous: globeltranslate.GRID_PREVIOUS,
                        display: "{0}-{1} / {2}",
                        itemsPerPage: globeltranslate.GRID_ITEMSPERPAGE,
                        empty: globeltranslate.GRID_EMPTY
                    },
                    pageSizes: [20,30,40,50,60,"all"]
                },
                resizable: true,
                scrollable: true,
                culture: "zh-CN",
                excel: {
                    allPages: true
                },
                dataSource:dataSource,
                editable: true,
                selectable: true,
                columns: [
                {
                    title: globeltranslate.CUT_LINE,
                    field: "CUT_LINE",
                    width: 75,
                    aggregates: ["count"],
                    groupHeaderTemplate: "#= value # #= count # 条记录",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }

                },{
                    title: globeltranslate.PRODUCTION_LINE_CD,
                    field: "PRODUCTION_LINE_CD",
                    width: 75,
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.JOB_ORDER_NO,
                    field: "JOB_ORDER_NO",
                    width: 100,
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.COLOR_CD,
                    field: "COLOR_CD",
                    width: 75,
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.ORDER_QTY,
                    field: "ORDER_QTY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.CUT_QTY_TODAY,
                    field: "CUT_QTY_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                    
                },{
                    title: globeltranslate.CUT_QTY_TOTAL,
                    field: "CUT_QTY_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.REDUCE_QTY_TODAY,
                    field: "REDUCE_QTY_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    hidden: true,
                    attributes: { style: "text-align: center; font-size: 11px" }
                   
                },{
                    title: globeltranslate.REDUCE_QTY_TOTAL,
                    field: "REDUCE_QTY_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    hidden: false,
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.CUT_OUT_TODAY,
                    field: "CUT_OUT_TODAY",
                    width: 80                    ,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.CUT_OUT_TOTAL,
                    field: "CUT_OUT_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.CUT_OUT_WIP,
                    field: "CUT_OUT_WIP",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.PRT_IN_TODAY,
                    field: "PRT_IN_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.PRT_IN_TOTAL,
                    field: "PRT_IN_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.PRT_OUT_TODAY,
                    field: "PRT_OUT_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.PRT_OUT_TOTAL,
                    field: "PRT_OUT_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.PRT_IN_OUT_WIP,
                    field: "PRT_IN_OUT_WIP",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.EMB_OUT_TODAY,
                    field: "EMB_OUT_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.EMB_OUT_TOTAL,
                    field: "EMB_OUT_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.EMB_OUT_WIP,
                    field: "EMB_OUT_WIP",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.FUSING_OUT_TODAY,
                    field: "FUSING_OUT_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.FUSING_OUT_TOTAL,
                    field: "FUSING_OUT_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.FUSING_OUT_WIP,
                    field: "FUSING_OUT_WIP",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.MATCHING_OUT_TODAY,
                    field: "MATCHING_OUT_TODAY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.MATCHING_OUT_TOTAL,
                    field: "MATCHING_OUT_TOTAL",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.MATCHING_OUT_WIP,
                    field: "MATCHING_OUT_WIP",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.DC_BEFORE_WIP,
                    field: "DC_BEFORE_WIP",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.DC_SEW_S_TOTAL,
                    field: "DC_SEW_S_TOTAL",
                    width: 80,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.DC_SEW_C,
                    field: "DC_SEW_C",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.DC_SEW_C_TOTAL,
                    field: "DC_SEW_C_TOTAL",
                    width: 80,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.TWIP,
                    field: "TWIP",
                    width: 70,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.DCT,
                    field: "DCT",
                    width: 70,
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    attributes: { style: "text-align: center; font-size: 11px" }
                },{
                    title: globeltranslate.CUT_QTY,
                    field: "CUT_QTY",
                    width: 75,
                    groupable: false,
                    aggregates: ["sum"],
                    groupFooterTemplate: "#= sum #",
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    hidden: true
                },{
                    title: globeltranslate.SEQ,
                    field: "SEQ",
                    width: 75,
                    headerAttributes: {
                        "class": "table-cell",
                        style: "text-align: center; font-size: 11px"
                    },
                    hidden: true,
                    attributes: { style: "text-align: center; font-size: 11px" }
                }
                ],
                groupable: true,
                sortable: true,
                sortable: {
                    mode: "multiple"
                }

            });
        }

        function RefreshGridData(datasource) {
            //alert(JSON.stringify(DataGrid_Datasource));
            var GriddataSource = new kendo.data.DataSource({
                data: datasource,
                //group: {
                //field:"CUT_LINE",
                //aggregates: [
                //    { field: "CUT_QTY_TODAY", aggregate: "sum" },
                //    { field: "CUT_OUT_TODAY", aggregate: "sum" },
                //    { field: "CUT_OUT_WIP", aggregate: "sum" },
                //    { field: "PRT_OUT_TODAY", aggregate: "sum" },
                //    { field: "PRT_IN_OUT_WIP", aggregate: "sum" },
                //    { field: "EMB_OUT_TODAY", aggregate: "sum" },
                //    { field: "EMB_OUT_WIP", aggregate: "sum" },
                //    { field: "FUSING_OUT_TODAY", aggregate: "sum" },
                //    { field: "FUSING_OUT_WIP", aggregate: "sum" },
                //    { field: "MATCHING_OUT_TODAY", aggregate: "sum" },
                //    { field: "MATCHING_OUT_WIP", aggregate: "sum" },
                //    { field: "DC_BEFORE_WIP", aggregate: "sum" },
                //    { field: "DC_SEW_S_TOTAL", aggregate: "sum" },
                //    { field: "DC_SEW_C", aggregate: "sum" },
                //    { field: "ORDER_QTY", aggregate: "sum" },
                //    { field: "CUT_QTY", aggregate: "sum" }
                //    ]
                //},
                batch: true,
                schema: {
                    model: {
                        fields: {
                            CUT_LINE: { editable: false },
                            PRODUCTION_LINE_CD: { editable: false },
                            JOB_ORDER_NO: { editable: false },
                            COLOR_CD: { editable: false },
                            ORDER_QTY: { editable: false },
                            CUT_QTY_TODAY: { editable: false },
                            CUT_QTY_TOTAL: { editable: false },
                            REDUCE_QTY_TODAY: { editable: false },
                            REDUCE_QTY_TOTAL: { editable: false },
                            CUT_OUT_TODAY: { editable: false },
                            CUT_OUT_TOTAL: { editable: false },
                            CUT_OUT_WIP: { editable: false },
                            PRT_IN_TODAY: { editable: false },
                            PRT_IN_TOTAL: { editable: false },
                            PRT_OUT_TODAY: { editable: false },
                            PRT_OUT_TOTAL: { editable: false },
                            PRT_IN_OUT_WIP: { editable: false },
                            EMB_OUT_TODAY: { editable: false },
                            EMB_OUT_TOTAL: { editable: false },
                            EMB_OUT_WIP: { editable: false },
                            FUSING_OUT_TODAY: { editable: false },
                            FUSING_OUT_TOTAL: { editable: false },
                            FUSING_OUT_WIP: { editable: false },
                            MATCHING_OUT_TODAY: { editable: false },
                            MATCHING_OUT_TOTAL: { editable: false },
                            MATCHING_OUT_WIP: { editable: false },
                            DC_BEFORE_WIP: { editable: false },
                            DC_SEW_S_TOTAL: { editable: false },
                            DC_SEW_C: { editable: false },
                            DC_SEW_C_TOTAL: { editable: false },
                            TWIP: { editable: false },
                            DCT: { editable: false },
                            CUT_QTY: { editable: false },
                            SEQ: { editable: false }
                        }
                    }
                },
                pageSize: 20,
            });
            GriddataSource.fetch();

            var grid = $("#DataGrid").data("kendoGrid");
            grid.setDataSource(GriddataSource);
            //grid.dataSource.sync();
            //或者
            //grid.dataSource.read();
            //grid.refresh();
            GriddataSource.read();
            GriddataSource.refresh;
        }

        $(function () {
            init();
            NotificationInit();
            GridInit();
        })

    </script>

        <div data-role="header" data-theme="b">
            <table style="width: 100%">
                <tr>
                    <td style="width: 8%">
                        <button id="functionmenubutton" data-icon="grid" data-iconpos="left">
                        </button>
                    </td>
                    <td style="width: 30%">
                        <table id="messagetitle" style="text-align: center">
                        </table>
                    </td>
                    <td style="width: 24%; text-align: center">
                        <label id="headertitle" style="text-align: center">
                        </label>
                    </td>
                    <td style="width: 30%">
                    </td>
                    <td style="width: 8%; text-align: right">
                        <button id="cancelbutton" data-icon="back" data-iconpos="right">
                        </button>
                    </td>
                </tr>
            </table>
        </div>

        <div data-role="content" style="position: absolute; top: 10%; width: 100%; bottom: 2%;">
            <div class="ui-grid-d">        

                <div class="ui-block-a">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="jotext" id="jolabel"></label></td>
                            <td style="width: 80%; text-align: right;">
                                <input id="jotext" name="jotext" type="text" placeholder="JOB_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"></td>
                            <td style="width: 80%; text-align: right;"></td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                </div>

                <div class="ui-block-b">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="gotext" id="golabel"></label></td>
                            <td style="width: 80%; text-align: right;">
                                <input id="gotext" name="gotext" type="text" placeholder="GARMENT_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                </div>

                <div class="ui-block-c">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="fromdate" id="fromdatelabel">From</label></td>
                            <td style="width: 80%; text-align: right;">
                                <input type="text" data-role="datebox" id="fromdate" name="fromdate"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                </div>

                <div class="ui-block-d">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="todate" id="todatelabel">To</label></td>
                            <td style="width: 80%; text-align: right;">
                                <input type="text" data-role="datebox" id="todate" name="todate"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                </div>

                <div class="ui-block-e">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                    <table style="width: 80%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center">
                                <input type="button" style="text-align:center" id="querybutton" name="querybutton" onclick="querydata()" />
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: right;"></td>
                            <td style="width: 80%; text-align: center"></td>
                        </tr>
                    </table>
                </div>

            </div>

            <table style="width:95%;border:0;align-self:center">
                    <tr style="width:100%">
                        <td style="width:100%">
                            <span id="GRID_SPAN1" style="font-size:20px"></span>
                            <div id="DataGrid" style="width:85%"></div>
                        </td>
                    </tr>
            </table>
            
        </div>

        <div id="loading"></div>

        <span id="notification" style="display:none;"></span>
        <script id="errorTemplate" type="text/x-kendo-template">
            <div class="wrong-pass">
                <h3>#= title #</h3>
                <p>#= message #</p>
            </div>
        </script>
        <script id="successTemplate" type="text/x-kendo-template">
            <div class="upload-success">
                <h3>#= message #</h3>
            </div>
        </script>

        <div data-role="footer" data-position="fixed" data-fullscreen="true"> </div>

    <nav id="my-menu">
        <ul>
            <li id="packageli">
                <a id="packagea" href="Package.aspx" onclick="packagejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="matchingli">
                <a id="matchinga" href="Matching.aspx" onclick="matchingjump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="transactionli">
                <a id="transactiona" href="Transaction.aspx" onclick="transactionjump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="receiveli">
                <a id="receivea" href="Receive.aspx" onclick="receivejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="bundlereduceli">
                <a id="bundlereducea" href="Reduce.aspx" onclick="reducejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="printli">
                <a id="printbarcodea" href="Printbarcode.aspx" onclick="printbarcodejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="printsparebarcodeli">
               <a id="printsparebarcodea" href="PrintSparebarcode.aspx"   onclick="printsparebarcodejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="employeeoutputli">
                <a id="employeeoutputa" href="Employeeoutput.aspx" onclick="employeeoutputjump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="externalreceiveli">
                <a id="externalreceivea" href="#mm-1"></a>
                <ul>
                    <li><a id="bundleinputa" href="Bundleinput.aspx" onclick="bundleinputjump(fty, environment);return false;" data-ajax="false"></a></li>
                </ul>
            </li>
            <li id="reportli">
                <a id="reporta" href="#mm-2"></a>
                <ul>
                    <li id="wipreportli"><a id="wipreporta" href="Wipreport.aspx" onclick="wipreportjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="outputreportli"><a id="outputreporta" href="" data-ajax="false"></a></li>
                    <li id="dctreportli"><a id="dctreporta" href="Outputreport.aspx" onclick="dctreportjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="docnoli"><a id="docnoinquirya" href="Barcode_Inquiry.aspx" onclick="docnoinquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="bundleorcartonli"><a id="barcodeinquirya" href="Barcode_Information_Inquiry.aspx" onclick="bundleorcartoninquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="wipandoutputli"><a id="wipandoutputlia" href="WipAndOutputNEW.aspx" onclick="WipAndOutputJump(fty, environment);return false;" data-ajax="false"></a></li>
                </ul>
            </li>
        </ul>
    </nav>

</body>
</html>
