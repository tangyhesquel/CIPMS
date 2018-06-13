<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Wipreport.aspx.cs" Inherits="Wipreport" %>

<!DOCTYPE>
<html>
<head>
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="KendoUI/kendo.common.min.css" rel="stylesheet" type="text/css" />
    <link href="KendoUI/kendo.default.min.css" rel="stylesheet" type="text/css" />
    <link href="KendoUI/kendo.mobile.all.min.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/jquery.min.js" type="text/javascript"></script>
    <script src="Scripts/kendo.all.min.js" type="text/javascript"></script>
    <script src="Scripts/jszip.min.js" type="text/javascript"></script>
    <script src="Scripts/cultures/kendo.culture.zh-CN.min.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
    <style>
        .demo-section p {
            margin: 3px 0 20px;
            line-height: 50px;
        }

        .demo-section .k-button {
            width: 250px;
        }

        .k-notification {
            border: 0;
        }

        /* Error template */
        .k-notification-error.k-group {
            background: rgba(100%,0%,0%,.7);
            color: #ffffff;
        }

        .wrong-pass {
            width: 300px;
            height: 100px;
        }
        .wrong-pass h3 {
            font-size: 1em;
            padding: 32px 10px 5px;
        }

        .wrong-pass img {
            float: left;
            margin: 30px 15px 30px 30px;
        }

        /* Success template */
        .k-notification-upload-success.k-group {
            background: rgba(0%,60%,0%,.7);
            color: #fff;
        }

        .upload-success {
            width: 240px;
            height: 100px;
            padding: 0 30px;
            line-height: 100px;
        }
        .upload-success h3 {
            font-weight: normal;
            display: inline-block;
            vertical-align: middle;
        }

        .upload-success img {
            display: inline-block;
            vertical-align: middle;
            margin-right: 10px;
        }
    </style>
