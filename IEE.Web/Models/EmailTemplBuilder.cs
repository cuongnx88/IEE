using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using PagedList;
using System.IO;

namespace IEE.Web
{
    public class EmailTemplBuilder
    {
        public static string Builder(int type)
        {
            string content = string.Empty;
            switch (type)
            {
                case 1:
                    content = System.Web.HttpContext.Current.Server.MapPath("~/Emailtemplate/1.StudentRegisterRecord.html");
                    break;
                case 2:
                    content = System.Web.HttpContext.Current.Server.MapPath("~/Emailtemplate/2.StudentUploadDraffSendToHelper.html");
                    break;
                case 3:
                    content = System.Web.HttpContext.Current.Server.MapPath("~/Emailtemplate/3.StudentUploadDraffSendToStudent.html");
                    break;
                case 4:
                    content = System.Web.HttpContext.Current.Server.MapPath("~/Emailtemplate/4.EditorUploadDraffSendToAll.html");
                    break;
                case 5:
                    content = System.Web.HttpContext.Current.Server.MapPath("~/Emailtemplate/5.DraffIsFinal.html");
                    break;
                case 6:
                    content = System.Web.HttpContext.Current.Server.MapPath("~/Emailtemplate/6.RecordDocIsFinal.html");
                    break;
            }
            return content;
        }
    }
}