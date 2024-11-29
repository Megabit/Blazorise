using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

namespace Blazorise.Weavers.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    
    public override void Execute()
    {
        WriteMessage("-----InExecute", MessageImportance.High);

        // Remove attribute usages
        RemoveAttributeUsages("Blazorise.Generator.Features.GenerateEqualityAttribute");
         RemoveAttributeUsages("Blazorise.Generator.Features.GenerateIgnoreEqualityAttribute");
         
         var isPackValue = Config.Attribute("IsPack")?.Value ?? "false";//get IsPack (see csproj)
         // Convert the value to a boolean
         var isPack = bool.TryParse(isPackValue, out var result) && result;

         // Debug output
         WriteMessage($"IsPack property value: {isPack}", MessageImportance.High);
         
         
        RemoveReference("Blazorise.Generator.Features");
        RemoveClassesFromAssembly("Blazorise.ApiDocsDtos");
        RemoveReference("Blazorise.ApiDocsDtos");
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
    
    /// <summary>
    /// In case of "forgoten usage", this will act like it removes the reference,
    /// but opposite is the truth. In other words - it wont let the .dll without
    /// the reference if it still needs it 
    /// </summary>
    /// <param name="referenceName"></param>
    private void RemoveReference(string referenceName)
    {
        var reference = ModuleDefinition.AssemblyReferences
            .FirstOrDefault(r => r.Name == referenceName);

        if (reference != null)
        {
            ModuleDefinition.AssemblyReferences.Remove(reference);
            WriteMessage($"Removed reference to {referenceName}", MessageImportance.High);
        }
        else
        {
            WriteMessage($"Reference {referenceName} not found", MessageImportance.Low);
        }
    }
    
    private void RemoveClassesFromAssembly(string assemblyName)
    {
        // Iterate through all types in the current module
        var typesToRemove = ModuleDefinition.Types
            .Where(t => t.CustomAttributes
                .Any(attr => attr.AttributeType.Scope.Name == assemblyName))
            .ToList();

        foreach (var type in typesToRemove)
        {
            ModuleDefinition.Types.Remove(type);
            WriteMessage($"Removed class {type.FullName} from assembly {assemblyName}", MessageImportance.High);
        }

        if (typesToRemove.Count == 0)
        {
            WriteMessage($"No classes found to remove from assembly {assemblyName}", MessageImportance.Low);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        // Use default assemblies for scanning
        return ["mscorlib", "System", "System.Core"];
    }

    public override bool ShouldCleanReference => true;
}
