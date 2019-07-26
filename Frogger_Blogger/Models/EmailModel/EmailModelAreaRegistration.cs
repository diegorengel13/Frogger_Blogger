using System.Web.Mvc;

namespace Frogger_Blogger.Areas.EmailModel
{
    public class EmailModelAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EmailModel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EmailModel_default",
                "EmailModel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}