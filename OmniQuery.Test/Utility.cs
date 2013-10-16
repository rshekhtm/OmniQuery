using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OmniQuery.CodeAnalytics.Linq;

namespace OmniQuery.Test
{
    public static class Utility
    {
        public static string[] GetSystemAssemblies()
        {
            string assemblyFolder1 = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727";
            string assemblyFolder2 = @"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0";
            string assemblyFolder3 = @"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.5";
            List<string> assemblies = new List<string>();

            assemblies.AddRange(Directory.GetFiles(assemblyFolder1, "System*.dll"));
            assemblies.AddRange(Directory.GetFiles(assemblyFolder2, "System*.dll"));
            assemblies.AddRange(Directory.GetFiles(assemblyFolder3, "System*.dll"));
            string[] exceptions = new string[] { "System.EnterpriseServices.Thunk.dll", "System.EnterpriseServices.Wrapper.dll" };

            foreach (string exception in exceptions)
            {
                assemblies.Remove(assemblyFolder1 + exception);
            }

            return assemblies.ToArray();
        }

        public static void CreateAssemblyFolder(string assemblyFolder, string[] assemblyPaths)
        {
            if (Directory.Exists(assemblyFolder))
            {
                Directory.Delete(assemblyFolder, true);
            }
            Directory.CreateDirectory(assemblyFolder);

            foreach (string assemblyPath in assemblyPaths)
            {
                string assemblyName = Path.GetFileName(assemblyPath);
                File.Copy(assemblyPath, Path.Combine(assemblyFolder, assemblyName));
            }            
        }
    }

    internal class PerfWatch : IDisposable
    {
        string _action;
        long _initialMemory;
        Stopwatch _watch = new Stopwatch();

        public PerfWatch(string action)
        {
            _watch.Start();
            _action = action;
            _initialMemory = GC.GetTotalMemory(true);
        }

        public void Dispose()
        {
            long bytesUsed = GC.GetTotalMemory(true) - _initialMemory;
            Console.WriteLine(_action + " (memory): " + bytesUsed / 1024 + " KB");

            _watch.Stop();
            Console.WriteLine(_action + " (timing): " + _watch.ElapsedMilliseconds + " ms");
            Console.WriteLine();
        }
    }
}
