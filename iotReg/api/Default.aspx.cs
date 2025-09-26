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
        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

        if (Request.HttpMethod == "OPTIONS")
        {
            Response.StatusCode = 200;
            Response.End();
            return;
        }

        try
        {
            // Получаем ID студента из параметров запроса
            string studentIdParam = Request.QueryString["studentId"];
            if (string.IsNullOrEmpty(studentIdParam))
            {
                SendError(400, "Отсутствует параметр studentId");
                return;
            }

            int studentId = int.Parse(studentIdParam);

            // Вызываем хранимую процедуру
            List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("publicbase.[dbo].[iot_proc]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@operation", SqlDbType.Int).Value = 2; // Операция выбора курсов
                command.Parameters.Add("@id", SqlDbType.Int).Value = studentId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i];
                        }
                        resultList.Add(row);
                    }
                }
            }

            // Возвращаем данные в формате JSON
            var json = new JavaScriptSerializer().Serialize(new { data = resultList });
            Response.ContentType = "application/json";
            Response.Write(json);
        }
        catch (Exception ex)
        {
            SendError(500, "Ошибка: " + ex.Message);
        }
        finally
        {
            Response.End();
        }
    }

    private void SendError(int code, string message)
    {
        Response.StatusCode = code;
        Response.ContentType = "text/html; charset=utf-8";
        Response.Write("<h3 style='color:red;'>Ошибка: " + HttpUtility.HtmlEncode(message) + "</h3>");
        Response.End();
    }
}