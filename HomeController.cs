using Microsoft.AspNetCore.Mvc;

namespace GM.Service.GLProcess.Controllers
{
    public class HomeController : Controller  
    {
        public string Index()   
        {
           return "Repo GL Process API";   
        }
    }
}