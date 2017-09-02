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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;

namespace GoogleCalenderSyncUp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> IndexAsync(CancellationToken cancellationToken,string getDataFromGoogleApi)
        {

            //Authenticate with Google
            string userName = User.Identity.Name;
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = userManager.FindByName(userName);
            var result = await new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                        AuthorizeAsync(cancellationToken);

            if (!user.IsGoogleConnected)
            {
                if (result.Credential != null)
                {

                    var service = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = result.Credential,
                        ApplicationName = "ASP.NET MVC Sample"
                    });

                    user.IsGoogleConnected = true;
                    IdentityResult userUpdateResult = await userManager.UpdateAsync(user);

                    if (userUpdateResult.Succeeded)
                    {


                        // YOUR CODE SHOULD BE HERE..
                        // SAMPLE CODE:
                        var list = await service.Files.List().ExecuteAsync();
                        ViewBag.Message = "FILE COUNT IS: " + list.Items.Count();
                    }
                }
                else
                {
                    return new RedirectResult(result.RedirectUri);
                }
            }
            ViewBag.getDataFromGoogleApi = getDataFromGoogleApi;
            return View("CalenderEvents");

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

        public ActionResult CalenderEvents()
        {
            
                        return View();
        }

        public IList<CalenderEventsViewModel> GetCalenderEvents()
        {
            IList<CalenderEventsViewModel> userCalenderEvents = new List<CalenderEventsViewModel>();
            string userName = User.Identity.Name;

            if (ViewBag.getDataFromGoogleApi) {

                //Get the credential object to check if the user is authorized to access Google API
                
                    //Calender Service used to authenticate using OAuth
                    string[] Scopes = { CalendarService.Scope.CalendarReadonly };
                    string ApplicationName = "Google Calendar API .NET Quickstart";
                    UserCredential userCredential;

                    using (var stream =
                        new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                    {
                        string credPath = System.Environment.GetFolderPath(
                            System.Environment.SpecialFolder.Personal);
                        credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                        userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)).Result;

                        CredentialDbContext credentialDbContext = new CredentialDbContext();

                        credentialDbContext.Set(userCredential.GetType()).Attach(userCredential);
                        credentialDbContext.SaveChanges();
                    }

                    //Save response from GoogleAPI with all the token information in a new CredentialResponse table



                    var result = new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                                AuthorizeAsync(CancellationToken.None);


                    // Create Google Calendar API service.
                    var service = new CalendarService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = userCredential,
                        ApplicationName = ApplicationName,
                    });

                    // Define parameters of request.
                    EventsResource.ListRequest request = service.Events.List("primary");
                    request.TimeMin = DateTime.Now;
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 10;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                    // List events.
                    Events events = request.Execute();
                    //events object has all the paremeter required for the table
                    if (events.Items != null && events.Items.Count > 0)
                    {
                        foreach (var eventItem in events.Items)
                        {
                            string when = eventItem.Start.DateTime.ToString();
                            if (String.IsNullOrEmpty(when))
                            {
                                when = eventItem.Start.Date;
                            }

                        }
                    }
                

                //Add a dummy data in the GoogleCalenderEvents table. Once value is retrieved from the google api , it can be put using GoCalenderEvents directly here.


                /*
                GoogleCalenderEventsDbContext context = new GoogleCalenderEventsDbContext();

                var events1 = new List<CalenderEventsViewModel>
                { new CalenderEventsViewModel
                { AccessRole="SampleAccessRole",Description="SampleDescription",
                ETag="SampleETag",Kind="SampleKind",NextPageToken="SampleNextPageToken",
                NextSyncToken="SampleNextSyncToken",Summary="SampleSummary",TimeZone="SampleTimeZone",
                Updated=DateTime.Now,UpdatedRaw="SampleUpdatedRaw",UserName=User.Identity.Name}
                };

                events1.ForEach(e => context.events.Add(e));
                context.SaveChanges();
                */


            }
            else
            {
                //get data from db
                GoogleCalenderEventsDbContext context = new GoogleCalenderEventsDbContext();
                var eventList = from c in context.events where c.UserName == userName
                            select c;
                foreach(var item in eventList)
                {
                    userCalenderEvents.Add(item);
                }
            }
            return userCalenderEvents;
        }

        public UserCredential getUserCredential()
        {
            UserCredential userCredential=null;
            string userName = User.Identity.Name;
            CredentialDbContext credentialContext = new CredentialDbContext();
            //TODO
            //put where condition to get only credential of that user
            var query = from c in credentialContext.responses where c.UserName==userName select c;

            //TODO: get expiration date and check if it is still valid
            foreach(var item in query)
            {
                TokenResponse tokenResponse = new TokenResponse();
                tokenResponse.AccessToken = item.AccessToken;
                tokenResponse.ExpiresInSeconds = item.ExpiresInSeconds;
                tokenResponse.IdToken = item.IdToken;
                tokenResponse.IssuedUtc = item.IssuedUtc;
                tokenResponse.Scope = item.Scope;

                userCredential = new UserCredential(null, item.UserId, tokenResponse);
            }

            return userCredential;
        }
    }
}