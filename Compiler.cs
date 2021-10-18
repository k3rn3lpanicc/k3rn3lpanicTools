using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k3rn3lpanicTools
{
    public class Compiler
    {
        public static string compileCsharpCode(string Outputfilename,string code,bool runafterCompile)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            string Output = Outputfilename;
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            string codetext = code;
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, codetext);
            string errors = "";
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                   errors +=
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine;
                }
                return errors;
            }
            else
            {                
                if (runafterCompile)
                    Process.Start(Output);
                return "True";
            }
        }
        public static string compileCsharpFile(string Outputfilename, string file, bool runafterCompile)
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            string Output = Outputfilename;
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            string codetext = "";
            using(StreamReader sr = new StreamReader(file))
            {
                codetext = sr.ReadToEnd();
            }
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, codetext);
            string errors = "";
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    errors +=
                                 "Line number " + CompErr.Line +
                                 ", Error Number: " + CompErr.ErrorNumber +
                                 ", '" + CompErr.ErrorText + ";" +
                                 Environment.NewLine + Environment.NewLine;
                }
                return errors;
            }
            else
            {
                if (runafterCompile)
                    Process.Start(Output);
                return "True";
            }
        }
    }
}
