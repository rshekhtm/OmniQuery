using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;

using OmniQuery.CodeAnalytics.Linq;

namespace OmniQuery.CodeAnalytics.Ui
{
    public partial class Main : Form
    {
        private CodeAnalysis _analysis;
        private bool _queryRunning = false;

        public Main()
        {
            InitializeComponent();

            string analysisPath = @"C:\Dev\OmniQuery\OmniQuery.Test\bin\Debug\OmniQuery.Test.dll";
            _analysis = new CodeAnalysis(analysisPath);

            TreeNode parentNode = new TreeNode("System.Web.dll");
            TreeNode assemblyNode = new TreeNode("Assemblies" );
            TreeNode moduleNode = new TreeNode("Modules");
            TreeNode typeNode = new TreeNode("Types");
            TreeNode methodNode = new TreeNode("Methods");
            TreeNode resourceNode = new TreeNode("Resources");

            parentNode.Nodes.AddRange(new TreeNode[] { assemblyNode, moduleNode, typeNode, methodNode, resourceNode });
            parentNode.Expand();
            AnalysisTree.Nodes.Add(parentNode);

            AnalysisTree.DoubleClick += new EventHandler(AnalysisTree_DoubleClick);
            QueryText.KeyDown += new KeyEventHandler(QueryText_KeyDown);
            QueryText.KeyUp += new KeyEventHandler(QueryText_KeyUp);
        }

        void AnalysisTree_DoubleClick(object sender, EventArgs e)
        {
            QueryText.Text = string.Empty;
            string selectedNode = AnalysisTree.SelectedNode.Text;

            if (selectedNode == "Assemblies")
            {
                QueryText.Text = "from a in analysis.Assemblies" + Environment.NewLine + "select a";
            }
            else if (selectedNode == "Modules")
            {
                QueryText.Text = "from mod in analysis.Modules" + Environment.NewLine + "select mod";
            }
            else if (selectedNode == "Types")
            {
                QueryText.Text = "from t in analysis.Types" + Environment.NewLine + "select t";
            }
            else if (selectedNode == "Methods")
            {
                QueryText.Text = "from m in analysis.Methods" + Environment.NewLine + "select m";
            }
            else if (selectedNode == "Resources")
            {
                QueryText.Text = "from r in analysis.Resources" + Environment.NewLine + "select r";
            }

            _runQuery();
        }

        void QueryText_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_queryRunning && e.KeyCode == Keys.F5)
            {
                _queryRunning = true;
                _runQuery();
            }
        }

        void QueryText_KeyUp(object sender, KeyEventArgs e)
        {
            _queryRunning = false;
        }

        private void _runQuery()
        {
            BindingSource bs = new BindingSource();
            if (QueryText.Text.Length > 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                bs.DataSource = _evaluateExpression(QueryText.Text);
            }

            ResultGrid.DataSource = bs;
            Cursor.Current = Cursors.Default;
        }

        private object _evaluateExpression(string expr)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
            
            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.AddRange(new[] { "mscorlib.dll", "System.Core.dll", "OmniQuery.CodeAnalytics.dll" });
            parameters.GenerateInMemory = true;

            string source = "using System.Linq; using OmniQuery.CodeAnalytics.Linq;";
            source += "class Evaluator { public static object Eval(CodeAnalysis analysis) { return " + expr + "; } }";

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, source);
            
            if (results.Errors.HasErrors)
            {
                string errors = null;
                foreach (CompilerError error in results.Errors)
                {
                    errors += error.ErrorText + Environment.NewLine;
                }
                MessageBox.Show(errors, "Query Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            MethodInfo method = results.CompiledAssembly.GetType("Evaluator").GetMethod("Eval");
            
            return method.Invoke(null, new object[] { _analysis });
        }
    }
}
