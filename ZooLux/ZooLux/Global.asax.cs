using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Linq;
using System.Security.Principal;
using ZooLux.Models.Data;

namespace ZooLux
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Create method of processing authentication requests
        protected void Application_AuthenticateRequest()
        {
            // Check, that the user is authorized
            if (User == null)
                return;

            // Get the name of user
            string userName = Context.User.Identity.Name;

            // Assign an array of roles
            string[] roles = null;

            using (Db db = new Db())
            {
                // Fill the roles
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                if (dto == null)
                    return;

                roles = db.UserRoles
                    .Where(x => x.UserId == dto.Id)
                    .Select(x => x.Role.Name)
                    .ToArray();
            }

            // Create the object of interface IPrincipal
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // Assign and initialize of data Context.User
            Context.User = newUserObj;
        }
    }
}