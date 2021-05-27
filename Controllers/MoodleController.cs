using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MySqlConnector;
using Dapper;

namespace paskaita.Controllers
{
    public class MoodleController : Controller
    {
        private string cn_string = @"server=localhost; user id=root; password=Namai19; database=moodle2; pooling=false;";

        // GET: Moodle
        public ActionResult Index()
        {

            return View();
        }

        #region Rooms
        [HttpGet]
        public async Task<ActionResult> LoadClassRooms()
        {
            return await Task.Factory.StartNew(() =>
            {
                List<Models.Course.ClassRoom> account;
                using (var cn = new MySqlConnection(cn_string))
                {
                    string sQuery = "SELECT * FROM classrooms";

                    account = cn.QueryAsync<Models.Course.ClassRoom>(sQuery).Result.ToList();
                } // Visos pamokos
                return View(account);
            });
        }

        #endregion

        #region Themes
        [HttpGet]
        public async Task<ActionResult> GetThemes(Models.Course.ClassRoom classRoom)
        {
            if (classRoom != null)
            {
                return await Task.Factory.StartNew(() =>
                {
                    List<Models.Course.Theme> theme;
                    using (var cn = new MySqlConnection(cn_string))
                    {
                        string sQuery = "SELECT * FROM themes WHERE id='" + classRoom.Id + "'";

                        theme = cn.QueryAsync<Models.Course.Theme>(sQuery).Result.ToList();

                        for (int i=0; i<theme.Count(); i++)
                        {
                            theme[i].ThemeBlocks=GetBlocks(theme[i].id);
                        }
                    } // Visos pasirinktos klases temos
                    return View();
                });
            }
            // Truksta klases id
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddTheme(Models.Course.Theme theme, Models.Course.ClassRoom classRoom)
        {
            if (theme != null && classRoom != null)
            {
                return await Task.Factory.StartNew(() =>
                {
                    using (var cn = new MySqlConnection(cn_string))
                    {
                        try
                        {
                            string sQuery = "INSERT INTO theme (`class_id`, `Title`) VALUES (@id, @title)";
                            var ids2 = cn.ExecuteAsync(sQuery, new
                            {
                                id = classRoom.Id,
                                title = theme.Title,

                            }).Result;
                        }
                        catch  // Klaida pridedant tema
                        {
                            return View();
                        }

                    }
                    return View();  // Viskas ok 
                });
            }
            return View();  // Truksta duomenu
        }

        [HttpPost]
        public async Task<ActionResult> AddThemeBlock(Models.Course.Theme theme, Models.Course.ThemeBlock themeBlock)
        {
            if (theme != null)
            {
                return await Task.Factory.StartNew(() =>
                {
                    using (var cn = new MySqlConnection(cn_string))
                    {
                        try
                        {
                            string sQuery = "INSERT INTO theme_blocks (`Theme_id`, `Title`, `Text`) VALUES (@id, @title, @text)";
                            var ids2 = cn.ExecuteAsync(sQuery, new
                            {
                                id = theme.id,
                                title = themeBlock.Title,
                                text = themeBlock.Text
                            }).Result;
                        }
                        catch  // Klaida pridedant tema
                        {
                            return View();
                        }

                    }
                    return View();  // Viskas ok 
                });
            }
            return View();  // Truksta duomenu
        }

        public async Task<ActionResult> GetBlocks(Models.Course.Theme theme)
        {
            if (theme != null)
            {
                return await Task.Factory.StartNew(() =>
                {
                    List<Models.Course.ThemeBlock> themeBlock;
                    using (var cn = new MySqlConnection(cn_string))
                    {
                        string sQuery = "SELECT * FROM themes_blocks WHERE Theme_id='" + theme.id + "'";

                        themeBlock = cn.QueryAsync<Models.Course.ThemeBlock>(sQuery).Result.ToList();
                    } // Visos pasirinktos klases temos
                    return View();
                });
            }
            // Truksta klases id
            return View();

            #endregion
        }

        public List<Models.Course.ThemeBlock> GetBlocks(int id)
        {
                         
                    List<Models.Course.ThemeBlock> themeBlock;
                    using (var cn = new MySqlConnection(cn_string))
                    {
                        string sQuery = "SELECT * FROM themes_blocks WHERE Theme_id='" + id + "'";

                        themeBlock = cn.QueryAsync<Models.Course.ThemeBlock>(sQuery).Result.ToList();
                    } // Visos pasirinktos klases temos
                    return themeBlock;


        }
    }
}