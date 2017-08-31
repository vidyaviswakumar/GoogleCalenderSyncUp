using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using GoogleCalenderSyncUp.App_Start;
using Microsoft.AspNet.Identity.EntityFramework;
using GoogleCalenderSyncUp.Models;
using Microsoft.AspNet.Identity;

namespace GoogleCalenderSyncUp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> IndexAsync(CancellationToken cancellationToken)
        {          
            
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                    AuthorizeAsync(cancellationToken);

                if (result.Credential != null)
                {
                    var service = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = result.Credential,
                        ApplicationName = "ASP.NET MVC Sample"
                    });
                string userName = User.Identity.Name;
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                ApplicationUser user=userManager.FindByName(userName);
                user.IsGoogleConnected = true;
                IdentityResult userUpdateResult = await userManager.UpdateAsync(user);

                if (userUpdateResult.Succeeded)
                {

                    // YOUR CODE SHOULD BE HERE..
                    // SAMPLE CODE:
                    var list = await service.Files.List().ExecuteAsync();
                    ViewBag.Message = "FILE COUNT IS: " + list.Items.Count();
                }
                    return View("CalenderEvents");
                
                }
                else
                {
                    return new RedirectResult(result.RedirectUri);
                }
            }

        public ActionResult Index()
        {     

            return View();
        }

        public ActionResult ConnectToGoogleAccount()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}