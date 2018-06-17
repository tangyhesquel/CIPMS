<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PrintPageNew.aspx.vb" Inherits="PrintPageNew" %>

<!DOCTYPE html>
<%--<meta http-equiv="X-UA-Compatible" content="IE=Edge"> --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="css/JQM/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <script src="js/JQM/jquery.js" type="text/javascript"></script>
    <script src="js/jquery.jqprint-0.3.js" type="text/javascript"></script>
    <script src="js/JQM/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <link href="css/jquery.mmenu.all.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mmenu.min.all.js" type="text/javascript"></script>
    <script src="js/Common.js?v1.0" type="text/javascript"></script>
    <script src="js/jquery-barcode.js" type="text/javascript"></script>
    <script src="js/qrcode.js" type="text/javascript"></script>

    <link href="KendoUI/kendo.common.min.css" rel="stylesheet" />
    <link href="KendoUI/kendo.common-material.min.css" rel="stylesheet" />
    <link href="KendoUI/kendo.default.min.css" rel="stylesheet" />
  <%--  <script src="Scripts/jquery.min.js"></script>--%>
    <script src="Scripts/kendo.all.min.js"></script>
    <script src="Scripts/cultures/kendo.culture.zh-CN.min.js"></script>
</head>
<body>
    <div id="printcontent"></div>

   <%--<div data-role="page">
     <div data-role="header" data-theme="b" >
        <table>
            <tr>
                <td>123</td>
            </tr>
        </table>
    </div>

    <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
    <table>
            <tr>
                <td>234</td>
            </tr>
        </table>
           </div>

       <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
        <div class="ui-grid-d">
             <div class="ui-block-a"><table style="width:100%">
                    <tr>
                        <td style="width:30%; text-align:center"><label for="joborderno" id="jobordernolabel"></label></td>
                        <td style="width:65%">
                            <input type="text" name="joborderno" id="joborderno" placeholder="JOB_ORDER_NO" />
                        </td>
                        <td style="width:5%"></td>
                    </tr>

             </div>
                  <div class="ui-block-b"></div>
                       <div class="ui-block-c"></div>
                            <div class="ui-block-d"></div>
                                 <div class="ui-block-e"></div>
            </div>
           </div>
      <div id="printcontent"></div>

           <div data-role="footer" data-theme="b" data-position="fixed" data-fullscreen="true">
        <h1 id="footertitle"></h1>
    </div>
           <nav id="my-menu">
               <ul>
               <li id="packageli">
                <a id="packagea" href="Package.aspx" onclick="packagejump(fty, environment);return false;" data-ajax="false"></a>
                </li>
               </ul>
           </nav>--%>

        <script type="text/javascript">
            //var content = $('#myprinttest').html();
            //$('#printcontent').html(content);
            //$('#printcontent').jqprint();
            ////window.parent.closewin();
            //debugger;
            //this.closed;

            //function GetQueryString(sProp) {
            //    var re = new RegExp("[&,?]" + sProp + "=([^\\&]*)", "i");
            //    var a = re.exec(document.location.search);
            //    if (a == null)
            //        return "";
            //    return a[1];
            //}

            //var content = GetQueryString("Parameter");
            //$('#printcontent').html(content);
            //$('#printcontent').jqprint();
            // window.close();

            var PrintContent1 = sessionStorage.getItem("PrintContent1");
            var width1 = sessionStorage.getItem("width1");
            var height1 = sessionStorage.getItem("height1");
            $('#printcontent').html(PrintContent1);
            $("#printcontent").css("width", width1);
            $("#printcontent").css("height", height1);
            $('#printcontent').jqprint();
            //window.close();

        </script>
    <%--</div>--%>
</body>
</html>
