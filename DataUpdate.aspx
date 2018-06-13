<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataUpdate.aspx.cs" Inherits="DataUpdate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
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
                        
                    }
                });
            }

        </script>

        <div data-role="header" data-theme="b">
         
            <div data-role="navbar" id='navbar'>
                <div data-role="navbar">
                    
                </div>
            </div>
        </div>

        <div data-role="content" style="position: absolute; top: 17%; width: 100%; bottom: 2%;">
            <div class="ui-grid-c">
                <div class="ui-block-a">

                        <table >
                            <tr>
                                <td style="width: 15%; text-align:right">
                                    <label for="jojoborderno" id="lb_jojoborderno" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="jojoborderno"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="fromjolayno" id="lb_fromjolayno" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="fromjolayno"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="tojolayno" id="lb_tojolayno" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="tojolayno"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="jocolor" id="lb_jocolor" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="jocolor"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="jooldcutline" id="lb_jooldcutline" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="jooldcutline"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="jonewcutline" id="lb_jonewcutline" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="jonewcutline"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="jooldsewline" id="lb_jooldsewline" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="jooldsewline"></div>
                                </td>

                                <td style="width: 15%; text-align:right">
                                    <label for="jonewsewline" id="lb_jonewsewline" style="color:Red"></label>
                                </td>
                                <td style="width: 5%; text-align:left">
                                    <div style="color:Red" id="jonewsewline"></div>
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
                <a id="matchinga" href="#" data-ajax="false"></a>
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
