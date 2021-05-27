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


            return View();
        }
        [HttpPost]
        public ActionResult Verify(Account acc)
        {
            using (var cn = new MySqlConnection(cn_string))
            {
                string sQuery = "SELECT Name, Role FROM accounts WHERE Name='" + acc.Name + "' AND Password='" + acc.Password + "'";

                List<Account> account = cn.QueryAsync<Account>(sQuery).Result.ToList();
                if (account.Count > 1)  // Klaida nes toki vartotojai yra keli ( taip neturietu butu ) 
                {
                    ViewBag.Message = "Username or password is wrong";
                    return RedirectToAction("Login", "Account");
                }
                else if (account.Count > 0)
                {
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
                        sQuery = "UPDATE accounts SET" +
                        "`Username` = @userName, `Name`=@name, `Surname`=@surname, `Role`=@role WHERE (`id` = @ID); ";
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

    }
}