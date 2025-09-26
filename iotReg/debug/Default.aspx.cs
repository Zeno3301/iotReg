using System;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.Script.Serialization;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Web.UI.WebControls;
using DataModule.Security;
using DataModule.SQL;
using DevExpress.Web;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
{
    Response.Clear();
    Response.ContentEncoding = System.Text.Encoding.UTF8;
    Response.Charset = "utf-8";
    Response.ContentType = "text/plain"; // Явно указываем тип контента
    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
    HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

    if (Request.HttpMethod == "OPTIONS")
    {
        Response.StatusCode = 200;
        Response.Write("OPTIONS-запрос обработан");
        Response.End();
        return;
    }


    User cuser = DataModule.Security.User.CurentUser();

    Response.Write("UserID: " + cuser.ID + "\n");

    
    Response.End();
}


    private void SendError(int code, string message)
    {
        Response.StatusCode = code;
        Response.ContentType = "text/html; charset=utf-8";
        Response.Write("<h3 style='color:red;'>Ошибка: " + HttpUtility.HtmlEncode(message) + "</h3>");
        Response.End();
    }
}
