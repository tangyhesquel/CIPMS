<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WIPtemp.aspx.cs" Inherits="WIPtemp" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/mobiscroll.custom-2.5.0.min.css" rel="stylesheet" type="text/css" />
    <script src="js/mobiscroll.custom-2.5.0.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
    <script src="js/jquery.chromatable.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
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
        .table-tr-1
        {
            background: #e9e9e9;
        }
        .table-tr-2
        {
            background: #FFBB1C;
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
                    $('#jolabel').html(globeltranslate.jolabel);
                    $('#golabel').html(globeltranslate.golabel);
                    $('#processlabel').html(globeltranslate.processlabel);
                    $('#bundlecheckboxlabel').html(globeltranslate.bundlecheckboxlabel);
                    $('#partcheckboxlabel').html(globeltranslate.partcheckboxlabel);
                    $('#colorsizecheckboxlabel').html(globeltranslate.colorsizecheckboxlabel);
                    $('#wiplabel').html(globeltranslate.wiplabel);
                    $('#linetablelabel').html(globeltranslate.linetablelabel);
                    $('#jotablelabel').html(globeltranslate.jotablelabel);
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
                    $('#querybutton').val(globeltranslate.querybutton).button('refresh');
                    $('#exportbutton').val(globeltranslate.exportbutton).button('refresh');
                    $('#docnoinquirya').html(globeltranslate.docnoinquirya);
                    $('#barcodeinquirya').html(globeltranslate.barcodeinquirya);
                    $('#wipandoutputlia').html(globeltranslate.wipandoutputlia);
                    select = globeltranslate.select;
                }
            });
        };

        function iprocess() {
            $.ajax({
                type: "POST",
                url: "Wipreport.aspx/Process",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function (data) {
                    $('#processselect').append(data.d);
                    $('#processselect').selectmenu('refresh');
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "009");
                }
            });
        };

        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                //语言选择
                if (window.localStorage.languageid == "1") {
                    window.language = "WIPreport/Wipreport-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "WIPreport/Wipreport-en-US.js";
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

                //表格样式
                var screenwidth = document.body.clientWidth;
                var screenheight = document.body.clientHeight;
                $('#wiptable').chromatable({
                    width: screenwidth * 0.96,
                    height: screenheight * 0.7,
                    scrolling: "yes"
                });
                $('#linetable').chromatable({
                    width: screenwidth * 0.96,
                    height: screenheight * 0.25,
                    scrolling: "yes"
                });
                $('#jotable').chromatable({
                    width: screenwidth * 0.96,
                    height: screenheight * 0.8,
                    scrolling: "yes"
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
                    $('#checkboxfieldset').hide();
                    $('#processalltable').show();
                    $('#processonetable1').hide();
                    $('#processonetable2').hide();
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
                                $.mobile.loading('hide');
                                //判断用户的权限按钮
                                var result = eval(data.d);
                                //判断用户的权限模块
                                Accessmoduleshow(data.d);
                            },
                            error: function (err) {
                                $.mobile.loading('hide');
                                alert(globeltranslate.errormessage + "002");
                            }
                        });
                    }, 10);

                    $('#cancelbutton').click(function () {
                        Loginout(fty, environment);
                    });

                    $('#querybutton').click(function () {
                        var bybundle = "";
                        var bypart = "";
                        var bycolorsize = "";
                        if ($('#bundlecheckbox').prop('checked') == true)
                            bybundle = "true";
                        else
                            bybundle = "false";
                        if ($('#partcheckbox').prop('checked') == true)
                            bypart = "true";
                        else
                            bypart = "false";
                        if ($('#colorsizecheckbox').prop('checked') == true)
                            bycolorsize = "true";
                        else
                            bycolorsize = "false";
                        $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                        $('#navbar').hide();
                        setTimeout(function () {
                            $.ajax({
                                type: "POST",
                                url: "Wipreport.aspx/Goquery",
                                contentType: "application/json;charset=utf-8",
                                //async: false,
                                dataType: "json",
                                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'joborderno': '" + $('#jotext').val() + "', 'go': '" + $('#gotext').val() + "', 'process': '" + $('#processselect').val() + "', 'bybundle': '" + bybundle + "', 'bypart': '" + bypart + "', 'bycolorsize': '" + bycolorsize + "' }",
                                success: function (data) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    var result = eval(data.d);
                                    $('#wiptable tbody tr').empty();
                                    $('#wiptable').trigger('create');
                                    $('#linetable tbody tr').empty();
                                    $('#linetable').trigger('create');
                                    $('#jotable tbody tr').empty();
                                    $('#jotable').trigger('create');
                                    if (result[0].TABLE2 == "") {
                                        $('#wiptable tbody tr').empty();
                                        $('#wiptable tbody').append(result[0].TABLE1);
                                        $('#wiptable').trigger('create');
                                        if ($('#jotext').val() == "" && $('#gotext').val() == "") {
                                            $("#wiptabletitle thead tr").empty();
                                            $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                            $('#wiptabletitle').trigger('create');
                                            $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 5) {
                                                    $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else {
                                            if (bybundle == "false" && bypart == "false" && bycolorsize == "false") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 7) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else if (bybundle == "true" && bypart == "false" && bycolorsize == "false") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 8) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else if (bybundle == "false" && bypart == "true" && bycolorsize == "false") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 8) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else if (bybundle == "false" && bypart == "false" && bycolorsize == "true") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 8) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else if (bybundle == "true" && bypart == "true" && bycolorsize == "false") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 9) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else if (bybundle == "false" && bypart == "true" && bycolorsize == "true") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 10) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else if (bybundle == "true" && bypart == "false" && bycolorsize == "true") {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 10) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                            else {
                                                $("#wiptabletitle thead tr").empty();
                                                if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                else {
                                                    $('#wiptabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table7 + "</th><th>" + globeltranslate.table8 + "</th><th>" + globeltranslate.table9 + "</th><th>" + globeltranslate.table10 + "</th><th>" + globeltranslate.table11 + "</th><th></th></tr>");
                                                }
                                                $('#wiptabletitle').trigger('create');
                                                $('#wiptabletitle thead').find('tr th').each(function (i) {
                                                    if (i <= 11) {
                                                        $(this).width($('#wiptable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                    }
                                                });
                                            }
                                        }
                                    }
                                    else {
                                        $('#linetable tbody tr').empty();
                                        $('#linetable tbody').append(result[0].TABLE1);
                                        $('#linetable').trigger('create');
                                        $('#jotable tbody tr').empty();
                                        $('#jotable tbody').append(result[0].TABLE2);
                                        $('#jotable').trigger('create');
                                        $("#linetabletitle thead tr").empty();
                                        if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                            $('#linetabletitle thead').append("<tr><th>" + globeltranslate.processlabel + "</th><th>" + globeltranslate.table17 + "</th><th>WIP</th><th></th></tr>");
                                        }
                                        else {
                                            $('#linetabletitle thead').append("<tr><th>" + globeltranslate.processlabel + "</th><th>" + globeltranslate.table14 + "</th><th>WIP</th><th></th></tr>");
                                        }
                                        $('#linetabletitle').trigger('create');
                                        $('#linetabletitle thead').find('tr th').each(function (i) {
                                            if (i <= 2) {
                                                $(this).width($('#linetable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                            }
                                        });
                                        if (bybundle == "false" && bypart == "false" && bycolorsize == "false") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 5) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else if (bybundle == "true" && bypart == "false" && bycolorsize == "false") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 6) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else if (bybundle == "false" && bypart == "true" && bycolorsize == "false") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 6) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else if (bybundle == "false" && bypart == "false" && bycolorsize == "true") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 7) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else if (bybundle == "true" && bypart == "true" && bycolorsize == "false") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 7) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else if (bybundle == "false" && bypart == "true" && bycolorsize == "true") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 8) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else if (bybundle == "true" && bypart == "false" && bycolorsize == "true") {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 8) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                        else {
                                            $("#jotabletitle thead tr").empty();
                                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table17 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            else {
                                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.table1 + "</th><th>" + globeltranslate.table14 + "</th><th>" + globeltranslate.table2 + "</th><th>" + globeltranslate.table3 + "</th><th>" + globeltranslate.table4 + "</th><th>" + globeltranslate.table12 + "</th><th>" + globeltranslate.table5 + "</th><th>" + globeltranslate.table15 + "</th><th>" + globeltranslate.table16 + "</th><th>WIP</th><th></th></tr>");
                                            }
                                            $('#jotabletitle').trigger('create');
                                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                                if (i <= 9) {
                                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                                }
                                            });
                                        }
                                    }
                                },
                                error: function (err) {
                                    $.mobile.loading('hide');
                                    $('#navbar').show();
                                    alert(globeltranslate.errormessage + "028");
                                }
                            });
                        }, 10);
                    });

                    $('#processselect').change(function () {
                        $("#wiptabletitle thead tr").empty();
                        $("#linetabletitle thead tr").empty();
                        $("#jotabletitle thead tr").empty();
                        $('#wiptable tbody tr').empty();
                        $('#jotable tbody tr').empty();
                        $('#linetable tbody tr').empty();
                        if ($(this).val() == 'all') {
                            if ($('#jotext').val() == "" && $('#gotext').val() == "") {
                                $('#checkboxfieldset').hide();
                                $("#bundlecheckbox").attr("checked", false).checkboxradio('refresh');
                                $("#partcheckbox").attr("checked", false).checkboxradio('refresh');
                                $("#colorsizecheckbox").attr("checked", false).checkboxradio('refresh');
                            }
                            $('#processalltable').show();
                            $('#processonetable1').hide();
                            $('#processonetable2').hide();
                        } else {
                            $('#checkboxfieldset').show();
                            $('#processalltable').hide();
                            $('#processonetable1').show();
                            $('#processonetable2').show();
                        }
                        if ($(this).val() == 'DC') {
                            $('#partcheckbox').hide();
                            $('#partcheckboxlabel').hide();
                            $("#partcheckbox").attr("checked", false).checkboxradio('refresh');
                        }
                        else {
                            $('#partcheckbox').show();
                            $('#partcheckboxlabel').show();
                        }
                    });

                    $('#jotext').bind('input propertychange', function () {
                        if ($('#processselect').val() == 'all') {
                            if ($(this).val() == "" && $('#gotext').val() == "") {
                                $('#checkboxfieldset').hide();
                                $("#bundlecheckbox").attr("checked", false).checkboxradio('refresh');
                                $("#partcheckbox").attr("checked", false).checkboxradio('refresh');
                                $("#colorsizecheckbox").attr("checked", false).checkboxradio('refresh');
                            } else {
                                $('#checkboxfieldset').show();
                            }
                        }
                    });

                    $('#gotext').bind('input propertychange', function () {
                        if ($('#processselect').val() == 'all') {
                            if ($(this).val() == "" && $('#jotext').val() == "") {
                                $('#checkboxfieldset').hide();
                                $("#bundlecheckbox").attr("checked", false).checkboxradio('refresh');
                                $("#partcheckbox").attr("checked", false).checkboxradio('refresh');
                                $("#colorsizecheckbox").attr("checked", false).checkboxradio('refresh');
                            } else {
                                $('#checkboxfieldset').show();
                            }
                        }
                    });
                }
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
            <div data-role="navbar" id="navbar">
                <div data-role="navbar">
                    <table width="100%">
                        <tr>
                            <td id="closeboxtd" style="width:10%">
                                <ul>
                                    <li>
                                        <input id="querybutton" name="querybutton" type="button" />
                                    </li>
                                </ul>
                            </td>
                            <td id="openboxtd" style="width:10%">
                                <ul>
                                    <li>
                                        <input id="exportbutton" name="exportbutton" type="button" />
                                    </li>
                                </ul>
                            </td>
                            <td style="width:10%">
                            </td>
                            <td style="width:10%">
                            </td>
                            <td style="width:10%">
                            </td>
                            <td style="width:10%">
                            </td>
                            <td style="width:10%">
                            </td>
                            <td style="width:10%"></td>
                            <td style="width:10%"></td>
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
                            <td style="width: 30%; text-align: center"><label for="jotext" id="jolabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="jotext" name="jotext" type="text" placeholder="JOB_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="gotext" id="golabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <input id="gotext" name="gotext" type="text" placeholder="GARMENT_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table width="100%">
                        <tr>
                            <td style="width: 30%; text-align: center"><label for="processselect" id="processlabel"></label></td>
                            <td style="width: 70%; text-align: right;">
                                <select id="processselect" name="processselect" data-native-menu="false">
                                    <option value="all" selected="selected">ALL(全部)</option>
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table width="100%">
                        <tr>
                            <td style="width: 10%; text-align: center"></td>
                            <td style="width: 70%; text-align: right;">
                                <fieldset data-role="controlgroup" id="checkboxfieldset">
                                    <label for="bundlecheckbox" id="bundlecheckboxlabel"></label>
                                    <input value='bundlecheckbox' id="bundlecheckbox" name="bundlecheckbox" type="checkbox"/>
                                    <label for="partcheckbox" id="partcheckboxlabel"></label>
                                    <input value='partcheckbox' id="partcheckbox" name="partcheckbox" type="checkbox"/>
                                    <label for="colorsizecheckbox" id="colorsizecheckboxlabel"></label>
                                    <input value='colorsizecheckbox' id="colorsizecheckbox" name="colorsizecheckbox" type="checkbox"/>
                                </fieldset>
                            </td>
                            <td style="width: 20%; text-align: center"></td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                </div>
                <div class="ui-block-d">
                </div>
            </div>

            <table width="100%" id="processalltable">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table width="100%">
                            <tr>
                                <td style="width: 10%"><label id="wiplabel"></label></td>
                                <td style="width: 90%"></td>
                            </tr>
                        </table>
                        <table class="stripe" id="wiptabletitle" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="wiptable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                    <td style="width: 3%">
                    </td>
                </tr>
            </table>
            <table width="100%" id="processonetable1">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table width="100%">
                            <tr>
                                <td style="width: 10%"><label id="linetablelabel"></label></td>
                                <td style="width: 90%"></td>
                            </tr>
                        </table>
                        <table class="stripe" id="linetabletitle" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="linetable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                    <td style="width: 3%">
                    </td>
                </tr>
            </table>
            <table width="100%" id="processonetable2">
                <tr>
                    <td style="width: 3%">
                    </td>
                    <td style="width: 94%; text-align: left">
                        <table width="100%">
                            <tr>
                                <td style="width: 10%"><label id="jotablelabel"></label></td>
                                <td style="width: 90%"></td>
                            </tr>
                        </table>
                        <table class="stripe" id="jotabletitle" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="jotable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                    <td style="width: 3%">
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
                    <li id="wipreportli"><a id="wipreporta" href="" data-ajax="false"></a></li>
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
