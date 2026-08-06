using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldOS
{
    public class User
    {
        public String name { get; set; }
        public String pw {  get; set; }

        public ArrayList Memory { get; set; }

        public User (String name, String pw)
        {
            this.name = name;
            this.pw = pw;
        }



    }
}
