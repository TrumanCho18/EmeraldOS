using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldOS
{
    public class File
    {

        public String name {  get; set; }
        public Directory Parent { get; set; }
        public ArrayList body {  get; set; }

        public File(String name, ArrayList bodyTxt)
        {
            this.name = name;
            this.body = bodyTxt;
        }

        public File(String name)
        {
            this.name = name;
            this.body = new ArrayList();
        }

        public void Oras()
        {

        }

    }
}
