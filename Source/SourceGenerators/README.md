# Source Generator

This Source Generator can be applied to projects for source generation.

To do so apply the same as an analyzer to your target project:
	<ItemGroup>
		<ProjectReference Include="..\..\..\SourceGenerators\Blazorise.Generator\Blazorise.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
		<!-- Exclude the output of source generators from the compilation -->
		<Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs" />
	</ItemGroup>

To generate the files on a fixed folder on disk:
	<PropertyGroup>
		<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
		<CompilerGeneratedFilesOutputPath>__SOURCEGENERATED__</CompilerGeneratedFilesOutputPath>
	</PropertyGroup>

### Debugging
For debugging the Source Generator make sure you have installed the .NET Compiler Platform SDK (Visual Studio Installer).

### Just Get It To Work
These are made on a "Just Get It To Work" basis. However, improve this in the future if maintenance or readability gets more complex.