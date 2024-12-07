#region Using directives
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
#endregion

namespace Blazorise.Weavers.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    #region Members

    const string GeneratorFeaturesNamespace = "Blazorise.Generator.Features";

    #endregion

    #region Methods

    public override void Execute()
    {
        // Remove attribute usages
        RemoveAttributeUsages( $"{GeneratorFeaturesNamespace}.GenerateEqualityAttribute" );
        RemoveAttributeUsages( $"{GeneratorFeaturesNamespace}.GenerateIgnoreEqualityAttribute" );

        RemoveReference( assemblyReferenceName: GeneratorFeaturesNamespace );
    }

    private void RemoveAttributeUsages( string attributeFullName )
    {
        foreach ( var type in ModuleDefinition.Types )
        {
            // Remove from types (classes)
            RemoveAttributeFromType( type, attributeFullName );

            // Remove from properties, fields, and events
            foreach ( var property in type.Properties )
                RemoveAttributeFromMember( property, attributeFullName );

            foreach ( var field in type.Fields )
                RemoveAttributeFromMember( field, attributeFullName );

            foreach ( var eventDef in type.Events )
                RemoveAttributeFromMember( eventDef, attributeFullName );
        }
    }

    private void RemoveAttributeFromType( TypeDefinition type, string attributeFullName )
    {
        var customAttributes = type.CustomAttributes;
        for ( int i = customAttributes.Count - 1; i >= 0; i-- )
        {
            if ( customAttributes[i].AttributeType.FullName == attributeFullName )
                customAttributes.RemoveAt( i );
        }
    }

    private void RemoveAttributeFromMember( IMemberDefinition member, string attributeFullName )
    {
        var customAttributes = member.CustomAttributes;
        for ( int i = customAttributes.Count - 1; i >= 0; i-- )
        {
            if ( customAttributes[i].AttributeType.FullName == attributeFullName )
                customAttributes.RemoveAt( i );
        }
    }

    private void RemoveReference( string assemblyReferenceName )
    {
        var reference = ModuleDefinition.AssemblyReferences
            .FirstOrDefault( r => r.Name == assemblyReferenceName );

        if ( reference != null )
        {
            ModuleDefinition.AssemblyReferences.Remove( reference );
            WriteMessage( $"Removed reference to {assemblyReferenceName}", MessageImportance.High );
        }
        else
        {
            WriteMessage( $"Reference {assemblyReferenceName} not found", MessageImportance.Low );
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        // Use default assemblies for scanning
        return ["mscorlib", "System", "System.Core"];
    }

    #endregion

    #region Properties

    public override bool ShouldCleanReference => true;

    #endregion
}
