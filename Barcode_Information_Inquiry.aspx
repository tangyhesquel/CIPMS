<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Barcode_Information_Inquiry.aspx.cs" Inherits="Barcode_Information_Inquiry" %>

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
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/qrcode.js" type="text/javascript"></script>
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

        //翻译函数
        function translation(URL) {
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
                    $('#lbl_color').html(globeltranslate.lbl_color);
                    $('#lbl_layno').html(globeltranslate.lbl_layno);
                    $('#lbl_bundleno').html(globeltranslate.lbl_bundleno);
                    $('#lbl_barcode').html(globeltranslate.lbl_barcode);
                    $('#btn_query').children('.km-text').html(globeltranslate.btn_query);
                    $('#btn_print').children('.km-text').html(globeltranslate.btn_print);
                    $('#btn_printreal').html(globeltranslate.btn_print);
                    $('#btn_cancel').html(globeltranslate.btn_cancel);
                    $('#title_dialog').html(globeltranslate.title_dialog);

                    $('#btn_module').children('.km-text').html(globeltranslate.btn_module);
                    $('#btn_logout').children('.km-text').html(globeltranslate.btn_logout);
                    $('#lbl_title').html(globeltranslate.lbl_title);

                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                    $('#wipandoutputlia').html(globeltranslate.wipandoutputlia);
                    initdocnogrid();

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
                    $('#wipandoutputlia').html(commonglobeltranslate.wipandoutputlia);
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
                    translation("Language/zh_CN/Barcode_Information_Inquiry.js");
                    commontranslation("Language/zh_CN/Common.js");
                } else if (window.localStorage.languageid == "2") {
                    translation("Language/en_US/Barcode_Information_Inquiry.js");
                    commontranslation("Language/en_US/Common.js");
                }
            });

        }
    </script>
    <div data-role="view" id="drawer-home" data-layout="drawer-layout" data-title="BUNDLECARTONINQUIRY" data-use-native-scrolling="true">
        <table style="width:100%">
            <tr style="width: 100%">
                <td style="width:10%;text-align:center">
                    <label id="lbl_go" for="go"></label>
                </td>
                <td style="width:15%">
                    <input id="go" style="width:auto"/>
                    <script>
                        $("#go").kendoComboBox({
                            dataTextField: "GARMENT_ORDER_NO",
                            dataValueField: "GARMENT_ORDER_NO",
                            suggest: true,
                            change: function (e) {
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

                                            var combobox = $('#color').data("kendoComboBox");
                                            combobox.setDataSource([]);
                                            combobox.value("");

                                            var combobox = $('#layno').data("kendoComboBox");
                                            combobox.setDataSource([]);
                                            combobox.value("");

                                            var combobox = $('#bundleno').data("kendoComboBox");
                                            combobox.setDataSource([]);
                                            combobox.value("");
                                        }
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        notification.show({
                                            title: globeltranslate.msg_wrongtitle + '134',
                                            message: globeltranslate.msg_wrongmessage2
                                        }, "error");
                                    }
                                });
                            }
                        });

                                             ;
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <label id="lbl_jo" for="jo"></label>
                </td>
                <td style="width:15%">
                    <input style="width:auto" id="jo" />
                    <script>
                        $("#jo").kendoComboBox({
                            dataTextField: "JOB_ORDER_NO",
                            dataValueField: "JOB_ORDER_NO",
                            suggest: true,
                            change: function (e) {
                                $.ajax({
                                    async: true,
                                    dataType: 'json',
                                    contentType: "application/json;charset=utf-8",
                                    type: 'POST',
                                    url: 'Barcode_Information_Inquiry.aspx/GetColorLaynoBundleByJO',
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'JO': '" + $("#jo").data("kendoComboBox").value() + "' }",
                                    success: function (data) {
                                        var result = eval(data.d);

                                        if (result[0].SUCCESS == true) {
                                            var combobox = $('#color').data("kendoComboBox");
                                            combobox.setDataSource(result[0].COLOR);
                                            combobox.value("");

                                            var combobox = $('#layno').data("kendoComboBox");
                                            combobox.setDataSource(result[0].LAYNO);
                                            combobox.value("");

                                            var combobox = $('#bundleno').data("kendoComboBox");
                                            combobox.setDataSource(result[0].BUNDLENO);
                                            combobox.value("");
                                        }
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        notification.show({
                                            title: globeltranslate.msg_wrongtitle + '111',
                                            message: globeltranslate.msg_wrongmessage2
                                        }, "error");
                                    }
                                });
                            }
                        });

                        $(function () {
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
                                        title: globeltranslate.msg_wrongtitle + '198',
                                        message: globeltranslate.msg_wrongmessage2
                                    }, "error");
                                }
                            });
                        });
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <label id="lbl_color" for="color"></label>
                </td>
                <td style="width:15%">
                    <input id="color" style="width:auto" />
                    <script>
                        $("#color").kendoComboBox({
                            dataTextField: "COLOR_CD",
                            dataValueField: "COLOR_CD",
                            suggest: true,
                            change: function (e) {
                                $.ajax({
                                    async: true,
                                    dataType: 'json',
                                    contentType: "application/json;charset=utf-8",
                                    type: 'POST',
                                    url: 'Barcode_Information_Inquiry.aspx/GetBundlenoByJOColorLayno',
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'JO': '" + $("#jo").data("kendoComboBox").value() + "', 'COLOR': '" + $("#color").data("kendoComboBox").value() + "', 'LAYNO': '" + $("#layno").data("kendoComboBox").value() + "' }",
                                    success: function (data) {
                                        var result = eval(data.d);

                                        if (result[0].SUCCESS == true) {
                                            var combobox = $('#bundleno').data("kendoComboBox");
                                            combobox.setDataSource(result[0].BUNDLENO);
                                            combobox.value("");
                                        }
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        notification.show({
                                            title: globeltranslate.msg_wrongtitle + '111',
                                            message: globeltranslate.msg_wrongmessage2
                                        }, "error");
                                    }
                                });
                            }
                        });
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <label id="lbl_layno" for="layno"></label>
                </td>
                <td style="width:15%">
                    <input id="layno" style="width:auto" />
                    <script>
                        $("#layno").kendoComboBox({
                            dataTextField: "LAY_NO",
                            dataValueField: "LAY_NO",
                            suggest: true,
                            change: function (e) {
                                $.ajax({
                                    async: true,
                                    dataType: 'json',
                                    contentType: "application/json;charset=utf-8",
                                    type: 'POST',
                                    url: 'Barcode_Information_Inquiry.aspx/GetBundlenoByJOColorLayno',
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'JO': '" + $("#jo").data("kendoComboBox").value() + "', 'COLOR': '" + $("#color").data("kendoComboBox").value() + "', 'LAYNO': '" + $("#layno").data("kendoComboBox").value() + "' }",
                                    success: function (data) {
                                        var result = eval(data.d);

                                        if (result[0].SUCCESS == true) {
                                            var combobox = $('#bundleno').data("kendoComboBox");
                                            combobox.setDataSource(result[0].BUNDLENO);
                                            combobox.value("");
                                        }
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                                        notification.show({
                                            title: globeltranslate.msg_wrongtitle + '111',
                                            message: globeltranslate.msg_wrongmessage2
                                        }, "error");
                                    }
                                });
                            }
                        });
                    </script>
                </td>
            </tr>
            <tr style="width: 100%">
                <td style="width:10%;text-align:center">
                    <label id="lbl_bundleno" for="bundleno"></label>
                </td>
                <td style="width:15%">
                    <input id="bundleno" style="width:auto" />
                    <script>
                        $("#bundleno").kendoComboBox({
                            dataTextField: "BUNDLE_NO",
                            dataValueField: "BUNDLE_NO"
                        });
                    </script>
                </td>
                <td style="width:10%;text-align:center">
                    <label id="lbl_barcode" for="barcode"></label>
                </td>
                <td style="width:15%">
                    <input id="barcode" class="k-textbox" />
                </td>
                <td style="width:10%"></td>
                <td style="width:15%"></td>
                <td style="width:10%"></td>
                <td style="width:15%"></td>
            </tr>
        </table>
        <br />
        <div id="barcodelist"></div>
        <script>
            function initdocnogrid() {
                $("#barcodelist").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span>扎/箱详细信息表</span>")
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
                    height: 550,
                    sortable: {
                        mode: "multiple",
                        allowUnsort: true
                    },
                    selectable: "multiple cell",
                    navigatable: true,
                    columnMenu: true,
                    resizable: true,
                    allowCopy: true,
                    pageable: {
                        refresh: true,
                        pageSizes: true

                    },
                    columns: [{
                        title: globeltranslate.table_process,
                        field: "PROCESS_CD",
                        width: 75,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_production,
                        field: "PRODUCTION_LINE_CD",
                        template: "#if(PROCESS_CD == 'SEW') {# <span>#= SEW_LINE #</span> #} " +
                            "else {# <span>#= CUT_LINE #</span> #}#",
                        width: 70,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_go,
                        field: "GARMENT_ORDER_NO",
                        template: "<a onclick='Query(\"#= GARMENT_ORDER_NO #\", \"\", \"\")'>#= GARMENT_ORDER_NO #</a>",
                        width: 90,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_jo,
                        field: "JOB_ORDER_NO",
                        template: "<a onclick='Query(\"\", \"#= JOB_ORDER_NO #\", \"\")'>#= JOB_ORDER_NO #</a>",
                        width: 110,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_color,
                        field: "COLOR_CD",
                        width: 90,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_size,
                        field: "SIZE_CD",
                        width: 80,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_layno,
                        field: "LAY_NO",
                        width: 65,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_bundle,
                        field: "BUNDLE_NO",
                        width: 65,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_bundlebarcode,
                        field: "BARCODE",
                        template: "<a onclick='Query(\"\", \"\", \"#= BARCODE #\")'>#= BARCODE #</a>",
                        width: 120,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_part,
                        field: "PART_DESC",
                        width: 90,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_wip,
                        field: "WIP",
                        width: 65,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_reduce,
                        field: "DISCREPANCY_QTY",
                        width: 65,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_cartonbarcode,
                        field: "CARTON_BARCODE",
                        template: "#if(CARTON_STATUS == 'C') {# <a onclick='Query(\"\", \"\", \"#= CARTON_BARCODE #\")'>#= CARTON_BARCODE #</a> #} " +
                            "else {# <span></span> #}#",
                        width: 110,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_docno,
                        field: "DOC_NO",
                        template: "#if(DOC_NO != \"\" && DOC_NO != \"null\" && DOC_NO != null) {# <a onclick='Query(\"\", \"\", \"#= DOC_NO #\")'>#= DOC_NO #</a> #} " +
                            "else {# <span></span> #}#",
                        width: 140,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_ismatching,
                        field: "IS_MATCHING",
                        template: "#if(IS_MATCHING == \"Y\") {# <span>YES</span> #} " +
                            "else {# <span>NO</span> #}#",
                        width: 80,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }],
                    excel: {
                        allPages: true
                    },
                    excelExport: function (e) {
                        e.workbook.fileName = "BARCODE_DETAIL_LIST.xlsx";
                    }
                });
            }
        </script>
    </div>

    <div data-role="modalview" id="print-dialog" style="width: 40%; height:90%">
        <div data-role="header">
            <div data-role="navbar">
                <span id="title_dialog"></span>
                <script>
                    function closeModalViewStart() {
                        $("#print-dialog").kendoMobileModalView("close");
                    }
                </script>
            </div>
        </div>
        <table id="checkboxtable" width="100%">
            <tr>
                <td style="width: 100%; text-align: right;">
                    <input value='summarybycolor' id="summarybycolor" name="summarybycolor" type="checkbox"/>
                    <label for="summarybycolor" id="summarybycolorlabel">Summary By Color</label>
                    <input value='summarybyjo' id="summarybyjo" name="summarybyjo" type="checkbox"/>
                    <label for="summarybyjo" id="summarybyjolabel">Summary By JO</label>
                </td>
            </tr>
        </table>
        <%--<div id="printdiv" style="width:17.0cm;font-size:8px; margin-left: 0cm;">--%>
           <div id="printdiv" style="width:8.0cm;font-size:8px; margin-left: 0cm;">
        </div>
        <div data-role="footer">
            <div data-role="navbar">
                <a id="btn_printreal" data-role="button" data-align="right"></a>
                <a data-click="closeModalViewStart" id="btn_cancel" data-role="button" data-align="right"></a>
            </div>
        </div>
    </div>
    <script>
        $(function () {
            $('#btn_printreal').click(function () {
                $('#printdiv').jqprint();
            });
        });

        function printdialogopen() {
            $("#print-dialog").kendoMobileModalView("open");
            //初始化打印界面
            var barcodetype = GetBarcodeType(printbarcode);
            if (barcodetype != "D") {
                $('#checkboxtable').hide();
            } else {
                $('#checkboxtable').show();
            }
            $('#printdiv').empty();
            $('#printdiv').append(printhtml);
            var qrcode = new QRCode("barcodediv", {
                width: 150,
                height: 150,
                text: printbarcode
            });
            $("div[name='hide1']").each(function () {
                $(this).hide();
            });
            $("table[name='hide2']").each(function () {
                $(this).hide();
            });
            $("div[name='hide3']").each(function () {
                $(this).hide();
            });
            $("table[name='hide4']").each(function () {
                $(this).hide();
            });

            $('#summarybycolor').change(function () {
                if ($(this).is(':checked')) {
                    $("div[name='hide1']").each(function () {
                        $(this).show();
                    });
                    $("table[name='hide2']").each(function () {
                        $(this).show();
                    });
                } else {
                    $("div[name='hide1']").each(function () {
                        $(this).hide();
                    });
                    $("table[name='hide2']").each(function () {
                        $(this).hide();
                    });
                }
            });

            $('#summarybyjo').change(function () {
                if ($(this).is(':checked')) {
                    $("div[name='hide3']").each(function () {
                        $(this).show();
                    });
                    $("table[name='hide4']").each(function () {
                        $(this).show();
                    });
                } else {
                    $("div[name='hide3']").each(function () {
                        $(this).hide();
                    });
                    $("table[name='hide4']").each(function () {
                        $(this).hide();
                    });
                }
            });
        };
    </script>

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
                    <li id="printsparebarcodeli">
                       <a id="printsparebarcodea" href="PrintSparebarcode.aspx"   onclick="printsparebarcodejump(fty, environment);return false;" data-ajax="false"></a>
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
                    <li id="wipandoutputli">
                        <a id="wipandoutputlia" href="WipAndOutputNEW.aspx" onclick="WipAndOutputJump(fty, environment);return false;" data-ajax="false">
                       </a>
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
                                <a id="btn_query" onclick="Query(1, 1, 1)" style="width:100%; line-height:2.8em; background-color:#1C1C1C; border-radius:0.6em" data-icon="search" data-role="button">a</a>
                                <script>
                                    var barcodedata = null;
                                    var barcodeModel = null;

                                    function Query(garmentorderno, joborderno, barcode) {
                                        var go;
                                        var jo;
                                        var bar;

                                        if (garmentorderno == '1' && joborderno == '1' && barcode == '1') {
                                        }
                                        else {
                                            if (barcode != "" && barcode != null && barcode != "null") {
                                                $("#btn_print").data("kendoMobileButton").enable(true);
                                                printbarcode = barcode;
                                            }
                                            else
                                                $("#btn_print").data("kendoMobileButton").enable(false);
                                        }

                                        if (garmentorderno == '1')
                                            go = $("#go").data("kendoComboBox").value();
                                        else
                                            go = garmentorderno;
                                        if (joborderno == '1')
                                            jo = $("#jo").data("kendoComboBox").value();
                                        else
                                            jo = joborderno;
                                        if (barcode == '1')
                                            bar = $('#barcode').val();
                                        else
                                            bar = barcode;

                                        if ($("#go").data("kendoComboBox").value() == '' && $("#jo").data("kendoComboBox").value() == '' && $('#barcode').val() == '' && garmentorderno == '1') {
                                            notification.show({
                                                title: globeltranslate.msg_wrongtitle + '1027',
                                                message: globeltranslate.msg_wrongmessage1
                                            }, "error");
                                            return false;
                                        }

                                        kendo.ui.progress($("#loading"), true);
                                        $.ajax({
                                            async: true,
                                            dataType: 'json',
                                            contentType: "application/json;charset=utf-8",
                                            type: 'POST',
                                            url: 'Barcode_Information_Inquiry.aspx/GetBarcodeInformationDetail',
                                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'GO': '" + go + "', 'JO': '" + jo + "', 'COLOR': '" + $('#color').data("kendoComboBox").value() + "', LAYNO: '" + $('#layno').data("kendoComboBox").value() + "', 'BUNDLENO': '" + $('#bundleno').data("kendoComboBox").value() + "', 'BARCODE': '" + bar + "' }",
                                            success: function (data) {
                                                var result = eval(data.d);

                                                if (result[0].SUCCESS == true) {
                                                    barcodedata = [];
                                                    barcodeModel = null;

                                                    barcodedata = result[0].data;
                                                    barcodeModel = new kendo.data.DataSource({
                                                        data: barcodedata,
                                                        pageSize: 20
                                                    });

                                                    var grid = $("#barcodelist").data("kendoGrid");
                                                    grid.setDataSource(barcodeModel);

                                                    notification.show({
                                                        message: globeltranslate.msg_successtitle1
                                                    }, "upload-success");
                                                }
                                                else {
                                                    notification.show({
                                                        title: globeltranslate.msg_wrongtitle + '1022',
                                                        message: globeltranslate.msg_wrongmessage2
                                                    }, "error");
                                                }
                                                kendo.ui.progress($("#loading"), false);
                                            },
                                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                                notification.show({
                                                    title: globeltranslate.msg_wrongtitle + '1022',
                                                    message: globeltranslate.msg_wrongmessage2
                                                }, "error");

                                                kendo.ui.progress($("#loading"), false);
                                            }
                                        });
                                    }

                                    $('#barcode').bind('keypress', function (event) {
                                        var barcode = $('#barcode').val();
                                        if (event.keyCode == '13') {
                                            if (barcode == '') {
                                                notification.show({
                                                    title: globeltranslate.msg_wrongtitle + '1324',
                                                    message: globeltranslate.msg_wrongmessage3
                                                }, "error");
                                            }
                                            else {
                                                Query($("#go").data("kendoComboBox").value(), $("#jo").data("kendoComboBox").value(), barcode);
                                                $('#barcode').val("");
                                            }
                                        }
                                    });
                                </script>
                            </td>
                            <td style="width:10%">
                                <a id="btn_print" onclick="Print()" style="width:100%; line-height:2.8em; background-color:#1C1C1C; border-radius:0.6em" data-role="button">b</a>
                                <script>
                                    $(function () {
                                        $("#btn_print").data("kendoMobileButton").enable(false);
                                    });

                                    function Print() {
                                        kendo.ui.progress($("#loading"), true);
                                        $.ajax({
                                            async: true,
                                            dataType: 'json',
                                            contentType: "application/json;charset=utf-8",
                                            type: 'POST',
                                            url: 'Barcode_Information_Inquiry.aspx/PrintBarcode',
                                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'BARCODE': '" + printbarcode + "', 'LANGUAGE': '" + window.localStorage.languageid + "' }",
                                            success: function (data) {
                                                var result = eval(data.d);
                                                printhtml = result[0].data;
                                                cartonbarcode = printbarcode;
                                                printdialogopen();

                                                kendo.ui.progress($("#loading"), false);
                                            },
                                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                                notification.show({
                                                    title: globeltranslate.msg_wrongtitle + '1022',
                                                    message: globeltranslate.msg_wrongmessage2
                                                }, "error");

                                                kendo.ui.progress($("#loading"), false);
                                            }
                                        });
                                    }
                                </script>
                            </td>
         
                            <td style="width:60%"></td>

                            <%--//20170615-tangyh--%>
                            <td style="width:20%" hidden="hidden" id="queryfactory_td">
                                        <select data-native-menu="false" id="queryfactory" name="queryfactory"></select>
                            </td>
                            <script>
                                if (sessionStorage.getItem("process") == 'PRT') {
                                    $('#queryfactory_td').show();
                                }

                                function cleardata()
                                {
                                    var combobox = $('#go').data("kendoComboBox");
                                    combobox.value("");
                                    var combobox = $('#jo').data("kendoComboBox");
                                    combobox.value("");
                                    $('#barcode').val("");
                                    var combobox = $('#color').data("kendoComboBox");
                                    combobox.value("");
                                    var combobox = $('#layno').data("kendoComboBox");
                                    combobox.value("");
                                    var combobox = $('#bundleno').data("kendoComboBox");
                                    combobox.value("");

                                    var grid = $("#barcodelist").data("kendoGrid");
                                    var barcodeModel = new kendo.data.DataSource({
                                        data: null,
                                        pageSize: 20
                                    });
                                    grid.setDataSource(barcodeModel);
                   

                                    //$("#go").val("");
                                    //$("#jo").val("");
                                    //$("#barcode").val("");
                                    //$("#color").val("");
                                    //$("#layno").val("");
                                    //$("#bundleno").val("");

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
                                        cleardata();
                                        begin("F");
                                       
                                    }
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

        function begin(flag)
        {
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
                        title: globeltranslate.msg_wrongtitle + '176',
                        message: globeltranslate.msg_wrongmessage2
                    }, "error");
                }
            });
        }

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

        begin("T");
        //20170615-tangyh
        queryfactory("T", fty, '');
        // $('#queryfactory').selectmenu('refresh');

    </script>
</body>
</html>
