using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Web;
using System.Text;

namespace ConsoleApp4
{
    class APICommandManager : APIInterface
    {
        string testt;
        static string IPAddress;
        Class1 dc = new Class1();
        String LocalIP = "10.10.10.110";

        public APICommandManager(string test)
        {
            testt = test;
        }

        String HOME1 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\HOME1.txt";
        String HOME2 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\HOME2.txt";
        String lineplan1 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\LinePlan1.txt";
        String lineplan2 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\LinePlan2.txt";
        String lineplan3 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\LinePlan3.txt";
        String DASHBOARD1 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\DASHBOARD1.txt";
        String DASHBOARD2 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\DASHBOARD2.txt";
        String PROD = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\PRODUCTION.txt";
        String login1 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\Login1.txt";
        String tna1 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\TNA1.txt";
        String dailyprod1 = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\HTML\DailyProd1.txt";


        public String GetIPAddress()
        {
            try
            {
                IPHostEntry Host = default(IPHostEntry);
                string Hostname = null;
                Hostname = System.Environment.MachineName;
                Host = Dns.GetHostEntry(Hostname);

                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == AddressFamily.InterNetwork)
                    {
                        IPAddress = Convert.ToString(IP);
                    }
                }

                return IPAddress;
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex.ToString());
                return "";
            }
        }

        public Stream HOME(String usergroup)
        {
            String text1 = "";
            try
            {
                //read file
                String ip1 = GetIPAddress();
                if (File.Exists(HOME1))
                {
                    text1 = File.ReadAllText(HOME1);
                }

                //read file
                String text2 = "";
                if (File.Exists(HOME2))
                {
                    text2 = File.ReadAllText(HOME2);
                }

                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=MRT_GLOBALDB;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                text1 = text1 + "<label style=\"color:white;\"Cluster : </label></div><div class ='child_div_2'><select name=\"cluster\" id=\"cluster\" style=\"height:30px;width:200px\" onchange=\"hidebuffer()\" hidden>";

                DataTable dt = dc.SQLDataAdapter("select V_CLUSTER_ID,V_CLUSTER_IP_ADDRESS from CLUSTER_DB", con);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    text1 = text1 + "<option value=\"" + dt.Rows[i][1].ToString() + "\">" + dt.Rows[i][0].ToString() + "</option>";
                }

                text1 = text1 + "</select></div></div>";
                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";

                //get all prod line
                DataTable dt1 = dc.SQLDataAdapter("SELECT DISTINCT V_PROD_LINE from PROD_LINE_DB", con);

                text1 = text1 + "<div class=\"center\"><div class=\"dropdown\">";
                text1 = text1 + "<button class=\"button button5 dropdown-toggle\" style=\"width:350px\" type=\"button\" data-toggle=\"dropdown\"><img src=\"http://255.255.255.255:8091/file/dashboard.png\" style=\"vertical-align:left;margin:0px 25px\">Dashboard Line<span class=\"caret\"></span></button><ul class=\"dropdown-menu\">";

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    text1 = text1 + "<li><a href=\"#\" onclick=\"StopLines(" + dt1.Rows[i][0].ToString() + ")\">" + dt1.Rows[i][0].ToString() + "</a></li>";
                }

                text1 = text1 + "</ul></div>";
                text1 = text1 + text2;
                text1 = text1.Replace("255.255.255.255", ip1);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public Stream FetchImage(String imageName)
        {
            try
            {
                //get images from file
                String filePath = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\Images\" + imageName;
                if (File.Exists(filePath))
                {
                    FileStream fs = File.OpenRead(filePath);
                    WebOperationContext.Current.OutgoingRequest.ContentType = "image/png";

                    return fs;
                }
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(" Requested Image does not exist :(");
                    MemoryStream strm = new MemoryStream(byteArray);

                    return strm;
                }
            }
            catch (Exception ex)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(" Requested Image does not exist :(" + ex);
                MemoryStream strm = new MemoryStream(byteArray);

                return strm;
            }
        }

        public Stream RetrieveFile(String file)
        {
            try
            {
                //get files from folder
                String fileName = @"C:\Program Files (x86)\Athena Enterprise Planning\Athena Server\" + file;
                if (File.Exists(fileName))
                {
                    //WebOperationContext.Current.OutgoingResponse.ContentType = "application/octet-stream";
                    FileStream fs = File.OpenRead(fileName);
                    if (fileName.Contains(".css"))
                    {
                        WebOperationContext.Current.OutgoingResponse.ContentType = "text/css";
                    }
                    else
                    {
                        WebOperationContext.Current.OutgoingResponse.ContentType = "text/javascript";
                    }
                    return fs;
                }
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(" Requested Files does not exist :(");
                    MemoryStream strm = new MemoryStream(byteArray);

                    return strm;
                }
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex.ToString());

                byte[] byteArray = Encoding.UTF8.GetBytes(" Requested Files does not exist : " + ex);
                MemoryStream strm = new MemoryStream(byteArray);

                return strm;
            }
        }

        public System.IO.Stream LinePlan(String usergroup)
        {
            String ip1 = GetIPAddress();
            String text1 = "";
            String text2 = "";
            try
            {
                //read file
                if (File.Exists(lineplan1))
                {
                    text1 = File.ReadAllText(lineplan1);
                }

                if (File.Exists(lineplan2))
                {
                    text2 = File.ReadAllText(lineplan2);
                }

                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                if (!dc.GetProductLicence(con).Contains("LinePlan,"))
                {
                    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>Line Plan Not Enabled For This Key. </h1>");
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                    return new MemoryStream(resultBytes1);
                }

                text1 = text1 + "<div id='parent_div_2'> <div class='child_div_2'>	<select name=\"cluster\" id=\"cluster\" style=\"height:30px;width:200px\" onchange=\"GetLines()\">";
                //get hanger id length
                DataTable dt = dc.SQLDataAdapter("select distinct Factory_Description from Master_Lines", con);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    text1 = text1 + "<option value=\"" + dt.Rows[i][0].ToString() + "\">" + dt.Rows[i][0].ToString() + "</option>";
                }

                text1 = text1 + "</select></div>";
                text1 = text1 + "<div class ='child_div_2'>	<select name=\"line\" id=\"line\" style=\"height:30px;width:200px\" >";
                if (dt.Rows.Count > 0)
                {
                    dt = dc.SQLDataAdapter("select LineName from Master_Lines where Factory_Description='" + dt.Rows[0][0] + "'", con);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        text1 = text1 + "<option value=\"" + dt.Rows[i][0].ToString() + "\">" + dt.Rows[i][0].ToString() + "</option>";
                    }
                }

                text1 = text1 + "</select></div>";
                text1 = text1 + "<div class ='child_div_2'> <button class=\"button button5\" id=\"bottom1\" onclick=\"GetLinePlan()\" style=\"width:150px;height:30px\">Line Plan</button></div></div>";
                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 += text2;
                text1 = text1.Replace("255.255.255.255", ip1);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return new MemoryStream(resultBytes);
        }

        public String GetLines(String factory)
        {
            String lines = "";

            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                DataTable dt = dc.SQLDataAdapter("select LineName from Master_Lines where Factory_Description='" + factory + "'", con);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    lines += dt.Rows[i][0] + "+";
                }

                if (lines.Length > 0)
                {
                    lines = lines.Remove(lines.Length - 1, 1);
                }
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            return lines;
        }

        public Stream GetLinePlan(String factory, String line, String usergroup, String date)
        {
            String ip1 = GetIPAddress();
            String text1 = "";
            String text2 = "";
            String line1 = line;

            try
            {
                //read file
                if (File.Exists(lineplan1))
                {
                    text1 = File.ReadAllText(lineplan1);
                }

                if (File.Exists(lineplan3))
                {
                    text2 = File.ReadAllText(lineplan3);
                }

                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                if (!dc.GetProductLicence(con).Contains("LinePlan"))
                {
                    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>Line Plan Not Enabled For This Key. </h1>");
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                    return new MemoryStream(resultBytes1);
                }

                int minyear = int.Parse(DateTime.Now.ToString("yyyy")) - 5;
                int maxyear = int.Parse(DateTime.Now.ToString("yyyy")) + 5;

                String[] date1 = date.Split('-');
                if (date1.Length > 2)
                {
                    if (date1[1].Length == 1)
                    {
                        date1[1] = "0" + date1[1];
                    }

                    date = date1[0] + "-" + date1[1] + "-" + date1[2];
                }

                text1 = text1 + "<div id='parent_div_2'> </br><div class='child_div_2'><label style=\"vertical-align:left;color:white\">Date : </label></div>";
                text1 += "<div class='child_div_2'></div><input type=\"date\" id=\"curdate\" name=\"tnadate\" value=\"" + date + "\" min=\"" + minyear + "-01-01\" max=\"" + maxyear + "-12-31\" onchange=\"getTNA()\"></div></div>";

                text1 += "<table style=\"width:100%\" id=\"table\" class=\"table\"> <tr id=\"headerRow\">";
                text1 += "<th>Sales Order</th>";
                text1 += "<th>Sales Order Item</th>";
                text1 += "<th>Customer</th>";
                text1 += "<th>Product Type</th>";
                text1 += "<th>Order Type</th>";
                text1 += "<th>Season</th>";
                text1 += "<th>Style</th>";
                text1 += "<th>Fabric</th>";
                text1 += "<th>Wash</th>";
                text1 += "<th>FG</th>";
                text1 += "<th>Order Quantity</th>";
                text1 += "<th>Delivery Date</th>";
                text1 += "<th>Planned Start</th>";
                text1 += "<th>Planned End</th>";
                text1 += "<th>Actual Start</th>";
                text1 += "<th>Actual End</th>";
                text1 += "<th>Completed</th>";
                text1 += "</tr>	";

                line = dc.SQLCommand_ExecuteScalar("select Line from Master_Lines where LineName='" + line + "' and Factory_Description='" + factory + "'", con);

                if (line != "")
                {
                    DataTable dt = dc.SQLDataAdapter("select * from LinePlan where pStart>='" + date + " 00:00:00' and Factory='" + factory + "' and Line='" + line + "'", con);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        text1 += "<tr>";
                        text1 += "<td>" + dt.Rows[i]["SalesOrder"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["SalesOrderItem"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Customer"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["ProductType"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["OrderType"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Season"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Style"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Fabric"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Wash"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["FGDesc"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Quantity"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["DeliveryDate"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["pStart"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["pEnd"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["aStart"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["aEnd"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["Complete"] + "</td>";
                        text1 += "</tr>";
                    }
                }

                text1 += "</table> ";
                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 = text1 + "<label id=\"factory\" for=\"Remote\" hidden>" + factory + "</label>";
                text1 = text1 + "<label id=\"factoryline\" for=\"Remote\" hidden>" + line1 + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 += text2;
                text1 = text1.Replace("255.255.255.255", ip1);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public Stream WashPlan(String usergroup)
        {
            String ip1 = GetIPAddress();
            String text1 = "";
            String text2 = "";

            try
            {
                //read file
                if (File.Exists(lineplan1))
                {
                    text1 = File.ReadAllText(lineplan1);
                }

                if (File.Exists(lineplan3))
                {
                    text2 = File.ReadAllText(lineplan3);
                }

                String date = DateTime.Now.ToString("yyyy-MM-dd");

                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                if (!dc.GetProductLicence(con).Contains("WashPlan,"))
                {
                    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>Wash Plan Not Enabled For This Key. </h1>");
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                    return new MemoryStream(resultBytes1);
                }

                int nodept = 0;
                String temp = dc.SQLCommand_ExecuteScalar("select NO_DEPT from Setup", con);
                if (temp != "")
                {
                    nodept = int.Parse(temp);
                }

                text1 += "<table style=\"width:100%\" id=\"table\" class=\"table\"> <tr id=\"headerRow\">";
                text1 += "<th>Sales Order</th>";
                text1 += "<th>Sales Order Item</th>";
                text1 += "<th>customer</th>";
                text1 += "<th>Style</th>";
                text1 += "<th>Fabric</th>";
                text1 += "<th>Wash</th>";
                text1 += "<th>Wash Quantity</th>";
                for (int i = 1; i <= nodept; i++)
                {
                    text1 += "<th>Route-" + i;
                }
                text1 += "</tr>";

                DataTable data1 = new DataTable();
                data1.Columns.Add("Sales Order");
                data1.Columns.Add("Sales Order Item");
                data1.Columns.Add("Customer");
                data1.Columns.Add("Style");
                data1.Columns.Add("Fabric");
                data1.Columns.Add("Wash");
                data1.Columns.Add("Wash Quantity");
                for (int i = 1; i <= nodept; i++)
                {
                    data1.Columns.Add("Route-" + i);
                }


                DataTable dt = dc.SQLDataAdapter("select * from WashPlan where aDate='" + date + "'", con);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String[] dept = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
                    for (int j = 1; j <= 50; j++)
                    {
                        int route = int.Parse(dt.Rows[i]["D" + j].ToString());
                        if (route == 0)
                        {
                            continue;
                        }
                        dept[route] = dt.Rows[i]["Dept" + j].ToString();
                    }

                    String salesorderitem = dc.SQLCommand_ExecuteScalar("select SalesOrderItem from LinePlan where SalesOrder='" + dt.Rows[i]["SalesOrder"] + "'", con);

                    String customer = dc.SQLCommand_ExecuteScalar("select Customer from LinePlan where SalesOrder='" + dt.Rows[i]["SalesOrder"] + "'", con);

                    data1.Rows.Add(dt.Rows[i]["SalesOrder"].ToString(), salesorderitem, dt.Rows[i]["Style"].ToString(), dt.Rows[i]["Fabric"].ToString(), dt.Rows[i]["Wash"].ToString(), dt.Rows[i]["WashQty"].ToString());

                    for (int j = 1; j < dept.Length; j++)
                    {
                        if (dept[j] != "")
                        {
                            data1.Rows[i]["Route-" + j] = dept[j];
                        }
                    }

                    data1.Rows[i]["Customer"] = customer;
                }

                for (int i = 0; i < data1.Rows.Count; i++)
                {
                    text1 += "<tr>";
                    for (int j = 0; j < data1.Columns.Count; j++)
                    {
                        text1 += "<td>" + data1.Rows[i][j] + "</td>";
                    }
                    text1 += "</tr>";
                }

                text1 += "</table>";
                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 += text2;
                text1 = text1.Replace("255.255.255.255", ip1);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public Stream Dashboard(String ipaddress, String usergroup)
        {
            String text1 = "";
            try
            {
                //read file
                if (File.Exists(DASHBOARD1))
                {
                    text1 = File.ReadAllText(DASHBOARD1);
                }

                DateTime start = Convert.ToDateTime("1970-01-01 00:00:00");
                String[] color = { "#990000", "#669900", "#000099", "#009999", "#999900", "#269900", "#990073", "#997300", "#00994d", "#4d0099", "#990000", "#994d00", "#004d99", "#992600", "#99004d", "#009973", "#990099", "#739900", "#990026", "#007399", "#4d9900", "#730099", "#009900", "#002699", "#009926", "#260099 " };
                String date = DateTime.Now.ToString("yyyy-MM-dd");

                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=MRT_GLOBALDB;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                //if (!dc.GetProductLicence(con).Contains("GlobalDashBoard,"))
                //{
                //    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>Global Dashboard Not Enabled For This Key. </h1>");
                //    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                //    return new MemoryStream(resultBytes1);
                //}

                //open connection

                text1 = text1 + "Highcharts.chart('container', {chart: {backgroundColor: '#201F1F'},title:{text: 'Global Hourly Production Report',style: {color: '#efefef'}},subtitle:{text: 'SmartMRT ',style: {color: '#efefef'}},yAxis:{title: {text: 'Piece Count',style: {color: '#efefef'}}}, legend:{layout: 'horizontal', align: 'center',verticalAlign: 'bottom',itemStyle: {font: '10pt Trebuchet MS, Verdana, sans-serif',color: 'white'},},plotOptions:{line: {dataLabels:{enabled: true,color: 'white',format: '{y} Pcs',inside: false,style: {fontWeight: 'bold'},}},series: { animation: false, label:{connectorAllowed: false }}},series: [";
                int hourlyflag = 0;

                //get production details
                SqlDataAdapter da = new SqlDataAdapter("SELECT MO_NO,MO_LINE,MIN(DATEPART(HOUR, TIME)) FROM HANGER_HISTORY where time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' and REMARKS='2' group by MO_NO,MO_LINE order by MIN(DATEPART(HOUR, TIME))", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                da.Dispose();
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    String mo = dt.Rows[j][0].ToString();
                    String moline = dt.Rows[j][1].ToString();
                    int count = 0;
                    hourlyflag = 1;

                    text1 = text1 + "\n{ name: \"" + mo + "-" + moline + "\", data :[";

                    da = new SqlDataAdapter("SELECT DATEPART(HOUR, TIME),MO_NO,MO_LINE,SUM(PC_COUNT) FROM HANGER_HISTORY where time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' and MO_NO='" + mo + "' and MO_LINE='" + moline + "' and REMARKS='2' GROUP BY DATEPART(HOUR, TIME),MO_NO,MO_LINE ORDER BY DATEPART(HOUR, TIME)", con);
                    DataTable dt5 = new DataTable();
                    da.Fill(dt5);
                    da.Dispose();
                    for (int i = 0; i < dt5.Rows.Count; i++)
                    {
                        String temp = dt5.Rows[i][3].ToString();
                        if (temp != "")
                        {
                            count = int.Parse(dt5.Rows[i][3].ToString());
                        }
                        else
                        {
                            count = 0;
                        }

                        DateTime date1 = Convert.ToDateTime(date + " " + dt5.Rows[i][0].ToString() + ":00:00");
                        double milliseconds = (date1 - start).TotalMilliseconds;
                        text1 += "[" + milliseconds + "," + count + "],";
                    }

                    text1 = text1.Remove(text1.Length - 1, 1);

                    text1 = text1 + "]},";
                }

                if (hourlyflag == 1)
                {
                    text1 = text1.Remove(text1.Length - 1, 1);
                }

                text1 += "],";
                text1 += "xAxis: {type: \"datetime\",title: {text: 'Hour of the Day'},labels: {formatter: function() {return Highcharts.dateFormat('%l:%M %p ', this.value );}}},";
                text1 += "responsive: {rules:[{condition:{ maxWidth: 500},chartOptions:{legend: {layout: 'horizontal', align: 'center', verticalAlign: 'bottom' }}}]}});";

                //text1 += "Highcharts.chart('container1', {chart:{plotBackgroundColor: null,plotBorderWidth: null,plotShadow: false, type: 'pie',backgroundColor: '#201F1F'},title: { text: 'MO Repair/Rework',style: {color: '#efefef'}},tooltip:{pointFormat: '{series.name}: <b>{point.percentage:.1f}% </b><br>Count : {point.y} Pcs'},accessibility:{point:{ valueSuffix: '%'} },plotOptions:{ pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: true,color: 'white',align: \"right\",format: '{y} Pcs',inside: false,style: {fontWeight: 'bold'}, format: '<b>{point.name}</b>: {point.percentage:.1f} % <br> Count : {point.y} Pcs' } } }, series:[{animation: false, name: 'Defects', colorByPoint: true, data: [";

                //int qcflag = 0;
                //int totalqc = 0;

                ////get production details
                //SqlDataAdapter sda = new SqlDataAdapter("select V_MO_NO,V_MO_LINE,SUM(I_QUANTITY) from QC_HISTORY where D_DATE_TIME>='" + date + " 00:00:00' and D_DATE_TIME<'" + date + " 23:59:59' group by V_MO_NO,V_MO_LINE ORDER BY V_MO_NO,V_MO_LINE", con);
                //dt = new DataTable();
                //sda.Fill(dt);
                //sda.Dispose();
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    String mo = dt.Rows[i][0].ToString();
                //    String moline = dt.Rows[i][1].ToString();
                //    int count = 0;

                //    String temp = dt.Rows[i][2].ToString();
                //    if (temp != "")
                //    {
                //        count = int.Parse(dt.Rows[i][2].ToString());
                //    }
                //    else
                //    {
                //        count = 0;
                //    }

                //    totalqc += count;
                //    text1 += "\n{ name: '" + mo + "-" + moline + "', y: " + count + "},\n";
                //    qcflag = 1;
                //}

                //if (qcflag == 1)
                //{
                //    text1 = text1.Remove(text1.Length - 1, 1);
                //}
                //text1 += "]}]});";

                //int unloadflag = 0;
                //int totalunload = 0;

                //text1 += "Highcharts.chart('container2', {chart:{plotBackgroundColor: null,plotBorderWidth: null,plotShadow: false, type: 'pie',backgroundColor: '#201F1F'},title: { text: 'MO Production',style: {color: '#efefef'}},tooltip:{pointFormat: '{series.name}: <b>{point.percentage:.1f}% </b><br>Count : {point.y} Pcs'},accessibility:{point:{ valueSuffix: '%'} },plotOptions:{ pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: true,color: 'white',align: \"right\",format: '{y} Pcs',inside: false,style: {fontWeight: 'bold'}, format: '<b>{point.name}</b>: {point.percentage:.1f} % <br> Count : {point.y} Pcs' } } }, series:[{animation: false, name: 'Production', colorByPoint: true, data: [";

                ////get production details
                //da = new SqlDataAdapter("SELECT MO_NO,MO_LINE,SUM(PC_COUNT) FROM HANGER_HISTORY where time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' and REMARKS='2' GROUP BY MO_NO,MO_LINE ORDER BY MO_NO,MO_LINE", con);
                //dt = new DataTable();
                //da.Fill(dt);
                //da.Dispose();
                //for (int j = 0; j < dt.Rows.Count; j++)
                //{
                //    String mo = dt.Rows[j][0].ToString();
                //    String moline = dt.Rows[j][1].ToString();

                //    int count = 0;
                //    String temp = dt.Rows[j][2].ToString();
                //    if (temp != "")
                //    {
                //        count = int.Parse(dt.Rows[j][2].ToString());
                //    }
                //    else
                //    {
                //        count = 0;
                //    }

                //    unloadflag = 1;
                //    totalunload += count;

                //    text1 += "\n{ name: '" + mo + "-" + moline + "', y: " + count + "},";
                //}
                //if (unloadflag == 1)
                //{
                //    text1 = text1.Remove(text1.Length - 1, 1);
                //}
                //text1 += "]}]});";

                //int loadflag = 0;
                //int totalload = 0;

                //text1 += "Highcharts.chart('container3', {chart:{plotBackgroundColor: null,plotBorderWidth: null,plotShadow: false, type: 'pie',backgroundColor: '#201F1F'},title: { text: 'MO Loaded',style: {color: '#efefef'}},tooltip:{pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b><br>Count : {point.y} Pcs'},accessibility:{point:{ valueSuffix: '%'} },plotOptions:{ pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: true,color: 'white',align: \"right\",format: '{y} Pcs',inside: false,style: {fontWeight: 'bold'}, format: '<b>{point.name}</b>: {point.percentage:.1f} %<br> Count : {point.y} Pcs' } } }, series:[{animation: false, name: 'Loaded', colorByPoint: true, data: [";

                ////get production details
                //da = new SqlDataAdapter("SELECT MO_NO,MO_LINE,SUM(PC_COUNT) FROM HANGER_HISTORY where time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' and REMARKS='1' GROUP BY MO_NO,MO_LINE ORDER BY MO_NO,MO_LINE", con);
                //dt = new DataTable();
                //da.Fill(dt);
                //da.Dispose();
                //for (int j = 0; j < dt.Rows.Count; j++)
                //{
                //    String mo = dt.Rows[j][0].ToString();
                //    String moline = dt.Rows[j][1].ToString();

                //    int count = 0;
                //    String temp = dt.Rows[j][2].ToString();
                //    if (temp != "")
                //    {
                //        count = int.Parse(dt.Rows[j][2].ToString());
                //    }
                //    else
                //    {
                //        count = 0;
                //    }

                //    loadflag = 1;
                //    totalload += count;
                //    text1 += "\n{ name: '" + mo + "-" + moline + "', y: " + count + "},";
                //}

                //if (loadflag == 1)
                //{
                //    text1 = text1.Remove(text1.Length - 1, 1);
                //}

                //text1 += "]}]});";
                text1 += "</script> ";

                //text1 = text1.Replace("unload000", totalunload.ToString());
                //text1 = text1.Replace("load000", totalload.ToString());
                //text1 = text1.Replace("qc000", totalqc.ToString());

                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + GetIPAddress() + "</label>";
                text1 = text1 + "<label id=\"remote\" for=\"Remote\" hidden>" + ipaddress + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 += "</body>";
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            text1 = text1.Replace("255.255.255.255", GetIPAddress());

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public System.IO.Stream Dashboard_Line(String lineno, String ipaddress, String usergroup)
        {
            String text1 = "";
            try
            {
                //read file
                if (File.Exists(DASHBOARD2))
                {
                    text1 = File.ReadAllText(DASHBOARD2);
                }

                DateTime start = Convert.ToDateTime("1970-01-01 00:00:00");
                String[] color = { "#990000", "#669900", "#000099", "#009999", "#999900", "#269900", "#990073", "#997300", "#00994d", "#4d0099", "#990000", "#994d00", "#004d99", "#992600", "#99004d", "#009973", "#990099", "#739900", "#990026", "#007399", "#4d9900", "#730099", "#009900", "#002699", "#009926", "#260099 " };
                String date = DateTime.Now.ToString("yyyy-MM-dd");

                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=MRT_GLOBALDB;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                //if (!dc.GetProductLicence(con).Contains("LineDashBoard,"))
                //{
                //    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>Line Dashboard Not Enabled For This Key. </h1>");
                //    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                //    return new MemoryStream(resultBytes1);
                //}

                //open connection
                MySqlConnection conn = new MySqlConnection("SERVER=" + ipaddress + ";DATABASE=mrt_local;UID=GUI;PASSWORD=octorite!;Connection Timeout=5;");
                conn.Open();

                text1 = text1 + "Highcharts.chart('container', {chart: {backgroundColor: '#201F1F'},title:{text: 'Hourly Production Report : Line - " + lineno + "',style: {color: '#efefef'}},subtitle:{text: 'SmartMRT ',style: {color: '#efefef'}},yAxis:{title: {text: 'Piece Count',style: {color: '#efefef'}}}, legend:{layout: 'horizontal', align: 'center',verticalAlign: 'bottom',itemStyle: {font: '10pt Trebuchet MS, Verdana, sans-serif',color: 'white'},},plotOptions:{line: {dataLabels:{enabled: true,color: 'white',format: '{y} Pcs',inside: false,style: {fontWeight: 'bold'},}},series: { animation: false, label:{connectorAllowed: false }}},series: [";

                int hourlyflag = 0;
                int totalunload = 0;

                //get production details
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT MO_NO,MO_LINE,MIN(HOUR(TIME)),MAX(SEQ_NO) FROM stationhistory s,stationdata h where time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' and h.INFEED_LINENO='" + lineno + "' and s.STN_ID=h.STN_ID group by MO_NO,MO_LINE order by MIN(HOUR(TIME))", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                da.Dispose();
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    String mo = dt.Rows[j][0].ToString();
                    String moline = dt.Rows[j][1].ToString();
                    String seqno = dt.Rows[j][3].ToString();

                    hourlyflag = 1;
                    int count = 0;
                    int flag = 0;

                    text1 = text1 + "\n{ name: \"" + mo + "-" + moline + "\", data :[";

                    da = new MySqlDataAdapter("SELECT HOUR(h.TIME),SUM(PC_COUNT) FROM stationdata s,stationhistory h WHERE s.STN_ID=h.STN_ID AND time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' AND s.INFEED_LINENO=" + lineno + " and h.MO_NO='" + mo + "' and h.MO_LINE='" + moline + "' and h.SEQ_NO='" + seqno + "' GROUP BY  HOUR(h.TIME)", conn);
                    DataTable dt5 = new DataTable();
                    da.Fill(dt5);
                    da.Dispose();
                    for (int i = 0; i < dt5.Rows.Count; i++)
                    {
                        String temp = dt5.Rows[i][1].ToString();
                        if (temp != "")
                        {
                            count = int.Parse(dt5.Rows[i][1].ToString());
                        }
                        else
                        {
                            count = 0;
                        }

                        totalunload += count;

                        DateTime date1 = Convert.ToDateTime(date + " " + dt5.Rows[i][0].ToString() + ":00:00");
                        double milliseconds = (date1 - start).TotalMilliseconds;

                        text1 += "[" + milliseconds + "," + count + "],";
                        flag = 1;
                    }

                    if (flag == 1)
                    {
                        text1 = text1.Remove(text1.Length - 1, 1);
                    }

                    text1 = text1 + "]},";
                }

                if (hourlyflag == 1)
                {
                    text1 = text1.Remove(text1.Length - 1, 1);
                }

                text1 += "],";
                text1 += "xAxis: {type: \"datetime\",title: {text: 'Hour of the Day'},labels: {formatter: function() {return Highcharts.dateFormat('%l:%M %p ', this.value);}}},";
                text1 += "responsive: {rules:[{condition:{ maxWidth: 500},chartOptions:{legend: {layout: 'horizontal', align: 'center', verticalAlign: 'bottom' }}}]}});";
                text1 += "Highcharts.chart('container1', {chart:{plotBackgroundColor: null,plotBorderWidth: null,plotShadow: false, type: 'pie',backgroundColor: '#201F1F'},title: { text: 'MO Repair/Rework',style: {color: '#efefef'}},tooltip:{pointFormat: '{series.name}: <b>{point.percentage:.1f}% </b><br>Count : {point.y} Pcs'},accessibility:{point:{ valueSuffix: '%'} },plotOptions:{ pie: { allowPointSelect: true, cursor: 'pointer', dataLabels: { enabled: true,color: 'white',align: \"right\",format: '{y} Pcs',inside: false,style: {fontWeight: 'bold'}, format: '<b>{point.name}</b>: {point.percentage:.1f} % <br> Count : {point.y} Pcs' } } }, series:[{animation: false, name: 'Defects', colorByPoint: true, data: [";

                int qcflag = 0;
                int totalqc = 0;

                //get production details
                SqlDataAdapter sda = new SqlDataAdapter("select V_MO_NO,V_MO_LINE,SUM(I_QUANTITY) from QC_HISTORY where D_DATE_TIME>='" + date + " 00:00:00' and D_DATE_TIME<'" + date + " 23:59:59' and I_STATION_ID like'" + lineno + ".%' group by V_MO_NO,V_MO_LINE ORDER BY V_MO_NO,V_MO_LINE", con);
                dt = new DataTable();
                sda.Fill(dt);
                sda.Dispose();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String mo = dt.Rows[i][0].ToString();
                    String moline = dt.Rows[i][1].ToString();
                    int count = 0;

                    String temp = dt.Rows[i][2].ToString();
                    if (temp != "")
                    {
                        count = int.Parse(dt.Rows[i][2].ToString());
                    }
                    else
                    {
                        count = 0;
                    }

                    totalqc += count;
                    text1 += "\n{ name: '" + mo + "-" + moline + "', y: " + count + "},\n";
                    qcflag = 1;
                }

                if (qcflag == 1)
                {
                    text1 = text1.Remove(text1.Length - 1, 1);
                }

                text1 += "]}]});";
                text1 += "Highcharts.chart('container2', {chart: {type: 'column',backgroundColor: '#201F1F'}, title: { text: 'Station WIP : Line - " + lineno + "',style: {color: '#efefef'}},legend: {itemStyle: {fontSize:'10px',font: '11pt Trebuchet MS, Verdana, sans-serif', color: 'white'}, itemHoverStyle: {color: '#FFF'}, itemHiddenStyle: { color: '#444' } },xAxis: {categories: ['Line : " + lineno + "'],crosshair: true},plotOptions: {series: {animation: false},column: {dataLabels: {enabled: true,crop: false,overflow: 'none',color: 'white'} }},credits: { enabled: false}, series: [";

                int wipflag = 0;

                //get production details
                da = new MySqlDataAdapter("SELECT sd.STN_NO_INFEED,COUNT(sh.HANGER_ID) FROM  balancehangers sh,stationdata sd WHERE sh.STN_ID=sd.STN_ID AND sd.INFEED_LINENO=" + lineno + " GROUP BY sd.STN_NO_INFEED ORDER BY sd.STN_NO_INFEED", conn);
                dt = new DataTable();
                da.Fill(dt);
                da.Dispose();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String mo = dt.Rows[i][0].ToString();
                    String count = dt.Rows[i][1].ToString();

                    text1 += "\n{ name: 'Station " + mo + "', data: [" + count + "]},\n";
                    wipflag = 1;
                }

                if (wipflag == 1)
                {
                    text1 = text1.Remove(text1.Length - 1, 1);
                }

                text1 += "]});";

                text1 += "</script> ";

                //get production details
                int totalload = 0;
                da = new MySqlDataAdapter("SELECT MO_NO,MO_LINE,MIN(SEQ_NO) FROM stationhistory s,stationdata h where time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' and h.INFEED_LINENO='" + lineno + "' and s.STN_ID=h.STN_ID group by MO_NO,MO_LINE order by MIN(HOUR(TIME))", conn);
                dt = new DataTable();
                da.Fill(dt);
                da.Dispose();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    String mo = dt.Rows[i][0].ToString();
                    String moline = dt.Rows[i][1].ToString();
                    int seq = int.Parse(dt.Rows[i][2].ToString());
                    int count = 0;

                    MySqlCommand cmd = new MySqlCommand("SELECT SUM(h.PC_COUNT) FROM stationdata s,stationhistory h WHERE s.STN_ID=h.STN_ID AND time>='" + date + " 00:00:00' and time<'" + date + " 23:59:59' AND s.INFEED_LINENO=" + lineno + " and h.MO_NO='" + mo + "' and h.MO_LINE='" + moline + "' and h.SEQ_NO='" + seq + "'", conn);
                    String temp = cmd.ExecuteScalar() + "";
                    if (temp != "")
                    {
                        count = int.Parse(cmd.ExecuteScalar() + "");
                    }
                    else
                    {
                        count = 0;
                    }

                    totalload += count;
                }

                text1 = text1.Replace("unload000", totalunload.ToString());
                text1 = text1.Replace("load000", totalload.ToString());
                text1 = text1.Replace("qc000", totalqc.ToString());
                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + GetIPAddress() + "</label>";
                text1 = text1 + "<label id=\"remote\" for=\"Remote\" hidden>" + ipaddress + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 += "</body>";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            text1 = text1.Replace("255.255.255.255", GetIPAddress());

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public Stream Production(String ipaddress, String usergroup)
        {
            String ip1 = GetIPAddress();
            String text1 = "";

            //read file
            if (File.Exists(PROD))
            {
                text1 = File.ReadAllText(PROD);
            }

            try
            {
                //open connection
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=MRT_GLOBALDB;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                //if (!dc.GetProductLicence(con).Contains("Production,"))
                //{
                //    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>Production Not Enabled For This Key. </h1>");
                //    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                //    return new MemoryStream(resultBytes1);
                //}

                //open connection
                MySqlConnection conn = new MySqlConnection("SERVER=" + ipaddress + ";" + "DATABASE=mrt_local;UID=GUI;PASSWORD=octorite!;");
                conn.Open();

                int total = 0;

                //get mo details
                MySqlDataAdapter sda = new MySqlDataAdapter("select distinct MO_NO, MO_LINE from stationhistory where time>='" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00'", conn);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                sda.Dispose();
                for (int p = 0; p < dt.Rows.Count; p++)
                {
                    String mo = dt.Rows[p][0].ToString();
                    String moline = dt.Rows[p][1].ToString();
                    String article = "";

                    DateTime op_starttime = DateTime.Now;
                    DateTime op_endtime = DateTime.Now;

                    int total_sam = 0;
                    int actual_production = 0;
                    int qc = 0;

                    //get article id
                    SqlCommand cmd = new SqlCommand("select V_ARTICLE_ID from MO_DETAILS where V_MO_NO='" + mo + "' and V_MO_LINE='" + moline + "'", con);
                    SqlDataReader sdr = cmd.ExecuteReader();
                    if (sdr.Read())
                    {
                        article = sdr.GetValue(0).ToString();
                    }
                    sdr.Close();

                    String getallop = "";
                    cmd = new SqlCommand("select GET_ALL_OPERATIONS FROM Setup", con);
                    sdr = cmd.ExecuteReader();
                    if (sdr.Read())
                    {
                        getallop = sdr.GetValue(0).ToString();
                    }
                    sdr.Close();

                    //get sum of sam
                    String temp = "";
                    if (getallop == "TRUE")
                    {
                        //get sum of sam , sum of piecerate and sum of overtime rate
                        cmd = new SqlCommand("select SUM(o.D_SAM) from DESIGN_SEQUENCE d,OPERATION_DB o where d.V_OPERATION_CODE=o.V_OPERATION_CODE and d.V_ARTICLE_ID=(select V_ARTICLE_ID from MO_DETAILS where V_MO_NO='" + mo + "' and V_MO_LINE='" + moline + "')", con);
                        temp = cmd.ExecuteScalar().ToString();
                        if (temp != "")
                        {
                            total_sam = int.Parse(temp + "");
                        }
                        else
                        {
                            total_sam = 0;
                        }
                    }
                    else
                    {
                        //get sum of sam , sum of piecerate and sum of overtime rate
                        cmd = new SqlCommand("select SUM(o.D_SAM) from DESIGN_SEQUENCE d,OPERATION_DB o where d.V_OPERATION_CODE=o.V_OPERATION_CODE and d.V_ARTICLE_ID=(select V_ARTICLE_ID from MO_DETAILS where V_MO_NO='" + mo + "' and V_MO_LINE='" + moline + "') and d.I_SEQUENCE_NO in(select s.I_SEQUENCE_NO from STATION_ASSIGN s where s.V_MO_NO='" + mo + "' and s.V_MO_LINE='" + moline + "' and s.I_STATION_ID!='0')", con);
                        temp = cmd.ExecuteScalar().ToString();
                        if (temp != "")
                        {
                            total_sam = int.Parse(temp + "");
                        }
                        else
                        {
                            total_sam = 0;
                        }
                    }

                    //get production details
                    MySqlCommand cmd1 = new MySqlCommand("select SUM(PC_COUNT) from stationhistory where MO_NO='" + mo + "' and MO_LINE='" + moline + "' and REMARKS='2' and TIME>='" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00'", conn);
                    temp = cmd1.ExecuteScalar() + "";
                    if (temp != "")
                    {
                        actual_production = int.Parse(cmd1.ExecuteScalar() + "");
                    }
                    else
                    {
                        actual_production = 0;
                    }

                    //get first and last hanger time
                    MySqlDataAdapter sda2 = new MySqlDataAdapter("SELECT MIN(TIME),MAX(TIME) FROM stationhistory where MO_NO='" + mo + "' and MO_LINE='" + moline + "' and TIME>='" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00'", conn);
                    DataTable dt2 = new DataTable();
                    sda2.Fill(dt2);
                    sda2.Dispose();
                    if (dt2.Rows.Count > 0)
                    {
                        if (dt2.Rows[0][0].ToString() != "")
                        {
                            op_starttime = Convert.ToDateTime(dt2.Rows[0][0].ToString());
                        }
                        if (dt2.Rows[0][1].ToString() != "")
                        {
                            op_endtime = Convert.ToDateTime(dt2.Rows[0][1].ToString());
                        }
                    }

                    //get sum repair
                    cmd = new SqlCommand("Select SUM(I_QUANTITY) from QC_HISTORY where D_DATE_TIME>='" + DateTime.Now.ToString("yyyy/MM/dd") + "' and V_MO_NO='" + mo + "' and V_MO_LINE='" + moline + "'", con);
                    temp = cmd.ExecuteScalar().ToString();
                    if (temp != "")
                    {
                        qc = int.Parse(temp + "");
                    }
                    else
                    {
                        qc = 0;
                    }

                    //calculate duration
                    TimeSpan ts = new TimeSpan();
                    ts = op_endtime - op_starttime;
                    int duration = (int)ts.TotalSeconds;
                    int target_production = duration / total_sam;

                    //calculate actual sam
                    decimal actual_sam = 0;
                    if (actual_production > 0)
                    {
                        actual_sam = (decimal)duration / (decimal)actual_production;
                    }

                    //calculate efficiency
                    decimal efficiency = 0;
                    if (actual_sam > 0)
                    {
                        efficiency = (total_sam / actual_sam) * 100;
                    }

                    //add rows to html table
                    text1 = text1 + "<tr>";
                    text1 = text1 + "<td>" + mo + "</td>";
                    text1 = text1 + "<td>" + moline + "</td>";
                    text1 = text1 + "<td>" + op_starttime.ToString("HH:mm:ss") + "</td>";
                    text1 = text1 + "<td>" + op_endtime.ToString("HH:mm:ss") + "</td>";
                    text1 = text1 + "<td>" + duration / 60 + "</td>";
                    text1 = text1 + "<td>" + target_production + "</td>";
                    text1 = text1 + "<td>" + actual_production + "</td>";
                    text1 = text1 + "<td>" + qc + "</td>";
                    text1 = text1 + "<td>" + total_sam + "</td>";
                    text1 = text1 + "<td>" + actual_sam.ToString("0.##") + "</td>";
                    text1 = text1 + "<td>" + efficiency.ToString("0.##") + "</td>";
                    text1 = text1 + "</tr>";

                    total += actual_production;
                }

                text1 += "<tr>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td>Total :</td>";
                text1 = text1 + "<td>" + total + "</td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";
                text1 = text1 + "<td></td>";

                text1 = text1 + "</tr></table>";
                text1 = text1 + "<label id=\"remote\" for=\"Remote\" hidden>" + ipaddress + "</label>";
                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 = text1 + "</body></html>";
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            text1 = text1.Replace("255.255.255.255", ip1);

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public Stream Login()
        {
            String ip1 = GetIPAddress();
            String text1 = "";
            String text2 = "";

            try
            {
                //read file
                if (File.Exists(lineplan1))
                {
                    text1 = File.ReadAllText(lineplan1);
                }
                if (File.Exists(login1))
                {
                    text2 = File.ReadAllText(login1);
                }

                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 += text2;
                text1 = text1.Replace("255.255.255.255", ip1);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public String GetLogin(String username, String password)
        {
            String loginstatus = "";
            //String sendOK = "";
            //sendOK += "\r\nHTTP/1.0 200 OK\r\n";
            //sendOK += "Content -Type:application/json\r\n";
            //sendOK += "Connection: close\r\n";
            //sendOK += "Access-Control-Allow-Origin: *\r\n";
            //sendOK += "\r\n";

            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                String pass = dc.EncryptPassword(password, false);

                SqlCommand cmd = new SqlCommand("Select V_USER_GROUP from USER_LOGIN where V_USERNAME='" + username + "' and V_PASSWORD='" + pass + "'", con);
                String usergroup = cmd.ExecuteScalar() + "";

                Console.WriteLine("Select V_USER_GROUP from USER_LOGIN where V_USERNAME='" + username + "' and V_PASSWORD='" + pass + "' :" + usergroup);
                if (usergroup == "")
                {
                    loginstatus += "FALSE;FALES";
                }
                else
                {
                    loginstatus += "TRUE;" + dc.EncryptPassword(username + "/" + usergroup, true) + "";
                }

                //sendOK += "\r\n";
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            return loginstatus;
        }

        public String GetUsergroup(String username)
        {
            String loginstatus = "FALSE";

            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                String pass = dc.DecryptPassword(username, true);
                String[] temp = pass.Split('/');

                if (temp.Length > 1)
                {
                    SqlCommand cmd = new SqlCommand("select V_ID from USER_LOGIN where V_USERNAME='" + temp[0] + "' and V_USER_GROUP='" + temp[1] + "'", con);
                    String id = cmd.ExecuteScalar() + "";

                    if (id != "")
                    {
                        loginstatus = "TRUE";
                    }
                }
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            return loginstatus;
        }

        public Stream TNA(String usergroup)
        {
            String ip1 = GetIPAddress();
            String text1 = "";
            String text2 = "";

            try
            {
                //read file
                if (File.Exists(lineplan1))
                {
                    text1 = File.ReadAllText(lineplan1);
                }
                if (File.Exists(tna1))
                {
                    text2 = File.ReadAllText(tna1);
                }

                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                if (!dc.GetProductLicence(con).Contains("TNA,"))
                {
                    byte[] resultBytes1 = Encoding.UTF8.GetBytes("<h1>TNA Not Enabled For This Key. </h1>");
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
                    return new MemoryStream(resultBytes1);
                }

                int minyear = int.Parse(DateTime.Now.ToString("yyyy")) - 5;
                int maxyear = int.Parse(DateTime.Now.ToString("yyyy")) + 5;

                text1 = text1 + "<div id='parent_div_2'> </br><div class='child_div_2'><label style=\"vertical-align:left;color:white\">Date : </label></div>";
                text1 += "<div class='child_div_2'></div><input type=\"date\" id=\"curdate\" name=\"tnadate\" value=\"" + DateTime.Now.ToString("yyyy-MM-dd") + "\" min=\"" + minyear + "-01-01\" max=\"" + maxyear + "-12-31\" onchange=\"getTNA()\"></div></div>";

                text1 += "<table style=\"width:100%\" id=\"table\" class=\"table\"> <tr id=\"headerRow\">";
                text1 += "<th>ID</th>";
                text1 += "<th>SALES ORDER</th>";
                text1 += "<th>TNA NAME</th>";
                text1 += "<th>TNA ACTIVITY</th>";
                text1 += "<th>TNA LEVEL</th>";
                text1 += "<th>START DATE</th>";
                text1 += "<th>END DATE</th>";
                text1 += "<th>STATUS</th>";
                text1 += "</tr>	";

                String pass = dc.DecryptPassword(usergroup, true);
                String[] temp = pass.Split('/');
                if (temp.Length > 1)
                {
                    DataTable dt = dc.SQLDataAdapter("select * from TNA_CONFIG c,TNA_ACTIVITY_DB a where CAST(GETDATE() AS DATE) BETWEEN cast(c.START_DATE as DATE) AND cast(c.END_DATE as DATE) and c.TNA_ACTIVITY=a.V_TNA_ACTIVITY and a.USER_GROUP='" + temp[1] + "'", con).Copy();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        text1 += "<tr>";
                        text1 += "<td>" + dt.Rows[i]["ID"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["SALES_ORDER"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["TNA"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["TNA_ACTIVITY"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["LEVEL"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["START_DATE"] + "</td>";
                        text1 += "<td>" + dt.Rows[i]["END_DATE"] + "</td>";

                        if (dt.Rows[i]["ACTIVITY_STATUS"].ToString() == "Complete")
                        {
                            text1 += "<td><input type=\"checkbox\" name=\"showHide\" onclick=\"Startload(this)\" /checked></td>";
                        }
                        else
                        {
                            text1 += "<td><input type=\"checkbox\" name=\"showHide\" onclick=\"Startload(this)\"/></td>";
                        }
                        text1 += "</tr>";
                    }
                }

                text1 += "</table>";

                text1 = text1 + "<label id=\"local\" for=\"Remote\" hidden>" + ip1 + "</label>";
                text1 = text1 + "<label id=\"usergroup\" for=\"Remote1\" hidden>" + usergroup + "</label>";
                text1 += text2;
                text1 = text1.Replace("255.255.255.255", ip1);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public String GetTNA(String usergroup, String date)
        {
            String loginstatus = "";
            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                String pass = dc.DecryptPassword(usergroup, true);
                String[] temp = pass.Split('/');

                if (temp.Length > 1)
                {
                    DataTable dt = dc.SQLDataAdapter("select * from TNA_CONFIG c,TNA_ACTIVITY_DB a where '" + date + "' BETWEEN cast(c.START_DATE as DATE) AND cast(c.END_DATE as DATE) and c.TNA_ACTIVITY=a.V_TNA_ACTIVITY and a.USER_GROUP='" + temp[1] + "'", con).Copy();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        loginstatus += dt.Rows[i]["ID"] + "*" + dt.Rows[i]["SALES_ORDER"] + "*" + dt.Rows[i]["TNA"] + "*" + dt.Rows[i]["TNA_ACTIVITY"] + "*" + dt.Rows[i]["LEVEL"] + "*" + dt.Rows[i]["START_DATE"] + "*" + dt.Rows[i]["END_DATE"] + "*" + dt.Rows[i]["ACTIVITY_STATUS"] + "+";
                    }
                }

                if (loginstatus != "")
                {
                    loginstatus = loginstatus.Remove(loginstatus.Length - 1, 1);
                }
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }
            return loginstatus;
        }

        public String TNAStatus(String id, String status)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();

                dc.SQLCommand_ExecuteNonQuery("UPDATE TNA_CONFIG SET ACTIVITY_STATUS='" + status + "',COMPLETED_DATE='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ID='" + id + "'", con);
            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }
            return "";
        }

        public Stream DailyProd(String usergroup)
        {
            String ip1 = GetIPAddress();
            String text1 = "";
            String text2 = "";

            try
            {
                //read file
                if (File.Exists(lineplan1))
                {
                    text1 = File.ReadAllText(lineplan1);
                }
                if (File.Exists(dailyprod1))
                {
                    text2 = File.ReadAllText(dailyprod1);
                }

                SqlConnection con = new SqlConnection("Data Source=" + LocalIP + ",1433;Network Library=DBMSSOCN;Initial Catalog=SAP_VPT;User ID=sa;Password=1234;Connection Timeout=5");
                con.Open();


            }
            catch (Exception ex)
            {
                dc.WriteToExFile(ex + "");
            }

            byte[] resultBytes = Encoding.UTF8.GetBytes(text1);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);
        }

        public string Echo(string text) { return text; }
        Stream StringToStream(string result)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(result));
        }
        public Stream GetSilverlightPolicy()
        {
            string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <access-policy>
                    <cross-domain-access>
                        <policy>
                            <allow-from http-request-headers=""*"">
                                <domain uri=""*""/>
                            </allow-from>
                            <grant-to>
                                <resource path=""/"" include-subpaths=""true""/>
                            </grant-to>
                        </policy>
                    </cross-domain-access>
                </access-policy>";
            return StringToStream(result);
        }

        public Stream GetFlashPolicy()
        {
            string result = @"<?xml version=""1.0""?>
                <!DOCTYPE cross-domain-policy SYSTEM ""http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd"">
                    <cross-domain-policy>
                        <allow-access-from domain=""*"" />
                    </cross-domain-policy>";
            return StringToStream(result);
        }
    }    
}

