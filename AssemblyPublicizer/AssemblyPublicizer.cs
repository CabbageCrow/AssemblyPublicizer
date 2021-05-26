using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

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
	/// Creates a copy of an assembly in which all members are public (types, methods, fields (Minus Events), getters and setters of properties).
	/// If you use the modified assembly as your reference and compile your dll with the option "Allow unsafe code" enabled, 
	/// you can access all private elements even when using the original assembly.
	/// Without "Allow unsafe code" you get an access violation exception during runtime when accessing private members except for types.  
	/// How to enable it: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/unsafe-compiler-option
	/// arg 0 / -i|--input:		Path to the assembly (absolute or relative)
	/// arg 1 / -o|--output:	[Optional] Output path/filename
	///							Can be just a (relative) path like "subdir1\subdir2"
	///							Can be just a filename like "CustomFileName.dll"
	///							Can be a filename with path like "C:\subdir1\subdir2\CustomFileName.dll"
	/// </summary>
	class AssemblyPublicizer
	{
		static void Main(string[] args)
		{
			var defaultOutputDir = "publicized_assemblies";
			int count = 0;
			foreach(string input in args)
			{
				try
				{
					AssemblyDefinition assembly = null;

					if(!File.Exists((string)input))
						continue;

					assembly = AssemblyDefinition.ReadAssembly((string)input);

					var allTypes = GetAllTypes(assembly.MainModule);
					var allMethods = allTypes.SelectMany(t => t.Methods);
					var allFields = FilterBackingEventFields(allTypes);


					#region Make everything public

					foreach(var type in allTypes)
					{
						if((!type?.IsPublic ?? false) || (!type?.IsNestedPublic ?? false))
						{
							if(type.IsNested)
								type.IsNestedPublic = true;
							else
								type.IsPublic = true;
						}
					}

					foreach(MethodDefinition method in allMethods)
					{
						if(!method?.IsPublic ?? false)
						{
							method.IsPublic = true;
						}
						method.Body = new Mono.Cecil.Cil.MethodBody(new MethodDefinition(method.Name, method.Attributes, method.ReturnType));
					}

					foreach(var field in allFields)
					{
						if(!field?.IsPublic ?? false)
						{
							field.IsPublic = true;
						}
					}
					#endregion

					var outputName = string.Format("{0}{1}{2}",
					   Path.GetFileNameWithoutExtension((string)input), "", Path.GetExtension((string)input));
					var outputPath = defaultOutputDir;
					var outputFile = Path.Combine(outputPath, outputName);

					if(outputPath != "" && !Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					assembly.Write(outputFile);
				}
				catch(Exception e) 
				{
					count++;
					Console.WriteLine($"{input} failed with Exception\n{e}");
				}
			}
			Exit(count);
		}

		public static void Exit(int exitCode)
		{
			if(exitCode != 0)
				Console.ReadKey();

			Environment.Exit(exitCode);
		}

		public static IEnumerable<FieldDefinition> FilterBackingEventFields(IEnumerable<TypeDefinition> allTypes)
		{
			List<string> eventNames = allTypes.SelectMany(t => t.Events).Select(eventDefinition => eventDefinition.Name).ToList();

			return allTypes.SelectMany(x => x.Fields).Where(fieldDefinition => !eventNames.Contains(fieldDefinition.Name));
		}

		/// <summary>
		/// Method which returns all Types of the given module, including nested ones (recursively)
		/// </summary>
		/// <param name="moduleDefinition"></param>
		/// <returns></returns>
		public static IEnumerable<TypeDefinition> GetAllTypes(ModuleDefinition moduleDefinition)
		{
			return _GetAllNestedTypes(moduleDefinition.Types);//.Reverse();
		}

		/// <summary>
		/// Recursive method to get all nested types. Use <see cref="GetAllTypes(ModuleDefinition)"/>
		/// </summary>
		/// <param name="typeDefinitions"></param>
		/// <returns></returns>
		private static IEnumerable<TypeDefinition> _GetAllNestedTypes(IEnumerable<TypeDefinition> typeDefinitions)
		{
			//return typeDefinitions.SelectMany(t => t.NestedTypes);

			if(typeDefinitions?.Count() == 0)
				return new List<TypeDefinition>();

			var result = typeDefinitions.Concat(_GetAllNestedTypes(typeDefinitions.SelectMany(t => t.NestedTypes)));

			return result;
		}


	}
}
