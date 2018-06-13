<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Outputreport.aspx.cs" Inherits="Outputreport" %>

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
    <script src="Scripts/kendo.all.min.js" type="text/javascript"></script>
    <script src="Scripts/jszip.min.js" type="text/javascript"></script>
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
                    $('#jolabel').html(globeltranslate.jotext);
                    $('#golabel').html(globeltranslate.gotext);
                    $('#colorcheckboxlabel').html(globeltranslate.colorcheckboxlabel);
                    $('#bundlecheckboxlabel').html(globeltranslate.bundlecheckboxlabel);
                    $('#processlabel').html(globeltranslate.processlabel);
                    $('#nextprocesslabel').html(globeltranslate.nextprocesslabel);
                    $('#prtcheckboxlabel').html(globeltranslate.prtcheckboxlabel);
                    $('#embcheckboxlabel').html(globeltranslate.embcheckboxlabel);
                    $('#matchingcheckboxlabel').html(globeltranslate.matchingcheckboxlabel);
                    $('#dccheckboxlabel').html(globeltranslate.dccheckboxlabel);
                    $('#fromdatelabel').html(globeltranslate.fromdatelabel);
                    $('#todatelabel').html(globeltranslate.todatelabel);
                    $('#jobutton').val(globeltranslate.jobutton).button('refresh');
                    $('#outputlabel').html(globeltranslate.outputlabel);
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
                    $('#docnoinquirya').html(globeltranslate.docnoinquirya);
                    $('#barcodeinquirya').html(globeltranslate.barcodeinquirya);
                    $('#wipandoutputlia').html(globeltranslate.wipandoutputlia);
                    $('#addfield22').html(globeltranslate.addfield22);
                    $('#addfield24').html(globeltranslate.addfield24);
                    select = globeltranslate.select;
                    $('#printsparebarcodea').html(globeltranslate.printsparebarcodea);  
                    $('#exportexcelbutton').val(globeltranslate.exportexcelbutton).button('refresh');
                    $('#exportexcelbutton_1').val(globeltranslate.exportexcelbutton).button('refresh');
                }
            });
        };

        function nextprocessselecttrans() {
            $('#nextprocessselect').empty();
            $('#nextprocessselect').append("<option value='all' selected='selected'>ALL(全部)</option>");
            $('#nextprocessselect').selectmenu('refresh');
        }

        function AddField(i) {
            //$('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4+fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
            var str = "";
            if (i == 0) {
                str = "";
            }
            if (i == 1) {
                str = str + "<th>" + globeltranslate.title1 + "</th>";
            }
            if (i == 2) {//by Sew
                str = str + "<th>" + globeltranslate.title18 + "</th>";
                str = str + "<th>" + globeltranslate.title9 + "</th>";
            }
            if (i == 3) {
                str = str + "<th>" + globeltranslate.title1 + "</th>";
                str = str + "<th>" + globeltranslate.title18 + "</th>";
                str = str + "<th>" + globeltranslate.title9 + "</th>";
            }
            if (i == 4) {
                str = str + "<th>" + globeltranslate.title9 + "</th>";
                str = str + "<th>" + globeltranslate.title20 + "</th>";
                str = str + "<th>" + globeltranslate.title21 + "</th>";
            }
            if (i == 5) {
                str = str + "<th>" + globeltranslate.title1 + "</th>";
                str = str + "<th>" + globeltranslate.title20 + "</th>";
                str = str + "<th>" + globeltranslate.title21 + "</th>";
            }
            if (i == 6) {
                str = str + "<th>" + globeltranslate.title18 + "</th>";
                str = str + "<th>" + globeltranslate.title9 + "</th>";
                str = str + "<th>" + globeltranslate.title20 + "</th>";
                str = str + "<th>" + globeltranslate.title21 + "</th>";
            }
            if (i == 7) {
                str = str + "<th>" + globeltranslate.title1 + "</th>";
                str = str + "<th>" + globeltranslate.title18 + "</th>";
                str = str + "<th>" + globeltranslate.title9 + "</th>";
                str = str + "<th>" + globeltranslate.title20 + "</th>";
                str = str + "<th>" + globeltranslate.title21 + "</th>";
            }

            return str;
        }

        

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
                    $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
                    $('#messagetable').trigger('create');
                    alert(globeltranslate.errormessage + "002");
                }
            });
        };

        function fusecell(fty) {
            if (fty == 'YMG')
                return "</th><th>" + globeltranslate.title4a;
            else
                return "";

        }


        //$('#jobutton').click(function () {
        function JObutton_click_inquiry() {


            var addi = 0;
            if (document.getElementById("bundlecheckbox").checked) {
                addi = addi + 1;
            }
            if (document.getElementById("add1checkbox").checked) {
                addi = addi + 2;
            }
            if (document.getElementById("add3checkbox").checked) {
                addi = addi + 4;
            }


            if ($('#processselect').val() != 'all') {
                $('#linetablelabel').html(globeltranslate.linetablelabel + '( ' + $('#processselect').val() + '-->' + $('#nextprocessselect').val() + ' )');
                $('#jotablelabel').html(globeltranslate.jotablelabel + '( ' + $('#processselect').val() + '-->' + $('#nextprocessselect').val() + ' )');
            }
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
            var bypart = "";
            if ($('#bundlecheckbox').prop('checked') == true)
                bypart = "true";
            else
                bypart = "false";
            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
            $('#outputtable tbody tr').empty();
            $('#linetable tbody tr').empty();
            $('#jotable tbody tr').empty();

            $.ajax({
                type: "POST",
                url: "Outputreport.aspx/Getoutput",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'jo': '" + $('#jotext').val() + "', 'go': '" + $('#gotext').val() + "', 'process': '" + $('#processselect').val() + "', 'nextprocess': '" + $('#nextprocessselect').val() + "', 'fromdate': '" + $('#fromdate').val() + "', 'todate': '" + $('#todate').val() + "', 'bypart': '" + bypart + "', 'addi': " + addi + " }",
                success: function (data) {
                    $.mobile.loading('hide');
                    if (data.d == "nodata") {

                        return;
                    }
                    if (data.d == "false1") {
                        alert(globeltranslate.querymessage6);
                    }
                    var result = eval(data.d);
                    if (result[0].TABLE2 == "") {
                        //$('#jotable').hide();
                        //$('#jotabletitle').hide();
                        //$('#jotablelabel').hide();
                        //$('#jotablewrapper').hide();

                        //$("#processalltable").css("height", "85%");

                        $('#outputtable tbody tr').empty();
                        $('#outputtable tbody').append(result[0].TABLE1);
                        $('#outputtable').trigger('create');
                        if (bydate == "true" && bypart == "false") {
                            if ($('#jotext').val() == "" && $('#gotext').val() == "") {
                                $("#outputtabletitle thead tr").empty();
                                $('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                                $('#outputtabletitle').trigger('create');
                                $('#outputtabletitle thead').find('tr th').each(function (i) {
                                    if (i <= 5) {
                                        $(this).width($('#outputtable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    }
                                });
                            }
                            else {
                                $("#outputtabletitle thead tr").empty();
                                $('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                                $('#outputtabletitle').trigger('create');
                                $('#outputtabletitle thead').find('tr th').each(function (i) {
                                    if (i <= 5) {
                                        $(this).width($('#outputtable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    }
                                });
                            }
                        }
                        else if (bydate == "true" && bypart == "true") {
                            if ($('#jotext').val() == "" && $('#gotext').val() == "") {
                                $("#outputtabletitle thead tr").empty();
                                $('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                                $('#outputtabletitle').trigger('create');
                                $('#outputtabletitle thead').find('tr th').each(function (i) {
                                    if (i <= 6) {
                                        $(this).width($('#outputtable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    }
                                });
                            }
                            else {
                                if (result[0].TABLE1 != "") {
                                    $('#linetable tbody tr').empty();
                                    $('#linetable tbody').append(result[0].TABLE1);
                                    $('#linetable').trigger('create');
                                }
                                $("#linetabletitle thead tr").empty();
                                //$('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4+fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                                var s;
                                if ($('#processselect').val() == 'all')
                                    s = "<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>";
                                else
                                    s = "<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.processlabel + "</th><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>";
                                $('#linetabletitle thead').append(s);

                                //$('#linetabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                                $('#linetabletitle').trigger('create');
                                $('#linetable thead').find('tr th').each(function (i) {
                                    if (i <= 6) {
                                        $(this).width($('#linetabletitle tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    }
                                });
                            }
                        }
                        else if (bydate == "false" && bypart == "false") {
                            if ($('#jotext').val() != "" || $('#gotext').val() != "") {
                                $("#outputtabletitle thead tr").empty();
                                $('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title9 + "</th>" + AddField(addi) + "<th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                                $('#outputtabletitle').trigger('create');
                                $('#outputtabletitle thead').find('tr th').each(function (i) {
                                    if (i <= 5) {
                                        $(this).width($('#outputtable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                    }
                                });
                            }
                        }
                        else {
                            $("#outputtabletitle thead tr").empty();
                            $('#outputtabletitle thead').append("<tr><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title2 + "</th><th>" + globeltranslate.title3 + "</th><th>" + globeltranslate.title4 + fusecell(fty) + "</th><th>" + globeltranslate.title5 + "</th><th>" + globeltranslate.title6 + "</th><th></th></tr>");
                            $('#outputtabletitle').trigger('create');
                            $('#outputtabletitle thead').find('tr th').each(function (i) {
                                if (i <= 6) {
                                    $(this).width($('#outputtable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                }
                            });
                        }
                    }
                    else { //result[0].TABLE2 != "
                        //$('#jotable').show();
                        //$('#jotabletitle').show();
                        //$('#jotablelabel').show();
                        //$('#jotablewrapper').show();
                        ////$("#processalltable").css("height", "5%");
                        //$("#processalltable").css("height", "35%");
                        //$("#processalltable").css("height", "40%");

                        $('#linetable tbody tr').empty();
                        $('#linetable tbody').append(result[0].TABLE1);
                        $('#linetable').trigger('create');
                        $('#jotable tbody tr').empty();

                        $('#jotable tbody').append(result[0].TABLE2);
                        $('#jotable').trigger('create');

                        $("#linetabletitle thead tr").empty();
                        if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                            $('#linetabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.processlabel + "</th><th>" + globeltranslate.title17 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                        } else {
                            $('#linetabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.processlabel + "</th><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                            // $('#linetabletitle thead').append("<tr><th>" + globeltranslate.title13 + "</th>" + AddField(addi) + "<th>" + globeltranslate.processlabel + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                        }
                        $('#linetabletitle').trigger('create');
                        $('#linetabletitle thead').find('tr th').each(function (i) {
                            if (i <= 3) {
                                $(this).width($('#linetable tr:last').find('td').parents('tr').children('td').eq(i).width());
                            }
                        });

                        if (bypart == "false") {
                            $("#jotabletitle thead tr").empty();
                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.title17 + "</th><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title15 + "</th><th>" + globeltranslate.title16 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                            } else {
                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title15 + "</th><th>" + globeltranslate.title16 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                            }
                            $('#jotabletitle').trigger('create');
                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                if (i <= 4) {
                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                }
                            });
                        }
                        else {
                            $("#jotabletitle thead tr").empty();
                            if ($('#processselect').val() == 'MATCHING' || $('#processselect').val() == 'DC') {
                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.title17 + "</th><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title20 + "</th><th>" + globeltranslate.title21 + "</th><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title15 + "</th><th>" + globeltranslate.title16 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                            } else {
                                $('#jotabletitle thead').append("<tr><th>" + globeltranslate.title8 + "</th><th>" + globeltranslate.title9 + "</th><th>" + globeltranslate.title20 + "</th><th>" + globeltranslate.title21 + "</th><th>" + globeltranslate.title1 + "</th><th>" + globeltranslate.title15 + "</th><th>" + globeltranslate.title16 + "</th><th>" + globeltranslate.title14 + "</th><th></th></tr>");
                            }
                            $('#jotabletitle').trigger('create');
                            $('#jotabletitle thead').find('tr th').each(function (i) {
                                if (i <= 5) {
                                    $(this).width($('#jotable tr:last').find('td').parents('tr').children('td').eq(i).width());
                                }
                            });
                        }
                    }
                },
                error: function (err) {
                    $.mobile.loading('hide');
                    alert(globeltranslate.errormessage + "031");
                }
            });

        };

        function begin() {
            iprocess();
            Accessmodulehide();
            $('#nextprocess').hide();
            $('#processalltable').show();
            $('#processonetable1').hide();
            $('#processonetable2').hide();
            $('#messagetitle').html(sessionStorage.getItem("name") + " , " + sessionStorage.getItem("factory") + "/" + sessionStorage.getItem("process") + "/" + sessionStorage.getItem("productionline"));
            var employeeno = sessionStorage.getItem("employeeno");
            var factory = sessionStorage.getItem("factory");
            var process = sessionStorage.getItem("process");
            var productionline = sessionStorage.getItem("productionline");
            var userbarcode = sessionStorage.getItem("userbarcode");
            $('#jotext').focus();
            $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });

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
            $('#outputtable').chromatable({
                width: screenwidth * 0.96,
                height: screenheight * 0.6,
                scrolling: "yes"
            }); $('#linetable').chromatable({
                width: screenwidth * 0.96,
                height: screenheight * 0.25,
                scrolling: "yes"
            }); $('#jotable').chromatable({
                width: screenwidth * 0.96,
                height: screenheight * 0.6,
                scrolling: "yes"
            });

            

            
            

            $('#processselect').change(function () {
                $("#outputtabletitle thead tr").empty();
                $("#linetabletitle thead tr").empty();
                $("#jotabletitle thead tr").empty();
                $('#outputtable tbody tr').empty();
                $('#jotable tbody tr').empty();
                $('#linetable tbody tr').empty();
                if ($(this).val() == 'all') {
                    $('#nextprocess').hide();
                    $('#processalltable').show();
                    $('#processonetable1').hide();
                    $('#processonetable2').hide();
                } else {
                    $('#nextprocess').show();
                    $('#processalltable').hide();
                    $('#processonetable1').show();
                    $('#processonetable2').show();
                    //查询下道部门
                    if ($('#processselect').val() != 'MATCHING' && $('#processselect').val() != 'DC') {
                        $.ajax({
                            type: "POST",
                            url: "Outputreport.aspx/getnextprocess",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + $('#processselect').val() + "', 'flowtype': 'TRUE' }",
                            success: function (data) {
                                nextprocessselecttrans();
                                $('#nextprocessselect').append(data.d);
                                $('#nextprocessselect').selectmenu('refresh');
                            },
                            error: function (err) {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "002");
                            }
                        });
                    }
                    else if ($('#processselect').val() == 'MATCHING') {
                        $('#nextprocessselect').empty();
                        $('#nextprocessselect').append("<option selected='selected' value='DC'>DC(配套)</option>");
                        $('#nextprocessselect').selectmenu('refresh');
                    } else if ($('#processselect').val() == 'DC') {
                        $.ajax({
                            type: "POST",
                            url: "Outputreport.aspx/getnextprocess",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + $('#processselect').val() + "', 'flowtype': 'TRUE' }",
                            success: function (data) {
                                $('#nextprocessselect').empty();
                                $('#nextprocessselect').append(data.d);
                                $('#nextprocessselect').selectmenu('refresh');
                            },
                            error: function (err) {
                                $('#messagetable tbody').after("<tr onmouseover=$(this).addClass('over') onmouseout=$(this).removeClass('over')><td>" + globeltranslate.errormessage + "002</td></tr>");
                                $('#messagetable').trigger('create');
                                alert(globeltranslate.errormessage + "002");
                            }
                        });
                    }
                }
            });
        }
        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                //语言选择
                if (window.localStorage.languageid == "1") {
                    window.language = "Outputreport/Outputreport-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Outputreport/Outputreport-en-US.js";
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
                    begin();
                    //20170615-tangyh
                    queryfactory("T", fty, 'li');
                    $('#queryfactory').selectmenu('refresh');
                }

                
            }
        });

        //Export Excel
        //部门为All 时
        function ExportExcelAll() {
            var stTableObj = [];
            var autoWidthNum = [];

            var headerTitle = loopTable("outputtabletitle", true,"thead tr","th");
            var excelContent = loopTable("outputtable", false,"tbody tr","td");

            stTableObj = headerTitle.tableConent.concat(excelContent.tableConent);
            autoWidthNum = headerTitle.tableColumnWidth.concat(excelContent.tableColumnWidth);

            var headerSheets = {
                columns: autoWidthNum,
                title: "ALL",
                rows: stTableObj
            };
            var sheets = [ headerSheets];
            var workbook = new kendo.ooxml.Workbook({
                sheets: sheets
            });
            kendo.saveAs({
                dataURI: workbook.toDataURL(),
                fileName: "OutputReport" + getNowFormatDate() + ".xlsx"
            });
        }

        //针对某个部门
        function ExportExcelByProcess() {
            var stTableObj1= [];
            var autoWidthNum1 = [];

            var stTableObj2= [];
            var autoWidthNum2 = [];
            var headerTitle1 = loopTable("linetabletitle", true,"thead tr","th");
            var excelContent1 = loopTable("linetable", false,"tbody tr","td");

            stTableObj1 = headerTitle1.tableConent.concat(excelContent1.tableConent);
            autoWidthNum1 = headerTitle1.tableColumnWidth.concat(excelContent1.tableColumnWidth);
      
            var headerTitle2 = loopTable("jotabletitle", true,"thead tr","th");
            var excelContent2 = loopTable("jotable", false, "tbody tr", "td");

            stTableObj2 = headerTitle2.tableConent.concat(excelContent2.tableConent);
            autoWidthNum2 = headerTitle2.tableColumnWidth.concat(excelContent2.tableColumnWidth);

            var headerSheets1 = {
                columns: autoWidthNum1,
                title: "Line level WIP",
                rows: stTableObj1
            };
            var headerSheets2 = {
                columns: autoWidthNum2,
                title: "JO level WIP",
                rows: stTableObj2
            };
            var sheets = [headerSheets1, headerSheets2];
            var workbook = new kendo.ooxml.Workbook({
                sheets: sheets
            });
            kendo.saveAs({
                dataURI: workbook.toDataURL(),
                fileName: "OutputReport" + getNowFormatDate() + ".xlsx"
            });
        }

        //循环table dom
        function loopTable(tableDom, isHeader,tr,td) {
            var stTableObj = [];
            var autoWidthNum = [];

            var trdom = $("#" + tableDom).find(tr);
            for (var i = 0; i < trdom.length; i++) {
                var tddom= $(trdom[i]).find(td);
           

                var obj = [];
                for (var j = 0; j < tddom.length; j++) {
                    if ($(tddom[j]).is(':visible')) {
                        if (i == 0 && isHeader) {
                            obj.push({ value: $(tddom[j]).text(), bold: false, background: "#dde6f5", vAlign: "center", hAlign: "center",width:"100" });
                            // autoWidthNum.push({ autoWidth: true });
                            autoWidthNum.push({ width: 200 });
                            
                        }
                        else {
                            obj.push({ value: $(tddom[j]).text(), vAlign: "center", hAlign: "center" });
                            autoWidthNum.push({ width: 200 });
                        }

                    }
                }
                stTableObj.push({ cells: obj });
            }

            return { "tableConent": stTableObj, "tableColumnWidth": autoWidthNum };

        }
        //获取时间
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var seperator2 = ":";
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                    + " " + date.getHours() + seperator2 + date.getMinutes()
                    + seperator2 + date.getSeconds();
            return currentdate;
        }

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
                    <td style="width: 10%">
                    </td>
                    <td style="width: 8%; text-align: right">
                        <button id="cancelbutton" data-icon="back" data-iconpos="right">
                        </button>
                    </td>

                    <%--//20170615-tangyh--%>
                            <td style="width:20%" hidden="hidden" id="queryfactory_td">
                                <select data-native-menu="false" id="queryfactory" name="queryfactory"></select>
                            </td>
                            <script>  //20170615-tangyh
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
                                        $('#outputtable tbody tr').empty();
                                        $('#linetable tbody tr').empty();
                                        $('#jotable tbody tr').empty();
                                        begin();

                                    }
                                });
                            </script>     

                </tr>
            </table>
        </div>

        <div data-role="content" style="position: absolute; top: 10%; width: 100%; bottom: 2%;">
            <div class="ui-grid-c">
                <div class="ui-block-a">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="jotext" id="jolabel"></label></td>
                            <td style="width: 60%; text-align: right;">
                                <input id="jotext" name="jotext" type="text" placeholder="JOB_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="gotext" id="golabel"></label></td>
                            <td style="width: 60%; text-align: right;">
                                <input id="gotext" name="gotext" type="text" placeholder="GARMENT_ORDER_NO"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 50%">
                        <tr>
                            <td style="width: 40%; text-align: right;">
                            </td>
                            <td style="width: 60%; text-align: center">
                                <input type="button" style="text-align:center" id="jobutton" name="jobutton" onclick="JObutton_click_inquiry()" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-b">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 40%; text-align: center"></td>
                            <td style="width: 60%; text-align: right;">
                                <fieldset data-role="controlgroup" id="checkboxfieldset">
                                    <label for="bundlecheckbox" id="bundlecheckboxlabel"></label>
                                    <input value='bundlecheckbox' id="bundlecheckbox" name="bundlecheckbox" type="checkbox"/>
                                    <%--<label for="colorcheckbox" id="colorcheckboxlabel"></label>
                                    <input value='colorcheckbox' id="colorcheckbox" name="colorcheckbox" type="checkbox"/>--%>
                                </fieldset>
                            </td>
                            <%--<td style="width: 10%; text-align: center"></td>--%>
                        </tr>
                    </table>
                    <table style="width:100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="processselect" id="processlabel"></label></td>
                            <td style="width: 60%; text-align: right;">
                                <select id="processselect" name="processselect" data-native-menu="false">
                                    <option value="all" selected="selected">ALL(全部)</option>
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table style="width:100%" id="nextprocess">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="nextprocessselect" id="nextprocesslabel"></label></td>
                            <td style="width: 60%; text-align: right;">
                                <select id="nextprocessselect" name="nextprocessselect" data-native-menu="false">
                                </select>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="ui-block-c">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="fromdate" id="fromdatelabel">From</label></td>
                            <td style="width: 60%; text-align: right;">
                                <input type="text" data-role="datebox" id="fromdate" name="fromdate"/>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%; text-align: center"><label for="todate" id="todatelabel">To</label></td>
                            <td style="width: 60%; text-align: right;">
                                <input type="text" data-role="datebox" id="todate" name="todate"/>
                            </td>
                        </tr>
                    </table>
                </div>

                <div class="ui-block-d">
                    <table style="width:100%">
                        <tr>
                            <td style="width: 20%; text-align: center"></td>
                            <td style="width: 60%; text-align: right;">
                                <fieldset data-role="controlgroup" id="werw">
                                    <label for="add1checkbox" id="addfield22"></label>
                                    <input value='1' id="add1checkbox" name="SEW_LINE" type="checkbox"/>
                                    <label for="add3checkbox" id="addfield24">333</label>
                                    <input value='4' id="add3checkbox" name="COLORSIZE" type="checkbox"/>
                                </fieldset>
                            </td>
                            <td style="width: 20%; text-align: center"></td>
                        </tr>
                    </table>
                </div>

            </div>
            
            <%--全部--%>
            <table style:"width:100%" id="processalltable">
                <tr>
                    <td style="width: 2%">
                    </td>
                    <td style="width: 96%; text-align: left; ">
                        <table style="width:100%;" >
                            <tr>
                                <%-- changed by lijer on 2018-05-18--%>
                                <td style="width: 90%;"><label id="outputlabel"></label></td>
                                <td style="width: 150px;"  align="right"> <input type="button" style="text-align:center" id="exportexcelbutton" name="exportexcelbutton" onclick="ExportExcelAll()" /></td>
                            </tr>
                        </table>
                        <table class="stripe" id="outputtabletitle" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <table class="stripe" id="outputtable" width="100%" border="0" cellspacing="0" cellpadding="0">
                            <thead>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                    <td style="width:2%">
                    </td>
                </tr>
            </table>
            <%--分process--%>
            <table style="width:100%;border:0;" id="processonetable1">
                <tr>
                    <td style="width: 2%">
                    </td>
                    <td style="width: 96%; text-align: left">
                        <table style="width:100%">
                            <tr>
                                <td style="width: 90%"><label id="linetablelabel"></label></td>
                                <td style="width: 150px;"><input type="button" style="text-align:center" id="exportexcelbutton_1" name="exportexcelbutton_1" onclick="ExportExcelByProcess()" /></td>
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
                    <td style="width: 2%">
                    </td>
                </tr>
            </table>
            <table style="width:100%" id="processonetable2">
                <tr>
                    <td style="width: 2%"> </td>
                    <td style="width: 96%; text-align: left">
                        <table style="width:100%">
                            <tr>
                                <td style="width: 30%"><label id="jotablelabel"></label></td>
                                <td style="width: 70%"></td>
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
                    <td style="width: 2%">
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
