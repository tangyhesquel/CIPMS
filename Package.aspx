<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Package.aspx.cs" Inherits="Package" %>

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
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/Dialog.js" type="text/javascript"></script>
    <script src="js/jquery-barcode.js" type="text/javascript"></script>
    <script src="js/jquery.simplePagination.js" type="text/javascript"></script>
    <link href="css/simplePagination.css" rel="stylesheet" type="text/css" />
    <script src="js/qrcode.js" type="text/javascript"></script>
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
            .border2
            {
                background-color: Red !important;
            }
        </style>
        <script type="text/javascript">
            var fty = "";
            var environment = "";
            //0为按钮禁用，1为按钮可用
            var isprocess = "0";//0为在本部门，1为不在本部门
            var closecarton = '0';
            var opencarton = '0';
            var matchingcarton = '0';
            var docno = "";
            var globeltranslate; //全局变量，用于保存翻译的json字符串;
            var dbtrans = ""; //删除按钮的翻译
            var arr; //全局定义查询结果数组
            var pagenum;
            var currentpagenum = 1;
            var barcodetype = null;

            //打印参数
            var printhtml = "";
            var cartonbarcode = "";
            var pdbutton = "";
            var pdtitle = "";
            var printwidth = 1;
            var printheight = 40;

            //翻译语言函数
            window.language;
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
                        pdbutton = globeltranslate.pdbutton;
                        pdtitle = globeltranslate.pdtitle;
                        $('#headertitle').html(globeltranslate.headertitle);
                        $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                        $('#cancelbutton').html(globeltranslate.cancelbutton);
                        $('#messageth').html(globeltranslate.messageth);
                        $('#closeboxbutton').val(globeltranslate.closeboxbutton).button('refresh');
                        $('#openboxbutton').val(globeltranslate.openboxbutton).button('refresh');
                        $('#checkbutton').val(globeltranslate.checkbutton).button('refresh');
                        $('#printbutton').val(globeltranslate.printbutton).button('refresh');
                        $('#savebutton').val(globeltranslate.savebutton).button('refresh');
                        $('#emptylistbutton').val(globeltranslate.emptylistbutton).button('refresh');

                        $('#totalbundlelabel').html(globeltranslate.totalbundlelabel);
                        $('#totalpcslabel').html(globeltranslate.totalpcslabel);
                        $('#totalgarmentpcslabel').html(globeltranslate.totalgarmentpcslabel);
                        $('#totalpartlabel').html(globeltranslate.totalpartlabel);

                        $('#jotextlabel').html(globeltranslate.jotextlabel);
                        $('#partselectlabel').html(globeltranslate.partselectlabel);
                        $('#bundlebarcodelabel').html(globeltranslate.bundlebarcodelabel);
                        $('#cartonbarcodelabel').html(globeltranslate.cartonbarcodelabel);
                        $('#laynoselectlabel').html(globeltranslate.laynoselectlabel);
                        $('#colorselectlabel').html(globeltranslate.colorselectlabel);
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
                        $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);
                        dbtrans = globeltranslate.deletebutton;
                    }
                });
            };
            //select控件的翻译
            function queryselecttrans() {
                $('#querybutton').empty();
                $('#querybutton').append("<option>" + globeltranslate.querybutton + "</option>");
                $('#querybutton').selectmenu('refresh');
            }
            function partselecttrans() {
                $('#partselect').empty();
                $('#partselect').append("<option>" + globeltranslate.select + "</option>");
                $('#partselect').append("<option>" + "" + "</option>");
                $('#partselect').selectmenu('refresh');
            }
            function laynoselecttrans() {
                $('#laynoselect').empty();
                $('#laynoselect').append("<option>" + globeltranslate.select + "</option>");
                $('#laynoselect').append("<option>" + "" + "</option>");
                $('#laynoselect').selectmenu('refresh');
            }
            function colorselecttrans() {
                $('#colorselect').empty();
                $('#colorselect').append("<option>" + globeltranslate.select + "</option>");
                $('#colorselect').append("<option>" + "" + "</option>");
                $('#colorselect').selectmenu('refresh');
            }

            //表格里的删除按钮监控
            function deletebtn(obj) {
                var carton = $(obj).parents('tr').children('td').eq(0).text();
                if (carton == '') {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Delete",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            //async: false,
                            data: "{ 'docno': '" + docno + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'id': '" + $(obj).closest('tr').attr('id') + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                    var result = eval(data.d);
                                    if (result[0].ISPROCESS == "true" && result[0].ISSUBMIT == "false") {
                                        if (result[0].OPENCARTON == "true")
                                            opencarton = '1';
                                        else
                                            opencarton = '0';
                                        if (result[0].CLOSECARTON == "true")
                                            closecarton = '1';
                                        else
                                            closecarton = '0';
                                    } else {
                                        closecarton = '0';
                                        opencarton = '0';
                                    }
                                    //tangyh 2017.03.28
                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                    $('#totalpartdiv').html(result[0].TOTALPART);
                                    $('#totalpcsdiv').html(result[0].TOTALPCS);
                                    
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + $(obj).parents('tr').children('td').eq(2).text() + globeltranslate.s + $(obj).parents('tr').children('td').eq(3).text() + "</td></tr>");
                                    $('#messagetable').trigger('create');

                                    var html = result[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    html += packagetablelogic1(arr, currentpagenum, dbtrans);
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
                                            html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 12)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });

                                    btnstatus();
                                } else if (data.d == "error1") {
                                    alert(globeltranslate.errormessage2 + "211");
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "211</td></tr>");
                                    $('#messagetable').trigger('create');
                                } else if (data.d == "error2") {
                                    alert(globeltranslate.errormessage2 + "212");
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "212</td></tr>");
                                    $('#messagetable').trigger('create');
                                } else if (data.d == "error3") {
                                    alert(globeltranslate.errormessage2 + "213");
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "213</td></tr>");
                                    $('#messagetable').trigger('create');
                                } else if (data.d == "false1") {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + $(obj).parents('tr').children('td').eq(2).text() + globeltranslate.s + $(obj).parents('tr').children('td').eq(3).text() + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    empty();
                                }
                                //$(obj).closest('tr').remove();
                                //$('#barcodelisttable').trigger('create');
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "002");
                            }
                        });
                    }, 10);
                }
                else {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Deletecarton",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            //async: false,
                            data: "{ 'docno': '" + docno + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'cartonbarcode': '" + carton + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                    var result = eval(data.d);
                                    if (result[0].ISPROCESS == "true" && result[0].ISSUBMIT == "false") {
                                        if (result[0].OPENCARTON == "true")
                                            opencarton = '1';
                                        else
                                            opencarton = '0';
                                        if (result[0].CLOSECARTON == "true")
                                            closecarton = '1';
                                        else
                                            closecarton = '0';
                                    } else {
                                        closecarton = '0';
                                        opencarton = '0';
                                    }
                                    //tangyh 2017.03.28--
                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                    $('#totalpartdiv').html(result[0].TOTALPART);
                                    $('#totalpcsdiv').html(result[0].TOTALPCS);
                                    
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage2 + $(obj).parents('tr').children('td').eq(0).text() + "</td></tr>");
                                    $('#messagetable').trigger('create');

                                    var html = result[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    html += packagetablelogic1(arr, currentpagenum, dbtrans);
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
                                            html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 12)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });

                                    btnstatus();
                                } else if (data.d == "false1") {
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage2 + $(obj).parents('tr').children('td').eq(0).text() + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    empty();
                                }
                                //                                $('#barcodelisttable tr').each(function () {
                                //                                    if ($(this).children('td').eq(0).text() == carton)
                                //                                        $(this).remove();
                                //                                    $('#barcodelisttable').trigger('create');
                                //                                });
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "003</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "003");
                            }
                        });
                    }, 10);
                }
            }

            function empty() {
        
                if (sessionStorage.getItem("receivecheck") == '1') {
                    sessionStorage.setItem("receivecheck", "0");
                    $('#savebutton').removeAttr('disabled').button('refresh');
                    $('#querybutton').removeAttr("disabled");
                }
                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                setTimeout(function () {
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Emptylist",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        async: false,
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                        success: function (data) {
                            arr = [];
                            $('#selector').pagination({
                                cssStyle: 'light-theme',
                                prevText: globeltranslate.prevbutton,
                                nextText: globeltranslate.nextbutton
                            });
                            $('#bundlebarcodetext').focus();
                            btninit();
                            $('#barcodelisttable tr').empty();
                            $('#bundlebarcodetext').val("");
                            $('#cartonbarcodetext').val("");
                            $('#totalbundlediv').html("0");
                            $('#totalpcsdiv').html("0");
                            $('#totalgarmentpcs').html("0");
                            $('#laynoselect').val("");
                            $('#colorselect').val("");                          
                            $('#printbutton').attr('disabled', 'true').button('refresh');
                            //laynoselecttrans();
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.emptylistmessage + "</td></tr>");
                            $('#messagetable').trigger('create');
                            $.mobile.loading('hide');
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            alert(globeltranslate.clearmessage);
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.clearmessage + "</td></tr>");
                            $('#messagetable').trigger('create');
                        }
                    });
                }, 10);
            }

            function partselect() {
                //加载part select
                $.ajax({
                    type: "POST",
                    url: "Package.aspx/Part",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                    success: function (data) {
                        partselecttrans();
                        $('#partselect').append(data.d);
                        $('#partselect').selectmenu('refresh');
                        if (sessionStorage.getItem("receivecheck") == '1') {
                            var partlist = sessionStorage.getItem("part").split(',');
                            $(partlist).each(function (i) {
                                $("#partselect option[value='" + partlist[i] + "']").attr("selected", true);
                            });
                            $('#partselect').selectmenu('refresh');
                        }
                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.partselectmessage1 + "</td></tr>");
                        $('#messagetable').trigger('create');
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
                    url: "Package.aspx/Query",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'functioncd': 'Package' }",
                    success: function (data) {
                        queryselecttrans();
                        $('#querybutton').append(data.d);
                        $('#querybutton').selectmenu('refresh');
                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.querymessage1 + "</td></tr>");
                        $('#messagetable').trigger('create');
                    },
                    error: function (err) {
                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "006</td></tr>");
                        $('#messagetable').trigger('create');
                        alert(globeltranslate.errormessage + "006");
                    }
                });
            }

            //按钮初始化函数
            function btninit() {
                //初始化禁用打印按钮和箱子核查
                $('#printbutton').attr('disabled', 'true').button('refresh');
                $('#checkbutton').attr('disabled', 'true').button('refresh');
                //0代表禁用，1代表激活
                closecarton = '0';
                opencarton = '0';
                btnstatus();
            }

            //按钮状态改变函数
            function btnstatus() {
                if (closecarton == '0') {
                    //禁用关箱按钮
                    $('#closeboxbutton').attr('disabled', 'true').button('refresh');
                } else {
                    //激活关箱按钮
                    $('#closeboxbutton').removeAttr('disabled').button('refresh');
                }
                if (opencarton == '0') {
                    $('#openboxbutton').attr('disabled', 'true').button('refresh');
                } else {
                    $('#openboxbutton').removeAttr('disabled').button('refresh');
                }
                
            }

            function alertflash(obj) {
                var borderFlag = false;
                var div = $("#" + obj + "");
                var times = 0;
                blinkBorder();
                function blinkBorder() {
                    time = 0;
                    for (var i = 0; i < 2; i++) {
                        time += 300;
                        setTimeout(function () {
                            modifyBorder();
                        }, time);
                    }
                    times++;
                    if (times < 5)
                        setTimeout(blinkBorder, 600);
                    else
                        return;
                }
                function modifyBorder() {
                    borderFlag = !borderFlag;
                    if (borderFlag) {
                        div.addClass('border2');
                    }
                    else {
                        div.removeClass('border2');
                    }
                }
            }

            function trclick(obj) {
                alert('');
            }

            $(function () {
                fty = getUrlParam('FTY');
                environment = getUrlParam('svTYPE');
                if (fty == null || environment == null) {
                    alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
                } else {
                    var d = new Date();
                    var vYear = d.getFullYear()
                    var vMon = d.getMonth() + 1;
                    var vDay = d.getDate();
                    var h = d.getHours();
                    var m = d.getMinutes();
                    var se = d.getSeconds();
                    docno = sessionStorage.getItem("userbarcode") + vYear + vMon + vDay + h + m + se;

                    //语言选择
                    if (window.localStorage.languageid == "1") {
                        window.language = "Package/Package-zh-CN.js";
                    } else if (window.localStorage.languageid == "2") {
                        window.language = "Package/Package-en-US.js";
                    }
                    translate();

                    //function menu定义
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
                        //function权限控制
                        Accessmodulehide();
                        $('#closeboxtd').hide();
                        $('#openboxtd').hide();
                        $('#checktd').hide();
                        $('#printtd').hide();
                        $('#laynotr').hide();
                        $('#laynotrdiv').hide();
                        partselecttrans();
                        $('#totalbundlediv').html("0");
                        $('#totalpcsdiv').html("0");
                        $('#totalgarmentpcs').html("0");
                        btninit();
                        $('#printtest').hide();
                        var userbarcode = sessionStorage.getItem("userbarcode");
                        var factory = sessionStorage.getItem("factory");
                        var productionlinecd = sessionStorage.getItem("productionline");
                        $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + productionlinecd);
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Accessfunction",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                            success: function (data) {
                                //判断用户的权限按钮
                                var result = eval(data.d);
                                $(result).each(function (i) {
                                    if (result[i].FUNCTION_ENG == 'Close carton') $('#closeboxtd').show();
                                    else if (result[i].FUNCTION_ENG == 'Open carton') $('#openboxtd').show();
                                    else if (result[i].FUNCTION_ENG == 'Check carton') $('#checktd').show();
                                    else if (result[i].FUNCTION_ENG == 'Print carton') $('#printtd').show();
                                    else if (result[i].FUNCTION_ENG == 'Layno') { $('#laynotr').show(); $('#laynotrdiv').show(); }
                                });
                                //判断用户的权限模块
                                Accessmoduleshow(data.d);
                            },
                            error: function (err) {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "004</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "004");
                            }
                        });

                        laynoselecttrans();
                        colorselecttrans();
                        $('#bundlebarcodetext').focus();
                        partselect();
                        query();

                        if (sessionStorage.getItem("receivecheck") == '1' || sessionStorage.getItem("oasreceivecheck") == '1') {
                            $('#savebutton').attr('disabled', 'true').button('refresh');
                            $('#querybutton').attr("disabled", "disabled");
                            $('#cartonbarcodetext').val(sessionStorage.getItem("DOC_NO"));
                            $('#bundlebarcodetext').focus();
                            $('#checkbutton').removeAttr('disabled').button('refresh');
                            $('#printbutton').attr('disabled', 'true').button('refresh');
                            //更新cipms_user_scanning_dft并显示到UI上
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Receivecheckload",
                                    contentType: "application/json;charset=utf-8",
                                    async: false,
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'olddocno': '" + sessionStorage.getItem("docno") + "', 'newdocno': '" + docno + "', 'factory': '" + sessionStorage.getItem("peerfactory") + "', svTYPE'': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'dbtrans': '" + dbtrans + "' }",
                                    success: function (data) {
                                        var result = eval(data.d);
                                        $('#barcodelisttable tbody').after(result[0].HTML);
                                        $('#barcodelisttable').trigger('create');
                                        //tangyh 2017.03.28 --
                                        $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                        $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                        $('#totalpartdiv').html(result[0].TOTALPART);
                                        $('#totalpcsdiv').html(result[0].TOTALPCS);

                                        $.mobile.loading('hide');
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                    }
                                });
                            }, 10);
                        }

                    }

                    //翻译表头
                    $("#barcodetabletitle thead tr").empty();
                    $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th>" + globeltranslate.title7 + "</th><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title10 + "</th><th>" + globeltranslate.title11 + "</th><th>" + globeltranslate.title12 + "</th><th>" + globeltranslate.title13 + "</th><th style='text-align: center'></th></tr>");
                    $('#barcodetabletitle').trigger('create');

                    //注销按钮：返回登录页面
                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });

                    //表格样式
                    var screenwidth = $(document.body).width();
                    var screenheight = $(document.body).height();
                    $('#barcodelisttable').chromatable({
                        width: screenwidth * 0.95,
                        height: screenheight * 0.75,
                        scrolling: "yes"
                    });
                    $('#messagetable').chromatable({
                        width: screenwidth * 0.46,
                        height: screenheight * 0.26,
                        scrolling: "yes"
                    });

                    //bundlebarcodetext回车监控
                    var joborderno = ""
                    var partselected = "";
                    

                    $('#bundlebarcodetext').bind('keypress', function (event) {
                        if (event.keyCode == '13') {
                            //tangyh 2017.03.22
                            var get_othercarton;
                            
                            if ($("input[id='bundlebarcodeother']").is(':checked')) {
                                get_othercarton = "T";
                            }
                            else {
                                get_othercarton = "F";
                                  }

                            var barcode = $.trim($(this).val());
                            barcodetype = GetBarcodeType(barcode);
                            if (sessionStorage.getItem("receivecheck") == '1') {//Peer receive/ PRT receive
                                if ($('#partselect').val() == null) {//判断是否选择了PART
                                    alert(globeltranslate.partselectmessage2);
                                } else if (barcodetype != 'B') {//判断是否是裁片条码
                                    alert(globeltranslate.barcodemessage1);
                                } else {
                                    var temp = "";
                                    if (sessionStorage.getItem("oasreceivecheck") == '1')
                                        temp = "oas";
                                    else
                                        temp = "null";
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Package.aspx/checkscan",
                                            contentType: "application/json;charset=utf-8",
                                            async: false,
                                            dataType: "json",
                                            data: "{ 'temp': '" + temp + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'newdocno': '" + docno + "', 'factory': '" + sessionStorage.getItem("peerfactory") + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'bundlebarcode': '" + barcode + "', 'part': '" + $('#partselect').val() + "', 'dbtrans': '" + dbtrans + "', 'docnobarcode': '" + $('#cartonbarcodetext').val() + "', 'colorcode': '" + $('#colorselect').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                if (data.d == "false") {//该裁片条码已经被扫描过
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false1") {//该裁片条码不存在
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage1);
                                                } else if (data.d == "false5") {//用户只允许扫描该流水单里的裁片
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage5 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage5);
                                                } else if (data.d == "false7") {//color check --tangyh 2017.03.28
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.colormessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.colormessage1);
                                                } else if (data.d == "false8") {//裁片已经外发接收或没有外发
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage6 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage6);
                                                } else {
                                                    var result = eval(data.d);
                                                    $('#barcodelisttable tbody').after(result[0].HTML);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });
                                                    //tangyh 2013.03.28 --
                                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result[0].TOTALPART);
                                                    $('#totalpcsdiv').html(result[0].TOTALPCS);

                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage3 + barcode + globeltranslate.s + result[0].PART + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                }
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage);
                                            }
                                        });
                                    }, 10);
                                }
                                //非接收复核
                            }
                            else {
                                if (barcodetype == 'B') {
                                    // alert(globeltranslate.partselectmessage2);
                                    // alertflash('partselecttr');
                                    partselected = $('#partselect').val();
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Package.aspx/Bundlescan",
                                            contentType: "application/json;charset=utf-8",
                                            //async: false,
                                            dataType: "json",
                                            data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "', 'part': '" + $('#partselect').val() + "', 'jo': '" + $('#jotext').val() + "', 'dbtrans': '" + dbtrans + "', 'colorcode': '" + $('#colorselect').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                if (data.d.substr(0, 5) != "error" && data.d.substr(0, 5) != "false") {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.btnstatus[0].ISPROCESS == "true" && result.btnstatus[0].ISSUBMIT == "false") {
                                                        if (result.btnstatus[0].OPENCARTON == "true")
                                                            opencarton = '1';
                                                        else
                                                            opencarton = '0';
                                                        if (result.btnstatus[0].CLOSECARTON == "true")
                                                            closecarton = '1';
                                                        else
                                                            closecarton = '0';
                                                    } else {
                                                        closecarton = '0';
                                                        opencarton = '0';
                                                    }
                                                    joborderno = result.bundles[0].JO;

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += packagetablelogic3(arr, dbtrans);
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });

                                                    //tangyh 2017;03.28
                                                    $('#totalbundlediv').html(result.btnstatus[0].TOTALBUNDLES);
                                                    $('#totalgarmentpcs').html(result.btnstatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.btnstatus[0].TOTALPART);
                                                    $('#totalpcsdiv').html(result.btnstatus[0].TOTALPCS);

                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage3 + barcode + globeltranslate.s + result.bundles[0].PART + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    btnstatus();
                                                } else if (data.d.substr(0, 5) == "error") {
                                                    alert(globeltranslate.errormessage2 + "31" + data.d.substr(5));
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "11" + data.d.substr(5) + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage7);
                                                } else if (data.d == 'false2') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage7 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage7);
                                                } else if (data.d == 'false3') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage10 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage10);
                                                } else if (data.d == 'false4') {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage11 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage11);
                                                } else if (data.d == "false5") {
                                                    alert(globeltranslate.errormessage2 + "119");
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage2 + "119</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false6") {
                                                    alert(globeltranslate.bundlescanmessage1);
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                } else if (data.d == "false7") {//color check --tangyh 2017.03.28
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.colormessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.colormessage1);
                                                }
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "008</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "008");
                                            }
                                        });
                                    }, 10);
                                }
                                else if (barcodetype == 'C') { 
                                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                    setTimeout(function () {
                                        $.ajax({
                                            type: "POST",
                                            url: "Package.aspx/Cartonscan",
                                            contentType: "application/json;charset=utf-8",
                                            dataType: "json",
                                            //async: false,
                                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'dbtrans': '" + dbtrans + "', 'get_othercarton': '" + get_othercarton + "', 'colorcode': '" + $('#colorselect').val() + "' }",
                                            success: function (data) {
                                                $.mobile.loading('hide');
                                                if (data.d == "notexist") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage1);
                                                } else if (data.d == "empty") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage2 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage2);
                                                } else if (data.d == "opened") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage3 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.cartonscanmessage3);
                                                } else if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                                    var result = $.parseJSON(data.d);
                                                    if (result.btnstatus[0].ISPROCESS == "true" && result.btnstatus[0].ISSUBMIT == "false") {
                                                        if (result.btnstatus[0].OPENCARTON == "true")
                                                            opencarton = '1';
                                                        else
                                                            opencarton = '0';
                                                        if (result.btnstatus[0].CLOSECARTON == "true")
                                                            closecarton = '1';
                                                        else
                                                            closecarton = '0';
                                                    } else {
                                                        closecarton = '0';
                                                        opencarton = '0';
                                                    }
                                                    btnstatus();

                                                    var html = result.bundles[0].HTML;
                                                    if (html == '') {
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        return;
                                                    }
                                                    arr = html.split('@');
                                                    pagenum = Math.ceil(arr.length / 10);
                                                    html = "";
                                                    $('#barcodelisttable tr').empty();
                                                    html += packagetablelogic3(arr, dbtrans);
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');

                                                    $('#selector').pagination({
                                                        items: arr.length,
                                                        itemsOnPage: 10,
                                                        cssStyle: 'light-theme',
                                                        prevText: globeltranslate.prevbutton,
                                                        nextText: globeltranslate.nextbutton,
                                                        onPageClick: function (pagenumber, event) {
                                                            currentpagenum = pagenumber;
                                                            $('#barcodelisttable tbody').empty();
                                                            html = "";
                                                            html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                                            $('#barcodelisttable tbody').append(html);
                                                            $('#barcodelisttable').trigger('create');
                                                        }
                                                    });

                                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                        if (i <= 12)
                                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    });
                                                    //tangyh 2017;03.28
                                                    $('#totalbundlediv').html(result.btnstatus[0].TOTALBUNDLES);
                                                    $('#totalgarmentpcs').html(result.btnstatus[0].TOTALGARMENTPCS);
                                                    $('#totalpartdiv').html(result.btnstatus[0].TOTALPART);
                                                    $('#totalpcsdiv').html(result.btnstatus[0].TOTALPCS);

                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.cartonscanmessage4 + barcode + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    laynoselecttrans();
                                                    colorselecttrans();
                                                } else if (data.d == "false1") {
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage9 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.bundlescanmessage9);
                                                } else if (data.d == "false7") {//color check --tangyh 2017.03.28
                                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.colormessage1 + "</td></tr>");
                                                    $('#messagetable').trigger('create');
                                                    alert(globeltranslate.colormessage1);
                                                }
                                                $('#bundlebarcodetext').val("");
                                            },
                                            error: function (err) {
                                                $.mobile.loading('hide');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "011</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.errormessage + "011");
                                            }
                                        });
                                    }, 10);
                                }
                            }
                            $('#bundlebarcodetext').val("");
                            $('#bundlebarcodetext').focus();
                           // $('#printbutton').removeAttr('disabled').button('refresh');
                            //$('#printbutton').attr('disabled',false).button('refresh');
                        }
                    });

                    $('#laynoselect').change(function () {
                        if (($('#laynoselect').val() == "") || ($('#laynoselect').val() == "--选择--"))
                            return;
                        if ($('#jotext').val() != "")
                            joborderno = $('#jotext').val();
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: 'Package.aspx/Laynobundle',
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                //async: false,
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'layno': '" + $('#laynoselect').val() + "', 'joborderno': '" + joborderno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'part': '" + $('#partselect').val() + "', 'dbtrans': '" + dbtrans + "', 'colorcd': '" + $('#colorselect').val() + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                        var result = $.parseJSON(data.d);
                                        if (result.btnstatus[0].ISPROCESS == "true" && result.btnstatus[0].ISSUBMIT == "false") {
                                            if (result.btnstatus[0].OPENCARTON == "true")
                                                opencarton = '1';
                                            else
                                                opencarton = '0';
                                            if (result.btnstatus[0].CLOSECARTON == "true")
                                                closecarton = '1';
                                            else
                                                closecarton = '0';
                                        } else {
                                            closecarton = '0';
                                            opencarton = '0';
                                        }
                                        btnstatus();

                                        var html = result.bundles[0].HTML;
                                        if (html == '') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            return;
                                        }
                                        arr = html.split('@');
                                        pagenum = Math.ceil(arr.length / 10);
                                        html = "";
                                        $('#barcodelisttable tr').empty();
                                        html += packagetablelogic3(arr, dbtrans);
                                        $('#barcodelisttable tbody').append(html);
                                        $('#barcodelisttable').trigger('create');

                                        $('#selector').pagination({
                                            items: arr.length,
                                            itemsOnPage: 10,
                                            cssStyle: 'light-theme',
                                            prevText: globeltranslate.prevbutton,
                                            nextText: globeltranslate.nextbutton,
                                            onPageClick: function (pagenumber, event) {
                                                currentpagenum = pagenumber;
                                                $('#barcodelisttable tbody').empty();
                                                html = "";
                                                html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                                $('#barcodelisttable tbody').append(html);
                                                $('#barcodelisttable').trigger('create');
                                            }
                                        });

                                        $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                            if (i <= 12)
                                                $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                        });
                                        //tangyh 2017.03.28
                                        $('#totalbundlediv').html(result.btnstatus[0].TOTALBUNDLES);
                                        $('#totalgarmentpcs').html(result.btnstatus[0].TOTALGARMENTPCS);
                                        $('#totalpartdiv').html(result.btnstatus[0].TOTALPART);
                                        $('#totalpcsdiv').html(result.btnstatus[0].TOTALPCS);
                                        
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage3 + globeltranslate.jo + ":" + joborderno + "," + globeltranslate.layno + ":" + $('#laynoselect').val() + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage2 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false12") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage4 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.laynoselectmessage4);
                                    } else if (data.d == "false4") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false13") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage6 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false11") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage7 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    }
                                    $('#bundlebarcodetext').focus();
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "012</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "012");
                                }
                            });
                        }, 10);
                    });

                    $('#colorselect').change(function () {
                        //alert($('#laynoselect').val());
                        if (($('#laynoselect').val() == "") || ($('#laynoselect').val() == "--选择--"))
                            return;
                        if ($('#jotext').val() != "")
                            joborderno = $('#jotext').val();
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: 'Package.aspx/Laynobundle',
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                //async: false,
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'layno': '" + $('#laynoselect').val() + "', 'joborderno': '" + joborderno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'part': '" + $('#partselect').val() + "', 'dbtrans': '" + dbtrans + "', 'colorcd': '" + $('#colorselect').val() + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                        var result = $.parseJSON(data.d);
                                        if (result.btnstatus[0].ISPROCESS == "true" && result.btnstatus[0].ISSUBMIT == "false") {
                                            if (result.btnstatus[0].OPENCARTON == "true")
                                                opencarton = '1';
                                            else
                                                opencarton = '0';
                                            if (result.btnstatus[0].CLOSECARTON == "true")
                                                closecarton = '1';
                                            else
                                                closecarton = '0';
                                        } else {
                                            closecarton = '0';
                                            opencarton = '0';
                                        }
                                        btnstatus();

                                        var html = result.bundles[0].HTML;
                                        if (html == '') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            return;
                                        }
                                        arr = html.split('@');
                                        pagenum = Math.ceil(arr.length / 10);
                                        html = "";
                                        $('#barcodelisttable tr').empty();
                                        html += packagetablelogic3(arr, dbtrans);
                                        $('#barcodelisttable tbody').append(html);
                                        $('#barcodelisttable').trigger('create');

                                        $('#selector').pagination({
                                            items: arr.length,
                                            itemsOnPage: 10,
                                            cssStyle: 'light-theme',
                                            prevText: globeltranslate.prevbutton,
                                            nextText: globeltranslate.nextbutton,
                                            onPageClick: function (pagenumber, event) {
                                                currentpagenum = pagenumber;
                                                $('#barcodelisttable tbody').empty();
                                                html = "";
                                                html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                                $('#barcodelisttable tbody').append(html);
                                                $('#barcodelisttable').trigger('create');
                                            }
                                        });

                                        $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                            if (i <= 12)
                                                $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                        });
                                        //tangyh 2017.03.28
                                        $('#totalbundlediv').html(result.btnstatus[0].TOTALBUNDLES);
                                        $('#totalgarmentpcs').html(result.btnstatus[0].TOTALGARMENTPCS);
                                        $('#totalpartdiv').html(result.btnstatus[0].TOTALPART);
                                        $('#totalpcsdiv').html(result.btnstatus[0].TOTALPCS);

                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage3 + globeltranslate.jo + ":" + joborderno + "," + globeltranslate.layno + ":" + $('#laynoselect').val() + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage2 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false12") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage4 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.laynoselectmessage4);
                                    } else if (data.d == "false4") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage5 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false13") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage6 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false11") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage7 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    }
                                    $('#bundlebarcodetext').focus();
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "012</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "012");
                                }
                            });
                        }, 10);
                    });

                    //Check按钮监控
                    $('#checkbutton').click(function () {
                        if (sessionStorage.getItem("receivecheck") == '1') {//接收复核
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Receivecheck",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + sessionStorage.getItem("peerfactory") + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'cartonbarcode': '" + $('#cartonbarcodetext').val() + "', 'translation': '" + globeltranslate.s + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        var result = $.parseJSON(data.d);
                                        var totalqty1 = result.lack[0].qty;
                                        var totalbundle1 = result.lack[0].bundle;
                                        var detail1 = result.lack[0].detail;
                                        var alertmessage = globeltranslate.less + " \n " + globeltranslate.bundles + totalbundle1 + " \n " + globeltranslate.pcs + totalqty1 + " \n " + globeltranslate.detail + detail1;

                                        //弹窗让用户选择是否需要接收
                                        $.MsgBox.Confirm(globeltranslate.dialogmessage3, alertmessage, globeltranslate.dialogmessage1, globeltranslate.dialogmessage2, function () {
                                            //返回接收界面，激活接收按钮
                                            sessionStorage.setItem("checkresult", result.CHECKRESULT[0].checkresult);
                                            sessionStorage.setItem("receivecheck", "2");
                                            sessionStorage.setItem("docno", docno);
                                            window.location.href = "Receive.aspx?FTY=" + fty;
                                        }, function () {
                                        });
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                    }
                                });
                            }, 10);
                        } else {//非接收复核
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Check",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'cartonbarcode': '" + $('#cartonbarcodetext').val() + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        var result = $.parseJSON(data.d);
                                        var totalqty1 = result.more[0].qty;
                                        var totalbundle1 = result.more[0].bundle;
                                        var detail1 = result.more[0].detail;
                                        var totalqty2 = result.less[0].qty;
                                        var totalbundle2 = result.less[0].bundle;
                                        var detail2 = result.less[0].detail;
                                        alert(globeltranslate.more + " \n " + globeltranslate.bundles + totalbundle1 + "  " + globeltranslate.pcs + totalqty1 + " \n " + globeltranslate.detail + detail1 + " \n " + " \n " + globeltranslate.less + " \n " + globeltranslate.bundles + totalbundle2 + "  " + globeltranslate.pcs + totalqty2 + " \n " + globeltranslate.detail + detail2);
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.more + globeltranslate.bundles + totalbundle1 + " " + globeltranslate.pcs + totalqty1 + " " + globeltranslate.detail + detail1 + "</td></tr>");
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.less + globeltranslate.bundles + totalbundle2 + " " + globeltranslate.pcs + totalqty2 + " " + globeltranslate.detail + detail2 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "014</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "014");
                                    }
                                });
                            }, 10);
                        }
                    });

                    //关箱按钮监控
                    $('#closeboxbutton').click(function () {
                        //关箱方式1：使用旧箱码
                        var barcode = $.trim($('#cartonbarcodetext').val());
                        barcodetype = GetBarcodeType(barcode);
                        if (barcodetype == 'C') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Closecarton",
                                    contentType: "application/json;charset=utf-8",
                                    async: false,
                                    dataType: "json",
                                    data: "{ 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'cartonbarcode': '" + $('#cartonbarcodetext').val() + "' }",
                                    async: false,
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                            empty();
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage4 + $('#cartonbarcodetext').val() + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.closecartonmessage4 + $('#cartonbarcodetext').val());
                                        }
                                        else if (data.d.substr(0, 5) == 'false') {
                                            if (data.d == 'false1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage1 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.closecartonmessage1);
                                            }
                                            else if (data.d == 'false2') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage2 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.closecartonmessage2);
                                            }
                                            else if (data.d == 'false3') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage3 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.closecartonmessage3);
                                            }
                                            else if (data.d == 'false4') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage13 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.closecartonmessage13);
                                            }
                                            else if (data.d == 'false5') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage14 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.closecartonmessage14);
                                            }
                                            else if (data.d == 'false6') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage12 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.closecartonmessage12);
                                            }
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "030</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "030");
                                    }
                                });
                            }, 10);
                        }
                        //关箱方式2：条码输入框为空，系统生成新的箱码并且打印
                        else if ($('#cartonbarcodetext').val() == "") {
                            //先判断是否已经关过主箱码。
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Checkfirstmaincarton",
                                    contentType: "application/json;charset=utf-8",
                                    //async: false,
                                    dataType: "json",
                                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        if (data.d != 'false') {//第一次关主箱码，则弹窗让用户确认
                                            var cartonpart = eval(data.d);
                                            $.MsgBox.Confirm(globeltranslate.dialogmessage3, globeltranslate.dialogmessage7 + cartonpart[0].CLOSEPART + globeltranslate.dialogmessage8 + globeltranslate.dialogmessage11 + cartonpart[0].REMAINPART + globeltranslate.dialogmessage12, globeltranslate.dialogmessage9, globeltranslate.dialogmessage10, function () {
                                                var languagearray = globeltranslate.customer + "*" + globeltranslate.date + "*" + globeltranslate.process + "*" + globeltranslate.production + "*" + globeltranslate.part + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.color + "*" + globeltranslate.cutqty + "*" + globeltranslate.layno + "*" + globeltranslate.bundle + "*" + globeltranslate.totalqty + "*" + globeltranslate.cartonbarcode + "*" + globeltranslate.sewline + "*" + globeltranslate.ttlbundle + "*" + globeltranslate.shortpart;
                                                //tangyh 2017.04.03
                                                languagearray = languagearray + "*" + globeltranslate.prt + "*" + globeltranslate.emb + "*" + globeltranslate.embafterprt + "*" + globeltranslate.prtsp;

                                                var temp = "";
                                                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                                setTimeout(function () {
                                                    $.ajax({
                                                        type: "POST",
                                                        url: "Package.aspx/Closenewcarton",
                                                        contentType: "application/json;charset=utf-8",
                                                        //async: false,
                                                        dataType: "json",
                                                        data: "{ 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'production': '" + productionlinecd + "', 'languagearray': '" + languagearray + "' }",
                                                        success: function (data) {
                                                            $.mobile.loading('hide');
                                                            if (data.d == 'false3') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage3 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage3);
                                                            } else if (data.d == 'false1') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage9 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage9);
                                                            } else if (data.d == 'false2') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage10 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage10);
                                                            } else if (data.d == 'false4') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage11 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage11);
                                                            } else if (data.d == 'false8') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage6 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage6);
                                                            } else if (data.d == 'false9') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage7 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage7);
                                                            } else if (data.d == 'false6') {
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage8 + "</td></tr>");
                                                                $('#messagetable').trigger('create');
                                                                alert(globeltranslate.closecartonmessage8);
                                                            } else {
                                                                var result = eval(data.d);
                                                                printhtml = result[0].HTML;
                                                                cartonbarcode = result[0].CARTON;
                                                                $('#jumpprint').trigger('click');
                                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage5 + result[0].CARTON + "</td></tr>");
                                                                empty();
                                                            }
                                                        },
                                                        error: function (err) {
                                                            $.mobile.loading('hide');
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "155</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.errormessage + "155");
                                                        }
                                                    });
                                                }, 10);
                                            }, function () {
                                                //用户取消操作则清空扫描表
                                                empty();
                                            });
                                        } else {
                                            var languagearray = globeltranslate.customer + "*" + globeltranslate.date + "*" + globeltranslate.process + "*" + globeltranslate.production + "*" + globeltranslate.part + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.color + "*" + globeltranslate.cutqty + "*" + globeltranslate.layno + "*" + globeltranslate.bundle + "*" + globeltranslate.totalqty + "*" + globeltranslate.cartonbarcode + "*" + globeltranslate.sewline + "*" + globeltranslate.ttlbundle + "*" + globeltranslate.shortpart;
                                            //tangyh 2017.04.03
                                            languagearray = languagearray + "*" + globeltranslate.prt + "*" + globeltranslate.emb + "*" + globeltranslate.embafterprt + "*" + globeltranslate.prtsp;

                                            var temp = "";
                                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                            setTimeout(function () {
                                                $.ajax({
                                                    type: "POST",
                                                    url: "Package.aspx/Closenewcarton",
                                                    contentType: "application/json;charset=utf-8",
                                                    //async: false,
                                                    dataType: "json",
                                                    data: "{ 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'production': '" + productionlinecd + "', 'languagearray': '" + languagearray + "' }",
                                                    success: function (data) {
                                                        $.mobile.loading('hide');
                                                        if (data.d == 'false3') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage3 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage3);
                                                        } else if (data.d == 'false1') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage9 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage9);
                                                        } else if (data.d == 'false2') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage10 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage10);
                                                        } else if (data.d == 'false4') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage11 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage11);
                                                        } else if (data.d == 'false8') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage6 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage6);
                                                        } else if (data.d == 'false9') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage7 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage7);
                                                        } else if (data.d == 'false6') {
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage8 + "</td></tr>");
                                                            $('#messagetable').trigger('create');
                                                            alert(globeltranslate.closecartonmessage8);
                                                        } else {
                                                            var result = eval(data.d);
                                                            printhtml = result[0].HTML;
                                                            cartonbarcode = result[0].CARTON;
                                                            $('#jumpprint').trigger('click');
                                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.closecartonmessage5 + result[0].CARTON + "</td></tr>");
                                                            empty();
                                                        }
                                                    },
                                                    error: function (err) {
                                                        $.mobile.loading('hide');
                                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "031</td></tr>");
                                                        $('#messagetable').trigger('create');
                                                        alert(globeltranslate.errormessage + "031");
                                                    }
                                                });
                                            }, 10);
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "081</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "081");
                                    }
                                });
                            }, 10);
                        }
                    });

                    //开箱按钮监控
                    $('#openboxbutton').click(function () {
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Package.aspx/Opencarton",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'docno': '" + docno + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "' }",
                                success: function (data) {
                                    if (data.d == 'false3') {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.opencartonmessage1 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.opencartonmessage1);
                                    } else if (data.d == 'false2') {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.opencartonmessage2 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.opencartonmessage2);
                                    } else if (data.d == 'false1') {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.opencartonmessage4 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.opencartonmessage4);
                                    } else if (data.d == 'false9') {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.opencartonmessage4 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.opencartonmessage4);
                                    } else {
                                        empty();
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.opencartonmessage3 + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.opencartonmessage3);
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "098</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "098");
                                }
                            });
                        }, 10);
                    });

                    //打印按钮监控
                    $('#printbutton').click(function () {
                        var barcode = $.trim($('#cartonbarcodetext').val());
                        barcodetype = GetBarcodeType(barcode);
                        var languagearray = globeltranslate.customer + "*" + globeltranslate.date + "*" + globeltranslate.process + "*" + globeltranslate.production + "*" + globeltranslate.part + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.color + "*" + globeltranslate.cutqty + "*" + globeltranslate.layno + "*" + globeltranslate.bundle + "*" + globeltranslate.totalqty + "*" + globeltranslate.cartonbarcode + "*" + globeltranslate.sewline + "*" + globeltranslate.ttlbundle + "*" + globeltranslate.shortpart;
                        //tangyh 2017.03.27
                        languagearray = languagearray + "*" + globeltranslate.prt + "*" + globeltranslate.emb + "*" + globeltranslate.embafterprt + "*" + globeltranslate.prtsp;

                        if (barcodetype == 'C') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Reprint",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + $('#cartonbarcodetext').val() + "', 'part': 'null', 'languagearray': '" + languagearray + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        if (data.d.substr(0, 5) != 'false') {
                                            var result = eval(data.d);
                                            printhtml = result[0].HTML;
                                            cartonbarcode = result[0].CARTON;
                                            $('#jumpprint').trigger('click');
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage1 + result[0].CARTON + "</td></tr>");
                                        } else {
                                            if (data.d == 'false1') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage2 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.reprintmessage2);
                                            } else if (data.d == 'false2') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage4 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.reprintmessage4);
                                            } else if (data.d == 'false3') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.laynoselectmessage1 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.laynoselectmessage1);
                                            }
                                        }
                                        $('#cartonbarcodetext').val("");
                                        $('#printbutton').attr('disabled', 'true').button('refresh');
                                        $('#checkbutton').attr('disabled', 'true').button('refresh');
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "061</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "061");
                                    }
                                });
                            }, 10);
                        } else if (barcodetype == 'B') {
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Package.aspx/Reprint",
                                    contentType: "application/json;charset=utf-8",
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'barcode': '" + $('#cartonbarcodetext').val() + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'part': '" + $('#partselect').val() + "', 'languagearray': '" + languagearray + "' }",
                                    success: function (data) {
                                        $.mobile.loading('hide');
                                        if (data.d == "false") {
                                            $('#cartonbarcodetext').val("");
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.partselectmessage2 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.partselectmessage2);
                                        } else if (data.d == "false2") {
                                            $('#cartonbarcodetext').val("");
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage3 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.reprintmessage3);
                                        } else {
                                            var result = eval(data.d);
                                            printhtml = result[0].HTML;
                                            cartonbarcode = result[0].CARTON;
                                            $('#jumpprint').trigger('click');
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.reprintmessage1 + result[0].CARTON + "</td></tr>");
                                        }
                                        $('#cartonbarcodetext').val("");
                                        $('#printbutton').attr('disabled', 'true').button('refresh');
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "062</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "062");
                                    }
                                });
                            }, 10);
                        }
                    });

                    //Save按钮监控
                    $('#savebutton').click(function () {
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Package.aspx/Save",
                                contentType: "application/json;charset=utf-8",
                                async: false,
                                dataType: "json",
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'module':'Package' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.savemessage1 + data.d + "</td></tr>");
                                    $('#messagetable').trigger('create');
                                    query();
                                    alert(globeltranslate.savemessage1 + data.d);
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "044</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "044");
                                }
                            });
                        }, 10);
                    });

                    //Query选择改变监控
                    $('#querybutton').change(function () {
                        var date = $(this).val();
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Package.aspx/Querychange",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                //async: false,
                                data: "{ 'docno': '" + docno + "', 'date': '" + date + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'functioncd': 'Package', 'dbtrans': '" + dbtrans + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    if (data.d.substr(0, 5) != 'false' && data.d.substr(0, 5) != 'error') {
                                        var result = $.parseJSON(data.d);
                                        if (result.btnstatus[0].ISPROCESS == "true") {
                                            if (result.btnstatus[0].OPENCARTON == "true")
                                                opencarton = '1';
                                            else
                                                opencarton = '0';
                                            if (result.btnstatus[0].CLOSECARTON == "true")
                                                closecarton = '1';
                                            else
                                                closecarton = '0';
                                        } else {
                                            closecarton = '0';
                                            opencarton = '0';
                                        }
                                        btnstatus();

                                        var html = result.bundles[0].HTML;
                                        if (html == '') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.bundlescanmessage1 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            return;
                                        }
                                        arr = html.split('@');
                                        pagenum = Math.ceil(arr.length / 10);
                                        html = "";
                                        $('#barcodelisttable tr').empty();
                                        html += packagetablelogic3(arr, dbtrans);
                                        $('#barcodelisttable tbody').append(html);
                                        $('#barcodelisttable').trigger('create');

                                        $('#selector').pagination({
                                            items: arr.length,
                                            itemsOnPage: 10,
                                            cssStyle: 'light-theme',
                                            prevText: globeltranslate.prevbutton,
                                            nextText: globeltranslate.nextbutton,
                                            onPageClick: function (pagenumber, event) {
                                                currentpagenum = pagenumber;
                                                $('#barcodelisttable tbody').empty();
                                                html = "";
                                                html += packagetablelogic2(arr, currentpagenum, dbtrans);
                                                $('#barcodelisttable tbody').append(html);
                                                $('#barcodelisttable').trigger('create');
                                            }
                                        });

                                        $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                            if (i <= 12)
                                                $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                        });
                                        // tangyh 2017.03.28 --
                                        $('#totalbundlediv').html(result.btnstatus[0].TOTALBUNDLES);
                                        $('#totalgarmentpcs').html(result.btnstatus[0].TOTALGARMENTPCS);
                                        $('#totalpartdiv').html(result.btnstatus[0].TOTALPART);
                                        $('#totalpcsdiv').html(result.btnstatus[0].TOTALPCS);

                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.querymessage3 + date + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    } else if (data.d == "false") {
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.querymessage2 + date + "</td></tr>");
                                        $('#messagetable').trigger('create');
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "054</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "054");
                                }
                            });
                        }, 10);
                    });

                    //清空按钮监控
                    $('#emptylistbutton').click(function () {
                        empty();
                        partselecttrans();
                        partselect();
                        laynoselecttrans();
                        colorselecttrans();
                        $('#jotext').val("");
                    });

                    //JO输入框回车监控
                    $('#jotext').bind('keypress', function (event) {
                        if (event.keyCode == '13') {
                            var jo = $('#jotext').val();
                            if (jo == '') {
                                alert(globeltranslate.joinputmessage1);
                            } else {
                                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                                setTimeout(function () {
                                    $.ajax({
                                        type: "POST",
                                        url: "Package.aspx/Getpartlayno",
                                        contentType: "application/json;charset=utf-8",
                                        dataType: "json",
                                        //async: false,
                                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'jo': '" + jo + "' }",
                                        success: function (data) {
                                            $.mobile.loading('hide');
                                            if (data.d == 'false') {
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.joinputmessage2 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                                alert(globeltranslate.joinputmessage2);
                                            } else {
                                                var result = eval(data.d);
                                                partselecttrans();
                                                //$('#laynoselect').append("");
                                                $('#partselect').append(result[0].PARTHTML);
                                                $('#partselect').selectmenu('refresh');
                                                laynoselecttrans();
                                                colorselecttrans();
                                                //$('#laynoselect').append("");
                                                $('#laynoselect').append(result[0].LAYNOHTML);
                                                $('#laynoselect').selectmenu('refresh');
                                                //tangyh 2017.03.27
                                                colorselecttrans();
                                                //$('#colorselect').append("");
                                                $('#colorselect').append(result[0].COLORHTML);
                                                $('#colorselect').selectmenu('refresh');

                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.partselectmessage1 + "</td></tr>");
                                                $('#messagetable').trigger('create');
                                            }
                                        },
                                        error: function (err) {
                                            $.mobile.loading('hide');
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "084</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.errormessage + "084");
                                        }
                                    });
                                }, 10);
                            }
                        }
                    });

                    //箱码输入监控
                    $('#cartonbarcodetext').bind('input propertychange', function () {
                        var barcode = $.trim($('#cartonbarcodetext').val());
                        barcodetype = GetBarcodeType(barcode);
                        if (barcodetype == 'B') {
                            $('#printbutton').removeAttr('disabled').button('refresh');
                            $('#checkbutton').attr('disabled', 'true').button('refresh');
                        } else if (barcodetype == 'C') {
                            $('#printbutton').removeAttr('disabled').button('refresh');
                            $('#checkbutton').removeAttr('disabled').button('refresh');
                        } else if (barcodetype == 'D') {
                            $('#checkbutton').removeAttr('disabled').button('refresh');
                            $('#printbutton').attr('disabled', 'true').button('refresh');
                        } else {
                            $('#printbutton').attr('disabled', 'true').button('refresh');
                            $('#checkbutton').attr('disabled', 'true').button('refresh');
                        }
                    });

                    //table的行双击defect事件
                    $('#barcodelisttable tr').click(function () {
                        alert('hah');
                    });

                    //滚动条监控
                    //                    var multiple = 0;
                    //                    var timer;
                    //                    $('#barcodelisttablewrapper').scroll(function () {
                    //                        $('#jotext').val($(this).scrollTop() + $(this).height());
                    //                        $('#cartonbarcodetext').val($('#barcodelisttable').height());
                    //                        if (($(this).scrollTop() + $(this).height()) > $('#barcodelisttable').height()) {
                    //                            clearInterval(timer);
                    //                            timer = setTimeout(function () {
                    //                                alert('hehe');
                    //                            }, 0);
                    //                        } else if (($(this).scrollTop() + $(this).height()) == $('#barcodelisttable').height()) {
                    //                            clearInterval(timer);
                    //                            timer = setTimeout(function () {
                    //                                alert('haha');
                    //                            }, 0);
                    //                        }
                    //                    });
                }
            });
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
            <div data-role="navbar" >
                <div data-role="navbar">
                    <table width="100%">
                        <tr>
                            <td id="closeboxtd" style="width:10%">
                                <ul>
                                    <li>
                                        <input type="button" style="text-align:center" id="closeboxbutton" name="closeboxbutton" />
                                    </li>
                                </ul>
                            </td>
                            <td id="openboxtd" style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="openboxbutton" name="openboxbutton" /></li>
                                </ul>
                            </td>
                            <td id="checktd" style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="checkbutton" name="checkbutton" /></li>
                                </ul>
                            </td>
                            <td id="printtd" style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="printbutton" name="printbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="emptylistbutton" name="emptylistbutton" /></li>
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
                            <td style="width:10%"></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
            <div class="ui-grid-c">
                <div class="ui-block-a">
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="jotext" id="jotextlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="jotext" name="jotext" type="text" placeholder="JOB_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr id="partselecttr">
                            <td style="width:30%; text-align:center"><label for="partselect" id="partselectlabel"></label></td>
                            <td style="width:70%">
                                <select data-native-menu="false" multiple="multiple" name="partselect" id="partselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="bundlebarcodetext" id="bundlebarcodelabel"></label></td>
                            <!--tangyh 2017.03.22-->
                            <td style="width: 65%; text-align: right;">
                                <input id="bundlebarcodetext" name="bundlebarcodetext" type="text" placeholder="Bundle/Carton"/>     
                            </td>
                            
                            <td style="width: 5%; text-align: right;">
                                <input value='1' id="bundlebarcodeother" name="bundlebarcodeother" type="checkbox"/>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table style="width:100%">
                        <tr id="laynotr">
                            <td style="width:30%; text-align:center"><label for="laynoselect" id="laynoselectlabel"></label></td>
                            <td style="width:70%">
                                <select data-native-menu="false" name="laynoselect" id="laynoselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                   <%-- --tangyh 2017.03.27--%>
                    <table style="width:100%">
                        <tr id="colortr">
                            <td style="width:30%; text-align:center"><label for="colorselect" id="colorselectlabel"></label></td>
                            <td style="width:70%">
                                <select data-native-menu="false" name="colorselect" id="colorselect">
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="cartonbarcodetext" id="cartonbarcodelabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="cartonbarcodetext" name="cartonbarcodetext" type="text" placeholder="Check/Reprint/Reuse"/>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 3%"></td>
                            <td style="width: 94%; text-align: center">
                                <table width="100%" id="messagetable" border="0" cellspacing="0" cellpadding="0">
                                    <thead>
                                        <tr>
                                            <th style="text-align: center" id="messageth">
                                            </th>
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
                <div class="ui-block-d">
                </div>
            </div>
            <table style="width:100%">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table style="width:100%">
                            <tr>
                                <td style="width: 10%"><label id="barcodelistlabel"></label></td>
                                <td style = "width: 10%"></td>

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
                               <%-- <--tangyh 2017.03.28>--%>
                                <td style="width: 15%; text-align:right">
                                    <label for="totalpartdiv" id="totalpartlabel" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="totalpartdiv"></div>
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
                            <tbody id="barcodelisttbody">
                            </tbody>
                        </table>
                        <br />
                        <div id="selector"></div>
                    </td>
                    <td style="width: 3%">
                    </td>
                </tr>
            </table>
            <div id="printtest" style="width:7.0cm;font-size:8px; margin-left:0.2cm">
                <a id='jumpprint' data-role='button' href='Printpage.aspx' data-rel='dialog'>hide</a>
            </div>
        </div>
        
        <div data-role="footer"  data-position="fixed" data-fullscreen="true">
        </div>
    </div>
    <nav id="my-menu">
        <ul>
            <li id="packageli">
                <a id="packagea" href="" data-ajax="false"></a>
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
