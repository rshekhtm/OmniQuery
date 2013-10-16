using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using OmniQuery.CodeAnalytics;
using OmniQuery.CodeAnalytics.Linq;
using Mono.Cecil;

namespace OmniQuery.Test
{
    public class TestCodeAnalysis
    {
        [Fact]
        public void MainTest()
        {
            string[] assemblyPaths = new string[] {
                @"C:\Dev\OmniQuery\OmniQuery.Test\bin\Debug\OmniQuery.Test.dll",
            };
            CodeAnalysis analysis = new CodeAnalysis(assemblyPaths);
        }

        [Fact]
        public void DBTest()
        {
            string analysisPath = @"C:\Dev\OmniQuery\test.db3";
            string[] assemblyPaths = new string[] {
                @"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\System.Web.dll",
            };

            assemblyPaths = Utility.GetSystemAssemblies();

            if (File.Exists(analysisPath))
            {
                File.Delete(analysisPath);
            }

            new CodeAnalysisDb(analysisPath, assemblyPaths);
            new CodeAnalysisDb(analysisPath);
        }

        [Fact]
        public void QueryTest()
        {
            CodeAnalysis analysis;

            using (new PerfWatch("Retrieving assemblies"))
            {
                //analysis = new CodeAnalysis(@"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\System.Web.dll");
                analysis = new CodeAnalysis(Utility.GetSystemAssemblies());
            }

            using (new PerfWatch("Query total"))
            {
                using (new PerfWatch("Running query 1"))
                {
                    var result =
                        from a in analysis.Assemblies
                        from mod in a.Modules
                        from t in mod.Types
                        from m in t.Methods
                        where m.Name.StartsWith("get_")
                        select m.FullName;

                    string[] arr = result.ToArray();
                }

                for (int i = 2; i < 5; i++)
                {
                    using (new PerfWatch("Running query " + i))
                    {
                        var result =
                            from a in analysis.Assemblies
                            from mod in a.Modules
                            from t in mod.Types
                            from m in t.Methods
                            where t.Methods.Count > 10
                            select m.FullName;

                        string[] arr = result.ToArray();
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
