using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

namespace Blazorise.Weavers.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    
    const string GeneratorFeaturesNamespace = "Blazorise.Generator.Features";
    public override void Execute()
    {
        // Remove attribute usages
        RemoveAttributeUsages($"{GeneratorFeaturesNamespace}.GenerateEqualityAttribute");
        RemoveAttributeUsages($"{GeneratorFeaturesNamespace}.GenerateIgnoreEqualityAttribute");
        
        string keepApiDocsGeneratedFilesString = Config.Attribute("KeepApiDocsGeneratedFiles")?.Value ?? "false"; // get KeepApiDocsGeneratedFiles (set in csproj)
        var keepApiDocsGeneratedFiles = bool.TryParse(keepApiDocsGeneratedFilesString, out var result) && result;
        
        WriteMessage($"KeepApiDocsGeneratedFiles property value: {keepApiDocsGeneratedFiles}", MessageImportance.High);
        
        if (!keepApiDocsGeneratedFiles)//remove the ApiDocs code
        {   
            // We could place all related code here and perform the removal only during the packing step.
            // However, always removing Features ensures that we run and test the stripped code consistently.

            // The ApiDocs (generated) code resides in the Blazorise assembly under the Blazorise.ApiDocsSourceGenerated namespace.
            RemoveClassesFromNamespace("Blazorise.ApiDocsSourceGenerated");
            
            // We cannot always remove the reference to GeneratorFeaturesNamespace because the ApiDocs code depends on it.
            RemoveReference(assemblyReferenceName: GeneratorFeaturesNamespace);
        }
    }


    private void RemoveAttributeUsages(string attributeFullName)
    {
        foreach (var type in ModuleDefinition.Types)
        {
            // Remove from types (classes)
            RemoveAttributeFromType(type, attributeFullName);

            // Remove from properties, fields, and events
            foreach (var property in type.Properties)
                RemoveAttributeFromMember(property, attributeFullName);

            foreach (var field in type.Fields)
                RemoveAttributeFromMember(field, attributeFullName);

            foreach (var eventDef in type.Events)
                RemoveAttributeFromMember(eventDef, attributeFullName);
        }
    }

    private void RemoveAttributeFromType(TypeDefinition type, string attributeFullName)
    {
        var customAttributes = type.CustomAttributes;
        for (int i = customAttributes.Count - 1; i >= 0; i--)
        {
            if (customAttributes[i].AttributeType.FullName == attributeFullName)
                customAttributes.RemoveAt(i);
        }
    }

    private void RemoveAttributeFromMember(IMemberDefinition member, string attributeFullName)
    {
        var customAttributes = member.CustomAttributes;
        for (int i = customAttributes.Count - 1; i >= 0; i--)
        {
            if (customAttributes[i].AttributeType.FullName == attributeFullName)
                customAttributes.RemoveAt(i);
        }
    }
    
    private void RemoveReference(string assemblyReferenceName)
    {
        var reference = ModuleDefinition.AssemblyReferences
            .FirstOrDefault(r => r.Name == assemblyReferenceName);

        if (reference != null)
        {
            ModuleDefinition.AssemblyReferences.Remove(reference);
            WriteMessage($"Removed reference to {assemblyReferenceName}", MessageImportance.High);
        }
        else
        {
            WriteMessage($"Reference {assemblyReferenceName} not found", MessageImportance.Low);
        }
    }
    
    private void RemoveClassesFromNamespace(string namespaceName)
    {
        WriteMessage($"Starting removal of classes from namespace: {namespaceName}", MessageImportance.High);

        // Iterate through all types in the current module and filter by namespace
        var typesToRemove = ModuleDefinition.Types
            .Where(t => t.Namespace == namespaceName)
            .ToList();

        WriteMessage($"Found {typesToRemove.Count} types to remove from namespace {namespaceName}", MessageImportance.High);

        // Remove the matching types from the module
        foreach (var type in typesToRemove)
        {
            ModuleDefinition.Types.Remove(type);
            WriteMessage($"Removed class {type.FullName} from namespace {namespaceName}", MessageImportance.High);
        }

        if (typesToRemove.Count == 0)
        {
            WriteMessage($"No classes found to remove from namespace {namespaceName}", MessageImportance.Low);
        }
    }


    public override IEnumerable<string> GetAssembliesForScanning()
    {
        // Use default assemblies for scanning
        return ["mscorlib", "System", "System.Core"];
    }

    public override bool ShouldCleanReference => true;
}
