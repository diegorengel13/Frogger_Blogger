using System;
using System.Web;

using System.Web.Mvc;

namespace Frogger_Blogger.Models
{
    public class AllowHtmlBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;

            var comment = bindingContext.ModelName;

            return request.Unvalidated[comment];
        }
    }
}