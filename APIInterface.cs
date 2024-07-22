using System.ServiceModel;
using System.ServiceModel.Web;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace ConsoleApp4
{
    [ServiceContract]

    public interface APIInterface
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Login/")]
        Stream Login();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetLogin/{username}/{password}/")]
        String GetLogin(String username, String password);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetUsergroup/{usergroup}/")]
        String GetUsergroup(String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/HOME/{usergroup}")]
        Stream HOME(String usergroup);

        [OperationContract]
        [WebGet(UriTemplate = "File/{imageName}")]
        Stream FetchImage(String imageName);

        [OperationContract]
        [WebGet(UriTemplate = "Files/{file}")]
        Stream RetrieveFile(String file);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/LinePlan/{usergroup}")]
        Stream LinePlan(String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetLines/{factory}")]
        String GetLines(String factory);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetLinePlan/{factory}/{line}/{usergroup}/{date}")]
        Stream GetLinePlan(String factory, String line, String usergroup, String date);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/WashPlan/{usergroup}")]
        Stream WashPlan(String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/DASHBOARD/{ipaddress}/{usergroup}")]
        Stream Dashboard(String ipaddress, String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/DASHBOARD/LINE/{lineno}/{ipaddress}/{usergroup}")]
        Stream Dashboard_Line(String lineno, String ipaddress, String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Production/{ipaddress}/{usergroup}")]
        Stream Production(String ipaddress, String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/TNA/{usergroup}")]
        Stream TNA(String usergroup);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetTNA/{usergroup}/{date}")]
        String GetTNA(String usergroup, String date);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/TNAStatus/{id}/{status}")]
        String TNAStatus(String id, String status);

        //[OperationContract]
        //[WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/DailyProd/{usergroup}")]
        //Stream DailyProd(String usergroup);

        //[OperationContract]
        //[WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, UriTemplate = "/GetLogin/{username}/{password}/")]
        //String GetLogin(String username, String password);

        //[OperationContract]
        //[WebInvoke(UriTemplate = "/GetLogin/{username}/{password}/", Method = "GET", RequestFormat = WebMessageFormat.Xml)]

        //String GetLogin(String username, String password);
    }    
}