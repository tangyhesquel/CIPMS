<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Barcode_Inquiry.aspx.cs" Inherits="Barcode_Inquiry" %>

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
                    $('#lbl_fromdate').html(globeltranslate.lbl_fromdate);
                    $('#lbl_docno').html(globeltranslate.lbl_docno);
                    $('#lbl_sendprocess').html(globeltranslate.lbl_sendprocess);
                    $('#lbl_status').html(globeltranslate.lbl_status);
                    $('#lbl_todate').html(globeltranslate.lbl_todate);
                    $('#lbl_jo').html(globeltranslate.lbl_jo);
                    $('#lbl_receiveprocess').html(globeltranslate.lbl_receiveprocess);
                    $('#btn_query').children('.km-text').html(globeltranslate.btn_query);
                    $('#btn_bundleinput').html(globeltranslate.btn_bundleinput);
                    $('#btn_feizhiprint').html(globeltranslate.btn_feizhiprint);
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                    $('#btn_package').html(globeltranslate.btn_package);
                    $('#btn_transfer').html(globeltranslate.btn_transfer);
                    $('#btn_receive').html(globeltranslate.btn_receive);
                    $('#btn_reduce').html(globeltranslate.btn_reduce);
                    $('#btn_matching').html(globeltranslate.btn_matching);
                    $('#btn_employeeoutput').html(globeltranslate.btn_employeeoutput);
   
                    $('#lbl_business').html(globeltranslate.lbl_business);
                    $('#lbl_report').html(globeltranslate.lbl_report);
                    $('#btn_module').children('.km-text').html(globeltranslate.btn_module);
                    $('#btn_logout').children('.km-text').html(globeltranslate.btn_logout);
                    $('#lbl_title').html(globeltranslate.lbl_title);

                    $('#spn_doclist').html(globeltranslate.spn_doclist);
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                     initdocnodetailgrid();
                     initdocnogrid();
                     initstatusselect();
                    $('#btn_detail').html(globeltranslate.btn_detail);
                    $('#btn_selectall').html(globeltranslate.btn_selectall);
                    $('#btn_selectnone').html(globeltranslate.btn_selectnone);

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
                    $('#btn_wipandoutputinquiry').html(commonglobeltranslate.btn_wipandoutputinquiry);

                    $('#lbl_business').html(commonglobeltranslate.lbl_business);
                    $('#lbl_report').html(commonglobeltranslate.lbl_report);
                    $('#printsparebarcodea').html(commonglobeltranslate.printsparebarcodea);
                    //$('#spn_doclist').html(commonglobeltranslate.spn_doclist);
                    //$('#btn_detail').html(commonglobeltranslate.btn_detail);
                    //$('#btn_selectall').html(commonglobeltranslate.btn_selectall);
                    //$('#btn_selectnone').html(commonglobeltranslate.btn_selectnone);
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
        var BYPART = "N"; //是否bypart的全局标记，用于查询的控制

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
                    url: 'Barcode_Inquiry.aspx/GetUserProfile',
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
                if (flag == "T") {
                    if (window.localStorage.languageid == "1") {
                        translation("Language/zh_CN/Barcode_Inquiry.js");
                        commontranslation("Language/zh_CN/Common.js");
                    } else if (window.localStorage.languageid == "2") {
                        translation("Language/en_US/Barcode_Inquiry.js");
                        commontranslation("Language/en_US/Common.js");
                    }
                }

            }

        }

    </script>

    <div data-role="view" id="drawer-home" data-layout="drawer-layout" data-title="DOCNO_INQUIRY" data-use-native-scrolling="true">
        <table style="width:100%">
            <tr style="width: 100%">
                <td width="10%" style="text-align:center">
                    <label id="lbl_fromdate" for="fromdate"></label>
                </td>
                <td width="15%">
                    <input id="fromdate" style="width:auto"/>
                    <script>
                        $("#fromdate").kendoDatePicker({
                            culture: "zh-CN",
                            value: addDate(new Date(), -30),
                            format: "{0:yyyy/MM/dd}"
                        });
                    </script>
                </td>
                <td width="10%" style="text-align:center">
                    <label id="lbl_docno" for="docno"></label>
                </td>
                <td width="15%">
                    <input id="docno" class="k-textbox" />
                </td>
                <td width="10%" style="text-align:center">
                    <label id="lbl_sendprocess" for="sendprocess"></label>
                </td>
                <td width="15%">
                    <input id="sendprocess" style="width:auto"/>
                    <script>
                        

                        $("#sendprocess").kendoComboBox({
                            dataTextField: "CIPMS_CHS",
                            dataValueField: "PRC_CD",
                            change: function () {
                                DocnoDetailByPart();
                            }
                        });

                        $(function () {
                            kendo.ui.progress($("#loading"), true);
                            $.ajax({
                                async: true,
                                dataType: 'json',
                                contentType: "application/json;charset=utf-8",
                                type: 'POST',
                                url: 'Barcode_Inquiry.aspx/GetProcessList',
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'garmenttype': 'K' }",
                                success: function (data) {
                                    var result = eval(data.d);
                                    if (result[0].SUCCESS == true) {
                                        var combobox = $('#sendprocess').data("kendoComboBox");
                                        combobox.setDataSource(result[0].data);
                                        combobox.value("DC");
                                        combobox.trigger("change");

                                        var combobox = $('#receiveprocess').data("kendoComboBox");
                                        combobox.setDataSource(result[0].data);
                                        combobox.value("SEW");
                                        combobox.trigger("change");
                                    }
                                    else {
                                        notification.show({
                                            title: globeltranslate.msg_wrongtitle + "1011",
                                            message: globeltranslate.msg_wrongmessage1
                                        }, "error");
                                    }
                                    kendo.ui.progress($("#loading"), false);
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    notification.show({
                                        title: globeltranslate.msg_wrongtitle + "1011",
                                        message: globeltranslate.msg_wrongmessage1
                                    }, "error");

                                    kendo.ui.progress($("#loading"), false);
                                }
                            });
                        });
                    </script>
                </td>
                <td width="10%" style="text-align:center">
                    <label id="lbl_status" for="status"></label>
                </td>
                <td width="15%">
                    <input id="status" style="width:auto"/>
                    <script>
                        function initstatusselect() {
                            $("#status").kendoComboBox({
                                dataSource: [{
                                    STATUS_CD: "S",
                                    STATUS_NAME: globeltranslate.select_submit
                                }, {
                                    STATUS_CD: "C",
                                    STATUS_NAME: globeltranslate.select_confirm
                                }],
                                dataTextField: "STATUS_NAME",
                                dataValueField: "STATUS_CD"
                            });
                        }
                    </script>
                </td>
            </tr>
            <tr style="width:100%">
                <td width="10%" style="text-align:center">
                    <label id="lbl_todate" for="todate"></label>
                </td>
                <td width="15%">
                    <input id="todate" style="width:auto"/>
                    <script>
                        $("#todate").kendoDatePicker({
                            culture: "zh-CN",
                            value: new Date(),
                            format: "{0:yyyy/MM/dd}"
                        });
                    </script>
                </td>
                <td width="10%" style="text-align:center">
                    <label id="lbl_jo" for="jo"></label>
                </td>
                <td width="15%">
                    <input id="jo" class="k-textbox" />
                </td>
                <td width="10%" style="text-align:center">
                    <label id="lbl_receiveprocess" for="receiveprocess"></label>
                </td>
                <td width="15%">
                    <input id="receiveprocess" style="width:auto"/>
                    <script>
                        $("#receiveprocess").kendoComboBox({
                            dataTextField: "CIPMS_CHS",
                            dataValueField: "PRC_CD",
                            change: function () {
                                //如果接收或者发送是SEW，则不是bypart
                                DocnoDetailByPart();
                            }
                        });
                    </script>
                </td>
                <td width="10%" style="text-align:center">
                </td>
                <td width="15%">
                </td>
            </tr>
        </table>
        <br />
        <div id="docnolist"></div>

        <script id="toolbardetail" type="text/x-kendo-template" style="color:blue">
            <a class="k-button" href="\#" onclick="detail()" id="btn_detail"></a>
        </script>
        <script id="toolselectall" type="text/x-kendo-template" style="color:blue">
            <a class="k-button" href="\#" onclick="selectall()" id="btn_selectall"></a>
        </script>
        <script id="toolselectnone" type="text/x-kendo-template" style="color:blue">
            <a class="k-button" href="\#" onclick="selectnone()" id="btn_selectnone"></a>
        </script>

        <script>
            var docnoselected = "";
         
            function GetDOCNODetail(obj, DOC_NO) {
                docnoModel.fetch(function () {
                    $.each(docnoModel.data(), function (i, o) {
                        if ($.trim(o.DOC_NO) == $.trim(DOC_NO)) {
                            if ($.trim(o.PROCESS_CD) == 'SEW' || $.trim(o.NEXT_PROCESS_CD) == 'SEW') {
                                //bybundle
                                BYPART = 'N';
                                var grid = $("#docnodetaillist").data("kendoGrid");
                                grid.hideColumn("PART_DESC");
                                grid.hideColumn("CARTON_BARCODE");
                            }
                            else {
                                //bypart
                                BYPART = 'Y';
                                var grid = $("#docnodetaillist").data("kendoGrid");
                                grid.showColumn("PART_DESC");
                                grid.showColumn("CARTON_BARCODE");
                            }
                            return false;
                        }
                    });
                });

                var docnodetaildata = null;
                var docnodetailModel = null;
                //var param;
                //param = "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'DOC_NO': '" + DOC_NO + "', 'BYPART': '" + BYPART + "' }";
                //alert(param);
                $.ajax({
                    async: true,
                    dataType: 'json',
                    contentType: "application/json;charset=utf-8",
                    type: 'POST',
                    url: 'Barcode_Inquiry.aspx/GetDOCNODetailList',
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'DOC_NO': '" + DOC_NO + "', 'BYPART': '" + BYPART + "' }",
                    success: function (data) {
                        var result = eval(data.d);
                        if (result[0].SUCCESS == true) {
                            docnodetaildata = result[0].data;
                            docnodetailModel = new kendo.data.DataSource({
                                data: docnodetaildata,
                                pageSize: 10
                            });

                            var grid = $("#docnodetaillist").data("kendoGrid");
                            grid.setDataSource(docnodetailModel);

                            docnoselected = DOC_NO;

                            $('#span_docno').html(docnoselected);

                            notification.show({
                                message: globeltranslate.msg_successtitle1
                            }, "upload-success");
                        }
                        else {
                            notification.show({
                                title: globeltranslate.msg_wrongtitle + '1033',
                                message: globeltranslate.msg_wrongmessage3
                            }, "error");
                        }
                        kendo.ui.progress($("#loading"), false);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        notification.show({
                            title: "Wrong",
                            message: XMLHttpRequest.status
                        }, "error");

                        kendo.ui.progress($("#loading"), false);
                    }
                });
            }
      
            function initdocnogrid() {
                $("#docnolist").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span id='spn_doclist'></span>")
                    }, {
                        name: "excel", text: "导出到EXCEL"
                    }, {
                        template: kendo.template($("#toolselectall").html())
                    }, {
                        template: kendo.template($("#toolselectnone").html())
                    }, {
                        template: kendo.template($("#toolbardetail").html())
                    }
                    ],
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
                    height: 350,
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
                    columns: [
                    {
                        title: "", //globeltranslate.table_checkbox,
                        field: "SELECTED",
                        template: "<input type='checkbox' id='table_checkbox' class='table_checkbox' value='#: SELECTED #' />",
                        click: function(e) {  
                                      var tr = $(e.target).closest("tr"); // get the current table row (tr)  
                                      var data = this.dataItem(tr);  
                                      console.log("Details for: " + data.name);  
                        },
                       width: 60,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }


                    }, {
                        title: globeltranslate.table_docno,
                        field: "DOC_NO",
                        template: "<a onclick='GetDOCNODetail(this, \"#= DOC_NO #\")'>#= DOC_NO #</a>",
                        width: 120,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_createdate,
                        field: "CREATE_DATE",
                        width: 75,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_updatedate,
                        field: "LAST_MODIFIED_DATE",
                        template: "#if(STATUS == 'C') {# <span>#= CONFIRM_DATE #</span> #} " +
                            "else if(STATUS == 'S') {# <span>#= SUBMIT_DATE #</span> #}#",
                        width: 75,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        //tangyh 2017.03.29
                        title: globeltranslate.table_qty,
                        field: "QTY",
                        width: 50,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_sendfactory,
                        field: "PROCESS_PRODUCTION_FACTORY",
                        width: 60,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_sendprocess,
                        field: "PROCESS_CD",
                        width: 75,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_sendproduction,
                        field: "PRODUCTION_LINE_CD",
                        template: "#if(PRODUCTION_LINE_CD == 'NA') {# <span></span> #} " +
                            "else {# <span>#= PRODUCTION_LINE_CD #</span> #}#",
                        width: 75,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_receivefactory,
                        field: "NEXT_PROCESS_PRODUCTION_FACTORY",
                        width: 60,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_receiveprocess,
                        field: "NEXT_PROCESS_CD",
                        width: 60,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_receiveproduction,
                        field: "NEXT_PRODUCTION_LINE_CD",
                        template: "#if(NEXT_PRODUCTION_LINE_CD == 'NA') {# <span></span> #} " +
                            "else {# <span>#= NEXT_PRODUCTION_LINE_CD #</span> #}#",
                        width: 60,
                        headerAttributes: {
                            "class": "table-cell",
                            style: "text-align: center; font-size: 12px"
                        },
                        attributes: { style: "text-align: center; font-size: 12px" }
                    }, {
                        title: globeltranslate.table_status,
                        field: "STATUS",
                        template: "#if(STATUS == 'C') {# <span>Confirm</span> #} " +
                            "else if(STATUS == 'S') {# <span>Submit</span> #}#",
                        width: 75,
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
                        e.workbook.fileName = "DOCNO_LIST.xlsx";
                    }
                });
            }

            function selectall()
            {
                $(".table_checkbox").each(function () {
                    $(this).prop("checked", true); //此处设置每行的checkbox选中，必须用prop方法
                    //$(this).attr("value")  记录好每行的id
                    $(this).closest("tr").addClass("k-state-selected");  //设置grid 每一行选中
                });

                //$('input:checkbox').each(function () {
                //    $(this).attr('checked', true);
                //});
            }
            function selectnone()
            {
                $(".table_checkbox").each(function(){
                    $(this).prop("checked", false); //此处设置每行的checkbox不选中，必须用prop方法
                    //$(this).attr("value")  移除每行的id
                    $(this).closest("tr").removeClass("k-state-selected");  //设置grid 每一行不选中
                });

                //$('input:checkbox').each(function () {
                //    $(this).attr('checked', false);
                //});
            }
            function detail()
            {
                var grid = $("#docnolist").data("kendoGrid");
                var gridrow = grid.dataSource.total();
                var DOCNO="";

                //for (var i = 0; i <= gridrow - 1; i++) {
                    
                //    ////dd = grid.tbody.find("tr:1 td:0");
                //    //dd = grid.dataItem(i);
                //    //alert(dd);
                //    ////if (grid.getItem(i).table_checkbox.checked == "checked")
                //    ////{
                //    //    alert(grid.getItem(i).table_checkbox.value);
                //    //}
                    
                //    if (DOCNO == "")
                //        DOCNO = grid.dataSource.at(i).SELECTED;
                //    else
                //        DOCNO = DOCNO + ","+grid.dataSource.at(i).SELECTED;
                //}

                //GetDOCNODetail(this, DOCNO)

                $('#docnolist').find(':checkbox').each(function () {

              // $(".table_checkbox").each(function () {
                   // alert($(this).attr("checked"));
                  //  alert($(this).val());
                  //  alert($(this).is(":checked"));
                    //if($(this).attr("checked")==true)
                    if ($(this).is(":checked")) 
                    {
                        if (DOCNO == "")
                            DOCNO = $(this).val();
                        else
                            DOCNO = DOCNO + "," + $(this).val();
                    }
                    
                });
                if (DOCNO == "") { }
                else
                { GetDOCNODetail(this, DOCNO); }


            }

        </script>
        <br />
        <div id="docnodetaillist"></div>
        <script>
            function initdocnodetailgrid() {
                $("#docnodetaillist").kendoGrid({
                    toolbar: [{
                        template: kendo.template("<span style='color: red' id='span_docno'></span><span>流水单详情</span>")
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
                    columns: [{
                        title: globeltranslate.table_jo,
                        field: "JOB_ORDER_NO",
                        width: 150
                    }, {
                        title: globeltranslate.table_color,
                        field: "COLOR_CD",
                        width: 170
                    }, {
                        title: globeltranslate.table_size,
                        field: "SIZE_CD",
                        width: 170
                    }, {
                        title: globeltranslate.table_layno,
                        field: "LAY_NO",
                        width: 60
                    }, {
                        title: globeltranslate.table_bundle,
                        field: "BUNDLE_NO",
                        width: 60
                    }, {
                        title: globeltranslate.table_part,
                        field: "PART_DESC",
                        hidden: true,
                        width: 120
                    }, {
                        title: globeltranslate.table_quantity,
                        field: "QTY",
                        width: 60
                    }, {
                        title: globeltranslate.table_bundlebarcode,
                        field: "BARCODE",
                        width: 170
                    }, {
                        title: globeltranslate.table_cartonbarcode,
                        field: "CARTON_BARCODE",
                        template: "#if(CARTON_STATUS == 'C') {# <span>#= CARTON_BARCODE #</span> #} " +
                            "else {# <span></span> #}#",
                        hidden: true,
                        width: 170
                    }],
                    excel: {
                        allPages: true
                    },
                    excelExport: function (e) {
                        e.workbook.fileName = docnoselected + "_DOCNO_DETAIL.xlsx";
                    }
                });
            }

            function DocnoDetailByPart() {
                if ($("#sendprocess").data("kendoComboBox").value() == 'SEW' || $("#receiveprocess").data("kendoComboBox").value() == 'SEW') {
                    //bybundle
                    BYPART = 'N';
                    var grid = $("#docnodetaillist").data("kendoGrid");
                    grid.hideColumn("PART_DESC");
                    grid.hideColumn("CARTON_BARCODE");
                }
                else {
                    //bypart
                    BYPART = 'Y';
                    var grid = $("#docnodetaillist").data("kendoGrid");
                    grid.showColumn("PART_DESC");
                    grid.showColumn("CARTON_BARCODE");
                }
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
                        <a id="btn_wipandoutputinquiry" onclick="pageredirect('wipandoutput')" data-transition="none"></a>
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
                                <a id="btn_query" onclick="btn_query_GetDOCNOList()" style="width:100%; line-height:2.8em; background-color:#1C1C1C; border-radius:0.6em" data-icon="search" data-role="button">a</a>
                                <script>
                                    var docnodata = null;
                                    var docnoModel = null;
                                    function ClearGrid()
                                    {
                                        $('#span_docno').html("");
                                        docnoModel = new kendo.data.DataSource({
                                            data: null
                          
                                        });
                                        
                                        var grid = $("#docnolist").data("kendoGrid");
                                        grid.setDataSource(docnoModel);
                                        docnoModel.fetch();
                                        docnoModel.read;
                                        
                                        var grid1 = $("#docnodetaillist").data("kendoGrid");
                                        grid1.setDataSource(new kendo.data.DataSource({
                                            data: null
                                        }));

                                        //initdocnogrid();

                                    }

                                    function btn_query_GetDOCNOList()
                                    {
                                         //ClearGrid();
                                        //$("#docnolist").data("kendoGrid").ClearGrid();
                                        //$("#docnodetaillist").data("kendoGrid").ClearGrid;

                                            kendo.ui.progress($("#loading"), true);
                                            $.ajax({
                                                async: true,
                                                dataType: 'json',
                                                contentType: "application/json;charset=utf-8",
                                                type: 'POST',
                                                url: 'Barcode_Inquiry.aspx/GetDOCNOList',
                                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'DOC_NO': '" + $('#docno').val() + "', 'JOB_ORDER_NO': '" + $('#jo').val() + "', 'STATUS': '" + $("#status").data("kendoComboBox").value() + "', 'SENDPROCESS': '" + $("#sendprocess").data("kendoComboBox").value() + "', 'RECEIVEPROCESS': '" + $("#receiveprocess").data("kendoComboBox").value() + "', 'CREATEDATEFROM': '" + $('#fromdate').val() + "', 'CREATEDATETO': '" + $('#todate').val() + "' }",
                                                success: function (data) {
                                                    var result = eval(data.d);
                                                    if (result[0].SUCCESS == true) {
                                                        docnodata = result[0].data;
                                                        docnoModel = new kendo.data.DataSource({
                                                            data: docnodata

                                                        });
                                                        docnoModel.fetch();
                                                        var grid = $("#docnolist").data("kendoGrid");
                                                        grid.setDataSource(docnoModel);

                                                        //var grid1 = $("#docnodetaillist").data("kendoGrid");
                                                        //grid1.setDataSource(new kendo.data.DataSource({
                                                        //    data: null,
                                                        //    pageSize:10
                                                        //}));

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
                                    
                                </script>
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

                                        begin("F");
                                        ClearGrid();
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

            begin("T");
            //20170615-tangyh
            queryfactory("T", fty, '');
            // $('#queryfactory').selectmenu('refresh');

        });
    </script>
</body>
</html>
