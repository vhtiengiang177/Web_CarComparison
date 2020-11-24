using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarComparison.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult Index()
        {

            ViewBag.mess = "https://www.youtube.com/embed/ABGSfMZOjUM";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult MyAcc()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Comparing()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        //SqlConnection con = new SqlConnection();
        //SqlCommand com = new SqlCommand();
        //SqlDataReader dr;
        // GET: Account
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //void connectionString()
        //{
        //    con.ConnectionString = "data source=LAPTOP-GVT76T3A; database=Web2710; integrated security=SSPI";
        //}

        //bool Login(string username, string password)
        //{
        //    return LoginDAO.Instance.Login(username, password);
        //}

        //[HttpGet]
        //public ActionResult Verify(string username, string password)
        //{
        //    //if (Login(username, password))
        //    //{
        //    //    return View("Index");
        //    //}
        //    //else
        //    //{
        //    //    return View("Login");

        //    //}

             
        //}

        public ActionResult Register(string username, string password, string password2)
        {
            //if (LoginDAO.Instance.Register(username, password, password2))
            //{
            //    return View("Success");
            //}
            //else
                return View("Error");
           
        }


        public ActionResult RegisterView()
        {
            return View("Register");
        }

    }
}