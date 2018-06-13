<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Matching.aspx.cs" Inherits="Matching" %>

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
        </style>
        <script type="text/javascript">
            var fty = "";
            var environment = "";
            //0为按钮禁用，1为按钮可用
            var isprocess = "0";//0为在本部门，1为不在本部门
            var matchingcarton = '0';
            var docno = "";
            var globeltranslate; //全局变量，用于保存翻译的json字符串;
            var dbtrans = ""; //删除按钮的翻译
            var arr; //全局定义查询结果数组
            var pagenum;
            var currentpagenum = 1;

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
                        dbtrans = globeltranslate.deletebutton;
                        $('#headertitle').html(globeltranslate.headertitle);
                        $('#functionmenubutton').html(globeltranslate.functionmenubutton);
                        $('#cancelbutton').html(globeltranslate.cancelbutton);
                        $('#messageth').html(globeltranslate.messageth);
                        $('#matchingbutton').val(globeltranslate.matchingbutton).button('refresh');
                        $('#savebutton').val(globeltranslate.savebutton).button('refresh');
                        $('#emptylistbutton').val(globeltranslate.emptylistbutton).button('refresh');
                        $('#totalbundlelabel').html(globeltranslate.totalbundlelabel);
                        $('#totalgarmentpcslabel').html(globeltranslate.totalgarmentpcslabel);
                        $('#totalpcslabel').html(globeltranslate.totalpcslabel);
                        $('#partselectlabel').html(globeltranslate.partselectlabel);
                        $('#bundlebarcodelabel').html(globeltranslate.bundlebarcodelabel);
                        $('#cartonbarcodelabel').html(globeltranslate.cartonbarcodelabel);
                        $('#laynoselectlabel').html(globeltranslate.laynoselectlabel);
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
                    }
                });
            }

            //select控件的翻译
            function queryselecttrans() {
                $('#querybutton').empty();
                $('#querybutton').append("<option>" + globeltranslate.querybutton + "</option>");
                $('#querybutton').selectmenu('refresh');
            }

            function query() {
                $.ajax({
                    type: "POST",
                    url: "Package.aspx/Query",
                    contentType: "application/json;charset=utf-8",
                    dataType: "json",
                    data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'functioncd': 'Matching' }",
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

            function empty() {
                $('#bundlebarcodetext').val("");
                $('#bundlebarcodetext').focus();
                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                setTimeout(function () {
                    $.ajax({
                        type: "POST",
                        url: "Package.aspx/Emptylist",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        //async: false,
                        data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "' }",
                        success: function (data) {
                            arr = [];
                            $('#selector').pagination({
                                cssStyle: 'light-theme',
                                prevText: globeltranslate.prevbutton,
                                nextText: globeltranslate.nextbutton
                            });
                            $('#matchingbutton').attr('disabled', 'true').button('refresh');
                            $('#bundlebarcodetext').focus();
                            $('#barcodelisttable tr').empty();
                            $('#bundlebarcodetext').val("");
                            $('#totalbundlediv').html("0");
                            $('#totalgarmentpcs').html("0");
                            $('#totalpcsdiv').html("0");
                            $('#totalpartdiv').html("0");

                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td></td></tr>");
                            $('#messagetable').trigger('create');
                            $.mobile.loading('hide');
                        },
                        error: function (err) {
                            $.mobile.loading('hide');
                            alert(globeltranslate.errormessage);
                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td></td></tr>");
                            $('#messagetable').trigger('create');
                        }
                    });
                }, 10);
            }

            //表格里的删除按钮监控
            function deletebtn(obj) {
                var carton = $(obj).parents('tr').children('td').eq(0).text();
                var bundle = $(obj).parents('tr').children('td').eq(2).text();
                if (carton == '') {
                    $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "Matching.aspx/Delete",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'docno': '" + docno + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'bundleno': '" + $(obj).closest('tr').children('td').eq(2).text() + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d == "false") {
                                    empty();
                                } else {
                                    var result = eval(data.d);
                                    if (result[0].ISMATCHING == 'true') {
                                        $('#matchingbutton').removeAttr('disabled').button('refresh');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage6 + "</td></tr>");
                                    } else {
                                        $('#matchingbutton').attr('disabled', 'true').button('refresh');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage5 + "</td></tr>");
                                    }
                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                    $('#totalpcsdiv').html(result[0].TOTALPCS);
                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                    var html = result[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    if (arr.length >= currentpagenum * 10) {
                                        for (var i = (currentpagenum - 1) * 10; i <= (currentpagenum - 1) * 10 + 9; i++) {
                                            var arrtemp = arr[i].split('|');
                                            html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                        }
                                    }
                                    else {
                                        for (var i = (currentpagenum - 1) * 10; i < arr.length; i++) {
                                            var arrtemp = arr[i].split('|');
                                            html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                        }
                                    }
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
                                            if ((pagenumber - 1) * 10 + 9 > (arr.length - 1)) {
                                                for (var i = (pagenumber - 1) * 10; i < arr.length; i++) {
                                                    var arrtemp = arr[i].split('|');
                                                    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                }
                                            } else {
                                                for (var i = (pagenumber - 1) * 10; i <= (pagenumber - 1) * 10 + 9; i++) {
                                                    var arrtemp = arr[i].split('|');
                                                    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                }
                                            }
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });

                                }
