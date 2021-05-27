using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using MySqlConnector;
using Dapper;
using paskaita.Models;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;


namespace paskaita.Controllers
{
    public class HomeController : Controller
    {
        private string cn_string = @"server=localhost; user id=root; password=Namai19; database=moodle2; pooling=false;";
        MySqlConnection db_contex = new MySqlConnection("server=localhost; user id=root; password=Namai19; database=moodle2; pooling=false;");
        public ActionResult Index()
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