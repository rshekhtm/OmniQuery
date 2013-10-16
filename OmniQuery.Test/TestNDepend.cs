using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using OmniQuery.CodeAnalytics.Linq;

namespace OmniQuery.Test
{
    public class TestNDepend
    {
        [Fact]
        public void TestMethods()
        {
            string assemblyFolder = "temp";
            string[] systemAssemblies = Utility.GetSystemAssemblies();
            //string[] systemAssemblies = new string[] { @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.EnterpriseServices.dll" };
            Utility.CreateAssemblyFolder(assemblyFolder, systemAssemblies);

            foreach (string assemblyPath in systemAssemblies)
            {
                string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
                string query = string.Format("SELECT METHODS FROM ASSEMBLIES \"{0}\" ORDER BY NbMethods", assemblyName);
                NDependProject project = new NDependProject("MethodProject", assemblyFolder, null);
                NDependResults result = project.AnalyzeQuery(null, query);

                CodeAnalysis analysis = new CodeAnalysis(new string[] { assemblyPath });
                Console.WriteLine("{0} ( {1} , {2} )", assemblyName, result.Count, analysis.Methods.Count);
                //Assert.True(result.Count <= analysis.Methods.Count);
            }
        }
    }
}