//                                $('#barcodelisttable tr').each(function () {
//                                    if ($(this).children('td').eq(2).text() == bundle)
//                                        $(this).remove();
//                                    $('#barcodelisttable').trigger('create');
//                                });
//                                $('#barcodelisttable').trigger('create');
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage1 + bundle + "</td></tr>");
                                $('#messagetable').trigger('create');
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
                            url: "Matching.aspx/Deletecarton",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'docno': '" + docno + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'cartonbarcode': '" + carton + "' }",
                            success: function (data) {
                                $.mobile.loading('hide');
                                if (data.d == "false") {
                                    empty();
                                } else {
                                    var result = eval(data.d);
                                    if (result[0].ISMATCHING == 'true') {
                                        $('#matchingbutton').removeAttr('disabled').button('refresh');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage6 + "</td></tr>");
                                    } else {
                                        $('#matchingbutton').attr('disabled', 'true').button('refresh');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage5 + "</td></tr>");
                                    }
                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                    $('#totalpcsdiv').html(result[0].TOTALPCS);
                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                    $('#totalpartdiv').html(result.bundlestatus[0].TOTALPART);

                                    var html = result[0].HTML;
                                    arr = html.split('@');
                                    pagenum = Math.ceil(arr.length / 10);
                                    if (currentpagenum > pagenum) {
                                        currentpagenum = pagenum;
                                    }
                                    html = "";
                                    $('#barcodelisttable tr').empty();
                                    if (arr.length >= currentpagenum * 10) {
                                        for (var i = (currentpagenum - 1) * 10; i <= (currentpagenum - 1) * 10 + 9; i++) {
                                            var arrtemp = arr[i].split('|');
                                            html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                        }
                                    }
                                    else {
                                        for (var i = (currentpagenum - 1) * 10; i < arr.length; i++) {
                                            var arrtemp = arr[i].split('|');
                                            html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                        }
                                    }
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
                                            if ((pagenumber - 1) * 10 + 9 > (arr.length - 1)) {
                                                for (var i = (pagenumber - 1) * 10; i < arr.length; i++) {
                                                    var arrtemp = arr[i].split('|');
                                                    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                }
                                            } else {
                                                for (var i = (pagenumber - 1) * 10; i <= (pagenumber - 1) * 10 + 9; i++) {
                                                    var arrtemp = arr[i].split('|');
                                                    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                }
                                            }
                                            $('#barcodelisttable tbody').append(html);
                                            $('#barcodelisttable').trigger('create');
                                        }
                                    });
                                }
