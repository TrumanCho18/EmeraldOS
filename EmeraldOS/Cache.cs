using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldOS
{
    public class Cache
    {
        public String name {  get; set; }
        public String value { get; set; }
        public String type {  get; set; }

        public Cache(String name, String value)
        {
            this.name = name;
            this.value = value;

            if (int.TryParse(value, out _))
            {
                type = "int";
            } else if (double.TryParse(value, out _))
            {
                type = "double";
            } else
            {
                type = "String";
            }
        }

        public void Set(String value)
        {
            this.value = value;
            if (int.TryParse(value, out _))
            {
                type = "int";
            }
            else if (double.TryParse(value, out _))
            {
                type = "double";
            }
            else
            {
                type = "String";
            }
        }
    }
}
