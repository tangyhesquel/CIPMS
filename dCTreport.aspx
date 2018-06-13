<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dCTreport.aspx.cs" Inherits="dCTreport" %>

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
</head>
<body>
    <div data-role="page">
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
            tr:nth-child(even)
            {
                background: #e9e9e9;
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

            //语言翻译函数
            window.language; //全局变量，保存用户选择了哪个json语言文件
            function translate() {
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
                        $('#fromdatelabel').html(globeltranslate.fromdatelabel);
                        $('#todatelabel').html(globeltranslate.todatelabel);
                        $('#datebutton').val(globeltranslate.datebutton).button('refresh');
                        $('#dctlabel').html(globeltranslate.dctlabel);
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
                    }
                });
            };

            function datebutton_quiry() {
                if ($('#fromdate').val() > $('#todate').val()) {
                    alert(globeltranslate.querymessage1);
                } else {
                    $.mobile.loading('show');
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "dCTreport.aspx/GetdCTtabledate",
                            contentType: "application/json;charset=utf-8",
                            async: false,
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'fromdate': '" + $('#fromdate').val() + "', 'todate': '" + $('#todate').val() + "' }",
                            beforeSend: function () { $.mobile.loading('show'); },
                            success: function (data) {
                                $.mobile.loading('hide');
                                $('#dcttable tr').empty();
                                if (data.d == "false") {
                                    alert(globeltranslate.querymessage2);
                                } else {
                                    $('#dcttable tbody').append(data.d);
                                    $('#dcttabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 5)
                                            $(this).width($('#dcttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                }
                                $('#dcttable').trigger('create');
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                alert(globeltranslate.errormessage + "021");
                            }
                        });
                    }, 10);
                }
            };

            function begin(flag) {
                Accessmodulehide();
                $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                var employeeno = sessionStorage.getItem("employeeno");
                var factory = sessionStorage.getItem("factory");
                var process = sessionStorage.getItem("process");
                var productionline = sessionStorage.getItem("productionline");
                var userbarcode = sessionStorage.getItem("userbarcode");
                $.mobile.loading('show');

                $.ajax({
                    type: "POST",
                    url: "Package.aspx/Accessfunction",
                    contentType: "application/json;charset=utf-8",
                    async: false,
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                    beforeSend: function () { $.mobile.loading('show'); },
                    success: function (data) {
                        //判断用户的权限按钮
                        var result = eval(data.d);
                        //判断用户的权限模块
                        Accessmoduleshow(data.d);
                        $.mobile.loading('hide');
                    },
                    error: function (err) {
                        $.mobile.loading('hide');
                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + errormessage() + "002</td></tr>");
                        $('#messagetable').trigger('create');
                        alert(errormessage() + "002");
                    }
                });

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
                $('#dcttable').chromatable({
                    width: screenwidth * 0.50,
                    height: screenheight * 0.3,
                    scrolling: "yes"
                });

                //加载表头
                $("#dcttabletitle thead tr").empty();
                $('#dcttabletitle thead').append("<tr><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                $('#dcttabletitle').trigger('create');
            }

            var docnodata = null;
            var docnoModel = null;

            $(function () {
                fty = getUrlParam('FTY');
                environment = getUrlParam('svTYPE');
                if (fty == null || environment == null) {
                    alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
                } else {
                    //语言选择
                    if (window.localStorage.languageid == "1") {
                        window.language = "dCTreport/dCTreport-zh-CN.js";
                    } else if (window.localStorage.languageid == "2") {
                        window.language = "dCTreport/dCTreport-en-US.js";
                    }
                    translate();

                    //function menu定义
                    $("#my-menu").mmenu({
                        "footer": {
                            "add": true,
                            "title": "CPMIS"
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
                        begin("T");
                        //20170615-tangyh
                        queryfactory("T", fty, 'li');
                    }
                }
            });
        </script>
        <div data-role="header" data-theme="b">
            <table style="width: 100%">
                <tr>
                    <td style="width: 8%">
                        <button id="functionmenubutton" data-icon="grid" data-iconpos="left"></button>
                    </td>
                    <td style="width: 30%">
                        <label id="messagetitle" style="text-align: left">
                        </label>
                    </td>
                    <td style="width: 24%; text-align: center">
                        <label id="headertitle"></label>
                    </td>


                    <%--//20170615-tangyh--%>
                     <td style="width:30%;display:table-cell" hidden="hidden" id="queryfactory_td" >
                         <select  id="queryfactory" name="queryfactory"></select>
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
                                        begin("T");
                                        
                                    }
                                });
                            </script>   
                    <td style="width: 8%; text-align: right">
                        <button id="cancelbutton" data-icon="back" data-iconpos="right"></button>
                    </td>
                </tr>
            </table>
        </div>
        <div data-role="content" style="position: absolute; top: 10%; width: 100%; bottom: 2%;">
            <div class="ui-grid-c">
                <div class="ui-block-a">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="fromdate" id="fromdatelabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input type="text" data-role="datebox" id="fromdate" name="fromdate"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="todate" id="todatelabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input type="text" data-role="datebox" id="todate" name="todate"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <input type="button" onclick="datebutton_quiry()" style="text-align:center" id="datebutton" name="datebutton" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                </div>
                <div class="ui-block-c">
                </div>
                <div class="ui-block-d">
                </div>
            </div>
            <table style="width:100%">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 50%; text-align: left">
                        <table style="width:100%">
                            <tr>
                                <td style="width: 30%"><label id="dctlabel"></label></td>
                                <td style="width: 70%"></td>
                            </tr>
                        </table>
                        <table class="stripe" id="dcttabletitle" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="dcttable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                    <td style="width: 47%">
                    </td>
                </tr>
            </table>
        </div>
        <div data-role="footer" data-position="fixed" data-fullscreen="true">
        </div>
    </div>
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
                    <li id="outputreportli"><a id="outputreporta" href="Outputreport.aspx" onclick="outputreportjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="dctreportli"><a id="dctreporta" href="" data-ajax="false"></a></li>
                    <li id="docnoli"><a id="docnoinquirya" href="Barcode_Inquiry.aspx" onclick="docnoinquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="bundleorcartonli"><a id="barcodeinquirya" href="Barcode_Information_Inquiry.aspx" onclick="bundleorcartoninquiryjump(fty, environment);return false;" data-ajax="false"></a></li>
                    <li id="wipandoutputli"><a id="wipandoutputlia" href="WipAndOutputNEW.aspx" onclick="WipAndOutputJump(fty, environment);return false;" data-ajax="false"></a></li>
                </ul>
            </li>
        </ul>
    </nav>
</body>
</html>