//                                $('#barcodelisttable tr').each(function () {
//                                    if ($(this).children('td').eq(0).text() == carton)
//                                        $(this).remove();
//                                    $('#barcodelisttable').trigger('create');
//                                });
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.deletemessage2 + carton + "</td></tr>");
                                $('#messagetable').trigger('create');
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
                        window.language = "Matching/Matching-zh-CN.js";
                    } else if (window.localStorage.languageid == "2") {
                        window.language = "Matching/Matching-en-US.js";
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
                        queryselecttrans();
                        $('#bundlebarcodetext').focus();
                        $('#totalbundlediv').html("0");
                        $('#totalgarmentpcs').html("0");
                        $('#totalpcsdiv').html("0");
                        $('#totalpartdiv').html("0");

                        $('#printtest').hide();
                        $('#matchingbutton').attr('disabled', 'true').button('refresh');
                        var userbarcode = sessionStorage.getItem("userbarcode");
                        var factory = sessionStorage.getItem("factory");
                        var productionlinecd = sessionStorage.getItem("productionline");
                        $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + productionlinecd);
                        $.ajax({
                            type: "POST",
                            url: "Package.aspx/Accessfunction",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode': '" + userbarcode + "' }",
                            success: function (data) {
                                //判断用户的权限模块
                                Accessmoduleshow(data.d);
                            },
                            error: function (err) {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "004</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "004");
                            }
                        });
                        query();
                    }


                    //注销按钮：返回登录页面
                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });

                    //翻译表头
                    $("#barcodetabletitle thead tr").empty();
                    $('#barcodetabletitle thead').append("<tr><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th>" + globeltranslate.title7 + "</th><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title10 + "</th><th>" + globeltranslate.title11 + "</th><th>" + globeltranslate.title12 + "</th><th>" + globeltranslate.title13 + "</th><th style='text-align: center'></th></tr>");
                    $('#barcodetabletitle').trigger('create');

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
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'module': 'Matching' }",
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
                                url: "Matching.aspx/Querychange",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'docno': '" + docno + "', 'date': '" + date + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'functioncd': 'Matching', 'dbtrans': '" + globeltranslate.deletebutton + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    var result = eval(data.d);
                                    if (result[0].ISMATCHING == 'true') {
                                        $('#matchingbutton').removeAttr('disabled').button('refresh');
                                    } else {
                                        $('#matchingbutton').attr('disabled', 'true').button('refresh');
                                    }
                                    var html = result[0].HTML;
                                    $('#barcodelisttable tbody').after(html);
                                    $('#barcodelisttable').trigger('create');

                                    $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                        if (i <= 12)
                                            $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    });
                                    $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                    $('#totalpcsdiv').html(result[0].TOTALPCS);
                                    $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                    $('#totalpartdiv').html(result[0].totalpartdiv);

                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td></td></tr>");
                                    $('#messagetable').trigger('create');
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
                    });

                    //Matching按钮监控
                    $('#matchingbutton').click(function () {
                        var languagearray = globeltranslate.customer + "*" + globeltranslate.date + "*" + globeltranslate.process + "*" + globeltranslate.production + "*" + globeltranslate.part + "*" + globeltranslate.summary + "*" + globeltranslate.jo + "*" + globeltranslate.color + "*" + globeltranslate.cutqty + "*" + globeltranslate.layno + "*" + globeltranslate.bundle + "*" + globeltranslate.totalqty + "*" + globeltranslate.cartonbarcode + "*" + globeltranslate.sewline + "*" + globeltranslate.ttlbundle + "*" + globeltranslate.shortpart;
                        //tangyh 2017.04.03
                        languagearray = languagearray + "*" + globeltranslate.prt + "*" + globeltranslate.emb + "*" + globeltranslate.embafterprt + "*" + globeltranslate.prtsp;

                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        $('#navbar').hide();
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Matching.aspx/Matchingmethod",
                                contentType: "application/json;charset=utf-8",
                                dataType: "json",
                                data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'docno': '" + docno + "', 'process': '" + sessionStorage.getItem("process") + "', 'languagearray': '" + languagearray + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    if (data.d.substr(0, 5) != 'false') {
                                        var result = eval(data.d);
                                        alert(globeltranslate.matchingmessage3 + result[0].CARTON);
                                        printhtml = result[0].HTML;
                                        cartonbarcode = result[0].CARTON;
                                        $('#jumpprint').trigger('click');
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage3 + result[0].CARTON + "</td></tr>");
                                        empty();
                                    } else {
                                        if (data.d == "false2") {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage1 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.matchingmessage1);
                                        } 
                                        else if (data.d == 'false4') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage7 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.matchingmessage7);
                                        } 
                                        else if (data.d == "false5") {
                                            empty();
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage3 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.matchingmessage3);
                                        } 
                                        else if (data.d == "false3") {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage2 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.matchingmessage2);
                                        }
                                        else if (data.d == 'false6') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage8 + "970</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.matchingmessage8 + "970");
                                        }
                                        else if (data.d == 'false7') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage9 + "971</td></tr>");
                                            $('#messagetable').trigger('create');
                                            alert(globeltranslate.matchingmessage9 + "971");
                                        }
                                    }
                                },
                                error: function () {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "013</td></tr>");
                                    $('#messagetable').trigger('create');
                                    alert(globeltranslate.errormessage + "013");
                                }
                            });
                        }, 10);
                    });

                    //扫描回车
                    $('#bundlebarcodetext').bind('keypress', function (event) {
                        if (event.keyCode == '13') {
                            var barcode = $(this).val();
                            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                            setTimeout(function () {
                                $.ajax({
                                    type: "POST",
                                    url: "Matching.aspx/Bundlescan",
                                    contentType: "application/json;charset=utf-8",
                                    //async: false,
                                    dataType: "json",
                                    data: "{ 'userbarcode': '" + sessionStorage.getItem("userbarcode") + "', 'docno': '" + docno + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + sessionStorage.getItem("process") + "', 'barcode': '" + barcode + "', 'dbtrans': '" + globeltranslate.deletebutton + "' }",
                                    success: function (data) {
                                        $('#bundlebarcodetext').val("");
                                        $('#bundlebarcodetext').focus();
                                        $.mobile.loading('hide');
                                        if (data.d == 'false4') {
                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage4 + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                        } else {
                                            var result = eval(data.d);
                                            if (result[0].ISMATCHING == 'true' || result[0].ISSUBMIT == 'true') {
                                                $('#matchingbutton').removeAttr('disabled').button('refresh');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage6 + "</td></tr>");
                                            } else if (result[0].ISMATCHING == "false") {
                                                $('#matchingbutton').attr('disabled', 'true').button('refresh');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage5 + "</td></tr>");
                                            } else if (result[0].ISMATCHING == "false2") {
                                                $('#matchingbutton').attr('disabled', 'true').button('refresh');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage4 + "</td></tr>");
                                            } else if (result[0].ISMATCHING == "false3") {
                                                $('#matchingbutton').attr('disabled', 'true').button('refresh');
                                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage7 + "</td></tr>");
                                            }

                                            var html = result[0].HTML;
                                            arr = html.split('@');
                                            pagenum = Math.ceil(arr.length / 10);
                                            html = "";
                                            $('#barcodelisttable tr').empty();
                                            if (arr.length > 10) {
                                                for (var i = 0; i < 10; i++) {
                                                    var arrtemp = arr[i].split('|');
                                                    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                }
                                            }
                                            else {
                                                for (var i = 0; i < arr.length; i++) {
                                                    var arrtemp = arr[i].split('|');
                                                    html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                    html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                    html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                }
                                            }
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
                                                    if ((pagenumber - 1) * 10 + 9 > (arr.length - 1)) {
                                                        for (var i = (pagenumber - 1) * 10; i < arr.length; i++) {
                                                            var arrtemp = arr[i].split('|');
                                                            html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                        }
                                                    } else {
                                                        for (var i = (pagenumber - 1) * 10; i <= (pagenumber - 1) * 10 + 9; i++) {
                                                            var arrtemp = arr[i].split('|');
                                                            html += "<tr id='" + arrtemp[0] + "'onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[1] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[2] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:11%;'>" + arrtemp[3] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[4] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:10%;'>" + arrtemp[5] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:8%;'>" + arrtemp[6] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[7] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[8] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:4%;'>" + arrtemp[9] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[10] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[11] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[12] + "</td>";
                                                            html += "<td style='vertical-align:middle; text-align:center; width:5%;'>" + arrtemp[13] + "</td>";
                                                            html += "<td><input type='button' onclick='deletebtn(this)' value='" + dbtrans + "' /></td></td></tr>";
                                                        }
                                                    }
                                                    $('#barcodelisttable tbody').append(html);
                                                    $('#barcodelisttable').trigger('create');
                                                }
                                            });

                                            $('#barcodetabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 12)
                                                    $(this).width($('#barcodelisttable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                            });

                                            $('#totalbundlediv').html(result[0].TOTALBUNDLES);
                                            $('#totalpcsdiv').html(result[0].TOTALPCS);
                                            $('#totalgarmentpcs').html(result[0].TOTALGARMENTPCS);
                                            $('#totalpartdiv').html(result[0].TOTALPART);

                                            $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.matchingmessage1 + barcode + "</td></tr>");
                                            $('#messagetable').trigger('create');
                                        }
                                    },
                                    error: function (err) {
                                        $.mobile.loading('hide');
                                        $('#bundlebarcodetext').val("");
                                        $('#bundlebarcodetext').focus();
                                        $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "065</td></tr>");
                                        $('#messagetable').trigger('create');
                                        alert(globeltranslate.errormessage + "065");
                                    }
                                });
                            }, 10);
                        }
                    });
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
            <div data-role="navbar" id='navbar'>
                <div data-role="navbar">
                    <table width="100%">
                        <tr>
                            <td id="matchingtd" style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="matchingbutton" name="matchingbutton" /></li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li><input type="button" style="text-align:center" id="emptylistbutton" name="emptylistbutton" /></li>
                                </ul>
                            </td>
                            <%--<td style="width:10%">
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
                            </td>--%>
                            <td style="width:60%"></td>
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
                            <td style="width: 30%; text-align: center"><label for="bundlebarcodetext" id="bundlebarcodelabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="bundlebarcodetext" name="bundlebarcodetext" type="text" placeholder="Bundle/Carton"/>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                </div>
                <div class="ui-block-c">
                    <table width="100%">
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
            <table width="100%">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table width="100%">
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
                <a id="packagea" href="Package.aspx" onclick="packagejump(fty, environment);return false;" data-ajax="false"></a>
            </li>
            <li id="matchingli">
                <a id="matchinga" href="" data-ajax="false"></a>
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
