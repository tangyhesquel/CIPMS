<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html>
<head>
     
     <meta http-equiv="X-UA-Compatible" content="IE=Edge">
    <%--<script src="js/jquery.js"></script>--%>
    <script src="js/JQM/jquery.js"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.js"></script>
    <script src="js/Common.js"></script>

    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    

</head>
<body>
    <div data-role="page">
    <style>
        .border2
        {
            background-color: Red !important;
        }
    </style>
    <script type="text/javascript">
        var fty = "";
        var environment = "";
        var globeltranslate; //全局变量，用于保存翻译的json字符串；
        //获取url的参数，用于连接相应的服务器
        function getUrlParam(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
            var r = window.location.search.substr(1).match(reg);  //匹配目标参数
            if (r != null) return unescape(r[2]); return null; //返回参数值
        }
    
        //翻译语言函数
        window.language;
        function translate() {
            // debugger;   ajaxOptions Unexpected token < in JSON at position 4 
           
              $.ajax({
                type:"GET",
                url: window.language,
                //url:"Login/Login-zh-CN.js",
                dataType: "json",
                //url: "Login/HtmlPage.html",
                //dataType: "html",
                //contentType:"application/json; charset=utf-8",
                async:false,
                cache:false,
                error: function () { alert("Failure! Please check your network and try again! Wrong Code:001") },
                success: function (json) {
                   // alert("111");
                    globeltranslate = eval(json);
                    $('#headertitle').html(globeltranslate.headertitle);
                    $('#footertitle').html(globeltranslate.footertitle);
                    $('#contenttitle').html(globeltranslate.contenttitle);
                    $('#userbarcodelabel').html(globeltranslate.userbarcodelabel);
                    $('#registerbutton').val(globeltranslate.registerbutton).button('refresh');
                    $('#loginbutton').html(globeltranslate.loginbutton);
                    //alert("111");
                }
                //,
                //error: function (xhr, ajaxOptions, thrownError) {
                //    alert(xhr.status);
                //    alert(thrownError);
                //    alert("Failure! Please check your network and try again! Wrong Code:001")
                //    //debugger;
                //}
            });


        };
        //ajax查询密码是否存在
        function check(){
            var password = $('#userbarcodetext').val();
            if (!password) {
                alert(globeltranslate.alert1);
            }
            else {
                $.mobile.loading('show', { text: globeltranslate.loadingmessage, textVisible: true });
                checkuser(fty, environment, password);

            }
        }

        function checkuser(fty, environment, password) {
            $.ajax({
                type: "POST",
                url: "Login.aspx/CheckUser",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                //async: false,
                data: "{ 'factory': '" + fty + "', 'svTYPE': '" + environment + "', 'password': '" + password + "' }",
                success: function (data) {
                    $.mobile.loading('hide');
                    if (data.d == "false") {
                        alert(globeltranslate.alert2);
                        $('#userbarcodetext').focus();
                        $('#userbarcodetext').val("");
                    } else if (data.d == "false1") {
                        alert(globeltranslate.alert3);
                        $('#userbarcodetext').focus();
                        $('#userbarcodetext').val("");
                    } else {
                        var temp1 = data.d;
                        var temp = new Array();
                        temp = temp1.split(",");
                        sessionStorage.setItem("employeeno", temp[0]);
                        sessionStorage.setItem("name", temp[1]);
                        sessionStorage.setItem("factory", temp[2]);
                        sessionStorage.setItem("process", temp[3]);
                        sessionStorage.setItem("productionline", temp[4]);
                        sessionStorage.setItem("userbarcode", temp[6]);
                        //跳转到用户定义的Default Module
                        if (temp[5] == 'DocnoInquiry')
                            temp[5] = 'Barcode_Inquiry';
                        if (temp[5] == 'BarcodeInquiry')
                            temp[5] = 'Barcode_Information_Inquiry';
                        window.location.href = temp[5] + ".aspx?FTY=" + fty + "&svTYPE=" + environment;
                    }
                },
                error: function (err) {
                    $.mobile.loading('hide');
                    alert(globeltranslate.errormessage + "004");
                }
            });
        }

        function alertflash() {
            var borderFlag = false;
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
                    $('#languagechoose').addClass('border2');
                }
                else {
                    $('#languagechoose').removeClass('border2');
                }
            }
        }

        $(function () {
            fty = getUrlParam('FTY');
            environment = getUrlParam('svTYPE');
            if (fty == null || environment == null) {
                alert('The url is wrong! Pls input like 192.168.x.x/CIPMS/Login.aspx?FTY=GEG&svTYPE=TEST');
            } else {
                var lStorage = window.localStorage; //标记选择的语言；
                if (!lStorage.languageid) {
                    alert(navigator.browserLanguage);
                    if (navigator.browserLanguage == "zh-CN") {
                        window.language = "Login/Login-zh-CN.js";
                        lStorage.languageid = "1";
                    } else if (navigator.browserLanguage == "en-US") {
                        window.language = "Login/Login-en-US.js";
                        lStorage.languageid = "2";
                    } else
                    {
                        window.language = "Login/Login-zh-CN.js";
                        lStorage.languageid = "1";
                    }
         
                    translate();
       
                }
                else {
                    if (window.localStorage.languageid == "1") {
                        window.language = "Login/Login-zh-CN.js";
                        translate();
                        $("#languagechoose option[value='chinese']").attr("selected", true);
                        $('#languagechoose').selectmenu('refresh');
                    }
                    else if (window.localStorage.languageid == "2") {
                        window.language = "Login/Login-en-US.js";
                        translate();
                        $("#languagechoose option[value='english']").attr("selected", true);
                        $('#languagechoose').selectmenu('refresh');
                    } else
                    {
                        window.language = "Login/Login-zh-CN.js";
                        lStorage.languageid = "1";
                    }
                }
                $("#languagechoose").change(function () {
                    if ($('#languagechoose').val() == "chinese") {
                        lStorage.languageid = "1";
                        window.language = "Login/Login-zh-CN.js"; ;
                    } else if ($('#languagechoose').val() == "english") {
                        lStorage.languageid = "2";
                        window.language = "Login/Login-en-US.js";
                    }
 
                    translate();
                });
                $('#userbarcodetext').focus();
                $('#registerbutton').click(function () {
                    window.location.href = "Register.aspx?FTY=" + fty + "&svTYPE=" + environment;
                });
                $('#userbarcodetext').bind('keypress', function (event) {
                    if (event.keyCode == '13') {
                        check();
                    }
                });

                //监控登录按钮的点击事件
                $('#loginbutton').click(function () {
                    check();
                    //alertflash();
                });
            }

        });
    </script>

    <div data-role="header" data-theme="b">
        <table style="width:100%">
            <tr>
                <td style="width:15%"></td>
                <td style="width:70%; text-align:center"><label id="headertitle" ></label></td>
                <td style="width:15%; text-align:right;">
                    <select name="languagechoose" id="languagechoose" data-native-menu="false">
                        <option value="chinese" id="chinese">简体中文</option>
                        <option value="english" id="english">English</option>
                    </select>
                </td>            
            </tr>
        </table>
    </div>

    <div data-role="content" style="position: absolute; width: 30%; top: 30%; left: 35%; bottom: 8%">
        <ul data-role="listview" data-inset="true" data-split-theme="e">
            <li>
                <h1 id="contenttitle" style="text-align: center"></h1>
            </li>
            <li>
                <label for="userbarcodetext" id="userbarcodelabel"></label>
                <input type="password" name="userbarcodetext" id="userbarcodetext"/>
                <div id="msg" style="color:red"></div>
            </li>
            <li>
                <div class="ui-grid-a">
                    <div class="ui-block-a">
                        <table width="100%">
                            <tr>
                                <td width="98%"><button id="loginbutton"></button></td>
                                <td width="1%"></td>
                            </tr>
                        </table>
                    </div>
                    <div class="ui-block-b">
                        <table width="100%">
                            <tr>
                                <td width="1%"></td>
                                <td width="98%"><input type="button" id="registerbutton" /></td>
                            </tr>
                        </table>
                    </div>
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