</head>
<body>
    <script>
        //打印参数
        var printhtml = "";
        var cartonbarcode = "";
        var pdbutton = "";
        var pdtitle = "";
        var flag = "T";

        //翻译函数
        function translation(URL,flag) {
            kendo.ui.progress($("#loading"), true);
            $.ajax({
                type: "GET",
                url: URL,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                error: function () {
                    kendo.ui.progress($("#loading"), false);
                    alert("错误代码：1  获取翻译文件失败！");
                },
                success: function (json) {
                    globeltranslate = eval(json);
                    $('#lbl_go').html(globeltranslate.lbl_go);
                    $('#lbl_jo').html(globeltranslate.lbl_jo);
                    $('#lbl_process').html(globeltranslate.lbl_process);
                    $('#lbl_bybundle').html(globeltranslate.lbl_bybundle);
                    $('#lbl_bypart').html(globeltranslate.lbl_bypart);
                    $('#lbl_bycolorsize').html(globeltranslate.lbl_bycolorsize);

                    $('#lbl_bysewline').html(globeltranslate.lbl_bysewline);
                    $('#lbl_byprocesspcs').html(globeltranslate.lbl_byprocesspcs);

                    $('#btn_query').children('.km-text').html(globeltranslate.btn_query);

                    $('#btn_module').children('.km-text').html(globeltranslate.btn_module);
                    $('#btn_logout').children('.km-text').html(globeltranslate.btn_logout);
                    $('#lbl_title').html(globeltranslate.lbl_title);

                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);

                    if (flag == "T") {
                        initbundlelistgrid();
                        initbundlelistgrid1();

                        initgroupbylinegrid();
                        initgroupbyjogrid();

                        GetProcesslist();
                    }
                    kendo.ui.progress($("#loading"), false);
                }
            });
        }

        

        function commontranslation(URL) {
            kendo.ui.progress($("#loading"), true);
            $.ajax({
                type: "GET",
                url: URL,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: true,
                error: function () {
                    kendo.ui.progress($("#loading"), false);
                    alert("错误代码：1  获取翻译文件失败！");
                },
                success: function (json) {
                    commonglobeltranslate = eval(json);
                    $('#btn_bundleinput').html(commonglobeltranslate.btn_bundleinput);
                    $('#btn_feizhiprint').html(commonglobeltranslate.btn_feizhiprint);
                    $('#btn_package').html(commonglobeltranslate.btn_package);
                    $('#btn_transfer').html(commonglobeltranslate.btn_transfer);
                    $('#btn_receive').html(commonglobeltranslate.btn_receive);
                    $('#btn_reduce').html(commonglobeltranslate.btn_reduce);
                    $('#btn_matching').html(commonglobeltranslate.btn_matching);
                    $('#btn_employeeoutput').html(commonglobeltranslate.btn_employeeoutput);
                    $('#btn_wipreport').html(commonglobeltranslate.btn_wipreport);
                    $('#btn_outputreport').html(commonglobeltranslate.btn_outputreport);
                    $('#btn_dctreport').html(commonglobeltranslate.btn_dctreport);
                    $('#btn_docnoinquiry').html(commonglobeltranslate.btn_docnoinquiry);
                    $('#btn_bundleorcartoninquiry').html(commonglobeltranslate.btn_bundleorcartoninquiry);
                    $('#lbl_business').html(commonglobeltranslate.lbl_business);
                    $('#lbl_report').html(commonglobeltranslate.lbl_report);
                    $('#printsparebarcodea').html(commonglobeltranslate.printsparebarcodea);
                    kendo.ui.progress($("#loading"), false);
                }
            });
        }
    </script>
    <script>
        //全局变量：

        var app = new kendo.mobile.Application(document.body, { skin: "nova" }); //定义皮肤

        var globeltranslate = null; //用于保存翻译字符串
        var commonglobeltranslate = null;

        var printbarcode = "";

        var userprofileModel = null;
        var fty = getUrlParam('FTY'); //用于连接相应的数据库服务器
        var environment = getUrlParam('svTYPE');

        //20170615-tangyh
        queryfactory("T", fty,'');

        var PERMISSION = true; //系统的操作权限

        if (fty == null || fty == "" || environment == null || environment == "") {
            PERMISSION = false;
            alert("错误代码：1001  网址输入有误！");
            Loginout(fty, environment);
        }
        else if (sessionStorage.getItem("userbarcode") == null || sessionStorage.getItem("userbarcode") == "") {
            PERMISSION = false;
            alert("错误代码：1002  用户信息有误！获取session的用户条码错误！");
            Loginout(fty, environment);
        }
        else {
            $(function () {
                begin("T");
                });
            };

            function begin(flag)
            {
                    //隐藏所有模块
                    Accessmodulehide();

                    //根据userbarcode获取用户信息
                    kendo.ui.progress($("#loading"), true);
                    $.ajax({
                        async: true,
                        dataType: 'json',
                        contentType: "application/json;charset=utf-8",
                        type: 'POST',
                        url: 'Barcode_Information_Inquiry.aspx/GetUserProfile',
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                        success: function (data) {
                            kendo.ui.progress($("#loading"), false);
                            var result = eval(data.d);
                            if (result[0].SUCCESS == true) {
                                userprofileModel = result[0].data;

                                $('#userinfo').html(userprofileModel[0].NAME + ", " + userprofileModel[0].FACTORY_CD + "/" + userprofileModel[0].PRC_CD + "/" + userprofileModel[0].PRODUCTION_LINE_CD);

                                //显示用户能使用的模块
                                Permissionmoduleshow(userprofileModel);
                            }
                            else {
                                PERMISSION = false;
                                alert("错误代码：1003  用户信息有误！获取用户信息错误！");
                                Loginout(fty, environment);
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            kendo.ui.progress($("#loading"), false);
                            PERMISSION = false;
                            alert("错误代码：1004  用户信息有误！获取用户信息错误！");
                            Loginout(fty, environment);
                        }
                    });

                    //翻译
                    if (window.localStorage.languageid == "1") {
                        translation("Language/zh_CN/WIPReport.js?v1.1",flag);
                        commontranslation("Language/zh_CN/Common.js?v1");
                    } else if (window.localStorage.languageid == "2") {
                        translation("Language/en_US/WIPReport.js?v1.1",flag);
                        commontranslation("Language/en_US/Common.js?v1");
                    }


                    $("#go").kendoComboBox({
                        dataTextField: "GARMENT_ORDER_NO",
                        dataValueField: "GARMENT_ORDER_NO",
                        suggest: true,
                        change: function (e) {
                            if (this.value() != '') {
                                if ($("#process").data("kendoComboBox").value() == '') {
                                    $('#bybundle').show();
                                    $('#bypart').show();
                                    $('#bysewline').show();
                                    $('#byprocesspcs').show();

                                    $('#bycolorsize').show();
                                    $('#lbl_bybundle').show();
                                    $('#lbl_bypart').show();
                                    $('#lbl_bycolorsize').show();

                                    $('#lbl_bysewline').show();
                                    $('#lbl_byprocesspcs').show();

                                    if (flag == "T") {
                                        var grid = $('#bundlegrid').data("kendoGrid");
                                        grid.showColumn("JOB_ORDER_NO");
                                        grid.showColumn("CUT_LINE");
                                    }
                                }
                                else {
                                    if (flag == "T") {
                                        var grid = $('#bundlegrid').data("kendoGrid");
                                        grid.showColumn("SEW_LINE");
                                    }
                                }
                            }
                            else {
                                if ($("#process").data("kendoComboBox").value() == '' && $("#go").data("kendoComboBox").value() == '') {
                                    $('#bybundle').hide();
                                    $("#bybundle").attr("checked", false);
                                    bybundle = false;

                                    $('#bypart').hide();
                                    $("#bypart").attr("checked", false);
                                    bypart = false;

                                    $('#bycolorsize').hide();
                                    $("#bycolorsize").attr("checked", false);
                                    bycolor = false;
                                    bysize = false;

                                    $('#bysewline').hide();
                                    $("#bysewline").attr("checked", false);
                                    bysewline = false;

                                    $('#byprocesspcs').hide();
                                    $("#byprocesspcs").attr("checked", false);
                                    byprocesspcs = false;

                                    $('#lbl_bybundle').hide();
                                    $('#lbl_bypart').hide();
                                    $('#lbl_bycolorsize').hide();
                                    $('#lbl_byprocesspcs').hide();
                                    $('#lbl_bysewline').hide();

                                    if (flag == "T") {
                                        var grid = $('#bundlegrid').data("kendoGrid");
                                        grid.hideColumn("JOB_ORDER_NO");
                                        grid.hideColumn("COLOR_CD");
                                        grid.hideColumn("SIZE_CD");
                                        grid.hideColumn("BUNDLE_NO");
                                        grid.hideColumn("PART_DESC");
                                        grid.hideColumn("CUT_LINE");
                                        grid.hideColumn("SEW_LINE");
                                    }
                                }
                            }

                            $.ajax({
                                async: true,
                                dataType: 'json',
                                contentType: "application/json;charset=utf-8",
                                type: 'POST',
                                url: 'Barcode_Information_Inquiry.aspx/GetJOByGO',
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'GO': '" + $("#go").data("kendoComboBox").value() + "' }",
                                success: function (data) {
                                    var result = eval(data.d);

                                    if (result[0].SUCCESS == true) {
                                        var combobox = $('#jo').data("kendoComboBox");
                                        combobox.setDataSource(result[0].data);
                                        combobox.value("");
                                    }
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    notification.show({
                                        title: globeltranslate.msg_wrongtitle,
                                        message: globeltranslate.msg_wrongmessage1 + '134'
                                    }, "error");
                                }
                            });
                        }
                    });

                    $.ajax({
                        async: true,
                        dataType: 'json',
                        contentType: "application/json;charset=utf-8",
                        type: 'POST',
                        url: 'Barcode_Information_Inquiry.aspx/GetGO',
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                        success: function (data) {
                            var result = eval(data.d);

                            if (result[0].SUCCESS == true) {
                                var combobox = $('#go').data("kendoComboBox");
                                combobox.setDataSource(result[0].data);
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            notification.show({
                                title: globeltranslate.msg_wrongtitle,
                                message: globeltranslate.msg_wrongmessage1 + '176'
                            }, "error");
                        }
                    });

                    $.ajax({
                        async: true,
                        dataType: 'json',
                        contentType: "application/json;charset=utf-8",
                        type: 'POST',
                        url: 'Barcode_Information_Inquiry.aspx/GetJO',
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                        success: function (data) {
                            var result = eval(data.d);

                            if (result[0].SUCCESS == true) {
                                var combobox = $('#jo').data("kendoComboBox");
                                combobox.setDataSource(result[0].data);
                            }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            notification.show({
                                title: globeltranslate.msg_wrongtitle,
                                message: globeltranslate.msg_wrongmessage1 + '198'
                            }, "error");
                        }
                    });

                    $("#jo").kendoComboBox({
                        dataTextField: "JOB_ORDER_NO",
                        dataValueField: "JOB_ORDER_NO",
                        suggest: true,
                        change: function (e) {
                            if (this.value() != '') {
                                if ($("#process").data("kendoComboBox").value() == '') {
                                    $('#bybundle').show();
                                    $('#bypart').show();
                                    $('#bycolorsize').show();
                                    $('#bysewline').show();
                                    $('#byprocesspcs').show();

                                    $('#lbl_bybundle').show();
                                    $('#lbl_bypart').show();
                                    $('#lbl_bycolorsize').show();
                                    $('#lbl_bysewline').show();
                                    $('#lbl_byprocesspcs').show();

                                    if (flag == "T") {
                                        var grid = $('#bundlegrid').data("kendoGrid");
                                        grid.showColumn("JOB_ORDER_NO");
                                        grid.showColumn("CUT_LINE");
                                    }
                                }
                                else {
                                    if (flag == "T") {
                                        var grid = $('#bundlegrid').data("kendoGrid");
                                        grid.showColumn("SEW_LINE");
                                    }
                                }
                            }
                            else {
                                if ($("#process").data("kendoComboBox").value() == '' && $("#go").data("kendoComboBox").value() == '') {
                                    $('#bybundle').hide();
                                    $("#bybundle").attr("checked", false);
                                    bybundle = false;

                                    $('#bypart').hide();
                                    $("#bypart").attr("checked", false);
                                    bypart = false;

                                    $('#bycolorsize').hide();
                                    $("#bycolorsize").attr("checked", false);
                                    bycolor = false;
                                    bysize = false;

                                    $('#bysewline').hide();
                                    $("#bysewline").attr("checked", false);
                                    bysewline = false;

                                    $('#byprocesspcs').hide();
                                    $("#byprocesspcs").attr("checked", false);
                                    byprocesspcs = false;

                                    $('#lbl_bybundle').hide();
                                    $('#lbl_bypart').hide();
                                    $('#lbl_bycolorsize').hide();
                                    $('#lbl_byprocesspcs').hide();
                                    $('#lbl_bysewline').hide();

                                    if (flag == "T") {
                                        var grid = $('#bundlegrid').data("kendoGrid");
                                        grid.hideColumn("JOB_ORDER_NO");
                                        grid.hideColumn("COLOR_CD");
                                        grid.hideColumn("SIZE_CD");
                                        grid.hideColumn("BUNDLE_NO");
                                        grid.hideColumn("PART_DESC");
                                        grid.hideColumn("CUT_LINE");
                                        grid.hideColumn("SEW_LINE");
                                    }
                                }
                            }
                        }
                    });

            }
    </script>

    <div data-role="view" id="drawer-home" data-layout="drawer-layout" data-title="WIP_Report" data-use-native-scrolling="true">
        <table style="width:100%">
            <tr style="width: 100%">
                <td style="width:10%;text-align:center">
                    <label id="lbl_go" for="go"></label>
                </td>
                <td style="width:15%">
                    <input id="go" style="width:auto"/>
                    <script>
                        $(function () {
                            
                        });
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <input value='bybundle' id="bybundle" name="bybundle" type="checkbox"/>
                    <script>
                        $('#bybundle').hide();
                        $("#bybundle").attr("checked", false);
                        //bybundle = false;
                        var bybundle = false;
                        $('#bybundle').change(function () {
                            if ($(this).is(':checked')) {
                                bybundle = true;
                            } else {
                                bybundle = false;
                            }
                        });
                    </script>
                </td>
                <td style="width:15%">
                    <label for="bybundle" id="lbl_bybundle"></label>
                    <script>
                        $('#lbl_bybundle').hide();
                    </script>
                </td>

                <td style="width:10%;text-align:center">
                    <input value='byprocesspcs' id="byprocesspcs" name="byprocesspcs" type="checkbox"/>
                    <script>
                        $('#byprocesspcs').hide();
                        $("#byprocesspcs").attr("checked", false);
                        byprocesspcs = false;
                        var byprocesspcs = false;
                        $('#byprocesspcs').change(function () {
                            if ($(this).is(':checked')) {
                                byprocesspcs = true;
                            } else {
                                byprocesspcs = false;
                            }
                        });
                    </script>
                </td>

                <td style="width:15%">
                    <label for="byprocesspcs" id="lbl_byprocesspcs"></label>
                    <script>
                        $('#lbl_byprocesspcs').hide();
                    </script>
                </td>

                <td style="width:10%;text-align:center">
                </td>
                <td style="width:15%">
                </td>
            </tr>
            <tr>
                <td style="width:10%;text-align:center">
                    <label id="lbl_jo" for="jo"></label>
                </td>
                <td style="width:15%">
                    <input style="width:auto" id="jo" />
                    <script>
                        $(function () {
                            
                        });
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <input value='bypart' id="bypart" name="bypart" type="checkbox"/>
                    <script>
                        $('#bypart').hide();
                        $("#bypart").attr("checked", false);
                        bypart = false;
                        var bypart = false;
                        $('#bypart').change(function () {
                            if ($(this).is(':checked')) {
                                bypart = true;
                            }
                            else {
                                bypart = false;
                            }
                        });
                    </script>
                </td>
                <td style="width:15%">
                    <label for="bypart" id="lbl_bypart"></label>
                    <script>
                        $('#lbl_bypart').hide();
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <input value='bysewline' id="bysewline" name="bysewline" type="checkbox"/>
                    <script>
                        $('#bysewline').hide();
                        $("#bysewline").attr("checked", false);
                        bysewline = false;
                        var bysewline = false;
                        $('#bysewline').change(function () {
                            if ($(this).is(':checked')) {
                                bysewline = true;
                            } else {
                                bysewline = false;
                            }
                        });
                    </script>
                </td>
                <td style="width:15%">
                    <label for="bysewline" id="lbl_bysewline"></label>
                    <script>
                        $('#lbl_bysewline').hide();
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                </td>
                <td style="width:15%">
                </td>
            </tr>
            <tr>
                <td style="width:10%;text-align:center">
                    <label id="lbl_process" for="process"></label>
                </td>
                <td style="width:15%">
                    <input style="width:auto" id="process" />
                    <script>
                        var byprocess = false;

                        $("#process").kendoComboBox({
                            dataTextField: "NM",
                            dataValueField: "PRC_CD",
                            change: function (e) {
                                var processcd = this.value();
                                if (this.value() == '') {
                                    byprocess = false;
                                    $('#groupbylinegrid').hide();
                                    $('#groupbyjogrid').hide();
                                    $('#bundlegrid').show();

                                    if ($("#go").data("kendoComboBox").value() == '' && $("#jo").data("kendoComboBox").value() == '') {
                                        $('#bybundle').hide();
                                        $("#bybundle").attr("checked", false);
                                        bybundle = false;

                                        $('#bypart').hide();
                                        $("#bypart").attr("checked", false);
                                        bypart = false;

                                        $('#bysewline').hide();
                                        $("#bysewline").attr("checked", false);
                                        bysewline = false;

                                        $('#byprocesspcs').hide();
                                        $("#byprocesspcs").attr("checked", false);
                                        byprocesspcs = false;

                                        $('#bycolorsize').hide();
                                        $("#bycolorsize").attr("checked", false);
                                        bycolor = false;
                                        bysize = false;
                                        $('#lbl_bybundle').hide();
                                        $('#lbl_bypart').hide();
                                        $('#lbl_bycolorsize').hide();
                                        $('#lbl_bysewline').hide();
                                        $('#lbl_byprocesspcs').hide();
                                    }
                                }
                                else {
                                    byprocess = true;
                                    $('#groupbylinegrid').show();
                                    $('#groupbyjogrid').show();
                                    $('#bundlegrid').hide();

                                    $('#bybundle').show();
                                    $('#bypart').show();
                                    $('#bycolorsize').show();

                                    $('#byprocess').show();
                                    $('#bysewline').show();

                                    $('#lbl_bybundle').show();
                                    $('#lbl_bypart').show();
                                    $('#lbl_bycolorsize').show();

                                    $('#lbl_byprocesspcs').show();
                                    $('#lbl_bysewline').show();

                                    if (processcd == 'DC' || processcd == 'SEW') {
                                        $('#bypart').hide();
                                        $("#bypart").attr("checked", false);
                                        bypart = false;
                                        $('#lbl_bypart').hide();
                                    }

                                    var grid = $("#groupbylinegrid").data("kendoGrid");
                                    if (processcd == 'DC' || processcd == 'SEW') {
                                        grid.hideColumn("CUT_LINE");
                                        grid.showColumn("SEW_LINE");
                                    }
                                    else {
                                        grid.hideColumn("SEW_LINE");
                                        grid.showColumn("CUT_LINE");
                                    }
                                    var grid = $("#groupbyjogrid").data("kendoGrid");
                                    if (processcd == 'DC' || processcd == 'SEW') {
                                        grid.hideColumn("CUT_LINE");
                                        grid.showColumn("SEW_LINE");
                                    }
                                    else {
                                        grid.hideColumn("SEW_LINE");
                                        grid.showColumn("CUT_LINE");
                                    }
                                }
                            }
                        });

                        function showfields(grid,flag)
                        {
                             if (flag == "F") {
                                grid.hideColumn("ORDER_QTY");
                                grid.hideColumn("CUT_QTY");
                                grid.hideColumn("REDUCE_QTY");
                                grid.hideColumn("ACTUAL_CUT_QTY");
                                grid.hideColumn("TOSEW");
                                grid.hideColumn("SEW_LINE");
                            }
                            else
                            {
                                grid.showColumn("ORDER_QTY");
                                grid.showColumn("CUT_QTY");
                                grid.showColumn("REDUCE_QTY");
                                grid.showColumn("ACTUAL_CUT_QTY");
                                grid.showColumn("TOSEW");
                                grid.showColumn("SEW_LINE");
                            }

                             if (byprocess == false) {
                                 if (bybundle == true)
                                     grid.showColumn("BUNDLE_NO");
                                 else
                                     grid.hideColumn("BUNDLE_NO");
                                 if (bypart == true)
                                     grid.showColumn("PART_DESC");
                                 else
                                     grid.hideColumn("PART_DESC");
                                 if (bycolor == true)
                                     grid.showColumn("COLOR_CD");
                                 else
                                     grid.hideColumn("COLOR_CD");
                                 if (bysize == true)
                                     grid.showColumn("SIZE_CD");
                                 else
                                     grid.hideColumn("SIZE_CD");
                                 if (bysewline == true)
                                     grid.showColumn("SEW_LINE");
                                 else
                                     grid.hideColumn("SEW_LINE");
                             }
                             else
                             {
                                      grid.hideColumn("BUNDLE_NO");
                                      grid.hideColumn("PART_DESC");
                                      grid.hideColumn("COLOR_CD");
                                      grid.hideColumn("SIZE_CD");
                             }
                        }

                        function showfields1(grid1) {
                            grid1.showColumn("JOB_ORDER_NO");
                            grid1.showColumn("CUT_LINE");
                            grid1.showColumn("ORDER_QTY");
                                grid1.hideColumn("CUT_QTY");
                                grid1.hideColumn("REDUCE_QTY");
                            grid1.showColumn("ACTUAL_CUT_QTY");
                            grid1.showColumn("TOSEW");
                            grid1.showColumn("SEW_LINE");
                                grid1.hideColumn("BUNDLE_NO");
                                grid1.hideColumn("PART_DESC");
                                grid1.showColumn("COLOR_CD");
                                grid1.hideColumn("SIZE_CD");
                        }

                        function GetProcesslist() {
                            $.ajax({
                                async: true,
                                dataType: 'json',
                                contentType: "application/json;charset=utf-8",
                                type: 'POST',
                                url: 'Wipreport.aspx/GetProcessList',
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                                success: function (data) {
                                    var result = eval(data.d);

                                    if (result[0].SUCCESS == true) {
                                        var combobox = $('#process').data("kendoComboBox");
                                        combobox.setDataSource(result[0].data);
                                        combobox.value("");
                                        combobox.trigger("change");

                                        $.each(result[0].data, function (i, o) {
                                            if (o.PRC_CD == 'FUSE') {
                                                var grid = $('#bundlegrid').data("kendoGrid");
                                                grid.showColumn("FUSE");
                                                return false;
                                            }
                                        });
                                        ;
                                    }
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    notification.show({
                                        title: globeltranslate.msg_wrongtitle,
                                        message: globeltranslate.msg_wrongmessage1 + '23415'
                                    }, "error");
                                }
                            });
                        }
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <input value='bycolorsize' id="bycolorsize" name="bycolorsize" type="checkbox"/>
                    <script>
                        $('#bycolorsize').hide();
                        $("#bycolorsize").attr("checked", false);
                        var bycolor = false;
                        var bysize = false;
                        $('#bycolorsize').change(function () {
                            if ($(this).is(':checked')) {
                                bycolor = true;
                                bysize = true;
                            } else {
                                bycolor = false;
                                bysize = false;
                            }
                        });
                    </script>
                </td>
                <td style="width:15%">
                    <label for="bycolorsize" id="lbl_bycolorsize"></label>
                    <script>
                        $('#lbl_bycolorsize').hide();
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                </td>
                <td style="width:15%">
                </td>
                <td style="width:10%;text-align:center">
                </td>
                <td style="width:15%">
                </td>
            </tr>
        </table>

        <br />

        <table style="width:100%">
            <tr style="width:100%">
                <td style="width:15%">
                    <div id="bundlegrid"></div>
                </td>
            </tr>
        </table>

        <table style="width:100%">
            <tr style="width:100%">
                <td style="width:15%">
                    <div id="bundlegrid1" hidden="hidden"></div>
                </td>
            </tr>
        </table>

        <script>
            function initbundlelistgrid() {
                 $("#bundlegrid").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span>Group By JO</span>")
                    }, {
                        name: "excel", text: "导出到EXCEL"
                    }],
                    pageable: {
                        numeric: false,
                        previousNext: false,
                        messages: {
                            empty: "空行",
                            display: "{0} - {1} / {2}"
                        }
                    },
                    scrollable: {
                        virtual: true
                    },
                    selectable: "multiple row",
                    resizable: true,
                    sortable: {
                        mode: "multiple",
                        allowUnsort: true
                    },
                    columnMenu: true,
                    height: 550,
                    columns: [{
                        title: globeltranslate.table_seq,
                        footerTemplate: "Total:",
                        field: "SEQ"
                    }, {
                        title: globeltranslate.table_cutline,
                        field: "CUT_LINE",
                        hidden: true
                    },
                    {
                        title: globeltranslate.table_sewline,
                        field: "SEW_LINE",
                        hidden: true
                    },
                    {
                        title: globeltranslate.table_jo,
                        field: "JOB_ORDER_NO",
                        width: 150,
                        hidden: true
                    },
                    {
                        title:  globeltranslate.table_orderqty,
                        field: "ORDER_QTY",
                        hidden: false
                    }, 
                    {
                        title: globeltranslate.table_cutqty,
                        field: "CUT_QTY",
                        hidden: false
                    }, {
                        title: globeltranslate.table_reduceqty,
                        field: "REDUCE_QTY",
                        hidden: false
                    }, {
                        title: globeltranslate.table_actualcutqty,
                        field: "ACTUAL_CUT_QTY",
                        hidden: false
                    },
                    {
                        title: globeltranslate.table_color,
                        field: "COLOR_CD",
                        hidden: true
                    }, {
                        title: globeltranslate.table_size,
                        field: "SIZE_CD",
                        hidden: true
                    }, {
                        title: globeltranslate.table_bundle,
                        field: "BUNDLE_NO",
                        hidden: true
                    }, {
                        title: globeltranslate.table_part,
                        field: "PART_DESC",
                        hidden: true
                    }, {
                        title: globeltranslate.table_cut,
                        field: "CUT",
                        footerTemplate: "#: data.CUT ? data.CUT.sum: 0 #",
                        template: "# if (CUT == '0') {# <span></span> #} else {# <span>#= CUT #</span> #} #"
                    }, {
                        title: globeltranslate.table_prt,
                        field: "PRT",
                        footerTemplate: "#: data.PRT ? data.PRT.sum: 0 #",
                        template: "# if (PRT == '0') {# <span></span> #} else {# <span>#= PRT #</span> #} #"
                    }, {
                        title: globeltranslate.table_emb,
                        field: "EMB",
                        footerTemplate: "#: data.EMB ? data.EMB.sum: 0 #",
                        template: "# if (EMB == '0') {# <span></span> #} else {# <span>#= EMB #</span> #} #"
                    }, {
                        title: globeltranslate.table_fuse,
                        field: "FUSE",
                        footerTemplate: "#: data.FUSE ? data.FUSE.sum: 0 #",
                        template: "# if (FUSE == '0') {# <span></span> #} else {# <span>#= FUSE #</span> #} #",
                        hidden: true
                    }, {
                        title: globeltranslate.table_matching,
                        field: "MATCHING",
                        footerTemplate: "#: data.MATCHING ? data.MATCHING.sum: 0 #",
                        template: "# if (MATCHING == '0') {# <span></span> #} else {# <span>#= MATCHING #</span> #} #"
                    }, {
                        title: globeltranslate.table_dc,
                        field: "DC",
                        footerTemplate: "#: data.DC ? data.DC.sum: 0 #",
                        template: "# if (DC == '0') {# <span></span> #} else {# <span>#= DC #</span> #} #"
                    }, {
                        title: globeltranslate.table_tosew,
                        field: "TOSEW",
                        footerTemplate: "#: data.TOSEW ? data.TOSEW.sum: 0 #",
                        template: "# if (TOSEW == '0') {# <span></span> #} else {# <span>#= TOSEW #</span> #} #"
                    }],
                    excelExport: function (e) {
                        e.workbook.fileName = "WIP_LIST.xlsx";
                    },
                    excel: {
                        allPages: true
                    }
                });
            }

            function initbundlelistgrid1() {
                $("#bundlegrid1").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span>Group By JO</span>")
                    }, {
                        name: "excel", text: "导出到EXCEL"
                    }],
                    pageable: {
                        numeric: false,
                        previousNext: false,
                        messages: {
                            empty: "空行",
                            display: "{0} - {1} / {2}"
                        }
                    },
                    scrollable: {
                        virtual: true
                    },
                    selectable: "multiple row",
                    resizable: true,
                    sortable: {
                        mode: "multiple",
                        allowUnsort: true
                    },
                    columnMenu: true,
                    height: 550,
                    columns: [{
                        title: globeltranslate.table_seq,
                        footerTemplate: "Total:",
                        field: "SEQ"
                    }, {
                        title: globeltranslate.table_cutline,
                        field: "CUT_LINE",
                        hidden: true
                    },
                    {
                        title: globeltranslate.table_sewline,
                        field: "SEW_LINE",
                        hidden: true
                    },
                    {
                        title: globeltranslate.table_jo,
                        field: "JOB_ORDER_NO",
                        width: 150,
                        hidden: true
                    },
                    {
                        title: globeltranslate.table_orderqty,
                        field: "ORDER_QTY",
                        hidden: false
                    },
                    {
                        title: globeltranslate.table_cutqty,
                        field: "CUT_QTY",
                        hidden: false
                    }, {
                        title: globeltranslate.table_reduceqty,
                        field: "REDUCE_QTY",
                        hidden: false
                    }, {
                        title: globeltranslate.table_actualcutqty,
                        field: "ACTUAL_CUT_QTY",
                        hidden: false
                    },
                    {
                        title: globeltranslate.table_color,
                        field: "COLOR_CD",
                        hidden: true
                    }, {
                        title: globeltranslate.table_size,
                        field: "SIZE_CD",
                        hidden: true
                    }, {
                        title: globeltranslate.table_bundle,
                        field: "BUNDLE_NO",
                        hidden: true
                    }, {
                        title: globeltranslate.table_part,
                        field: "PART_DESC",
                        hidden: true
                    }, {
                        title: globeltranslate.table_cut,
                        field: "CUT",
                        footerTemplate: "#: data.CUT ? data.CUT.sum: 0 #",
                        template: "# if (CUT == '0') {# <span></span> #} else {# <span>#= CUT #</span> #} #"
                    }, {
                        title: globeltranslate.table_prt,
                        field: "PRT",
                        footerTemplate: "#: data.PRT ? data.PRT.sum: 0 #",
                        template: "# if (PRT == '0') {# <span></span> #} else {# <span>#= PRT #</span> #} #"
                    }, {
                        title: globeltranslate.table_emb,
                        field: "EMB",
                        footerTemplate: "#: data.EMB ? data.EMB.sum: 0 #",
                        template: "# if (EMB == '0') {# <span></span> #} else {# <span>#= EMB #</span> #} #"
                    }, {
                        title: globeltranslate.table_fuse,
                        field: "FUSE",
                        footerTemplate: "#: data.FUSE ? data.FUSE.sum: 0 #",
                        template: "# if (FUSE == '0') {# <span></span> #} else {# <span>#= FUSE #</span> #} #",
                        hidden: true
                    }, {
                        title: globeltranslate.table_matching,
                        field: "MATCHING",
                        footerTemplate: "#: data.MATCHING ? data.MATCHING.sum: 0 #",
                        template: "# if (MATCHING == '0') {# <span></span> #} else {# <span>#= MATCHING #</span> #} #"
                    }, {
                        title: globeltranslate.table_dc,
                        field: "DC",
                        footerTemplate: "#: data.DC ? data.DC.sum: 0 #",
                        template: "# if (DC == '0') {# <span></span> #} else {# <span>#= DC #</span> #} #"
                    }, {
                        title: globeltranslate.table_tosew,
                        field: "TOSEW",
                        footerTemplate: "#: data.TOSEW ? data.TOSEW.sum: 0 #",
                        template: "# if (TOSEW == '0') {# <span></span> #} else {# <span>#= TOSEW #</span> #} #"
                    }],
                    excelExport: function (e) {
                        e.workbook.fileName = "WIP_LIST.xlsx";
                    },
                    excel: {
                        allPages: true
                    }
                });
            }
        </script>

        <script>
            var bundlelistdata = [];
            var bundlelistModel = null;

            var bundlelistdata1 = [];
            var bundlelistModel1 = null;

            var groupbylinedata = [];
            var groupbylineModel = null;
            var groupbyjodata = [];
            var groupbyjoModel = null;

            function cleargrid()
            {
                var grid = $("#bundlegrid").data("kendoGrid");
                var bundlelistModel = new kendo.data.DataSource({
                    data: null
                })
                grid.setDataSource(bundlelistModel);

                 grid = $("#bundlegrid1").data("kendoGrid");
                 bundlelistModel = new kendo.data.DataSource({
                    data: null
                })
                 grid.setDataSource(bundlelistModel);

                 grid = $("#groupbylinegrid").data("kendoGrid");
                 bundlelistModel = new kendo.data.DataSource({
                     data: null
                 })
                 grid.setDataSource(bundlelistModel);

                 grid = $("#groupbyjogrid").data("kendoGrid");
                 bundlelistModel = new kendo.data.DataSource({
                     data: null
                 })
                 grid.setDataSource(bundlelistModel);
            }

            function Query() {
                //var s;
                //s = "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'go': '" + $('#go').data("kendoComboBox").value() + "', 'jo': '" + $('#jo').data("kendoComboBox").value() + "', 'process': '" + $('#process').data("kendoComboBox").value() + "', 'bybundle': '" + bybundle + "', 'bypart': '" + bypart + "', 'bycolor': '" + bycolor + "', 'bysize': '" + bysize + "', 'bysewline': '" + bysewline + "', 'byprocesspcs': '" + byprocesspcs + "' }";
                //alert(s);

                if (byprocess == false) {
                    kendo.ui.progress($("#loading"), true);
                    $.ajax({
                        async: true,
                        dataType: 'json',
                        contentType: "application/json;charset=utf-8",
                        type: 'POST',
                        url: 'Wipreport.aspx/GetWIPReport',
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'go': '" + $('#go').data("kendoComboBox").value() + "', 'jo': '" + $('#jo').data("kendoComboBox").value() + "', 'process': '" + $('#process').data("kendoComboBox").value() + "', 'bybundle': '" + bybundle + "', 'bypart': '" + bypart + "', 'bycolor': '" + bycolor + "', 'bysize': '" + bysize + "', 'bysewline': '" + bysewline + "', 'byprocesspcs': '" + byprocesspcs + "' }",
                        success: function (data) {
                            var result = eval(data.d);

                           
                            //if (byprocess == false) {
                            //    if (bybundle == true)
                            //        grid.showColumn("BUNDLE_NO");
                            //    else
                            //        grid.hideColumn("BUNDLE_NO");
                            //    if (bypart == true)
                            //        grid.showColumn("PART_DESC");
                            //    else
                            //        grid.hideColumn("PART_DESC");
                            //    if (bycolor == true)
                            //        grid.showColumn("COLOR_CD");
                            //    else
                            //        grid.hideColumn("COLOR_CD");
                            //    if (bysize == true)
                            //        grid.showColumn("SIZE_CD");
                            //    else
                            //        grid.hideColumn("SIZE_CD");
                            //}
                            var grid = $("#bundlegrid").data("kendoGrid");
                            bundlelistModel = new kendo.data.DataSource({
                                pageSize: 20,
                                data: null
                            })
                            grid.setDataSource(bundlelistModel);

                            showfields(grid,"T");

                            if (result[0].SUCCESS == true) {
                                bundlelistdata = [];
                                bundlelistModel = null;

                                bundlelistdata = result[0].TABLE2;

                                bundlelistModel = new kendo.data.DataSource({
                                    pageSize: 20,
                                    data: bundlelistdata,
                                    aggregate: [
                                        { field: "CUT", aggregate: "sum" },
                                        { field: "PRT", aggregate: "sum" },
                                        { field: "EMB", aggregate: "sum" },
                                        { field: "FUSE", aggregate: "sum" },
                                        { field: "MATCHING", aggregate: "sum" },
                                        { field: "DC", aggregate: "sum" },
                                        { field: "TOSEW", aggregate: "sum" }
                                    ]
                                });
                                bundlelistModel.fetch();
                                grid.setDataSource(bundlelistModel);                                

                                //tangyh 2017.06.06
                                //showfields1();

                                var grid1 = $("#bundlegrid1").data("kendoGrid");
                                bundlelistModel1 = new kendo.data.DataSource({
                                    pageSize: 20,
                                    data: null
                                })
                                grid1.setDataSource(bundlelistModel1);

                                    bundlelistdata1 = [];
                                    bundlelistModel1 = null;
                                    bundlelistdata1 = result[0].TABLE3;
              
                                    if (bundlelistdata1.length == 0) {
                                        $('#bundlegrid1').hide();
                                    }
                                    else
                                    {
                                        //第一个grid需要隐藏下面字段
                                        grid.hideColumn("CUT_LINE");
                                        grid.hideColumn("JOB_ORDER_NO");
                                        grid.hideColumn("ORDER_QTY");
                                        grid.hideColumn("CUT_QTY");
                                        grid.hideColumn("REDUCE_QTY");
                                        grid.hideColumn("ACTUAL_CUT_QTY");
                                        grid.hideColumn("TOSEW");
                                        grid.hideColumn("SEW_LINE");
                                        grid.hideColumn("BUNDLE_NO");
                                        grid.hideColumn("PART_DESC");
                                        grid.hideColumn("COLOR_CD");
                                        grid.hideColumn("SIZE_CD");

                                        $('#bundlegrid1').show();
                                        showfields1(grid1);

                                        bundlelistModel1 = new kendo.data.DataSource({
                                            pageSize: 20,
                                            data: bundlelistdata1,
                                            aggregate: [
                                                { field: "CUT", aggregate: "sum" },
                                                { field: "PRT", aggregate: "sum" },
                                                { field: "EMB", aggregate: "sum" },
                                                { field: "FUSE", aggregate: "sum" },
                                                { field: "MATCHING", aggregate: "sum" },
                                                { field: "DC", aggregate: "sum" },
                                                { field: "TOSEW", aggregate: "sum" }
                                            ]
                                        });
                                        bundlelistModel1.fetch();
                                        grid1.setDataSource(bundlelistModel1);
                                    }
                                notification.show({
                                    message: globeltranslate.msg_successtitle
                                }, "upload-success");
                            }
                            else {
                                notification.show({
                                    title: globeltranslate.msg_wrongtitle,
                                    message: globeltranslate.msg_wrongmessage1 + '3546'
                                }, "error");
                            }
                            kendo.ui.progress($("#loading"), false);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            notification.show({
                                title: globeltranslate.msg_wrongtitle,
                                message: globeltranslate.msg_wrongmessage1 + '543'
                            }, "error");

                            kendo.ui.progress($("#loading"), false);
                        }
                    });
                }
                else {    
                    kendo.ui.progress($("#loading"), true);
                    $.ajax({
                        async: true,
                        dataType: 'json',
                        contentType: "application/json;charset=utf-8",
                        type: 'POST',
                        url: 'Wipreport.aspx/GetWIPReportByProcess',
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'go': '" + $('#go').data("kendoComboBox").value() + "', 'jo': '" + $('#jo').data("kendoComboBox").value() + "', 'process': '" + $('#process').data("kendoComboBox").value() + "', 'bybundle': '" + bybundle + "', 'bypart': '" + bypart + "', 'bycolor': '" + bycolor + "', 'bysize': '" + bysize + "', 'bysewline': '" + bysewline + "', 'byprocesspcs': '" + byprocesspcs + "' }",
                        success: function (data) {
                            var result = eval(data.d);

                            var grid = $("#groupbyjogrid").data("kendoGrid");
                            if (bybundle == true)
                                grid.showColumn("BUNDLE_NO");
                            else
                                grid.hideColumn("BUNDLE_NO");
                            if (bypart == true)
                                grid.showColumn("PART_DESC");
                            else
                                grid.hideColumn("PART_DESC");
                            if (bycolor == true)
                                grid.showColumn("COLOR_CD");
                            else
                                grid.hideColumn("COLOR_CD");
                            if (bysize == true)
                                grid.showColumn("SIZE_CD");
                            else
                                grid.hideColumn("SIZE_CD");
                            if (bysewline == true)
                                grid.showColumn("SEW_LINE");
                            else
                                grid.hideColumn("SEW_LINE");

                            if (result[0].SUCCESS == true) {
                                groupbylinedata = [];
                                groupbylineModel = null;
                                groupbyjodata = [];
                                groupbyjoModel = null;

                                groupbylinedata = result[0].TABLE1;

                                groupbylineModel = new kendo.data.DataSource({
                                    pageSize: 20,
                                    data: groupbylinedata,
                                    aggregate: [
                                        { field: "ORDER_QTY", aggregate: "sum" },
                                        { field: "CUT_QTY", aggregate: "sum" },
                                        { field: "WIP", aggregate: "sum" }
                                    ]
                                });

                                groupbylineModel.fetch();
                                var grid = $("#groupbylinegrid").data("kendoGrid");
                                grid.setDataSource(groupbylineModel);
                                
                                groupbyjodata = result[0].TABLE2;
                                groupbyjoModel = new kendo.data.DataSource({
                                    pageSize: 20,
                                    data: groupbyjodata,
                                    aggregate: [
                                        { field: "ORDER_QTY", aggregate: "sum" },
                                        { field: "CUT_QTY", aggregate: "sum" },
                                        { field: "WIP", aggregate: "sum" }
                                    ]
                                });
                                groupbyjoModel.fetch();
                                var grid = $("#groupbyjogrid").data("kendoGrid");
                                grid.setDataSource(groupbyjoModel);

                                notification.show({
                                    message: globeltranslate.msg_successtitle
                                }, "upload-success");
                            }
                            else {
                                notification.show({
                                    title: globeltranslate.msg_wrongtitle,
                                    message: globeltranslate.msg_wrongmessage1 + '3546'
                                }, "error");
                            }
                            kendo.ui.progress($("#loading"), false);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            notification.show({
                                title: globeltranslate.msg_wrongtitle,
                                message: globeltranslate.msg_wrongmessage1 + '684'
                            }, "error");

                            kendo.ui.progress($("#loading"), false);
                        }
                    });
                }
            }
        </script>

        <table style="width:100%">
            <tr style="width:100%">
                <td style="width:100%">
                    <div id="groupbylinegrid"></div>
                </td>
            </tr>
        </table>
        <script>
            function initgroupbylinegrid() {
                     $("#groupbylinegrid").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span>Group By Line</span>")
                    }, {
                        name: "excel", text: "导出到EXCEL"
                    }],
                    pageable: {
                        numeric: false,
                        previousNext: false,
                        messages: {
                            empty: "空行",
                            display: "{0} - {1} / {2}"
                        }
                    },
                    scrollable: {
                        virtual: true
                    },
                    selectable: "multiple row",
                    sortable: {
                        mode: "multiple",
                        allowUnsort: true
                    },
                    columnMenu: true,
                    height: 350,
                    columns: [{
                        title: globeltranslate.table_seq,
                        footerTemplate: "Total:",
                        field: "SEQ"
                    }, {
                        title: globeltranslate.table_process,
                        field: "PROCESS_CD"
                    }, {
                        title: globeltranslate.table_cutline,
                        field: "CUT_LINE",
                        hidden: true
                    }, {
                        title: globeltranslate.table_sewline,
                        field: "SEW_LINE",
                        hidden: true
                    }, {
                        title: globeltranslate.table_wip,
                        field: "WIP",
                        footerTemplate: "#: data.WIP ? data.WIP.sum: 0 #"
                    }],
                    excelExport: function (e) {
                        e.workbook.fileName = "WIP_BYLINE_LIST.xlsx";
                    },
                    excel: {
                        allPages: true
                    }
                });
            }
        </script>
        <br />

        <table style="width:100%">
            <tr style="width:100%">
                <td style="width:100%">
                    <div id="groupbyjogrid"></div>
                </td>
            </tr>
        </table>
        <script>
            function initgroupbyjogrid() {
                $("#groupbyjogrid").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span>Group By JO</span>")
                    }, {
                        name: "excel", text: "导出到EXCEL"
                    }],
                    pageable: {
                        numeric: false,
                        previousNext: false,
                        messages: {
                            empty: "空行",
                            display: "{0} - {1} / {2}"
                        }
                    },
                    scrollable: {
                        virtual: true
                    },
                    selectable: "multiple row",
                    sortable: {
                        mode: "multiple",
                        allowUnsort: true
                    },
                    columnMenu: true,
                    height: 550,
                    columns: [{
                        title: globeltranslate.table_seq,
                        footerTemplate: "Total:",
                        field: "SEQ",
                        width:75
                    }, {
                        title: globeltranslate.table_cutline,
                        field: "CUT_LINE",
                        hidden: true,
                        width: 75
                    }, {
                        title: globeltranslate.table_sewline,
                        field: "SEW_LINE",
                        hidden: true,
                        width: 75
                    }, {
                        title: globeltranslate.table_jo,
                        field: "JOB_ORDER_NO",
                        width: 150
                    }, {
                        title: globeltranslate.table_color,
                        field: "COLOR_CD",
                        hidden: true,
                        width: 170
                    }, {
                        title: globeltranslate.table_size,
                        field: "SIZE_CD",
                        hidden: true,
                        width: 170
                    }, {
                        title: globeltranslate.table_bundle,
                        field: "BUNDLE_NO",
                        hidden: true,
                        width: 75
                    }, {
                        title: globeltranslate.table_part,
                        field: "PART_DESC",
                        hidden: true,
                        width: 75
                    }, {
                        title: globeltranslate.table_orderqty,
                        field: "ORDER_QTY",
                        footerTemplate: "#: data.ORDER_QTY ? data.ORDER_QTY.sum: 0 #",
                        width: 75
                    }, {
                        title: globeltranslate.table_cutqty,
                        field: "CUT_QTY",
                        footerTemplate: "#: data.CUT_QTY ? data.CUT_QTY.sum: 0 #",
                        width: 75
                    }, {
                        title: globeltranslate.table_wip,
                        field: "WIP",
                        footerTemplate: "#: data.WIP ? data.WIP.sum: 0 #",
                        width: 75
                    }],
                    excelExport: function (e) {
                        e.workbook.fileName = "WIP_BYJO_LIST.xlsx";
                    },
                    excel: {
                        allPages: true
                    }
                });
            }
        </script>
    </div>

    <div data-role="drawer" id="my-drawer" style="width: 20em" data-views="['drawer-home']" data-use-native-scrolling="true">
        <ul data-role="listview" data-type="group" class="km-listview-icons">
            <li><label id="lbl_business"></label>
                <ul>
                    <li id="packageli">
                        <a id="btn_package" onclick="pageredirect('package')" data-transition="none"></a>
                    </li>
                    <li id="matchingli">
                        <a id="btn_matching" onclick="pageredirect('matching')" data-transition="none"></a>
                    </li>
                    <li id="transactionli">
                        <a id="btn_transfer" onclick="pageredirect('transfer')" data-transition="none"></a>
                    </li>
                    <li id="receiveli">
                        <a id="btn_receive" onclick="pageredirect('receive')" data-transition="none"></a>
                    </li>
                    <li id="bundlereduceli">
                        <a id="btn_reduce" onclick="pageredirect('reduce')" data-transition="none"></a>
                    </li>
                    <li id="printli">
                        <a id="btn_feizhiprint" onclick="pageredirect('printfeizhi')" data-transition="none"></a>
                    </li>
                    <!--added by lijer-->          
                    <li id="printsparebarcodeli">
                       <a id="printsparebarcodea"  onclick="pageredirect('printsparecode')" data-transition="none"></a>
                    </li>
                    <li id="employeeoutputli">
                        <a id="btn_employeeoutput" onclick="pageredirect('employeeoutput')" data-transition="none"></a>
                    </li>
                    <li id="bundleinputli">
                        <a id="btn_bundleinput" onclick="pageredirect('bundleinput')" data-transition="none"></a>
                    </li>
                </ul>
            </li>
            <li><label id="lbl_report"></label>
                <ul>
                    <li id="wipreportli">
                        <a id="btn_wipreport" onclick="pageredirect('wip')" data-transition="none"></a>
                    </li>
                    <li id="outputreportli">
                        <a id="btn_outputreport" onclick="pageredirect('transferoutput')" data-transition="none"></a>
                    </li>
                    <li id="dctreportli">
                        <a id="btn_dctreport" onclick="pageredirect('dct')" data-transition="none"></a>
                    </li>
                    <li id="docnoli">
                        <a id="btn_docnoinquiry" onclick="pageredirect('docno')" data-transition="none"></a>
                    </li>
                    <li id="bundleorcartonli">
                        <a id="btn_bundleorcartoninquiry" onclick="pageredirect('bundleorcarton')" data-transition="none"></a>
                    </li>
                </ul>
            </li>
        </ul>
    </div>

    <script>
        //页面跳转函数
        function pageredirect(page) {
            Redirect(fty, environment, page);
        }
    </script>

    <div data-role="layout" data-id="drawer-layout">
        <header data-role="header">
            <div data-role="navbar">
                <a id="btn_module" data-role="button" data-rel="drawer" href="#my-drawer" data-icon="drawer-icon" data-align="left">a</a>
                <span id="userinfo" data-align="left"></span>
                <span id="lbl_title" data-role="view-title"></span>
                <a id="btn_logout" data-align="right" data-role="button" class="nav-button" data-icon="reply" onclick="pageredirect('login')">a</a>
            </div>
            <div data-role="navbar" id='navbar'>
                <div data-role="navbar">
                    <table style="width:100%">
                        <tr style="width:100%">
                            <td style="width:10%">
                                <a id="btn_query" onclick="Query()" style="width:100%; line-height:2.8em; background-color:#1C1C1C; border-radius:0.6em" data-icon="search" data-role="button">a</a>
                            </td>
                            <td style="width:70%"></td>

                            <%--//20170615-tangyh--%>
                            <td style="width:20%" hidden="hidden" id="queryfactory_td">
                                        <select data-native-menu="false" id="queryfactory" name="queryfactory"></select>
                            </td>
                            <script> 
                                if (sessionStorage.getItem("process") == 'PRT') {
                                    $('#queryfactory_td').show();
                                }

                                $('#queryfactory').change(function () {
                                    if (($('#queryfactory').val() == "undefined") || ($('#queryfactory').val() == globeltranslate.queryfactory)) {
                                        return;
                                    }

                                    var int1 = $("#queryfactory").get(0).selectedIndex;
                                    var value1 = $('#queryfactory').get(0)[int1].innerText;

                                    if (value1 != fty) {
                                        var isokaccountid = checkaccountid("F", value1, sessionStorage.getItem("userbarcode"), environment);
                                        if (isokaccountid == "F") {
                                            return;
                                        }
                                        fty = value1;
                                        cleargrid();
                                        begin("F");                                    }
                                });
                            </script>     
                        </tr>
                    </table>
                </div>
            </div>
        </header>
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
    <script>

        var notification;
        $(function () {
            notification = $("#notification").kendoNotification({
                autoHideAfter: 2000,
                stacking: "up",
                templates: [{
                    type: "error",
                    template: $("#errorTemplate").html()
                }, {
                    type: "upload-success",
                    template: $("#successTemplate").html()
                }]
            }).data("kendoNotification");
        });

    </script>
    
</body>
</html>
