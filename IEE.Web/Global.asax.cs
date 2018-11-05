using IEE.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IEE.Web;
using System.Web.Security;

using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;

namespace IEE.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Run();

            //System.Data.Entity.Database.SetInitializer(new IeeData());
        }
        
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {

                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                Model.CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<Model.CustomPrincipalSerializeModel>(authTicket.UserData);
                Model.CustomPrincipal newUser = new Model.CustomPrincipal(authTicket.Name);
                newUser.UserId = serializeModel.UserId;
                newUser.Name = serializeModel.Name;
                newUser.Roles = serializeModel.Roles;

                HttpContext.Current.User = newUser;
            }

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            //handle exceptions, send them via email, whatever
            var exception = Server.GetLastError();


            if (IsMaxRequestExceededException(this.Server.GetLastError()))
            {
                this.Server.ClearError();
                Response.Redirect("~/ttn_content/media");

            }

        }
        //protected void Session_Start(Object sender, EventArgs e)
        //{

        //}

        const int TimedOutExceptionCode = -2147467259;
        private static bool IsMaxRequestExceededException(Exception e)
        {
            // unhandled errors = caught at global.ascx level
            // http exception = caught at page level

            Exception main;
            var unhandled = e as HttpUnhandledException;

            if (unhandled != null && unhandled.ErrorCode == TimedOutExceptionCode)
            {
                main = unhandled.InnerException;
            }
            else
            {
                main = e;
            }


            var http = main as HttpException;

            if (http != null && http.ErrorCode == TimedOutExceptionCode)
            {
                // hack: no real method of identifying if the error is max request exceeded as 
                // it is treated as a timeout exception
                if (http.StackTrace.Contains("GetEntireRawContent"))
                {
                    // MAX REQUEST HAS BEEN EXCEEDED
                    return true;
                }
            }

            return false;
        }
    }
    public static class StringHelpers
    {
        public static string ToSeoUrl(this string url)
        {
            // make the url lowercase
            string encodedUrl = (url ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl.ToAscii(), @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }
        static public String ToAscii(this String unicode)
        {
            unicode = Regex.Replace(unicode, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
            unicode = Regex.Replace(unicode, "[óòỏõọôồốổỗộơớờởỡợ]", "o");
            unicode = Regex.Replace(unicode, "[éèẻẽẹêếềểễệ]", "e");
            unicode = Regex.Replace(unicode, "[íìỉĩị]", "i");
            unicode = Regex.Replace(unicode, "[úùủũụưứừửữự]", "u");
            unicode = Regex.Replace(unicode, "[ýỳỷỹỵ]", "y");
            unicode = Regex.Replace(unicode, "[đ]", "d");
            //unicode = Regex.Replace(unicode, "[-\\s+/]+", "-");
            unicode = Regex.Replace(unicode, "\\W+", " "); //Nếu bạn muốn thay dấu khoảng trắng thành dấu "_" hoặc dấu cách " " thì thay kí tự bạn muốn vào đấu "-"
            return unicode;
        }

        public static string GenerateURL(this string Title)
        {
            if (string.IsNullOrEmpty(Title))
                return "";
            Title = ToAscii(Title);
            string strTitle = Title.ToString();

            //#region Generate SEO Friendly URL based on Title

            strTitle = strTitle.Trim();
            strTitle = strTitle.Trim('-');

            strTitle = strTitle.ToLower();
            char[] chars = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
            strTitle = strTitle.Replace("c#", "C-Sharp");
            strTitle = strTitle.Replace("vb.net", "VB-Net");
            strTitle = strTitle.Replace("asp.net", "Asp-Net");
            strTitle = strTitle.Replace(".", "-");
            for (int i = 0; i < chars.Length; i++)
            {
                string strChar = chars.GetValue(i).ToString();
                if (strTitle.Contains(strChar))
                {
                    strTitle = strTitle.Replace(strChar, string.Empty);
                }
            }
            strTitle = strTitle.Replace(" ", "-");

            strTitle = strTitle.Replace("--", "-");
            strTitle = strTitle.Replace("---", "-");
            strTitle = strTitle.Replace("----", "-");
            strTitle = strTitle.Replace("-----", "-");
            strTitle = strTitle.Replace("----", "-");
            strTitle = strTitle.Replace("---", "-");
            strTitle = strTitle.Replace("--", "-");
            strTitle = strTitle.Trim();

            return strTitle;
        }
    }

    public static class ExtensionMethods
    {
        public static MvcHtmlString DropDownGroupList<T>(
            this HtmlHelper helper,
            IEnumerable<T> items,
            Func<T, bool> parentItemsPredicate,
            Func<T, T, bool> parentChildAssociationPredicate,
            string dataValueField,
            string dataTextField)
        {
            var html = new StringBuilder("<select>");

            foreach (var item in items.Where(parentItemsPredicate))
            {
                html.Append(string.Format("<optgroup label=\"{0}\">", item.GetType().GetProperty(dataTextField).GetValue(item, null)));

                foreach (var child in items.Where(x => parentChildAssociationPredicate(x, item)))
                {
                    var childType = child.GetType();

                    html.Append(string.Format("<option value=\"{0}\">{1}</option>", childType.GetProperty(dataValueField).GetValue(child, null), childType.GetProperty(dataTextField).GetValue(child, null)));
                }

                html.Append("</optgroup>");
            }

            html.Append("</select>");

            return new MvcHtmlString(html.ToString());
        }
    }

    public static class DynamicProxyHelpers
    {
        public static T UnProxy<T>(DbContext context, T proxyObject) where T : class
        {
            var proxyCreationEnabled = context.Configuration.ProxyCreationEnabled;
            try
            {
                context.Configuration.ProxyCreationEnabled = false;
                T poco = context.Entry(proxyObject).CurrentValues.ToObject() as T;
                return poco;
            }
            finally
            {
                context.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            }
        }
    }
}
