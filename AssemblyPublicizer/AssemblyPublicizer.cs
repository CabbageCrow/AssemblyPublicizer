using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// AssemblyPublicizer - A tool to create a copy of an assembly in 
/// which all members are public (types, methods, fields, getters
/// and setters of properties).  
/// 
/// Copyright(c) 2018 CabbageCrow
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the GNU Lesser General Public
/// License as published by the Free Software Foundation; either
/// version 2.1 of the License, or(at your option) any later version.
/// 
/// Overview:
/// https://tldrlegal.com/license/gnu-lesser-general-public-license-v2.1-(lgpl-2.1)
/// 
/// This library is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
///	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU
/// Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public
/// License along with this library; if not, write to the Free Software
/// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
/// USA
/// </summary>

namespace CabbageCrow.AssemblyPublicizer
{
	/// <summary>
	/// Creates a copy of an assembly in which all members are public (types, methods, fields, getters and setters of properties).
	/// If you use the modified assembly as your reference and compile your dll with the option "Allow unsafe code", 
	/// you can access all private elements even when using the original assembly.
	/// arg 0: Path to the assembly (absolute or relative)
	/// arg 1:	[Optional] Output path/filename
	///			Can be just a (relative) path like "subdir1\subdir2"
	///			Can be just a filename like "CustomFileName.dll"
	///			Can be a filename with path like "C:\subdir1\subdir2\CustomFileName.dll"
	/// </summary>
	class AssemblyPublicizer
	{
		static void Main(string[] args)
		{
			var suffix = "_publicized";
			var defaultOutputDir = "publicized_assemblies";

			if (args == null || args.Length == 0)
			{
				Console.WriteLine("ERROR! No arguments. You need to provide the path to the assembly to publicize.");
				Console.WriteLine("On Windows you can even drag and drop the assembly on the .exe.");
				Exit(10);
			}

			var inputFile = args[0];
			AssemblyDefinition assembly = null;
			string outputPath = "", outputName = "";


			if (args.Length > 1)
			{
				try
				{
					outputPath = Path.GetDirectoryName(args[1]);
					outputName = Path.GetFileName(args[1]);
				}
				catch(Exception)
				{
					Console.WriteLine("ERROR! Invalid output argument.");
					Exit(20);
				}
			}


			if (!File.Exists(inputFile))
			{
				Console.WriteLine();
				Console.WriteLine("ERROR! File doesn't exist or you don't have sufficient permissions.");
				Exit(30);
			}

			try
			{
				assembly = AssemblyDefinition.ReadAssembly(inputFile);
			}
			catch (Exception)
			{
				Console.WriteLine();
				Console.WriteLine("ERROR! Cannot read the assembly. Please check your permissions.");
				Exit(40);
			}


			var allTypes = GetAllTypes(assembly.MainModule);
			var allMethods = allTypes.SelectMany(t => t.Methods);
			var allFields = allTypes.SelectMany(t => t.Fields);
			var allProperties = allTypes.SelectMany(t => t.Properties);
			var allGetters = allProperties.Select(p => p.GetMethod);
			var allSetters = allProperties.Select(p => p.SetMethod);

			int count;
			string reportString = "Changed {0} non-public {1} to public.";


			#region Make everything public

			count = 0;
			foreach (var element in allTypes)
			{
				if (!element?.IsPublic ?? false)
				{
					count++;
					element.IsPublic = true;
				}
			}
			Console.WriteLine(reportString, count, "types");

			count = 0;
			foreach (var element in allMethods)
			{
				if (!element?.IsPublic ?? false)
				{
					count++;
					element.IsPublic = true;
				}
			}
			Console.WriteLine(reportString, count, "methods");

			count = 0;
			foreach (var element in allFields)
			{
				if (!element?.IsPublic ?? false)
				{
					count++;
					element.IsPublic = true;
				}
			}
			Console.WriteLine(reportString, count, "fields");

			count = 0;
			foreach (var element in allGetters)
			{
				if (!element?.IsPublic ?? false)
				{
					count++;
					element.IsPublic = true;
				}
			}
			Console.WriteLine(reportString, count, "getters");

			count = 0;
			foreach (var element in allSetters)
			{
				if (!element?.IsPublic ?? false)
				{
					count++;
					element.IsPublic = true;
				}
			}
			Console.WriteLine(reportString, count, "setters");

			#endregion


			Console.WriteLine();
			Console.WriteLine("Saving the new assembly ...");

			if (outputName == "")
			{
				outputName = String.Format("{0}{1}{2}",
					Path.GetFileNameWithoutExtension(inputFile), suffix, Path.GetExtension(inputFile));
				Console.WriteLine("(Use default output name)");
			}

			if(outputPath == "")
			{
				outputPath = defaultOutputDir;
			}

			var outputFile = Path.Combine(outputPath, outputName);

			try
			{
				if (outputPath != "" && !Directory.Exists(outputPath))
					Directory.CreateDirectory(outputPath);
				assembly.Write(outputFile);
			}
			catch (Exception)
			{
				Console.WriteLine();
				Console.WriteLine("ERROR! Cannot create/overwrite the new assembly. ");
				Console.WriteLine("Please check the path and its permissions " +
					"and in case of overwriting an existing file ensure that it isn't currently used.");
				Exit(50);
			}

			Console.WriteLine("Completed.");
			Console.WriteLine();
			Console.WriteLine("Use the publicized library as your reference and compile your dll ");
			Console.WriteLine("with the option \"Allow unsafe code\".");
			Exit(0);
		}

		public static void Exit(int exitCode = 0)
		{
			Console.WriteLine();
			Console.WriteLine("Press any key to continue ...");
			Console.ReadKey();
			Environment.Exit(exitCode);
		}

		/// <summary>
		/// Method which returns all Types of the given module, including nested ones (recursively)
		/// </summary>
		/// <param name="moduleDefinition"></param>
		/// <returns></returns>
		public static IEnumerable<TypeDefinition> GetAllTypes(ModuleDefinition moduleDefinition)
		{
			return _GetAllNestedTypes(moduleDefinition.Types);
		}

		/// <summary>
		/// Recursive method to get all nested types. Use <see cref="GetAllTypes(ModuleDefinition)"/>
		/// </summary>
		/// <param name="typeDefinitions"></param>
		/// <returns></returns>
		private static IEnumerable<TypeDefinition> _GetAllNestedTypes(IEnumerable<TypeDefinition> typeDefinitions)
		{
			if (typeDefinitions?.Count() == 0)
				return new List<TypeDefinition>();

			var result = typeDefinitions.Concat(_GetAllNestedTypes(typeDefinitions.SelectMany(t => t.NestedTypes)));

			return result;			
		}


	}
}
