using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySqlConnector;
using Dapper;
using paskaita.Models;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

namespace paskaita.Controllers
{
    public class MoodlyController : Controller
    {
        private string cn_string = @"server=localhost; user id=root; password=Namai19; database=moodle2; pooling=false;";
        //MySqlConnection db_contex = new MySqlConnection("server=localhost; user id=root; password=Namai19; database=moodle2; pooling=false;");
        // GET: Moodly

        public ActionResult Classes()
        {

            List<Models.Course.ClassRoom> account;
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT * FROM classrooms";

                account = cn.QueryAsync<Models.Course.ClassRoom>(sQuery).Result.ToList();
            } // Visos pamokos

            ViewData["Modules"] = account;
            return View();
        }
        public ActionResult Class(int? id, FormCollection form)
        {
            List<Models.Course.Theme> theme = new List<Course.Theme>();

            if (id != null)
            {

                using (var cn = new MySqlConnection(cn_string))
                {
                    string sQuery = "SELECT * FROM themes WHERE class_id='" + id + "'";

                    theme = cn.QueryAsync<Models.Course.Theme>(sQuery).Result.ToList();

                    for (int i = 0; i < theme.Count(); i++)
                    {
                        theme[i].ThemeBlocks = GetBlocks(theme[i].id);
                    }
                } // Visos pasirinktos klases temos
                ViewData["mod_topics"] = theme;
                ViewBag.ClassId = id;
                return View("Theme");

            }
            ViewData["mod_topics"] = theme;
            ViewBag.ClassId = id;
            // Truksta klases id
            return View("Theme");



        }
        public List<Models.Course.ThemeBlock> GetBlocks(int id)
        {

            List<Models.Course.ThemeBlock> themeBlock;
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT * FROM theme_blocks WHERE Theme_id='" + id + "'";

                themeBlock = cn.QueryAsync<Models.Course.ThemeBlock>(sQuery).Result.ToList();
            } // Visos pasirinktos klases temos
            return themeBlock;


        }
        public ActionResult AddTheme()
        {
            return View();
        }
        [HttpPost]

        public ActionResult AddNewTheme(Models.Course.Theme theme, Models.Course.ClassRoom classRoom, int? id, FormCollection form)
        {
            if (theme != null && classRoom != null)
            {
                using (var cn = new MySqlConnection(cn_string))
                {
                    string courseId = Request.Form["class_id"];
                    theme.Title = form["Topic_name"];
                    classRoom.Id = Convert.ToInt32(courseId);
                    string title = theme.Title;
                    try
                    {
                        string sQuery = "INSERT INTO themes (`class_id`, `Title`) VALUES (@id, @title)";
                        var ids2 = cn.ExecuteAsync(sQuery, new
                        {
                            id = classRoom.Id,
                            title = theme.Title,

                        }).Result;


                    }
                    catch  // Klaida pridedant tema
                    {

                        return View("Theme");
                    }

                }

                return RedirectToAction("Class", "Moodly", new { id = classRoom.Id }); // Viskas ok 

            }
            return View("Theme");  // Truksta duomenu
        }
        public ActionResult RemoveTheme(Models.Course.Theme theme, Models.Course.ClassRoom classRoom, int? id, FormCollection form)
        {
            if (theme != null && classRoom != null)
            {
                using (var cn = new MySqlConnection(cn_string))
                {
                    string courseId = Request.Form["class_id"];
                    theme.Title = form["Topic_name"];
                    classRoom.Id = Convert.ToInt32(courseId);
                    string title = theme.Title;
                    try
                    {

                        string sQuery = "DELETE FROM themes WHERE class_id ='" + classRoom.Id + "' AND Title='" + theme.Title + "'";

                        var ids2 = cn.ExecuteAsync(sQuery, new
                        {
                            id = classRoom.Id,
                            title = theme.Title,

                        }).Result;


                    }
                    catch  // Klaida trinant tema
                    {

                        return View("Index");
                    }

                }

                return RedirectToAction("Class", "Moodly", new { id = classRoom.Id }); // Viskas ok 

            }
            return View("Theme");  // Truksta duomenu
        }
        public ActionResult AddNewThemeBlock(Models.Course.ThemeBlock ThemeBlock, Models.Course.ClassRoom classRoom, int? id, FormCollection form)
        {
            if (ThemeBlock != null && classRoom != null)
            {
                string courseId = Request.Form["class_id"];
                ThemeBlock.Title = form["Topic_name"];
                classRoom.Id = Convert.ToInt32(courseId);
                string title = ThemeBlock.Title;
                using (var cn = new MySqlConnection(cn_string))
                {

                    try
                    {
                        string sQuery = "INSERT INTO theme_blocks (`theme_id`, `title`, `text`) VALUES (@id, @title, @text)";
                        var ids2 = cn.ExecuteAsync(sQuery, new
                        {
                            id = classRoom.Id,
                            title = ThemeBlock.Title,
                            text = "",

                        }).Result;


                    }
                    catch  // Klaida pridedant tema
                    {

                        return View("Theme");
                    }

                }
                TempData["ClassId"] = courseId;
                return RedirectToAction("Class", "Moodly", new { id = classRoom.Id }); // Viskas ok 

            }
            return View("Theme");  // Truksta duomenu
        }
        public ActionResult AddStudent(FormCollection form)
        {
            string courseId = Request.Form["class_id"];
            int course_id = Convert.ToInt32(courseId);
            List<Models.Account> account;
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT * FROM accounts";

                account = cn.QueryAsync<Models.Account>(sQuery).Result.ToList();
            } // Visos pamokos

            ViewData["Accounts"] = account;
            ViewBag.ClassId = course_id;
            return View();

        }
        public ActionResult StudentList(FormCollection form)
        {
            string courseId = Request.Form["class_id"];
            int course_id = Convert.ToInt32(courseId);
            List<Models.Course.ClassStudents> account;
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT * FROM class_students WHERE class_id='" + course_id + "'";

                account = cn.QueryAsync<Models.Course.ClassStudents>(sQuery).Result.ToList();
            } // Visos pamokos

            ViewData["Accounts"] = account;
            ViewBag.ClassId = course_id;
            return View();

        }
        public ActionResult AddStudentToClass(FormCollection form, Course.ClassStudents ClassStudents)
        {

            string courseId = Request.Form["class_id"];
            string accountId = Request.Form["account_id"];
            string accountName = Request.Form["account_name"];
            string accountSurname = Request.Form["account_surname"];
            int course_id = Convert.ToInt32(courseId);
            int account_id = Convert.ToInt32(accountId);
            ViewBag.ClassId = course_id;
            ViewBag.Sucess = "Pridėta";



            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT account_id,class_id FROM class_students WHERE account_id='" + account_id + "' AND class_id='" + course_id + "'";
                List<Course.ClassStudents> account = cn.QueryAsync<Course.ClassStudents>(sQuery).Result.ToList();
                if (account.Count > 0)
                {
                    TempData["Sucess"] = "Studentas jau yra šioje klasėja";
                    TempData["ClassId"] = course_id;
                    return RedirectToAction("AddStudent", "Moodly", new { id = course_id });
                }
                else
                {
                    try
                    {
                        sQuery = "INSERT INTO class_students (`class_id`, `account_id`,`account_name`,`account_surname`) VALUES (@class_id, @account_id,@account_name,@account_surname)";
                        var ids3 = cn.ExecuteAsync(sQuery, new
                        {
                            class_id = course_id,
                            account_id = account_id,
                            account_name = accountName,
                            account_surname = accountSurname,

                        }).Result;


                    }
                    catch  // Klaida pridedant vartotoją    
                    {

                        TempData["Sucess"] = "Klaida";
                        TempData["ClassId"] = course_id;
                        return RedirectToAction("AddStudent", "Moodly", new { id = course_id });
                    }
                }
                TempData["Sucess"] = "Pridėta";
                TempData["ClassId"] = course_id;
                return RedirectToAction("AddStudent", "Moodly", new { id = course_id });

            }
        }
        public ActionResult DeleteStudentClass(FormCollection form, Course.ClassStudents ClassStudents)
        {

            string courseId = Request.Form["class_id"];
            string accountId = Request.Form["account_id"];
            int course_id = Convert.ToInt32(courseId);
            int account_id = Convert.ToInt32(accountId);
            ViewBag.ClassId = course_id;
            ViewBag.Sucess = "Pašalinta";



            using (var cn = new MySqlConnection(cn_string))
            {

                string sQuery = "DELETE FROM class_students WHERE account_id ='" + account_id + "' AND class_id='" + course_id + "'";
                cn.ExecuteAsync(sQuery, commandTimeout: 300);




                TempData["Sucess"] = "Pašalinta";
                TempData["ClassId"] = course_id;
                return RedirectToAction("StudentList", "Moodly", new { id = course_id });

            }
        }
    }
}

