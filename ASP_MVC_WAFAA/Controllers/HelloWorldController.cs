using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace ASP_MVC_WAFAA.Controllers
{
    public class HelloWorldController : Controller
    {
        // GET: /HelloWorld/
        public IActionResult Index()
        {
            return View();
        }

        /* public string Index()
         {
             return "This is my default action...";
         }*/
        //https:/ / localhost:{PORT}/HelloWorld/Welcome?name=Rick&numtimes=4
        // GET: /HelloWorld/Welcome/
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;
            return View();
        }
        /*public string Welcome()
        {
            return "This is the Welcome action method...";
        }*/
    }
}
