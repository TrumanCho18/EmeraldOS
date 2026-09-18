using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldOS
{
    public class EMInterpreter
    {
        public File inp { get; set; }

        ArrayList memory = new ArrayList();

        public EMInterpreter()
        {
            
        }

        public void Interpret()
        {
            //x = 4
            //print('x')
            ArrayList tokens = LexAnalyze();
            ArrayList outp = new ArrayList();

            memory.Clear();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].ToString() == "print")
                {
                    if (Type(tokens[i + 1].ToString()) == "literal")
                    {
                        Console.WriteLine(Dequote(tokens[i + 1].ToString()));
                    } else if (Type(tokens[i + 1].ToString()) == "var") {

                        if (MemSearch(tokens[i + 1].ToString()) != null)
                        {
                            Console.WriteLine(MemSearch(tokens[i + 1].ToString()).value);
                        } else
                        {
                            Console.WriteLine("ERROR, value " + tokens[i + 1].ToString() + " not found in system memory");
                        }   
                    }
                    i++;
                } else if (tokens[i].ToString() == "if")
                {
                    //if
                    //(x==5
                    //{
                    //print("hi")
                    //}
                    String condition = tokens[i + 1].ToString().Substring(1);
                    i += 2;
                    int iSkip = i;
                    while (tokens[iSkip].ToString() != "}")
                    {
                        iSkip++;
                    }

                    if (!Solve(condition))
                    {
                        i = iSkip;
                    }
                    

                } else
                {
                    if (i < tokens.Count - 2 && isOperator(tokens[i + 1].ToString()) && (Type(tokens[i + 2].ToString()) == "literal" || Type(tokens[i + 2].ToString()) == "var"))
                    {
                        Boolean stay = true;
                        String val = tokens[i + 2].ToString();

                        for (int j = 0; j < memory.Count; j++)
                        {
                            Cache c = (Cache)memory[j];
                            if (c.name == val)
                            {
                                val = c.value;
                                break;
                            }
                        }

                        for (int j = 0; j < memory.Count; j++)
                        {
                            Cache c = (Cache) memory[j];
                            if (c.name == tokens[i].ToString())
                            {
                                c.value = val;
                                stay = false;
                                break;
                            }
                        }

                        if (stay)
                        {
                            Cache cache = new Cache(tokens[i].ToString(), val);
                            memory.Add(cache);
                        }

                        i += 2;
                    } else if (i < tokens.Count - 2 && isOperator(tokens[i + 1].ToString()) && Type(tokens[i + 2].ToString()) == "function")
                    {
                        if (tokens[i + 2].ToString() == "readln[]")
                        {
                            Boolean stay = true;
                            for (int j = 0; j < memory.Count; j++)
                            {
                                Cache c = (Cache)memory[j];
                                if (c.name == tokens[i].ToString())
                                {
                                    c.value = Console.ReadLine();
                                    stay = false;
                                    break;
                                }
                            }

                            if (stay)
                            {
                                Cache cache = new Cache(tokens[i].ToString(), Console.ReadLine());
                                memory.Add(cache);
                            }

                            i += 2;
                        }
                    }
                }
            }

            for (int i = 0; i < outp.Count; i++)
            {
                Console.WriteLine(outp[i]);
            }
        }

        public void TokenList()
        {
            ArrayList tokens = LexAnalyze();

            for (int i = 0; i < tokens.Count; i++)
            {
                Console.WriteLine(tokens[i]);
            }
        }

        public ArrayList LexAnalyze()
        {

            ArrayList tokens = new ArrayList();

            for (int i = 0; i < inp.body.Count; i++)
            {
                String line = inp.body[i].ToString();
                
                String buffer = "";
                for (int j = 0; j <= line.Length; j++)
                {
                    if ((isPunctuator(line[j].ToString()) || j == line.Length) && buffer != "")
                    {
                        tokens.Add(buffer);
                        buffer = "";
                    } else
                    {
                        buffer += line[j].ToString();
                    }
                }
            }
            return tokens;
        }

        public Boolean isPunctuator(String str)
        {
            if (str.Equals(" ") || str.Equals("(") || str.Equals(")")   )
            {
                return true;
            }

            return false;
        }

        public Boolean isOperator(String str)
        {
            if (str.Equals("=") || str.Equals("+="))
            {
                return true;
            }

            return false;
        }

        public String Dequote(String str)
        {
            if (str.StartsWith("\"") && str.EndsWith("\"")) {
                return str.Substring(1, str.Length - 2);
            }
            return str;
        }
        public String Type(String str)
        {
            if (str.StartsWith("\"") || double.TryParse(str, out _) || int.TryParse(str, out _))
            {
                return "literal";
            } else if (str.EndsWith("[]"))
            {
                return "function";
            } else
            {
                return "var";
            }
        }

        public Cache MemSearch(String targetName)
        {
            for (int j = 0; j < memory.Count; j++)
            {
                Cache c = (Cache)memory[j];
                if (c.name == targetName)
                {
                    return c;
                }
            }
            return null;
        }

        public Boolean Solve(String str)
        {
            String temp1;
            String temp2;
            String op1;
            String op2;
            double i1;
            double i2;

            if (str.Contains(">="))
            {
                temp1 = str.Split(">=")[0];
                temp2 = str.Split(">=")[1];

                if (Type(temp1) == "var")
                {
                    i1 = int.Parse(MemSearch(temp1).value);
                }
                else
                {
                    i1 = int.Parse(temp1);
                }

                if (Type(temp2) == "var")
                {
                    i2 = int.Parse(MemSearch(temp2).value);
                }
                else
                {
                    i2 = int.Parse(temp2);
                }

                return (i1 >= i2);
            }
            else if (str.Contains(">"))
            {
                temp1 = str.Split(">")[0];
                temp2 = str.Split(">")[1];

                if (Type(temp1) == "var")
                {
                    i1 = int.Parse(MemSearch(temp1).value);
                }
                else
                {
                    i1 = int.Parse(temp1);
                }

                if (Type(temp2) == "var")
                {
                    i2 = int.Parse(MemSearch(temp2).value);
                }
                else
                {
                    i2 = int.Parse(temp2);
                }

                return (i1 > i2);
            } else if (str.Contains("<="))
            {
                temp1 = str.Split("<=")[0];
                temp2 = str.Split("<=")[1];

                if (Type(temp1) == "var")
                {
                    i1 = int.Parse(MemSearch(temp1).value);
                }
                else
                {
                    i1 = int.Parse(temp1);
                }

                if (Type(temp2) == "var")
                {
                    i2 = int.Parse(MemSearch(temp2).value);
                }
                else
                {
                    i2 = int.Parse(temp2);
                }

                return (i1 <= i2);
            } else if (str.Contains("<"))
            {
                temp1 = str.Split("<")[0];
                temp2 = str.Split("<")[1];

                if (Type(temp1) == "var")
                {
                    i1 = int.Parse(MemSearch(temp1).value);
                }
                else
                {
                    i1 = int.Parse(temp1);
                }

                if (Type(temp2) == "var")
                {
                    i2 = int.Parse(MemSearch(temp2).value);
                }
                else
                {
                    i2 = int.Parse(temp2);
                }

                return (i1 < i2);
            } else if (str.Contains("=="))
            {
                temp1 = str.Split("==")[0];
                temp2 = str.Split("==")[1];

                if (Type(temp1) == "var")
                {
                    op1 = MemSearch(temp1).value;
                } else
                {
                    op1 = Dequote(temp1);
                }

                if (Type(temp2) == "var")
                {
                    op2 = MemSearch(temp2).value;
                }
                else
                {
                    op2 = Dequote(temp2);
                }

                return (op1 == op2);
            }
            return false;
        }

    }
}
