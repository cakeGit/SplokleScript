using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace sks
{
    public static class script {
        public static List<sksvar> variables = new List<sksvar>();
        public static string scriptFile;
    }

    public static class ExtensionClass
    {
        // Extension method to append the element
        public static T[] Append<T>(this T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Add(item);

            return list.ToArray();
        }
       public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperties()
                .Single(PI => PI.Name == propertyName)
                .GetValue(obj, null);
        }
    }
    public class mainInterpreter
    {
        static readonly ConsoleColor DefConsCol = ConsoleColor.White;

        static void InfoLog(string text, bool onlycol = false) {
            Console.ForegroundColor = ConsoleColor.Magenta;
            if (!onlycol) {
                Console.WriteLine("[INFO <"+ DateTime.Now.TimeOfDay+"> ]: " + text);
            } else {
                Console.WriteLine(text);
            }
            Console.ForegroundColor = DefConsCol;
        }
        static void DebugLog(string text, bool onlycol = false) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!onlycol) {
                Console.WriteLine("[DEBUG <"+ DateTime.Now.TimeOfDay+"> ]: " + text);
            } else {
                Console.WriteLine(text);
            }
            Console.ForegroundColor = DefConsCol;
        }

        static void ErrorLog(string text, string line, bool onlycol = false) {
            Console.ForegroundColor = ConsoleColor.Red;
            if (!onlycol) {
                Console.WriteLine("[ERROR <"+ DateTime.Now.TimeOfDay+"> ]: " + text + " {From " + line + " }");
            } else {
                Console.WriteLine(text);
            }
            Console.ForegroundColor = DefConsCol;
        }

        static sksvar recallVariable (string varname) {
            return Array.Find(script.variables.ToArray(), element => element.getVarName() == varname);
        }

        static void LogBreak() {
            Console.WriteLine();
        }

        static int SaferIndexOf (string str, string splitter) { // lazy function for lazy people like me :) dont use if .IndexOf() will do the job
            return (str.IndexOf(splitter) == -1) ? str.Length : str.IndexOf(splitter);
        }
        static string SafeTextAfterStr (string str, String chara) {
            return str.Substring(SaferIndexOf(str, chara), str.Length-SaferIndexOf(str, chara));
        }
        static string CorrectedSafeTextAfterStr (string str, String chara) {
            return str.Substring(SaferIndexOf(str, chara)+1, str.Length-SaferIndexOf(str, chara)-1);
        }
        static string SafeTextBeforeStr (string str, String chara) {
            return str.Substring(0, SaferIndexOf(str, chara));
        }
        static string SafeTextBeforeEitherStr (string str, string[] chars) {
            int textLength = -1;
            foreach (string character in chars) {
                if (SaferIndexOf(str, character) < textLength || textLength == -1) {
                    textLength = SaferIndexOf(str, character);
                }
            }
            return str.Substring(0, textLength);
        }
        static string SafeTextAfterEitherStr (string str, string[] chars) {
            int textLength = -1;
            foreach (string character in chars) {
                if (SaferIndexOf(str, character) < textLength || textLength == -1) {
                    textLength = SaferIndexOf(str, character);
                }
            }
            return str.Substring(textLength+1, str.Length-textLength-1);
        }
        
        static bool debugmode;

        static void Main(string[] args)
        {
            string sFile = args[0];
            sks.script.scriptFile = sFile;

            debugmode = args.Contains("debug");

            foreach(var item in args)
            {
                Console.WriteLine(item.ToString());
            }

            if (debugmode == true) {
                InfoLog("Interpreter Startup");
                LogBreak();
                InfoLog("The interpreter has noticed that dev / debug mode is active", true);
                InfoLog("Currently this will just recompile the interpreter on run and give some logs about the interpreter when im going to make an actual build", true);
                InfoLog("Disable this via debugmode.txt in the Splookle Script directory, and change true to false", true);
                LogBreak();
                InfoLog("Given script file: "+sFile, true);
                LogBreak();
            }
            
            string[] lines = File.ReadAllLines(sFile);
            foreach (string lineUntrimmed in lines) {
                string line = lineUntrimmed.Trim();

                //Console.WriteLine(line);

                /*Getting the first "word" to see if it is any of the following
                    A variable  
                    A setting -- coomin soon mate dont rush me
                thats it.*/

                string lineprefix = SafeTextBeforeStr(line, " ");

                bool noprefix = false;
                string npText = SafeTextAfterStr(line, " ");

                switch (lineprefix) 
                {
                    case "var":
                        //InfoLog("Given var line prefix");

                        npText = npText.Trim();

                        string varName = SafeTextBeforeStr(npText, "=").Trim();

                        string varVal = SafeTextAfterStr(npText, "=").Trim().Substring(1, SafeTextAfterStr(npText, "=").Trim().Length-1);

                        sksvar newVar = new sksvar(varName, new sksvar.numValue(float.Parse(varVal)));

                        sksvar.numValue NVdat = (sks.sksvar.numValue) newVar.getDataObj();

                        //InfoLog(varName);
                        //Console.WriteLine(newVar.GetType());
                        //InfoLog(NVdat.getValue().ToString());
                        //Console.WriteLine(script.variables);

                        script.variables.Add(newVar);

                        /*script.variables.ForEach(delegate(sksvar var) {
                            Console.WriteLine(var.getVarName());
                        });*/
                        break;
                    default:
                        noprefix = true;
                        break;
                }
                
                if (noprefix) {
                    //DebugLog(SafeTextBeforeEitherStr(line, new string[] {"(", "."}));
                    switch (SafeTextBeforeEitherStr(line, new string[] {"(", "."})) {
                        case "Sks": // a manual overide 
                            string remainingText = SafeTextAfterEitherStr(line, new string[] {"(", "."});
                            string itemReferenced = SafeTextBeforeEitherStr(SafeTextAfterEitherStr(line, new string[] {"(", "."}), new string[] {"(", "."});
                            //DebugLog(itemReferenced);
                            switch (itemReferenced) {
                                case "log":
                                    //DebugLog(SafeTextBeforeStr(CorrectedSafeTextAfterStr(remainingText, "("), ")"));
                                    var argument = (sksvar.numValue) recallVariable(SafeTextBeforeStr(CorrectedSafeTextAfterStr(remainingText, "("), ")")).getDataObj();
                                    Console.WriteLine(argument.toString());
                                    break;
                            }
                            break;
                    }
                }
            }
        }
    }
}
