using IEE.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IEE.Web.Business
{
    public abstract class BaseViewPage : WebViewPage
    {
        public virtual new Model.CustomPrincipal User
        {
            get { return base.User as Model.CustomPrincipal; }
        }
    }
    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new Model.CustomPrincipal User
        {
            get { return base.User as Model.CustomPrincipal; }
        }
    }
}