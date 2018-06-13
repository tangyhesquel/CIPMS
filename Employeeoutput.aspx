<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Employeeoutput.aspx.cs" Inherits="Receive2" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
    <script src="js/jquery.chromatable.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
</head>
<body>
    
    <div data-role="page" id="mainpage">
        <br />
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

        var fty = "";
        var globeltranslate; //保存json字符串
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
                beforeSend: function () { $.mobile.loading('show'); },
                error: function () { $.mobile.loading('hide'); alert("error:001") },
                success: function (json) {
                    $.mobile.loading('hide');
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#barcodelabel').html(globeltranslate.barcodelabel);
                    $('#partlabel').html(globeltranslate.partlabel);
                    $('#userbarcodelabel').html(globeltranslate.userbarcodelabel);
                    $('#usernamelabel').html(globeltranslate.usernamelabel);
                    $('#totalbundlelabel').html(globeltranslate.totalbundlelabel);
                    $('#totalpcslabel').html(globeltranslate.totalpcslabel);
                    $('#processlabel').html(globeltranslate.processlabel);
                    $('#productionlinelabel').html(globeltranslate.productionlinelabel);
                    $('#messageth').html(globeltranslate.messageth);
                    $('#barcodelistlabel').html(globeltranslate.barcodelistlabel);
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
                    partselected = globeltranslate.select;
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                }
            });
        };

        function partselecttrans() {
            $('#partselect').empty();
            $('#partselect').append("<option>" + globeltranslate.select + "</option>");
            $('#partselect').selectmenu('refresh');
        }

        function empty() {
            $('#usernametext').val("");
            $('#processtext').val("");
            $('#productionlinetext').val("");
            part();
            $('#barcodetext').val("");
            $('#barcodetext').focus();
            $('#barcodelisttable tr:not(:first)').empty();
            totalbundles = 0;
            totalpcs = 0;
            $('#totalbundlediv').html(totalbundles);
            $('#totalpcsdiv').html(totalpcs);
            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.emptymessage + "</td></tr>");
            $('#messagetable').trigger('create');
        }

        function part() {
            partselecttrans();
            $.ajax({
                type: "POST",
                url: "Employeeoutput.aspx/Part",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function (data) {
                    $('#partselect').append(data.d);
                    $('#partselect').selectmenu('refresh');
                },
                error: function (err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "010</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(errormessage() + "010");
                }
            });
        }

        var partselected = "";
        var totalbundles = 0; //记录扫描的扎数和
        var totalpcs = 0; //记录扫描的件数和
        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "Employeeoutput/Employeeoutput-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Employeeoutput/Employeeoutput-en-US.js";
                }
                translate();

                //左侧栏
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
                    $('userbarcodetext').focus();
                    partselecttrans();
                    $('#totalbundlediv').html(totalbundles);
                    $('#totalpcsdiv').html(totalpcs);
                    //function权限控制
                    Accessmodulehide();
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    var employeeno = sessionStorage.getItem("employeeno");
                    var userbarcode = sessionStorage.getItem("userbarcode");
                    var factory = sessionStorage.getItem("factory");
                    var process = sessionStorage.getItem("process");
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Accessfunction",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        async: false,
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                        beforeSend: function () { $.mobile.loading('show'); },
                        success: function (data) {
                            $.mobile.loading('hide');
                            //判断用户的权限模块
                            Accessmoduleshow(data.d);
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "003</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(errormessage() + "003");
                        }
                    });
                    //加载part
                    part();
                }

                //表格样式
                var screenwidth = window.screen.width;
                var screenheight = window.screen.height;
                $('#barcodelisttable').chromatable({
                    width: screenwidth * 0.96,
                    height: screenheight * 0.7,
                    scrolling: "yes"
                });
                $('#messagetable').chromatable({
                    width: screenwidth * 0.17,
                    height: screenheight * 0.16,
                    scrolling: "yes"
                });

                //注销按钮：返回登录页面
                $('#cancelbutton').click(function () {
                    Loginout(fty, environment);
                });

                //用户barcodetext回车监控
                var barcodeuser = "";
                $('#userbarcodetext').bind('keypress', function (event) {
                    if (event.keyCode == '13') {
                        barcodeuser = $('#userbarcodetext').val();
                        if (barcodeuser.length == '8') {
                            //加载用户信息，用户名，部门，组别
                            $.ajax({
                                type: "POST",
                                url: "Employeeoutput.aspx/Userinformation",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                async: false,
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + barcodeuser + "' }",
                                success: function (data) {
                                    if (data.d != 'null') {
                                        var result = eval(data.d);
                                        if (result[0].PROCESS == process) {
                                            empty();
                                            $('#usernametext').val(result[0].NAME);
                                            $('#processtext').val(result[0].PROCESS);
                                            $('#productionlinetext').val(result[0].PRODUCTION);
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + result[0].NAME + globeltranslate.barcodescanmessage7 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                        } else {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage2 + process + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.barcodescanmessage2 + process);
                                            empty();
                                            barcodeuser = "";
                                        }
                                    } else {
                                        empty();
                                        barcodeuser = "";
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage1 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage1);
                                    }
                                },
                                error: function (err) {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "005</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "005");
                                }
                            });
                        } else {
                            empty();
                            barcodeuser = "";
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage1 + "</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.barcodescanmessage1);
                        }
                    }
                });

                //扫描条码
                $('#barcodetext').bind('keypress', function (event) {
                    if (event.keyCode == '13') {
                        //判断扫描的是箱码还是扎码
                        var barcode = $('#barcodetext').val();
                        var tempuserbarcode = "";
                        if (barcodeuser == "")
                            tempuserbarcode = userbarcode;
                        else
                            tempuserbarcode = barcodeuser;
                        if (barcode.length == '14') {
                            //判断是否已经选择部件
                            var part = $('#partselect').val();
                            if (part == partselected) {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.partselectmessage + "</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.partselectmessage);
                            } else {
                                $.ajax({
                                    type: "POST",
                                    url: "Employeeoutput.aspx/Bundlebarcodescan",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    async: false,
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process':'" + process + "', 'userbarcode': '" + tempuserbarcode + "', 'barcode': '" + barcode + "', 'part':'" + part + "' }",
                                    success: function (data) {
                                        if (data.d.substr(0, 6) == "false1") {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage3 + data.d.substr(6, data.d.length - 6) + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.barcodescanmessage3 + data.d.substr(6, data.d.length - 6));
                                        } else if (data.d == "false2") {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage4 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.barcodescanmessage4);
                                        } else if (data.d.substr(0, 6) == "false3") {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage5 + data.d.substr(6, data.d.length - 6) + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.barcodescanmessage5 + data.d.substr(6, data.d.length - 6));
                                        } else {
                                            $('#barcodetext').val("");
                                            $('#barcodetext').focus();
                                            var result = eval(data.d);
                                            $('#barcodelisttable tbody').after(result[0].HTML);
                                            $('#barcodelisttable').trigger('create');
                                            totalbundles++;
                                            totalpcs += parseInt(result[0].OUT_QTY);
                                            $('#totalbundlediv').html(totalbundles);
                                            $('#totalpcsdiv').html(totalpcs);
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage8 + barcode + ":" + part + globeltranslate.barcodescanmessage9 + tempuserbarcode + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                        }
                                    },
                                    error: function (err) {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "098</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "098");
                                    }
                                });
                            }
                        } else if (barcode.length == '13') {
                            $.ajax({
                                type: "POST",
                                url: "Employeeoutput.aspx/Cartonbarcodescan",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                async: false,
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process':'" + process + "', 'userbarcode': '" + tempuserbarcode + "', 'barcode': '" + barcode + "'}",
                                success: function (data) {
                                    if (data.d.substr(0, 6) == "false1") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage3 + data.d.substr(6, data.d.length - 6) + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage3 + data.d.substr(6, data.d.length - 6));
                                    } else if (data.d == "false2") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage6 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage6);
                                    } else if (data.d == "false3") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage4 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage4);
                                    } else if (data.d.substr(0, 6) == "false4") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage5 + data.d.substr(6, data.d.length - 6) + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage5 + data.d.substr(6, data.d.length - 6));
                                    } else {
                                        $('#barcodetext').val("");
                                        $('#barcodetext').focus();
                                        var result = eval(data.d);
                                        $('#barcodelisttable tbody').after(result[0].HTML);
                                        $('#barcodelisttable').trigger('create');
                                        totalbundles += parseInt(result[0].BUNDLE);
                                        totalpcs += parseInt(result[0].OUT_QTY);
                                        $('#totalbundlediv').html(totalbundles);
                                        $('#totalpcsdiv').html(totalpcs);
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage8 + barcode + globeltranslate.barcodescanmessage9 + tempuserbarcode + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    }
                                },
                                error: function (err) {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "098</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "098");
                                }
                            });
                        }
                    }
                });
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
                    <td style="width: 30%">
                    </td>
                    <td style="width: 8%; text-align: right">
                        <button id="cancelbutton" data-icon="back" data-iconpos="right"></button>
                    </td>
                </tr>
            </table>
        </div>
        <div data-role="content" style="position: absolute; top: 10%; width: 100%; bottom: 2%;">
            <div class="ui-grid-d">
                <div class="ui-block-a">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center">
                                <label for="userbarcodetext" id="userbarcodelabel" ></label>
                            </td>
                            <td style="width: 70%">
                                <input type="password" id="userbarcodetext" name="userbarcodetext" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center">
                                <label for="usernametext" id="usernamelabel" ></label>
                            </td>
                            <td style="width: 70%;">
                                <input readonly="readonly" type="text" id="usernametext" name="usernametext" style="color: Red" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center">
                                <label for="processtext" id="processlabel" ></label>
                            </td>
                            <td style="width: 70%;">
                                <input readonly="readonly" type="text" id="processtext" name="processtext" style="color: Red" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center">
                                <label for="productionlinetext" id="productionlinelabel" ></label>
                            </td>
                            <td style="width: 70%;">
                                <input readonly="readonly" type="text" id="productionlinetext" name="productionlinetext" style="color: Red" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                </div>
                <div class="ui-block-d">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center">
                                <label for="partselect" id="partlabel" ></label>
                            </td>
                            <td style="width: 70%; text-align: right">
                                <select id="partselect" name="partselect" data-native-menu="false">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center">
                                <label for="barcodetext" id="barcodelabel" ></label>
                            </td>
                            <td style="width: 70%; text-align: right">
                                <input type="text" id="barcodetext" name="barcodetext" placeholder="Bundle/Carton" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-e">
                    <table width="100%">
                        <tr>
                            <td style="width: 3%"></td>
                            <td style="width: 94%; text-align: center">
                                <table width="100%" id="messagetable" border="0" cellspacing="0" cellpadding="0">
                                    <thead>
                                        <tr>
                                            <th style="text-align: center" id="messageth"></th>
                                        </tr>
                                    </thead>
                                    <tbody id="messagetbody">
                                    </tbody>
                                </table>
                            </td>
                            <td style="width: 3%"></td>
                        </tr>
                    </table>
                </div>
                <table width="100%">
                    <tr>
                        <td style="width:3%"></td>
                        <td style="width: 94%">
                            <table width="100%">
                                <tr>
                                    <td style="width: 10%"><label id="barcodelistlabel"></label></td>
                                    <td style="width: 50%"></td>
                                    <td style="width: 15%"><label id="totalbundlelabel" style="color:Red; text-align:right"></label></td>
                                    <td style="width: 5%"><div style="color:Red" id="totalbundlediv"></div></td>
                                    <td style="width: 15%"><label id="totalpcslabel" style="color:Red; text-align:right"></label></td>
                                    <td style="width: 5%"><div style="color:Red" id="totalpcsdiv"></div></td>
                                </tr>
                            </table>
                            <table class="stripe" id="barcodelisttable" width="100%" border="0" cellspacing="0" cellpadding="0">
                                <thead>   
                                    <tr> 
                                        <th>
                                            ----Carton---
                                        </th>
                                        <th>
                                            -------Bundle-------
                                        </th>
                                        <th>
                                            --Part--
                                        </th>
                                        <th>
                                            JOB_ORDER_NO
                                        </th>
                                        <th>
                                            Bundle_No
                                        </th>
                                        <th>
                                            Lay_No
                                        </th>
                                        <th>
                                            Size
                                        </th>
                                        <th>
                                            Color
                                        </th>
                                        <th>
                                            Cut_Qty
                                        </th>
                                        <th>
                                            Out_Qty
                                        </th>
                                    </tr>   
                                </thead>   
                                <tbody>
                                </tbody>
                            </table>
                        </td>
                        <td style="width:3%"></td>
                    </tr>
                </table>
            </div>    
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
            <!--added by lijer-->          
            <li id="printsparebarcodeli">
                <a id="printsparebarcodea" href="PrintSparebarcode.aspx"   onclick="printsparebarcodejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="employeeoutputli">
                <a id="employeeoutputa" href="" data-ajax="false"></a>
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
