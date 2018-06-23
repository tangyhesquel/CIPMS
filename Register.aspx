<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" %>

<!DOCTYPE html>
<html>
<head>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
    <script src="js/jquery-barcode.js" type="text/javascript"></script>
    <script src="js/qrcode.js" type="text/javascript"></script>
</head>
<body>

    <div data-role="page" id="registerpage">
    
    
    <script type="text/javascript">

        var fty = "";
        var environment = "";
        var select = "";
        var modules = new Array();
        var permission = false;
        var globeltranslate; //全局变量，用于保存翻译的json字符串；

        //打印参数
        var printhtml = "";
        var cartonbarcode = "";
        var pdbutton = "";
        var pdtitle = "";
        var printwidth = 2;
        var printheight = 80;

        //获取url的参数，用于连接相应的服务器
        function getUrlParam(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
            var r = window.location.search.substr(1).match(reg);  //匹配目标参数
            if (r != null) return unescape(r[2]); return null; //返回参数值
        }
        //翻译函数
        window.language;//全局变量，标记语言选择文件的字符串
        function translate(){
            $.ajax({
                type: "GET",
                url: window.language,
                dataType:"json",
                contentType:"application/json; charset=utf-8",
                async:false,
                cache:false,
                error: function () { alert("Failure! Pls check your network and try again! Wrong Code:001") },
                success:function(json){
                    globeltranslate = eval(json);
                    pdbutton = globeltranslate.pdbutton;
                    pdtitle = globeltranslate.pdtitle;
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#footertitle').html(globeltranslate.footertitle);
                    $('#backbutton').html(globeltranslate.backbutton);
                    $('#printbutton').html(globeltranslate.printbutton);
                    $('#modifybutton').html(globeltranslate.modifybutton);
                    $('#reprintbutton').html(globeltranslate.reprintbutton);
                    $('#clearbutton').html(globeltranslate.clearbutton);
                    $('#contenttitle').html(globeltranslate.contenttitle);
                    $('#promisebarcodelabel').html(globeltranslate.promisebarcodelabel);
                    $('#userbarcodelabel').html(globeltranslate.userbarcodelabel);
                    $('#usernamelabel').html(globeltranslate.usernamelabel);
                    $('#employeenolabel').html(globeltranslate.employeenolabel);
                    $('#factorylabel').html(globeltranslate.factorylabel);
                    $('#productionlinelabel').html(globeltranslate.grouplabel);
                    $('#shiftlabel').html(globeltranslate.shiftlabel);
                    $('#processlabel').html(globeltranslate.processlabel);
                    $('#functionlabel').html(globeltranslate.functionlabel);
                    $('#defaultfunctionlabel').html(globeltranslate.defaultfunctionlabel);
                    select = globeltranslate.select;
                }
            });
        };

        function functionselecttrans() {
            $('#functionselect').empty();
            $('#functionselect').append("<option>" + globeltranslate.select + "</option>");
            $('#functionselect').selectmenu('refresh');
        }
        function defaultfunctionselecttrans() {
            $('#defaultfunctionselect').empty();
            $('#defaultfunctionselect').append("<option>" + globeltranslate.select + "</option>");
            $('#defaultfunctionselect').selectmenu('refresh');
        }
        function factoryselecttrans() {
            $('#factoryselect').empty();
            $('#factoryselect').append("<option>" + globeltranslate.select + "</option>");
            $('#factoryselect').append("<option selected='selected' value='" + fty.substr(0, 3) + "'>" + fty.substr(0, 3) + "</option>");
            $('#factoryselect').selectmenu('refresh');
        }
        function processselecttrans() {
            $('#processselect').empty();
            $('#processselect').append("<option>" + globeltranslate.select + "</option>");
            $('#processselect').selectmenu('refresh');
        }
        function productionselecttrans() {
            $('#productionlineselect').empty();
            $('#productionlineselect').append("<option>" + globeltranslate.select + "</option>");
            $('#productionlineselect').selectmenu('refresh');
        }
        function shiftselecttrans() {
            $('#shiftselect').empty();
            $('#shiftselect').append("<option>"+globeltranslate.select+"</option>");
            $('#shiftselect').append("<option class='shift' value='A'>"+"A"+"</option>");
            $('#shiftselect').append("<option class='shift' value='B'>" + "B" + "</option>");
            $('#shiftselect').append("<option class='shift' value='C'>" + "C" + "</option>");
            $('#shiftselect').selectmenu('refresh');
        }

        function accessfunctionload() {
            modules.length = 0;
            //获取系统功能列表
            $.ajax({
                type: "POST",
                url: "Register.aspx/Accessfunction",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '"+environment+"' }",
                success: function (data) {
                    debugger;
                    var result = eval(data.d);
                    var temp = new Array();
                    $(result).each(function (i) {
                        var j = 0;
                        for (j; j < temp.length; j++) {
                            if (temp[j] == result[i].MODULE) {
                                break;
                            }
                        }
                        if (j == temp.length)
                            temp[j] = result[i].MODULE;
                    });
                    $.each(temp, function (n, value) {
                        if (value == 'Package') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a1 + "'></optgroup>");
                        } else if (value == 'Matching') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a2 + "'></optgroup>");
                        } else if (value == 'Receive') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a3 + "'></optgroup>");
                        } else if (value == 'Transaction') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a4 + "'></optgroup>");
                        } else if (value == 'Reduce') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a5 + "'></optgroup>");
                        } else if (value == 'EmployeeOutput') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a8 + "'></optgroup>");
                        } else if (value == 'Bundleinput') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a27 + "'></optgroup>");
                        } else if (value == 'WIPReport') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a7 + "'></optgroup>");
                        } else if (value == 'OutputReport') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a28 + "'></optgroup>");
                        } else if (value == 'dCTReport') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a29 + "'></optgroup>");
                        } else if (value == 'Printbarcode') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a6 + "'></optgroup>");
                        } else if (value == 'DocnoInquiry') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a30 + "'></optgroup>");
                        } else if (value == 'BarcodeInquiry') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a31 + "'></optgroup>");
                        } else if (value == 'Printsparecode') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a34 + "'></optgroup>");
                        } else if (value == 'WipAndOutputNew') {
                            $('#functionselect').append("<optgroup class='group' value='" + value + "' id='" + value + "' label='" + globeltranslate.a36 + "'></optgroup>");
                        }
                    });
                    $(result).each(function (i) {
                        $('.group').each(function () {
                            if (result[i].MODULE == $(this).val()) {
                                if (result[i].FUNCTION_ENG == 'Close carton') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a9 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Open carton') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a10 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Matching carton') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a11 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Check carton') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a12 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Print carton') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a13 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Receive') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a14 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Reject') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a15 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Transaction') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a16 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Rework') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a17 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Adjustment') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a18 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Reduce') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a19 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Employeeoutput') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a20 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Externalreceive') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a21 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'WIPreport') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a22 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Outputreport') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a23 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'dCTreport') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a24 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Layno') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a25 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'Print' ) {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a26 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'DocnoInquiry') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a32 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'BarcodeInquiry') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a33 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'PrintSpareCode') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a35 + "</option>");
                                } else if (result[i].FUNCTION_ENG == 'WipAndOutputNew') {
                                    $(this).append("<option class='func' value='" + result[i].ID + "'>" + globeltranslate.a37 + "</option>");
                                }
                            }
                        });
                        modules[result[i].ID] = result[i].MODULE;
                    });
                    $('#functionselect').selectmenu('refresh');
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "005");
                }
            });
        }

        function processload() {
            $.ajax({
                type: "POST",
                url: "Register.aspx/Process",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "' }",
                success: function (data) {
                    processselecttrans();
                    $('#processselect').append(data.d);
                    $('#processselect').selectmenu('refresh');
                    productionselecttrans();
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "024");
                }
            });
        }

        function productionload() {
            $.ajax({
                type: "POST",
                url: "Register.aspx/Production",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                data: "{ 'factory': '" + fty + "', 'svTYPE': '"+environment+"', 'process': '" + $('#processselect').val() + "' }",
                success: function (data) {
                    productionselecttrans();
                    $('#productionlineselect').append(data.d);
                    $('#productionlineselect').selectmenu('refresh');
                },
                error: function (err) {
                    alert(globeltranslate.errormessage + "007");
                }
            });
        }

        function empty() {
            permission = false;
            $('#promisebarcodetext').val("");
            $('#usernametext').val("");
            $('#employeenotext').val("");
            $('#userbarcodetext').val("");
            $('#printbutton').attr('disabled', 'true');
            $('#reprintbutton').attr('disabled', 'true');
            $('#modifybutton').attr('disabled', 'true');
            $('#promisebarcodetext').focus();
            functionselecttrans();
            accessfunctionload();
            defaultfunctionselecttrans();
            shiftselecttrans();
            productionselecttrans();
            //productionload();
        }

        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=PROD');
            } else {
                var processcd;
                var productionlinecd;
                $('#promisebarcodetext').focus();
                $('#printbutton').attr('disabled', 'true');
                $('#reprintbutton').attr('disabled', 'true');
                $('#modifybutton').attr('disabled', 'true');
                //语言选择
                if (window.localStorage.languageid == "1") {
                    window.language = "Register/Register-zh-CN.js";
                } else if (window.localStorage.languageid == "2") {
                    window.language = "Register/Register-en-US.js";
                }
                translate();

                //下拉框初始化
                factoryselecttrans();
                processselecttrans();
                productionselecttrans();
                functionselecttrans();
                defaultfunctionselecttrans();
                shiftselecttrans();

                //加载功能
                debugger;
                accessfunctionload();
                debugger;
                processload();
                $('#printtest').hide();
                //Permission回车监控
                $('#promisebarcodetext').bind('keypress', function (event) {
                    var temp = false;
                    var permissionbarcode = $(this).val();
                    if (event.keyCode == 13) {
                        $.ajax({
                            type: "POST",
                            url: "Register.aspx/Permission",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            async: false,
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'barcode': '" + permissionbarcode + "' }",
                            success: function (data) {
                                if (data.d == 'false') {
                                    alert(globeltranslate.message2);
                                    $('#promisebarcodetext').val("");
                                    $('#promisebarcodetext').focus();
                                }
                                else {
                                    permission = true;
                                    //temp = true;
                                    $('#printbutton').removeAttr('disabled');
                                    $('#reprintbutton').removeAttr('disabled');
                                }
                            },
                            error: function (err) {
                                alert(globeltranslate.errormessage + "002");
                            }
                        });
                    }
                });

                //模块选择下拉框动态刷新
                var temp1 = new Array();
                var temp2 = new Array();
                $('#functionselect').change(function () {
                    temp1 = [];
                    temp2 = [];
                    var i = 0;
                    defaultfunctionselecttrans();
                    $('#functionselect option:selected').each(function () {
                        temp1[i++] = modules[$(this).val()];
                    });
                    for (i = 0; i < temp1.length; i++) {
                        var j = 0;
                        for (j; j < temp2.length; j++) {
                            if (temp2[j] == temp1[i]) {
                                break;
                            }
                        }
                        if (j == temp2.length)
                            temp2[j] = temp1[i];
                    }
                    for (i = 0; i < temp2.length; i++) {
                        debugger;
                        if (temp2[i] == 'Package') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a1 + "</option>");
                        } else if (temp2[i] == 'Matching') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a2 + "</option>");
                        } else if (temp2[i] == 'Receive') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a3 + "</option>");
                        } else if (temp2[i] == 'Transaction') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a4 + "</option>");
                        } else if (temp2[i] == 'Reduce') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a5 + "</option>");
                        } else if (temp2[i] == 'EmployeeOutput') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a8 + "</option>");
                        } else if (temp2[i] == 'Bundleinput') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a27 + "</option>");
                        } else if (temp2[i] == 'WIPReport') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a7 + "</option>");
                        } else if (temp2[i] == 'OutputReport') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a28 + "</option>");
                        } else if (temp2[i] == 'dCTReport') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a29 + "</option>");
                        } else if (temp2[i] == 'Printbarcode') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a6 + "</option>");
                        } else if (temp2[i] == 'DocnoInquiry') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a30 + "</option>");
                        } else if (temp2[i] == 'BarcodeInquiry') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a31 + "</option>");
                        } else if (temp2[i] == 'Printsparecode') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a34 + "</option>");
                        } else if (temp2[i] == 'WipAndOutputNew') {
                            $('#defaultfunctionselect').append("<option class='module' value='" + temp2[i] + "'>" + globeltranslate.a36 + "</option>");
                        }
                    }
                    $('#defaultfunctionselect').selectmenu('refresh');
                });

                //根据process_name获取production_line_cd
                $('#processselect').change(function () {
                    productionload();
                });

                //返回按钮
                $('#backbutton').click(function () {
                    Loginout(fty, environment);
                });

                //清空按钮
                $('#clearbutton').click(function () {
                    $('#test').empty();
                    empty();
                });

                var userbarcode = "";
                $('#userbarcodetext').bind('keypress', function (event) {
                    var temp = false;
                    if (event.keyCode == 13) {
                        $.ajax({
                            type: "POST",
                            url: "Register.aspx/LoadUser",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'userbarcode':'" + $('#userbarcodetext').val() + "' }",
                            success: function (data) {
                                if (data.d != "false1" && data.d != "nothisuser") {
                                    if (permission == true) {
                                        $('#modifybutton').removeAttr('disabled');
                                    }
                                    userbarcode = $('#userbarcodetext').val();
                                    var result = eval(data.d);
                                    $('#usernametext').val(result[0].NAME);
                                    $('#employeenotext').val(result[0].EMPLOYEENO);

                                    processselecttrans();
                                    $('#processselect').append(result[0].PROCESSLIST);
                                    $('#processselect').selectmenu('refresh');

                                    productionselecttrans();
                                    $('#productionlineselect').append(result[0].PRODUCTIONLIST);
                                    $('#productionlineselect').selectmenu('refresh');

                                    $('.shift').each(function () {
                                        if ($(this).val() == result[0].SHIFT) {
                                            $(this).attr("selected", true);
                                        }
                                    });
                                    $('#shiftselect').selectmenu('refresh');

                                    var funcidlist = result[0].FUNCID.split('/');
                                    $(funcidlist).each(function (i) {
                                        $('.func').each(function () {
                                            if ($(this).val() == funcidlist[i]) {
                                                $(this).attr("selected", true);
                                            }
                                        });
                                    });
                                    $('#functionselect').selectmenu('refresh');

                                    var module = result[0].MODULE.split('/');
                                    var defaultfunc = result[0].DEFAULTFUNC;
                                    $(module).each(function (i) {
                                        if (module[i] == 'Package') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a1 + "</option>");
                                        } else if (module[i] == 'Matching') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a2 + "</option>");
                                        } else if (module[i] == 'Receive') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a3 + "</option>");
                                        } else if (module[i] == 'Transaction') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a4 + "</option>");
                                        } else if (module[i] == 'Reduce') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a5 + "</option>");
                                        } else if (module[i] == 'EmployeeOutput') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a8 + "</option>");
                                        } else if (module[i] == 'Bundleinput') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a27 + "</option>");
                                        } else if (module[i] == 'WIPReport') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a7 + "</option>");
                                        } else if (module[i] == 'OutputReport') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a28 + "</option>");
                                        } else if (module[i] == 'dCTReport') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a29 + "</option>");
                                        } else if (module[i] == 'Printbarcode') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a6 + "</option>");
                                        } else if (module[i] == 'DocnoInquiry') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a32 + "</option>");
                                        } else if (module[i] == 'BarcodeInquiry') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a33 + "</option>");
                                        } else if (module[i] == 'WipAndOutputNew') {
                                            $('#defaultfunctionselect').append("<option class='module' value='" + module[i] + "'>" + globeltranslate.a36 + "</option>");
                                        }
                                    });
                                    $('.module').each(function () {
                                        if (defaultfunc == $(this).val())
                                            $(this).attr("selected", true);
                                    });
                                    $('#defaultfunctionselect').selectmenu('refresh');
                                }
                                else if (data.d == "false1") {
                                    alert(globeltranslate.message6);
                                }
                                else if (data.d == "nothisuser") {
                                    alert(globeltranslate.message8);
                                    $('#userbarcodetext').val("");
                                    $('#userbarcodetext').focus();
                                }
                            },
                            error: function (err) {
                                alert(globeltranslate.errormessage + "018");
                            }
                        });
                    }
                });

                //用户信息以及权限修改
                $('#modifybutton').click(function () {
                    var username = $('#usernametext').val();
                    var employeeno = $('#employeenotext').val();
                    var factory = $('#factoryselect').val();
                    var process = $('#processselect').val();
                    var productionline = $('#productionlineselect').val();
                    var shift = $('#shiftselect').val();
                    var functionselect;
                    var i = 0;
                    $('#functionselect option:selected').each(function () {
                        if (i == 0)
                            functionselect = $(this).val();
                        else
                            functionselect += "," + $(this).val();
                        i++;
                    });
                    var defaultfunction = $('#defaultfunctionselect').val();
                    if (username == "" || employeeno == "" || factory == select || process == select || functionselect == select || defaultfunction == select) {
                        alert(globeltranslate.message1);
                        return;
                    } else {
                        $.ajax({
                            type: "POST",
                            url: "Register.aspx/Modify",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'userbarcode':'" + userbarcode + "', 'username': '" + username.toString() + "', 'employeeno': '" + employeeno.toString() + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + process.toString() + "', 'production': '" + productionline.toString() + "', 'shift': '" + shift.toString() + "', 'selectfunction': '" + functionselect + "', 'defaultfunction': '" + defaultfunction.toString() + "', 'processlabel': '" + globeltranslate.processlabel + "', 'usernamelabel': '" + globeltranslate.usernamelabel + "', 'employeenolabel': '" + globeltranslate.employeenolabel + "', 'select': '" + select + "' }",
                            success: function (data) {
                                if (data.d != "false") {
                                    if (data.d == 'error1') {
                                        alert(globeltranslate.message3);
                                        return;
                                    }
                                    alert(globeltranslate.message4);
                                    var result = eval(data.d);
                                    printhtml = result[0].HTML;
                                    cartonbarcode = result[0].BARCODE;
                                    $('#jumpprint').trigger('click');
                                    empty();
                                }
                                else alert(globeltranslate.message3);
                            },
                            error: function (err) {
                                alert(globeltranslate.errormessage + "011");
                            }
                        });
                    }
                });

                //重新打印按钮
                $('#reprintbutton').click(function () {
                    var employeeno = $('#employeenotext').val();
                    if (employeeno == '') {
                        alert(globeltranslate.message7);
                        return;
                    } else {
                        $.ajax({
                            type: "POST",
                            url: "Register.aspx/Reprint",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'employeeno': '" + employeeno + "', 'processlabel': '" + globeltranslate.processlabel + "', 'usernamelabel': '" + globeltranslate.usernamelabel + "', 'employeenolabel': '" + globeltranslate.employeenolabel + "' }",
                            success: function (data) {
                                if (data.d != "false1") {
                                    alert(globeltranslate.reprintmessage);
                                    var result = eval(data.d);
                                    printhtml = result[0].HTML;
                                    cartonbarcode = result[0].BARCODE;
                                    $('#jumpprint').trigger('click');
                                    empty();
                                }
                                else alert(globeltranslate.message6);
                            },
                            error: function (err) {
                                alert(globeltranslate.errormessage + "018");
                            }
                        });
                    }
                });

                //打印按钮监控
                $('#printbutton').click(function () {
                    var username = $('#usernametext').val();
                    var employeeno = $('#employeenotext').val();
                    var factory = $('#factoryselect').val();
                    var process = $('#processselect').val();
                    var productionline = $('#productionlineselect').val();
                    var shift = $('#shiftselect').val();
                    var functionselect;
                    var i = 0;
                    $('#functionselect option:selected').each(function () {
                        if (i == 0)
                            functionselect = $(this).val();
                        else
                            functionselect += "," + $(this).val();
                        i++;
                    });
                    var defaultfunction = $('#defaultfunctionselect').val();
                    if (username == "" || employeeno == "" || factory == select || process == select || functionselect == select || defaultfunction == select) {
                        alert(globeltranslate.message1);
                        return;
                    } else {
                        $.ajax({
                            type: "POST",
                            url: "Register.aspx/Registerprint",
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            data: "{ 'username': '" + username.toString() + "', 'employeeno': '" + employeeno.toString() + "', 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'process': '" + process.toString() + "', 'production': '" + productionline.toString() + "', 'shift': '" + shift.toString() + "', 'selectfunction': '" + functionselect + "', 'defaultfunction': '" + defaultfunction.toString() + "', 'processlabel': '" + globeltranslate.processlabel + "', 'usernamelabel': '" + globeltranslate.usernamelabel + "', 'employeenolabel': '" + globeltranslate.employeenolabel + "', 'select': '" + select + "' }",
                            success: function (data) {
                                if (data.d != "false") {
                                    if (data.d == 'error1') {
                                        alert(globeltranslate.message3);
                                        return;
                                    }
                                    alert(globeltranslate.registernewbarcode);
                                    var result = eval(data.d);
                                    printhtml = result[0].HTML;
                                    cartonbarcode = result[0].BARCODE;
                                    $('#jumpprint').trigger('click');
                                    empty();
                                }
                                else alert(globeltranslate.message3);
                            },
                            error: function (err) {
                                alert(globeltranslate.errormessage + "011");
                            }
                        });
                    }
                });
            }

        });
    </script>

        <div data-role="header" data-theme="b">
            <table style="width:100%">
                <tr>
                    <td style="width:8%; text-align:left"><button type="button" id="backbutton" data-icon="arrow-l" ></button></td>
                    <td style="width:84%; text-align:center"><label id="headertitle"></label></td>
                    <td style="width:8%"></td>
                </tr>
            </table>
            <div data-role="navbar" >
                <div data-role="navbar">
                    <table width="100%">
                        <tr>
                            <td id="td1" style="width:10%">
                                <ul>
                                    <li>
                                        <button style="text-align:center" id="printbutton" name="printbutton" ></button>
                                    </li>
                                </ul>
                            </td>
                            <td id="td2" style="width:10%">
                                <ul>
                                    <li>
                                        <button style="text-align:center" id="modifybutton" name="modifybutton" ></button>
                                    </li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li>
                                        <button style="text-align:center" id="reprintbutton" name="reprintbutton" ></button>
                                    </li>
                                </ul>
                            </td>
                            <td style="width:10%">
                                <ul>
                                    <li>
                                        <button style="text-align:center" id="clearbutton" name="clearbutton" ></button>
                                    </li>
                                </ul>
                            </td>
                            <td style="width:60%"></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <div data-role="content" style="position: absolute; width: 60%; top: 20%; left: 20%; bottom: 8%">
            <ul data-role="listview" data-inset="true" data-split-theme="e">
                <li style="text-align: center">
                    <h1 id="contenttitle"></h1>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 22%; text-align: center"><label for="promisebarcodetext" id="promisebarcodelabel"></label></td>
                                <td style="width: 28%; text-align: right;"><input id="promisebarcodetext" type="password" name="promisebarcodetext" /></td>
                                <td style="width: 22%; text-align: center"><label for="userbarcodetext" id="userbarcodelabel"></label></td>
                                <td style="width: 28%; text-align: right;"><input id="userbarcodetext" type="password" name="userbarcodetext" /></td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 22%; text-align: center"><label for="usernametext" id="usernamelabel"></label></td>
                                <td style="width: 28%; text-align: right;"><input id="usernametext" type="text" name="usernametext" /></td>
                                <td style="width: 22%; text-align: center"><label for="employeenotext" id="employeenolabel"></label></td>
                                <td style="width: 28%; text-align: right;"><input id="employeenotext" type="text" name="employeenotext" /></td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 22%; text-align: center"><label for="factoryselect" id="factorylabel"></label></td>
                                <td style="width: 28%; text-align: right;">
                                    <select name="factoryselect" id="factoryselect" data-native-menu="false">
                                    </select>
                                </td>
                                <td style="width: 22%; text-align: center"><label for="processselect" id="processlabel"></label></td>
                                <td style="width: 28%; text-align: right;">
                                    <select name="processselect" id="processselect" data-native-menu="false">
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </div>
                </li>
                <li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 22%; text-align: center"><label for="productionlineselect" id="productionlinelabel"></label></td>
                                <td style="width: 28%; text-align: right;">
                                    <select name="productionlineselect" id="productionlineselect">
                                    </select>
                                </td>
                                <td style="width: 22%; text-align: center"><label for="shiftselect" id="shiftlabel"></label></td>
                                <td style="width: 28%; text-align: right;">
                                    <select name="shiftselect" id="shiftselect" data-native-menu="false">
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </div>
                </li><li style = "padding : 0px 0px">
                    <div style="margin: 0px 0px; padding : 0px 0px">
                        <table width="100%">
                            <tr>
                                <td style="width: 22%; text-align: center"><label for="functionselect" id="functionlabel"></label></td>
                                <td style="width: 28%; text-align: right;">
                                    <select name="functionselect" id="functionselect" multiple="multiple" data-native-menu="false">
                                    </select>
                                </td>
                                <td style="width: 22%; text-align: center"><label for="defaultfunctionselect" id="defaultfunctionlabel"></label></td>
                                <td style="width: 28%; text-align: right;">
                                    <select name="defaultfunctionselect" id="defaultfunctionselect" data-native-menu="false">
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="printtest" style="width:7.0cm;font-size:8px; margin-left:0cm">
                        <a id='jumpprint' data-role='button' href='Printpage.aspx' data-rel='dialog'>hide</a>
                    </div>
                </li>
            </ul>
        </div>

        <div data-role="footer" data-theme="b" data-position="fixed" data-fullscreen="true">
            <h1 id="footertitle"></h1>
        </div>

    </div>
</body>
</html>
