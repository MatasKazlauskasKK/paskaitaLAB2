using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using paskaita.Models;
using System.Threading.Tasks;
using MySqlConnector;
using Dapper;


namespace paskaita.Controllers
{
    public class AccountController : Controller
    {

        private string cn_string = @"server=localhost; user id=root; password=Namai19; database=moodle2; pooling=false;";

        // GET: Account
        public ActionResult Login()
        {
            if (Request.Cookies["UserName"] != null)
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
            }
            if (Request.Cookies["UserId"] != null)
            {
                Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);
            }
            if (Request.Cookies["UserRole"] != null)
            {
                Response.Cookies["UserRole"].Expires = DateTime.Now.AddDays(-1);
            }
            


            return View();
        }
        [HttpPost]
        public ActionResult Verify(Account acc)
        {
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT Name, Role, id, Surname FROM accounts WHERE UserName='" + acc.UserName + "' AND Password='" + acc.Password + "'";

                List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                if (account.Count > 1)  // Klaida nes toki vartotojai yra keli ( taip neturietu butu ) 
                {
                    ViewBag.Message = "Username or password is wrong";
                    return RedirectToAction("Login", "Account");
                }
                else if (account.Count > 0)
                {

                    //Cookies
                    HttpCookie userProfile = new HttpCookie("UserName")
                    {
                        Value = acc.Name,
                        Expires = DateTime.Now.AddMinutes(30)
                    };
                    Response.Cookies.Add(userProfile);
                    
                    
                    HttpCookie userProfileId = new HttpCookie("UserId")
                    {
                        Value = account[0].id.ToString(),
                        Expires = DateTime.Now.AddMinutes(30)
                    };
                    Response.Cookies.Add(userProfileId);
                    HttpCookie userProfileRole = new HttpCookie("UserRole")
                    {
                        Value = account[0].Role.ToString(),
                        Expires = DateTime.Now.AddMinutes(30)
                    };
                    Response.Cookies.Add(userProfileRole);
                    HttpCookie userProfileSurname = new HttpCookie("UserSurname")
                    {
                        Value = account[0].Surname.ToString(),
                        Expires = DateTime.Now.AddMinutes(30)
                    };
                    Response.Cookies.Add(userProfileSurname);
                    //end of cokies

                    TempData["userName"] = acc.Name;
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("CustomError", "Wrong username/password");

            return View("Login", acc);
        }

        [HttpPost]
        public async Task<ActionResult> LoadUser(Account acc)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var cn = new MySqlConnection(cn_string))
                {
                    string sQuery = "SELECT Name, Role, Surname, id FROM accounts WHERE UserName='" + acc.Name + "' AND Password='"+acc.Password+"'";

                    List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                    if (account.Count > 1)  // Klaida nes toki vartotojai yra keli ( taip neturietu butu ) 
                    {
                        return View();
                    }
                    else if (account.Count > 0) // Viskas gerai
                    {
                        return View();
                    }
                }
                return View(); // Tokio vartotojo nera
            });
        }
        public ActionResult Register()
        {


            return View();
        }
        [HttpPost]
        public ActionResult VerifyRegister(Account acc)
        {
            using (var cn = new MySqlConnection(cn_string))
            {

                string sQuery = "SELECT Name, Role, Surname, id FROM accounts WHERE UserName='" + acc.Name + "' AND Password='" + acc.Password + "'";

                List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                if (account.Count > 0) // Vartotojas su tokiu username jau yra
                {
                    ModelState.AddModelError("CustomError", "Toks vartatojas jau yra");

                    return View("Register", acc);
                }
                else
                {
                    try
                    {
                        sQuery = "INSERT INTO accounts (`UserName`, `Name`, `Surname`, `Password`) VALUES (@userName, @name, @surname, @password)";
                        var ids2 = cn.ExecuteAsync(sQuery, new
                        {
                            userName = acc.UserName,
                            name = acc.Name,
                            surname = acc.Surname,
                            password = acc.Password,

                        }).Result;
                    }
                    catch  // Klaida pridedant vartotoja
                    {
                        ModelState.AddModelError("CustomError", "Klaida pridedant vartotają");

                        return View("Register", acc);
                    }
                }
            }
            return RedirectToAction("Index", "Home");  // Viskas ok 
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(Account acc)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var cn = new MySqlConnection(cn_string))
                {

                    string sQuery = "SELECT Name accounts WHERE Name='" + acc.Name + "'";

                    List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                    if (account.Count > 0) // Vartotojas su tokiu username jau yra
                    {
                        return View();
                    }
                    else
                    {
                        try
                        {
                            sQuery = "INSERT INTO accounts (`UserName`, `Name`, `Surname`, `Password`) VALUES (@userName, @name, @surname, @password)";
                            var ids2 = cn.ExecuteAsync(sQuery, new
                            {
                                userName=acc.UserName,
                                name = acc.Name,
                                surname=acc.Surname,
                                password = acc.Password,

                            }).Result;
                        }
                        catch  // Klaida pridedant vartotoja
                        {
                            return View();
                        }
                    }
                }
                return View();  // Viskas ok 
            });
        }

        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var cn = new MySqlConnection(cn_string))
                {
                    string sQuery = "SELECT Username, Name, Role, Surname, id FROM accounts";

                    List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                }

                return View(); // Visi vartotojai 
            });
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(Account acc)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var cn = new MySqlConnection(cn_string))
                {

                    string sQuery = "SELECT Name accounts WHERE UsernName='" + acc.Name + "'";

                    List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                    if (account.Count > 0) // Vartotojas su tokiu username jau yra
                    {
                        sQuery = "UPDATE accounts SET" + "`Username` = @userName, `Name`=@name, `Surname`=@surname, `Role`=@role WHERE (`id` = @ID); ";
                        var ids2 = cn.ExecuteAsync(sQuery, new
                        {
                            ID=acc.id,
                            userName = acc.UserName,
                            name = acc.Name,
                            surname = acc.Surname,
                            role=acc.Role
                        }).Result;

                    }
                    else
                    {
                        // negalima tokio redaghuoti nes tokio nera
                    }
                }
                return View();  // Viskas ok 
            });
        }
        public ActionResult AccountManage()
        {
            


            return View();
        }
        public ActionResult ManageAccount(FormCollection form)
        {
            string User_Name = Request.Cookies["UserName"].Value;
            string User_Surname = Request.Form["User_Surname"];
            string User_Username = Request.Form["UserUserName"];
            int User_ProfileId = Convert.ToInt32(Request.Cookies["UserId"].Value);
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT Name FROM accounts WHERE UserName='" + User_Name + "'";

                List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                if (account.Count > 0) // Vartotojas su tokiu username jau yra
                {
                    sQuery = "UPDATE accounts SET" +
                    " `Surname` = '" + User_Surname + "' WHERE (`id` = '" + User_ProfileId + "'); "; 
                    var ids2 = cn.ExecuteAsync(sQuery, new
                    {
                        ID = User_ProfileId,
                        
                        name = User_Name,
                        
                        
                    }).Result;
                    ModelState.AddModelError("CustomError", "Vardas pakeistas sekmingai");
                    return View("AccountManage");
                }
                else
                {
                    // negalima tokio redaghuoti nes tokio nera
                    ModelState.AddModelError("CustomError", "Klaida keičiant vardą");
                    return View("AccountManage");
                }

            }

            
        }

    }
}