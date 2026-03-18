using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Blazorise.Utilities;

namespace Blazorise.Benchmark.Blazorise;

[MemoryDiagnoser]
public class AccessibilityBenchmark
{
    private readonly IIdGenerator idGenerator = new IdGenerator();

    private readonly BlazoriseOptions disabledOptions;

    private readonly BlazoriseOptions labelForOptions;

    private readonly BlazoriseOptions ariaLabelledByOptions;

    private readonly BlazoriseOptions fullyEnabledOptions;

    public AccessibilityBenchmark()
    {
        disabledOptions = CreateOptions();
        labelForOptions = CreateOptions( useLabelForAttribute: true );
        ariaLabelledByOptions = CreateOptions( useAriaLabelledByAttribute: true );
        fullyEnabledOptions = CreateOptions( useLabelForAttribute: true, useAriaLabelledByAttribute: true );
    }

    [Params( 10, 100, 1000 )]
    public int ComponentCount { get; set; }

    [Benchmark]
    public int LabelFor_Disabled()
        => BenchmarkLabelFor( disabledOptions );

    [Benchmark]
    public int LabelFor_Enabled()
        => BenchmarkLabelFor( labelForOptions );

    [Benchmark]
    public int AriaLabelledBy_Disabled()
        => BenchmarkAriaLabelledBy( disabledOptions );

    [Benchmark]
    public int AriaLabelledBy_Enabled()
        => BenchmarkAriaLabelledBy( ariaLabelledByOptions );

    [Benchmark]
    public int Mixed_Disabled()
        => BenchmarkMixed( disabledOptions );

    [Benchmark]
    public int Mixed_Enabled()
        => BenchmarkMixed( fullyEnabledOptions );

    private static BlazoriseOptions CreateOptions( bool useLabelForAttribute = false, bool useAriaLabelledByAttribute = false )
        => new( null, options =>
        {
            options.AccessibilityOptions.UseLabelForAttribute = useLabelForAttribute;
            options.AccessibilityOptions.UseAriaLabelledByAttribute = useAriaLabelledByAttribute;
        } );

    private int BenchmarkLabelFor( BlazoriseOptions options )
    {
        int associationCount = 0;

        for ( int i = 0; i < ComponentCount; i++ )
        {
            Field field = new();
            BenchmarkFieldLabel fieldLabel = new();
            BenchmarkLabelTargetInput input = new();

            fieldLabel.InitializeForBenchmark( field, options, idGenerator );
            input.InitializeForBenchmark( field, options, $"input-{i}" );
            input.RegisterLabelTargetForBenchmark();

            if ( fieldLabel.GetResolvedForValue() is not null )
            {
                associationCount++;
            }
        }

        return associationCount;
    }

    private int BenchmarkAriaLabelledBy( BlazoriseOptions options )
    {
        int associationCount = 0;

        for ( int i = 0; i < ComponentCount; i++ )
        {
            Field field = new();
            BenchmarkFieldLabel fieldLabel = new();
            BenchmarkAriaInput input = new();

            fieldLabel.InitializeForBenchmark( field, options, idGenerator );
            input.InitializeForBenchmark( field, options, $"control-{i}" );
            fieldLabel.RegisterLabelElementForBenchmark();

            if ( input.GetParentFieldLabelElementIdValue() is not null )
            {
                associationCount++;
            }
        }

        return associationCount;
    }

    private int BenchmarkMixed( BlazoriseOptions options )
    {
        int associationCount = 0;

        for ( int i = 0; i < ComponentCount; i++ )
        {
            Field field = new();
            BenchmarkFieldLabel fieldLabel = new();

            fieldLabel.InitializeForBenchmark( field, options, idGenerator );

            if ( i % 2 == 0 )
            {
                BenchmarkLabelTargetInput input = new();

                input.InitializeForBenchmark( field, options, $"input-{i}" );
                input.RegisterLabelTargetForBenchmark();

                if ( fieldLabel.GetResolvedForValue() is not null )
                {
                    associationCount++;
                }
            }
            else
            {
                BenchmarkAriaInput input = new();

                input.InitializeForBenchmark( field, options, $"control-{i}" );
                fieldLabel.RegisterLabelElementForBenchmark();

                if ( input.GetParentFieldLabelElementIdValue() is not null )
                {
                    associationCount++;
                }
            }
        }

        return associationCount;
    }

    private sealed class BenchmarkFieldLabel : FieldLabel
    {
        public void InitializeForBenchmark( Field parentField, BlazoriseOptions options, IIdGenerator idGenerator )
        {
            ParentField = parentField;
            Options = options;
            IdGenerator = idGenerator;
            ElementId = UseAriaLabelledByAttribute
                ? idGenerator.Generate
                : null;
        }

        public string GetResolvedForValue()
            => ResolvedFor;

        public void RegisterLabelElementForBenchmark()
        {
            if ( ParentField is not null && UseAriaLabelledByAttribute )
            {
                ParentField.NotifyFieldLabelInitialized( this );
            }
        }
    }

    private class BenchmarkLabelTargetInput : BaseInputComponent<string>
    {
        public void InitializeForBenchmark( Field parentField, BlazoriseOptions options, string elementId )
        {
            ParentField = parentField;
            Options = options;
            ElementId = elementId;
        }

        public string GetParentFieldLabelElementIdValue()
            => ParentFieldLabelElementId;

        public void RegisterLabelTargetForBenchmark()
        {
            if ( ParentField is not null && UseFieldLabelForAttribute && !string.IsNullOrWhiteSpace( FieldLabelTargetElementId ) )
            {
                ParentField.NotifyLabelTargetChanged( this, FieldLabelTargetElementId );
            }
        }

        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
            => Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    private sealed class BenchmarkAriaInput : BenchmarkLabelTargetInput
    {
        protected override string FieldLabelTargetElementId => null;
    }
}