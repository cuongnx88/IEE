using IEE.Infrastructure.DbContext;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System;
using System.IO;
using System.Data;
using System.Web;
using System.Data.OleDb;
using System.Collections.Generic;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected virtual new Model.CustomPrincipal User
        {
            get { return HttpContext.User as Model.CustomPrincipal; }
        }

        public string EmailBuilder(int type)
        {
            string content = string.Empty;
            string src = string.Empty;
            switch (type)
            {
                case 1:
                    src = "1.StudentRegisterRecord.html";
                    break;
                case 2:
                    src = "2.StudentUploadDraffSendToHelper.html";
                    break;
                case 3:
                    src = "3.StudentUploadDraffSendToStudent.html";
                    break;
                case 4:
                    src = "4.EditorUploadDraffSendToAll.html";
                    break;
                case 5:
                    src = "5.DraffIsFinal.html";
                    break;
                case 6:
                    src = "6.RecordDocIsFinal.html";
                    break;
            }
            var sr = new StreamReader(Server.MapPath("\\Emailtemplate\\") + src );
            content = sr.ReadToEnd();
            return content;
        }
        public MailAddress EmailTo { get; set; }
        public MailAddress EmailFrom { get; set; }
        public string EmailTitle { get; set; }
        public string EmailBody { get; set; }
        public void Send()
        {

            using (MailMessage mailMessage = new MailMessage())
            {

                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);

                mailMessage.Subject = EmailTitle;
                mailMessage.Body = EmailBody;
                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(EmailTo);

                SmtpClient smtp = new SmtpClient();

                smtp.Host = ConfigurationManager.AppSettings["Host"];
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

                NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];

                NetworkCred.Password = ConfigurationManager.AppSettings["Password"];

                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;

                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

                smtp.Send(mailMessage);

            }
        }
        public DataSet Render(HttpRequestBase request)
        {
            DataSet ds = new DataSet();
            if (request.Files["file"].ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/upload/") + request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }

                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();
                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                }

            }
            return ds;
        }
        public List<Program> Programs()
        {
            List<Program> progs = new List<Program>() {
                new Program {Id=1, Name="IELTS" },
                new Program {Id=2, Name="TOEFL" },
                new Program {Id=3, Name="SAT" }
            };
            return progs;
        }
        
    }

    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}