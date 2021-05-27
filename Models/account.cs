using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace paskaita.Models
{
    public class Account
    {
        public int id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }

        //testas

    }
}