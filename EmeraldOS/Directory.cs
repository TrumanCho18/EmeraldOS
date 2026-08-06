using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldOS
{
    public class Directory
    {
        public String name {  get; set; }
        public Directory Parent { get; set; }
        public ArrayList Children { get; set; }

        public Directory(String name, Directory Parent) { 
            this.name = name;
            this.Parent = Parent;
            Children = new ArrayList();
        }

    }
}
