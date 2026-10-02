using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
using Moq;

namespace Blazorise.Tests.Mocks;

internal class MockDatePicker<T> : DatePicker<T>
{
    public MockDatePicker( Validation validation = null, Expression<Func<T>> dateExpression = null )
    {
        var mockLocalizerService = new Mock<ITextLocalizerService>();
        LocalizerService = mockLocalizerService.Object;

        var mockLocalizer = new Mock<ITextLocalizer<DatePicker<T>>>();
        Localizer = mockLocalizer.Object;

        var mockIdGenerator = new Mock<IIdGenerator>();

        mockIdGenerator
            .Setup( r => r.Generate )
            .Returns( Guid.NewGuid().ToString() );

        base.IdGenerator = mockIdGenerator.Object;

        base.ParentValidation = validation;
        base.ValueExpression = dateExpression;

        this.OnInitialized();
    }

    public string TextValue
    {
        get { return base.CurrentValueAsString; }
    }

    public async Task<ParseValue<T>> ParseValueAsync( string value )
    {
        return await base.ParseValueFromStringAsync( value );
    }

    public void OnChange( ChangeEventArgs e )
    {
        base.OnChangeHandler( e );
    }
}