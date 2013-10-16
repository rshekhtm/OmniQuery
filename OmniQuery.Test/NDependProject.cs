using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OmniQuery.Test
{
    public class NDependProject
    {
        private const string NDEPEND_APPLICATION_PATH = @"C:\Dev\Tools.NET\NDepend\NDepend.Console.exe";

        private string _projectName;
        private string _assemblyPath;
        private string[] _assemblies;
        private string[] _frameworkAssemblies;

        public NDependProject(string projectName, string assemblyPath, string[] frameworkAssemblies)
        {
            _projectName = projectName;
            _assemblyPath = Path.GetFullPath(assemblyPath);
            _assemblies = Directory.GetFiles(_assemblyPath);
            _frameworkAssemblies = frameworkAssemblies;
        }

        public bool AnalyzeProject(bool overwrite)
        {
            string filePath = Path.Combine(_assemblyPath, _projectName + ".xml");
            if (!File.Exists(filePath) || overwrite)
            {
                AnalyzeQueries(null, null);
                return true;
            }
            return false;
        }

        public NDependResults AnalyzeQuery(NDependProject compareToProject, string query)
        {
            string[] queries = (query == null ? null : new string[] { query });
            NDependResults[] results = AnalyzeQueries(compareToProject, queries);
            return (results == null ? null : results[0]);
        }

        public NDependResults[] AnalyzeQueries(NDependProject compareToProject, string[] queries)
        {
            string filePath = _createProjectXml(compareToProject, queries);
            string args = string.Format("\"{0}\" /Silent", filePath);

            _executeProcess(NDEPEND_APPLICATION_PATH, args);

            if (queries != null)
            {
                NDependResults[] results = new NDependResults[queries.Length];

                string cqlResults = Path.Combine(_assemblyPath, Path.Combine(_projectName, "CQLResult.xml"));
                XElement root = XElement.Load(cqlResults);

                for (int i = 0; i < results.Length; i++)
                {
                    results[i] = _parseQueryResults(root, "Query" + i);
                }

                return results;
            }

            return null;
        }

        private string _createProjectXml(NDependProject compareToProject, string[] queries)
        {
            XElement root = new XElement("NDepend", new XAttribute("AppName", _projectName));

            XElement output = new XElement("OutputDir", new XAttribute("KeepHistoric", false));
            output.Add(Path.Combine(_assemblyPath, _projectName));
            root.Add(output);

            XElement dirs = new XElement("Dirs", new XElement("Dir", _assemblyPath));
            root.Add(dirs);

            if (_assemblies != null)
            {
                XElement assemblies = new XElement("Assemblies");
                foreach (string assembly in _assemblies)
                {
                    string assemblyName = Path.GetFileNameWithoutExtension(assembly);
                    assemblies.Add(new XElement("Name", assemblyName));
                }
                root.Add(assemblies);
            }

            if (_frameworkAssemblies != null)
            {
                XElement frameworkAssemblies = new XElement("FrameworkAssemblies");
                foreach (string frameworkAssembly in _frameworkAssemblies)
                {
                    string assemblyName = Path.GetFileNameWithoutExtension(frameworkAssembly);
                    frameworkAssemblies.Add(new XElement("Name", assemblyName));

                    string assemblyDir = Path.GetDirectoryName(Path.GetFullPath(frameworkAssembly));
                    dirs.Add(new XElement("Dir", assemblyDir));
                }
                root.Add(frameworkAssemblies);
            }

            if (compareToProject != null)
            {
                string comparePath = Path.Combine(compareToProject._assemblyPath, compareToProject._projectName);

                XElement compare = new XElement("BuildComparisonSetting");
                compare.Add(new XAttribute("ProjectMode", "AnotherProject"));
                compare.Add(new XAttribute("BuildMode", "BuildMadeNDaysAgo"));
                compare.Add(new XAttribute("NDaysAgo", "1"));
                compare.Add(new XAttribute("ProjectFileToCompareWith", comparePath + ".xml"));
                compare.Add(new XAttribute("BuildFileToCompareWith", Path.Combine(comparePath, "VisualNDepend.bin")));
                root.Add(compare);
            }

            if (queries != null)
            {
                XElement cql = new XElement("CQLQueries");
                for (int i = 0; i < queries.Length; i++)
                {
                    XElement group = new XElement("CQLGroup");
                    XAttribute active = new XAttribute("Active", true);
                    group.Add(active, new XAttribute("Name", "Query" + i));

                    XElement query = new XElement("CQLQuery");
                    query.Add(active, new XAttribute("DisplayList", true));
                    query.Add("WARN IF Count > 0 IN " + queries[i]);
                    group.Add(query);

                    cql.Add(group);
                }
                root.Add(cql);
            }

            string filePath = Path.Combine(_assemblyPath, _projectName + ".xml");
            root.Save(filePath);

            return filePath;
        }

        private NDependResults _parseQueryResults(XElement root, string groupName)
        {
            var results = from query in root.Descendants("Query")
                          where query.Parent.Attribute("Name").Value == groupName
                          select new NDependResults(
                                (int)query.Attribute("NbNodeTested"),
                                from row in query.Descendants("Row")
                                select new NDependResults.ResultRow(
                                        row.Attribute("Name").Value,
                                        row.Attribute("FullName").Value,
                                        (from val in row.Elements("Val")
                                         select _parseInteger(val.Value)).ToArray()
                                )
                          );

            return (results.Count() > 0 ? results.Single() : new NDependResults());
        }

        private void _executeProcess(string filePath, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = filePath;
            psi.Arguments = arguments;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            using (Process process = Process.Start(psi))
            {
                /*string line;
                while ((line = process.StandardOutput.ReadLine()) != null)
                {
                    ConsoleOutput.Out.WriteLine(line);
                }*/

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception(
                        String.Format("Process for [{0}] exited with error code {1}", filePath, process.ExitCode));
                }
            }
        }

        private int _parseInteger(string input)
        {
            int output = 0;
            int.TryParse(input, out output);
            return output;
        }
    }
}
