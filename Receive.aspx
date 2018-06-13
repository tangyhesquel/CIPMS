<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Receive.aspx.cs" Inherits="Receive" %>

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
    <script src="js/Dialog.js" type="text/javascript"></script>
    <script src="js/jquery.simplePagination.js" type="text/javascript"></script>
    <link href="css/simplePagination.css" rel="stylesheet" type="text/css" />
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
        var fty = "";
        var productionselected = "";
        var partselected = "";
        var globeltranslate; //保存json字符串
        var environment = "";
        var arr; //全局定义查询结果数组
        var pagenum;
        var currentpagenum = 1;

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
                error: function() {
                    alert("Sorry! Pls check your network and refresh your brower! Wrong Code::001");
                },
                success: function(json) {
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#cancelbutton').html(globeltranslate.cancelbutton);
                    $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                    $('#barcodelabel').html(globeltranslate.barcodelabel);
                    $('#partlabel').html(globeltranslate.partlabel);
                    $('#totalbundlelabel').html(globeltranslate.totalbundlelabel);
                    $('#totalgarmentpcslabel').html(globeltranslate.totalgarmentpcslabel);
                    $('#totalpcslabel').html(globeltranslate.totalpcslabel);
                    $('#fromfactoryselectlabel').html(globeltranslate.fromfactoryselectlabel);
                    $('#nextfactorylabel').html(globeltranslate.nextfactorylabel);
                    $('#confirmbutton').val(globeltranslate.confirmbutton).button('refresh');
                    $('#rejectbutton').val(globeltranslate.rejectbutton).button('refresh');
                    $('#savebutton').val(globeltranslate.savebutton).button('refresh');
                    $('#emptybutton').val(globeltranslate.emptybutton).button('refresh');
                    $('#messageth').html(globeltranslate.messageth);
                    $('#barcodelistlabel').html(globeltranslate.barcodelistlabel);
                    $('#docnolabel').html(globeltranslate.docnolabel);
                    $('#partdivlabel').html(globeltranslate.partdivlabel);
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
        function factoryselecttrans() {
            $('#fromfactoryselect').empty();
            $('#fromfactoryselect').append("<option>" + globeltranslate.factoryselect + "</option>");
            $('#fromfactoryselect').selectmenu('refresh');
            $('#nextfactoryselect').empty();
            $('#nextfactoryselect').append("<option>" + globeltranslate.factoryselect + "</option>");
            $('#nextfactoryselect').selectmenu('refresh');
        }
        function processselecttrans() {
            $('#fromprocessselect').empty();
            $('#fromprocessselect').append("<option>" + globeltranslate.processselect + "</option>");
            $('#fromprocessselect').selectmenu('refresh');
            $('#nextprocessselect').empty();
            $('#nextprocessselect').append("<option>" + globeltranslate.processselect + "</option>");
            $('#nextprocessselect').selectmenu('refresh');
        }
        function productionlineselecttrans() {
            $('#fromproductionlineselect').empty();
            $('#fromproductionlineselect').append("<option>" + globeltranslate.productionlineselect + "</option>");
            $('#fromproductionlineselect').selectmenu('refresh');
            $('#nextproductionlineselect').empty();
            $('#nextproductionlineselect').append("<option>" + globeltranslate.productionlineselect + "</option>");
            $('#nextproductionlineselect').selectmenu('refresh');
            productionselected = globeltranslate.productionlineselect;
        }
        function productiontrans() {
            $('#production').empty();
            $('#production').append("<option>" + globeltranslate.productionlineselect + "</option>");
            $('#production').selectmenu('refresh');
            productionselected = globeltranslate.productionlineselect;
        }
        function queryselecttrans() {
            $('#querybutton').empty();
            $('#querybutton').append("<option>" + globeltranslate.querybutton + "</option>");
            $('#querybutton').selectmenu('refresh');
        }

        function empty() {
            arr = [];
            $('#selector').pagination({
                cssStyle: 'light-theme',
                prevText: globeltranslate.prevbutton,
                nextText: globeltranslate.nextbutton
            });
            sessionStorage.setItem("firstscan","1");
            $('#barcodetext').val("");
            $('#barcodetext').focus();
            buttonstatus = '0';
            btnstatus();
            partselecttrans();
            part();
            factoryselecttrans();
            processselecttrans();
            productiontrans();
            productionselect();
            productionlineselecttrans();
            $('#nextproductionlineselect').selectmenu('refresh');
            $('#barcodelisttable tr').empty();
            $('#totalbundlediv').html("0");
            $('#totalgarmentpcs').html("0");
            $('#totalpcsdiv').html("0");
            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.clearmessage1 + "</td></tr>");
            $('#messagetable').trigger('create');
        }

        function part() {
            $.ajax({
                type: "POST",
                url: "Receive.aspx/Partselect",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function(data) {
                    $('#partselect').append(data.d);
                    $('#partselect').selectmenu('refresh');
                },
                error: function (err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "005</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "005");
                }
            });
        }

        function query() {
            $.ajax({
                type: "POST",
                url: "Receive.aspx/Query",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'module': 'Receive' }",
                success: function (data) {
                    queryselecttrans();
                    $('#querybutton').append(data.d);
                    $('#querybutton').selectmenu('refresh');
                },
                error: function (err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "006</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "006");
                }
            });
        }

        function productionselect() {
            $.ajax({
                type: "POST",
                url: "Receive.aspx/Production",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "' }",
                success: function (data) {
                    productiontrans();
                    $('#production').append(data.d);
                    $('#production').selectmenu('refresh');
                },
                error: function (err) {
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "006</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "006");
                }
            });
        }

        var buttonstatus = '0';//0为初始状态，1为允许接收状态，2为不允许接收状态
        function btnstatus() {
            if (buttonstatus == '0' || buttonstatus == '2') {
                $('#confirmbutton').attr('disabled', 'true').button('refresh');
            } else {
                $('#confirmbutton').removeAttr('disabled').button('refresh');
            }
        }

        var partselect = "";
        var fromfactory = "";

        $(function () {
            sessionStorage.setItem("firstscan", "1");
            if (sessionStorage.getItem("oasreceive") == '1') {
                $('input[name=radio]').attr("checked", true).checkboxradio("refresh");
            } else if (sessionStorage.getItem("oasreceive") == '0') {
                $('input[name=radio]').attr("checked", false).checkboxradio("refresh");
            } else {
                $('input[name=radio]').attr("checked", false).checkboxradio("refresh");
                sessionStorage.setItem("oasreceive", "0");
            }
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            }
            else {
                var d = new Date();
                var vYear = d.getFullYear()
                var vMon = d.getMonth() + 1;
                var vDay = d.getDate();
                var h = d.getHours();
                var m = d.getMinutes();
                var se = d.getSeconds();
                docno = sessionStorage.getItem("userbarcode") + vYear + vMon + vDay + h + m + se;


                //调用语言翻译函数
                if (window.localStorage.languageid == "1") {
                    window.language = "Receive/Receive-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Receive/Receive-en-US.js"; ;
                }
                translate();

                //左侧栏
                $("#my-menu").mmenu({
                    "footer": {
                        "add": true,
                        "title": globeltranslate.footertitle
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
                    productionselect();
                    $('#barcodetext').focus();
                    $('#totalbundlediv').html("0");
                    $('#totalgarmentpcs').html("0");
                    $('#totalpcsdiv').html("0");
                    btnstatus();
                    queryselecttrans();
                    partselecttrans();
                    factoryselecttrans();
                    processselecttrans();
                    productionlineselecttrans();
                    //function权限控制
                    Accessmodulehide();
                    $('#confirmbuttontd').hide();
                    $('#rejectbuttontd').hide();
                    $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
                    var employeeno = sessionStorage.getItem("employeeno");
                    var factory = sessionStorage.getItem("factory");
                    var process = sessionStorage.getItem("process");
                    var productionline = sessionStorage.getItem("productionline");
                    var userbarcode = sessionStorage.getItem("userbarcode");
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Accessfunction",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                        success: function (data) {
                            //判断用户的权限按钮
                            var result = eval(data.d);
                            $(result).each(function (i) {
                                if (result[i].FUNCTION_ENG == 'Receive') $('#confirmbuttontd').show();
                                else if (result[i].FUNCTION_ENG == 'Reject') $('#rejectbuttontd').show();
                            });
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
                    //part
                    part();
                    //query
                    query();

                    if (sessionStorage.getItem("receivecheck") == '2') {
                        sessionStorage.setItem("firstscan", "1");
                        //激活接收confirm按钮
                        if (sessionStorage.getItem("checkresult") == 'Y') {
                            $('#confirmbutton').removeAttr('disabled').button('refresh');
                        }
                        //更新cipms_user_scanning_dft并显示到UI上
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Receive.aspx/Receivecheckload",
                                contentType: "application/json;charset=utf-8",
                                async: false,
                                dataType: "json",
                                data: "{ 'docno': '" + sessionStorage.getItem("DOC_NO") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'olddocno': '" + sessionStorage.getItem("docno") + "', 'newdocno': '" + docno + "', 'factory': '" + sessionStorage.getItem("peerfactory") + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    var result = eval(data.d);
                                    $('#barcodelisttable tbody').after(result[0].HTML);
                                    $('#barcodelisttable').trigger('create');
                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                    $('#totalpcsdiv').html(result[0].TOTALPCS);
                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                    factoryselecttrans();
                                    $('#nextfactoryselect').append("<option selected='selected'>" + result[0].FACTORY + "</option>");
                                    $('#nextfactoryselect').selectmenu('refresh');
                                    processselecttrans();
                                    $('#nextprocessselect').append("<option selected='selected'>" + result[0].PROCESS + "</option>");
                                    $('#nextprocessselect').selectmenu('refresh');
                                    productionlineselecttrans();
                                    $('#nextproductionlineselect').append("<option selected='selected'>" + result[0].PRODUCTIONLINE + "</option>");
                                    $('#nextproductionlineselect').selectmenu('refresh');
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                }
                            });
                        }, 10);
                    }
                }

                //表格样式
                var screenwidth = document.body.clientWidth;
                var screenheight = document.body.clientHeight;
                $('#barcodelisttable').chromatable({
                    width: screenwidth * 0.96,
                    height: screenheight * 0.7,
                    scrolling: "yes"
                });
                $('#messagetable').chromatable({
                    width: screenwidth * 0.22,
                    height: screenheight * 0.23,
                    scrolling: "yes"
                });

                //清空按钮监控
                $('#emptybutton').click(function () {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Emptylist",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                            success: function (data) {
                                $.mobile.loading("hide");
                                empty();
                            },
                            error: function (err) {
                                $.mobile.loading("hide");
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "024</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "024");
                            }
                        });
                    }, 10);
                });

                //Save按钮监控
                $('#savebutton').click(function () {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $('#navbar').hide();
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Save",
                            contentType: "application/json;charset=utf-8",
                            //async: false,
                            dataType: "json",
                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'module':'Receive' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                $('#navbar').show();
                                $('#barcodetext').focus();
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.savemessage1 + data.d + "</td></tr>");
                                $('#messagetable').trigger('create');
                                query();
                                alert(globeltranslate.savemessage1 + data.d);
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#navbar').show();
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "044</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "044");
                            }
                        });
                    }, 10);
                });

                //注销按钮：返回登录页面
                $('#cancelbutton').click(function () {
                    Loginout(fty, environment);
                });

                //barcode扫描监控
                $('#barcodetext').bind('keypress', function (event) {
                    if (event.keyCode == '13') {
                        var barcode = $('#barcodetext').val();
                        barcodetype = GetBarcodeType(barcode);
                        if (barcodetype != 'B' && barcodetype != 'C' && barcodetype != 'D') {
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage1 + "</td></tr>");
                            $('#messagetable').trigger('create');
                            alert(globeltranslate.barcodescanmessage1);
                            $('#barcodetext').val("");
                            $('#barcodetext').focus();
                            return;
                        }
                        else if (sessionStorage.getItem("oasreceive") == '0') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            $('#navbar').hide();
                            var receiveprocess;
                            receiveprocess = sessionStorage.getItem("process");
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Receive.aspx/Barcodescan",
                                    contentType: "application/json;charset=utf-8",
                                    //async: false,
                                    dataType: "json",
                                    data: "{ 'factory':'" + fty + "', 'svTYPE':'" + environment + "', 'process':'" + receiveprocess + "', 'barcode':'" + barcode + "', 'docno':'" + docno + "', 'userbarcode':'" + sessionStorage.getItem("userbarcode") + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#barcodetext').val("");
                                        $('#barcodetext').focus();
                                        if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                            var result = eval(data.d);
                                            if (result[0].STATUS == 'N') {
                                                buttonstatus = '2';
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + bundlestatusalertmessage(result, globeltranslate) + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(bundlestatusalertmessage(result, globeltranslate));
                                            }
                                            else {
                                                //激活接收按钮标志
                                                buttonstatus = '1';

                                                //获取bundle信息
                                                var html = result[0].HTML;
                                                if (html == '') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    return;
                                                }

                                                //更新界面的接收部门和发送部门以及发送类型以及table的title
                                                $('#fromfactoryselect').append("<option selected='selected' value='" + result[0].SENDFACTORY + "'>" + result[0].SENDFACTORY + "</option>");
                                                $('#fromprocessselect').append("<option selected='selected' value='" + result[0].SENDPROCESS + "'>" + result[0].SENDPROCESS + "</option>");
                                                $('#fromproductionselect').append("<option selected='selected' value='" + result[0].SENDPRODUCTION + "'>" + result[0].SENDPRODUCTION + "</option>");
                                                $('#nextfactoryselect').append("<option selected='selected' value='" + result[0].RECEIVEFACTORY + "'>" + result[0].RECEIVEFACTORY + "</option>");
                                                $('#nextprocessselect').append("<option selected='selected' value='" + result[0].RECEIVEPROCESS + "'>" + result[0].RECEIVEPROCESS + "</option>");
                                                $('#nextproductionlineselect').append("<option selected='selected' value='" + result[0].RECEIVEPRODUCTION + "'>" + result[0].RECEIVEPRODUCTION + "</option>");
                                                $('#fromfactoryselect').selectmenu('refresh');
                                                $('#fromprocessselect').selectmenu('refresh');
                                                $('#fromproductionselect').selectmenu('refresh');
                                                $('#nextfactoryselect').selectmenu('refresh');
                                                $('#nextprocessselect').selectmenu('refresh');
                                                $('#nextproductionlineselect').selectmenu('refresh');
                                                if (result[0].RECEIVEFACTORY == result[0].SENDFACTORY)
                                                    $('#processtypeselect').append("<option selected='selected' value='I'>I</option>");
                                                else
                                                    $('#processtypeselect').append("<option selected='selected' value='P'>P</option>");
                                                $('#processtypeselect').selectmenu('refresh');
                                                if ($('#nextprocessselect').val() == 'SEW') {
                                                    $("#barcodetabletitle thead tr").empty();
                                                    $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation0 + "</th><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th><th></th></tr>");
                                                    $('#barcodetabletitle').trigger('create');
                                                }
                                                else {
                                                    $("#barcodetabletitle thead tr").empty();
                                                    $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation0 + "</th><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation4 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th></tr>");
                                                    $('#barcodetabletitle').trigger('create');
                                                }

                                                //刷新table的title
                                                if ($('#nextprocessselect').val() == 'SEW') {
                                                    //把bundle信息添加到table里
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += receive(arr, 'true');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += receivechange(arr, currentpagenum, 'true');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });
                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });
                                                }
                                                else {
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    if (currentpagenum > pagenum) {
                                                        currentpagenum = pagenum;
                                                    }
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += receive(arr, 'false');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        currentPage: currentpagenum,
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += receivechange(arr, currentpagenum, 'false');
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });
                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 13)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });
                                                }

                                                //更新界面的扎数等数量信息
                                                $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                                $('#totalpcsdiv').html(result[0].TOTALPCS);
                                                $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);

                                                //扫描成功信息提示
                                                if (barcode.length == 14) {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage14 + result[0].DOC_NO + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                                else if (barcode.length == 13 || barcode.length == 15) {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage15 + result[0].DOC_NO + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                                else if (barcode.length == 16) {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage16 + result[0].DOC_NO + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == "false") {
                                            buttonstatus = '2';
                                            if (data.d == 'false1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage11 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.barcodescanmessage11);
                                            }
                                            ////else if (data.d == "false2") {
                                            ////    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage17 + "</td></tr>");
                                            ////    $('#messagetable').trigger('create');
                                            ////    alert(globeltranslate.barcodescanmessage17);
                                            //}
                                            else if (data.d == "false3") {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage18 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.barcodescanmessage18);
                                            }
                                            else if (data.d == "false4") {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage19 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.barcodescanmessage19);
                                            }
                                        }
                                        else if (data.d.substr(0, 5) == "error") {
                                            buttonstatus = '2';
                                            if (data.d == 'error1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage20 + "890</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.barcodescanmessage20 + "890");
                                            }
                                        }
                                        btnstatus();
                                        $('#barcodetext').val("");
                                        $('#barcodetext').focus();
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#navbar').show();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "125</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "125");
                                    }
                                });
                            }, 10);
                        }
                        else {
                            //OAS接收：只能扫描裁片条码，并且必须进行复核
                            //先找到接收的DOC_NO
                            //跳转去复核页面进行流水单复核
                        }
                    }
                });

                //confirm按钮接收监控
                $('#confirmbutton').click(function () {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $('#navbar').hide();
                    if ($('#nextprocessselect').val() != 'SEW') {
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Receive.aspx/Transactionconfirm",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                //async: false,
                                data: "{ 'factory':'" + fty + "', 'svTYPE':'" + environment + "', 'process':'" + sessionStorage.getItem("process") + "', 'docno':'" + docno + "', 'userbarcode':'" + userbarcode + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                        if (data.d == 'success') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage1 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage1);
                                            empty();
                                        }
                                    }
                                    else if (data.d.substr(0, 5) == 'false') {
                                        if (data.d == 'false1') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "1</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.errormessage2 + '1');
                                        }
                                        else if (data.d == 'false2') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage11 + "2</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.barcodescanmessage11 + '2');
                                        }
                                        else if (data.d == 'false3') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage5 + "3</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage5 + '3');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false4') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage6 + "4</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage6 + '4');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false5') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage7 + "5</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage7 + '5');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false6') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage8 + "6</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage8 + '6');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false11') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage10 + "11</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage10 + '11');
                                            buttonstatus = '2';
                                        }
                                        btnstatus();
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "074</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "074");
                                }
                            });
                        }, 10);
                    }
                    else {
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Receive.aspx/GTNTransactionconfirm",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                //async: false,
                                data: "{ 'factory':'" + fty + "', 'svTYPE':'" + environment + "', 'process':'" + sessionStorage.getItem("process") + "', 'docno':'" + docno + "', 'userbarcode':'" + userbarcode + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                        if (data.d == 'Y') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage9 + "Y</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage9 + 'Y');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'success') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage1 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage1);
                                            empty();
                                        }
                                    }
                                    else if (data.d.substr(0, 5) == 'error') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + data.d.substr(5) + "21" + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage2 + "21" + data.d.substr(5));
                                    }
                                    else if (data.d.substr(0, 5) == 'false') {
                                        if (data.d == 'false1') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "1</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.errormessage2 + '1');
                                        }
                                        else if (data.d == 'false2') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage11 + "2</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.barcodescanmessage11 + '2');
                                        }
                                        else if (data.d == 'false3') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage5 + "3</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage5 + '3');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false4') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage6 + "4</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage6 + '4');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false5') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage7 + "5</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage7 + '5');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false6') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage8 + "6</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage8 + '6');
                                            buttonstatus = '2';
                                        }
                                        else if (data.d == 'false100') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage10 + "100</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage10 + '100');
                                        }
                                        else if (data.d == 'false200') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage10 + "200</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage10 + '200');
                                        }
                                        else if (data.d == 'false11') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage10 + "11</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.confirmmessage10 + '11');
                                            buttonstatus = '2';
                                        }
                                    }
                                    btnstatus();
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "777</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "777");
                                }
                            });
                        }, 10);
                    }
                });

                //Query选择改变监控
                $('#querybutton').change(function () {
                    var date = $(this).find('option:selected').text();
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    $('#navbar').hide();
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Receive.aspx/Querychange",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            //async: false,
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'docno':'" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', date'':'" + date + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                $('#navbar').show();
                                $('#barcodetext').val("");
                                $('#barcodetext').focus();
                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                    var result = eval(data.d);
                                    if (result[0].STATUS == 'N') {
                                        buttonstatus = '2';
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + bundlestatusalertmessage(result, globeltranslate) + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(bundlestatusalertmessage(result, globeltranslate));
                                    }
                                    else {
                                        //激活接收按钮标志
                                        buttonstatus = '1';

                                        //获取bundle信息
                                        var html = result[0].HTML;
                                        if (html == '') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage15 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            return;
                                        }

                                        //更新界面的接收部门和发送部门以及发送类型以及table的title
                                        $('#fromfactoryselect').append("<option selected='selected' value='" + result[0].SENDFACTORY + "'>" + result[0].SENDFACTORY + "</option>");
                                        $('#fromprocessselect').append("<option selected='selected' value='" + result[0].SENDPROCESS + "'>" + result[0].SENDPROCESS + "</option>");
                                        $('#fromproductionselect').append("<option selected='selected' value='" + result[0].SENDPRODUCTION + "'>" + result[0].SENDPRODUCTION + "</option>");
                                        $('#nextfactoryselect').append("<option selected='selected' value='" + result[0].RECEIVEFACTORY + "'>" + result[0].RECEIVEFACTORY + "</option>");
                                        $('#nextprocessselect').append("<option selected='selected' value='" + result[0].RECEIVEPROCESS + "'>" + result[0].RECEIVEPROCESS + "</option>");
                                        $('#nextproductionlineselect').append("<option selected='selected' value='" + result[0].RECEIVEPRODUCTION + "'>" + result[0].RECEIVEPRODUCTION + "</option>");
                                        $('#fromfactoryselect').selectmenu('refresh');
                                        $('#fromprocessselect').selectmenu('refresh');
                                        $('#fromproductionselect').selectmenu('refresh');
                                        $('#nextfactoryselect').selectmenu('refresh');
                                        $('#nextprocessselect').selectmenu('refresh');
                                        $('#nextproductionlineselect').selectmenu('refresh');
                                        if (result[0].RECEIVEFACTORY == result[0].SENDFACTORY)
                                            $('#processtypeselect').append("<option selected='selected' value='I'>I</option>");
                                        else
                                            $('#processtypeselect').append("<option selected='selected' value='P'>P</option>");
                                        $('#processtypeselect').selectmenu('refresh');
                                        if ($('#nextprocessselect').val() == 'SEW') {
                                            $("#barcodetabletitle thead tr").empty();
                                            $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation0 + "</th><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th><th></th></tr>");
                                            $('#barcodetabletitle').trigger('create');
                                        }
                                        else {
                                            $("#barcodetabletitle thead tr").empty();
                                            $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.tabletranslation0 + "</th><th>" + globeltranslate.tabletranslation1 + "</th><th>" + globeltranslate.tabletranslation2 + "</th><th>" + globeltranslate.tabletranslation3 + "</th><th>" + globeltranslate.tabletranslation4 + "</th><th>" + globeltranslate.tabletranslation5 + "</th><th>" + globeltranslate.tabletranslation6 + "</th><th>" + globeltranslate.tabletranslation7 + "</th><th>" + globeltranslate.tabletranslation8 + "</th><th>" + globeltranslate.tabletranslation9 + "</th><th>" + globeltranslate.tabletranslation10 + "</th><th>" + globeltranslate.tabletranslation11 + "</th><th>" + globeltranslate.tabletranslation12 + "</th><th>" + globeltranslate.tabletranslation13 + "</th></tr>");
                                            $('#barcodetabletitle').trigger('create');
                                        }

                                        //刷新table的title
                                        if ($('#nextprocessselect').val() == 'SEW') {
                                            //把bundle信息添加到table里
                                            arr = html.split('@');
                                            pagenum = Math.ceil(arr.length / 10);
                                            if (currentpagenum > pagenum) {
                                                currentpagenum = pagenum;
                                            }
                                            html = "";
                                            $('#barcodelisttable tr').empty();
                                            html += receive(arr, 'true');
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');

                                            $('#selector').pagination({
                                                items: arr.length,
                                                itemsOnPage: 10,
                                                cssStyle: 'light-theme',
                                                currentPage: currentpagenum,
                                                prevText: globeltranslate.prevbutton,
                                                nextText: globeltranslate.nextbutton,
                                                onPageClick: function (pagenumber, event) {
                                                    currentpagenum = pagenumber;
                                                    $('#barcodelisttable tbody').empty();
                                                    html = "";
                                                    html += receivechange(arr, currentpagenum, 'true');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');
                                                }
                                            });
                                            $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 12)
                                                    $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                            });
                                        }
                                        else {
                                            arr = html.split('@');
                                            pagenum = Math.ceil(arr.length / 10);
                                            if (currentpagenum > pagenum) {
                                                currentpagenum = pagenum;
                                            }
                                            html = "";
                                            $('#barcodelisttable tr').empty();
                                            html += receive(arr, 'false');
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');

                                            $('#selector').pagination({
                                                items: arr.length,
                                                itemsOnPage: 10,
                                                cssStyle: 'light-theme',
                                                currentPage: currentpagenum,
                                                prevText: globeltranslate.prevbutton,
                                                nextText: globeltranslate.nextbutton,
                                                onPageClick: function (pagenumber, event) {
                                                    currentpagenum = pagenumber;
                                                    $('#barcodelisttable tbody').empty();
                                                    html = "";
                                                    html += receivechange(arr, currentpagenum, 'false');
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');
                                                }
                                            });
                                            $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 13)
                                                    $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                            });
                                        }

                                        //更新界面的扎数等数量信息
                                        $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                        $('#totalpcsdiv').html(result[0].TOTALPCS);
                                        $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);

                                        //扫描成功信息提示
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.querychangemessage + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    }
                                }
                                else if (data.d.substr(0, 5) == "false") {
                                    if (data.d == 'false1') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "1</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage2 + '1');
                                    }
                                    else if (data.d == 'false2') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage11 + "2</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage11 + '2');
                                    }
                                    else if (data.d == 'false3') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage5 + "3</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.confirmmessage5 + '3');
                                        buttonstatus = '2';
                                    }
                                    else if (data.d == 'false4') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage6 + "4</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.confirmmessage6 + '4');
                                        buttonstatus = '2';
                                    }
                                    else if (data.d == 'false5') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage7 + "5</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.confirmmessage7 + '5');
                                        buttonstatus = '2';
                                    }
                                    else if (data.d == 'false6') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.confirmmessage8 + "6</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.confirmmessage8 + '6');
                                        buttonstatus = '2';
                                    }
                                }
                                else if (data.d.substr(0, 5) == "error") {
                                    if (data.d == 'error1') {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.barcodescanmessage20 + "890</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.barcodescanmessage20 + "890");
                                    }
                                }
                                btnstatus();
                                $('#barcodetext').val("");
                                $('#barcodetext').focus();
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#navbar').show();
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "054</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "054");
                            }
                        });
                    }, 10);
                });

                //oas的选择监控
                $('input[name=radio]').change(function () {
                    var radioselect = $("input[name='radio']:checked").val();
                    if (radioselect == 'oasreceive') {
                        sessionStorage.setItem("oasreceive", "1");
                    } else {
                        sessionStorage.setItem("oasreceive", "0");
                    }
                });
            }
        });
    </script>
        <div data-role="header" data-theme="b" >
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
            <div data-role="navbar" id='navbar'>
                <div data-role="navbar">
                    <table width="100%">
                        <tr>
                            <td id="confirmbuttontd" style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="confirmbutton" name="confirmbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="emptybutton" name="emptybutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="savebutton" name="savebutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li>
                                        <select data-native-menu="false" id="querybutton" name="querybutton">
                                        </select>
                                    </li>
                                </ul>
                            </td>
                            <td style="width:60%">
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
            <div class="ui-grid-c">
                <div class="ui-block-a">
                    <%--<input type='checkbox' name='oascheckbox' id='oasreceive' value='oasreceive'/>
                    <label for='oasreceive' id='oasreceivelabel'>OAS Receive</label>--%>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="partselect" id="partlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="partselect" id="partselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="barcodetext" id="barcodelabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="barcodetext" name="barcodetext" type="text" placeholder="DOC_NO/Bundle/Carton" />
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="docnotext" id="docnolabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="docnotext" name="docnotext" type="text" placeholder="check DOC_NO" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="fromfactoryselect" id="fromfactoryselectlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="fromfactoryselect" id="fromfactoryselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="fromprocessselect" id="fromprocessselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="fromproductionlineselect" id="fromproductionlineselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="nextfactorytext" id="nextfactorylabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="nextfactoryselect" id="nextfactoryselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="nextprocessselect" id="nextprocessselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <select data-native-menu="false" name="nextproductionlineselect" id="nextproductionlineselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-d">
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
            </div>
            <table width="100%">
                <tr>
                    <td style="width: 1%">
                    </td>
                    <td style="width: 98%; text-align: left">
                        <table width="100%">
                            <tr>
                                <td style="width: 10%"><label id="barcodelistlabel"></label></td>
                                <td style = "width: 30%"></td>
                                <td style="width: 15%; text-align:right">
                                    <label for="totalbundlediv" id="totalbundlelabel" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="totalbundlediv"></div>
                                </td>
                                <td style="width: 15%; text-align:right">
                                    <label for="totalgarmentpcs" id="totalgarmentpcslabel" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="totalgarmentpcs"></div>
                                </td>
                                <td style="width: 15%; text-align:right">
                                    <label for="totalpcsdiv" id="totalpcslabel" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="totalpcsdiv"></div>
                                </td>
                            </tr>
                        </table>
                        <table class="stripe" id="barcodetabletitle" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="barcodelisttable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <br />
                        <div id="selector"></div>
                    </td>
                    <td style="width: 1%">
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
                <a id="receivea" href="" data-ajax="false"></a>
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

