using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;

using System.Data.SQLite;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace OmniQuery.CodeAnalytics
{
    /// <summary>
    /// Code analysis storage class
    /// </summary>
    public sealed partial class CodeAnalysisDb
    {
        private string _analysisPath;

        /// <summary>
        /// Analysis results file path
        /// </summary>
        public string AnalysisPath
        {
            get { return _analysisPath; }
            set { _analysisPath = value; }
        }

        /// <summary>
        /// Open existing analysis results or analyze a new set of assemblies
        /// </summary>
        /// <param name="analysisPath">Analysis results file path</param>
        /// <param name="assemblyPaths">Optional file paths for assemblies to analyze</param>
        public CodeAnalysisDb(string analysisPath, params string[] assemblyPaths)
        {
            _analysisPath = analysisPath;

            if (assemblyPaths == null || assemblyPaths.Length == 0)
            {
                if (!File.Exists(analysisPath))
                {
                    throw new ArgumentException("Analysis results file not found at the specified path: " + analysisPath);
                }

                if (!IsAnalysisDatabaseValid())
                {
                    throw new ArgumentException("Invalid analysis results file specified: " + analysisPath);
                }
            }
            else
            {
                if (File.Exists(analysisPath))
                {
                    throw new ArgumentException("A file already exists at the specified analysis path: " + analysisPath);
                }

                foreach (string assemblyPath in assemblyPaths)
                {
                    if (!File.Exists(assemblyPath))
                    {
                        throw new ArgumentException("Assembly not found at the specified path: " + assemblyPath);
                    }
                }

                Analyze(assemblyPaths);  
            } 
        }

        /// <summary>
        /// Create a new SQLite connection to the analysis results file
        /// </summary>
        /// <returns>Connection object</returns>
        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + _analysisPath);
        }

        /// <summary>
        /// Create a new command object
        /// </summary>
        /// <param name="conn">Connection object</param>
        /// <param name="resourceName">Resource to pull command SQL from</param>
        /// <returns>Command object</returns>
        private DbCommand GetCommand(SQLiteConnection conn, string resourceName)
        {
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = Utility.GetEmbeddedResource(resourceName);
            return cmd;
        }

        /// <summary>
        /// Verify that the analysis database contains correct tables
        /// </summary>
        /// <returns></returns>
        private bool IsAnalysisDatabaseValid()
        {
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE name IN ('Assembly', 'Module', 'Type', 'Method')";
                    long tableCount = (long)cmd.ExecuteScalar();
                    if (tableCount != 4)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Analyze a set of assemblies
        /// </summary>
        /// <param name="assemblyPaths">File paths for assemblies to analyze</param>
        private void Analyze(string[] assemblyPaths)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                using (DbCommand cmdSchema = GetCommand(conn, "CodeAnalyticsSchema.sql"))
                {
                    cmdSchema.ExecuteNonQuery();
                }
            }

            foreach (string assemblyPath in assemblyPaths)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (SQLiteConnection conn = GetConnection())
                    {
                        conn.Open();
                        using (CommandFactory command = new CommandFactory(conn))
                        {
                            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
                            CreateAssemblyRecord(command, assemblyDefinition);
                        }
                    }
                    ts.Complete();
                }
            }
        }

        /// <summary>
        /// Create an Assembly record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="assemblyDefinition">Assembly definition</param>
        private void CreateAssemblyRecord(CommandFactory command, AssemblyDefinition assemblyDefinition)
        {
            int resourceCount = 0, typeCount = 0, methodCount = 0;
            foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
            {
                resourceCount += moduleDefinition.Resources.Count;
                typeCount += moduleDefinition.Types.Count;
                foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
                {
                    methodCount += typeDefinition.Methods.Count;
                }
            }

            command.Insert<AssemblyField>(
                assemblyDefinition.Name.Name,
                assemblyDefinition.Name.FullName,
                assemblyDefinition.Name.Version,
                assemblyDefinition.Name.Culture,
                assemblyDefinition.Name.PublicKeyToken,
                null, // MainModuleId
                assemblyDefinition.Modules.Count,
                resourceCount,
                typeCount,
                methodCount
            );

            foreach (CustomAttribute attribute in assemblyDefinition.CustomAttributes)
            {
                CreateAttributeRecord(command, attribute, AttributeType.Assembly);
            }

            foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
            {
                CreateModuleRecord(command, moduleDefinition);
            }
        }

        /// <summary>
        /// Create a Module record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="moduleDefinition">Module definition</param>
        private void CreateModuleRecord(CommandFactory command, ModuleDefinition moduleDefinition)
        {
            int methodCount = 0;
            foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
            {
                methodCount += typeDefinition.Methods.Count;
            }

            string name = moduleDefinition.Name;
            if (name == null || name.Trim() == string.Empty)
            {
                name = Path.GetFileName(moduleDefinition.FullyQualifiedName);
            }

            ModuleType moduleType = ModuleType.Library;
            if (moduleDefinition.Kind != ModuleKind.Dll)
            {
                moduleType = Utility.ConvertEnum<ModuleType>(moduleDefinition.Kind);
            }

            TargetPlatform? platform = null;
            if (moduleDefinition.Architecture == TargetArchitecture.I386 &&
                (moduleDefinition.Attributes & ModuleAttributes.ILOnly) == ModuleAttributes.ILOnly &&
                (moduleDefinition.Attributes & ModuleAttributes.Required32Bit) != ModuleAttributes.Required32Bit)
            {
                platform = TargetPlatform.AnyCPU;
            }
            else if (moduleDefinition.Architecture == TargetArchitecture.I386)
            {
                platform = TargetPlatform.x86;
            }
            else if (moduleDefinition.Architecture == TargetArchitecture.AMD64)
            {
                platform = TargetPlatform.x64;
            }
            else if (moduleDefinition.Architecture == TargetArchitecture.IA64)
            {
                platform = TargetPlatform.Itanium;
            }

            command.Insert<ModuleField>(
                command.GetLastId<AssemblyField>(),
                name,
                moduleDefinition.FullyQualifiedName,
                moduleType,
                moduleDefinition.Runtime.ToString().Replace("Net_", "").Replace("_", "."),
                platform,
                (moduleDefinition.Attributes & ModuleAttributes.ILOnly) != ModuleAttributes.ILOnly,
                (moduleDefinition.Attributes & ModuleAttributes.StrongNameSigned) == ModuleAttributes.StrongNameSigned,
                null, // EntryPointMethodId
                moduleDefinition.Resources.Count,
                moduleDefinition.Types.Count,
                methodCount
            );

            if (moduleDefinition.Assembly != null && moduleDefinition == moduleDefinition.Assembly.MainModule)
            {
                command.Update(AssemblyField.MainModuleId, command.GetLastId<ModuleField>());
            }

            foreach (Resource resource in moduleDefinition.Resources)
            {
                CreateResourceRecord(command, resource);
            }

            foreach (CustomAttribute attribute in moduleDefinition.CustomAttributes)
            {
                CreateAttributeRecord(command, attribute, AttributeType.Module);
            }

            foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
            {
                CreateTypeRecord(command, typeDefinition);
            }
        }

        /// <summary>
        /// Create a Resource record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="resource">Resource</param>
        private void CreateResourceRecord(CommandFactory command, Resource resource)
        {
            command.Insert<ResourceField>(
                command.GetLastId<AssemblyField>(),
                command.GetLastId<ModuleField>(),
                resource.Name,
                Utility.ConvertEnum<ResourceType>(resource.ResourceType).ToString(),
                resource.IsPublic
            );
        }

        /// <summary>
        /// Create a Type record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="typeDefinition">Type definition</param>
        private void CreateTypeRecord(CommandFactory command, TypeDefinition typeDefinition)
        {
            command.Insert<TypeField>(
                command.GetLastId<AssemblyField>(),
                command.GetLastId<ModuleField>(),
                typeDefinition.Name,
                typeDefinition.FullName,
                GetTypeName(typeDefinition, false, true),
                typeDefinition.Namespace,
                typeDefinition.Methods.Count
            );

            foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
            {
                CreateMethodRecord(command, methodDefinition);
            }

            foreach (TypeDefinition nestedType in typeDefinition.NestedTypes)
            {
                CreateTypeRecord(command, nestedType);
            }
        }

        /// <summary>
        /// Create a Method record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="methodDefinition">Method definition</param>
        private void CreateMethodRecord(CommandFactory command, MethodDefinition methodDefinition)
        {
            command.Insert<MethodField>(
                command.GetLastId<AssemblyField>(),
                command.GetLastId<ModuleField>(),
                command.GetLastId<TypeField>(),
                methodDefinition.Name,
                methodDefinition.FullName,
                GetTypeName(methodDefinition.ReturnType, true, false),
                GetTypeName(methodDefinition.ReturnType, false, false)
            );

            CreateTypeReferenceRecord(command, methodDefinition.ReturnType);

            if (methodDefinition == methodDefinition.Module.EntryPoint)
            {
                command.Update(ModuleField.EntryPointMethodId, command.GetLastId<MethodField>());
            }

            /*if (methodDefinition.Body != null)
            {
                foreach (Instruction instruction in methodDefinition.Body.Instructions)
                {
                    CreateInstructionRecord(command, instruction);
                }
            }*/
        }

        /// <summary>
        /// Create an Instruction record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="instruction">Instruction</param>
        private void CreateInstructionRecord(CommandFactory command, Instruction instruction)
        {
            command.Insert<InstructionField>(
                command.GetLastId<AssemblyField>(),
                command.GetLastId<ModuleField>(),
                command.GetLastId<TypeField>(),
                command.GetLastId<MethodField>(),
                instruction.ToString()
            );
        }

        /// <summary>
        /// Create a TypeReference record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="typeReference">Type reference</param>
        private void CreateTypeReferenceRecord(CommandFactory command, TypeReference typeReference)
        {
            if (!typeReference.IsGenericParameter)
            {
                //if (string.IsNullOrEmpty(GetNamespace(typeReference))) System.Diagnostics.Debugger.Break();

                command.Insert<TypeReferenceField>(
                    GetTypeName(typeReference, true, true),
                    GetTypeName(typeReference, false, true),
                    GetNamespace(typeReference)
                );

                if (typeReference.IsGenericInstance)
                {
                    GenericInstanceType genericInstance = (GenericInstanceType)typeReference;
                    foreach (TypeReference genericArgument in genericInstance.GenericArguments)
                    {
                        CreateTypeReferenceRecord(command, genericArgument);
                    }
                }
            }
        }

        /// <summary>
        /// Create an Attribute record in the database
        /// </summary>
        /// <param name="command">Command factory object</param>
        /// <param name="attribute">Custom attribute</param>
        /// <param name="attributeType">Attribute type</param>
        private void CreateAttributeRecord(CommandFactory command, CustomAttribute attribute, AttributeType attributeType)
        {
            command.Insert<AttributeField>(
                attributeType == AttributeType.Assembly ? command.GetLastId<AssemblyField>() : new long?(),
                attributeType == AttributeType.Module ? command.GetLastId<ModuleField>() : new long?(),
                attribute.AttributeType.Name,
                attribute.AttributeType.FullName,
                attributeType
            );
        }

        /// <summary>
        /// Format a type name
        /// </summary>
        /// <param name="typeReference">Type reference</param>
        /// <param name="removeContainers">Remove namespace and parent classes</param>
        /// <param name="removeGenericParameters">Remove generic parameters</param>
        /// <returns>Formatted type name</returns>
        private string GetTypeName(TypeReference typeReference, bool removeContainers, bool removeGenericParameters)
        {
            string name = typeReference.FullName.Replace("/", "+").Replace("0...", string.Empty);

            if (name.StartsWith("<"))
            {
                return name;
            }

            if (removeContainers)
            {
                name = Regex.Replace(name, "[\\w`]+[\\.\\+]", string.Empty);
            }

            string genericRegex = removeGenericParameters ? "<.*>" : "`\\d+";
            name = Regex.Replace(name, genericRegex, string.Empty);

            return name;
        }

        private string GetNamespace(TypeReference typeReference)
        {
            string ns = typeReference.Namespace;
            while (string.IsNullOrEmpty(ns) && typeReference.DeclaringType != null)
            {
                typeReference = typeReference.DeclaringType;
                ns = typeReference.Namespace;
            }
            return ns;
        }
    }
}
