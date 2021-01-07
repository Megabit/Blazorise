using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Moq;

namespace Blazorise.Tests.Mocks
{
    internal class MockDateEdit<T> : DateEdit<T>
    {
        public MockDateEdit( Validation validation = null, Expression<Func<T>> dateExpression = null )
        {
            var mockRunner = new Mock<IJSRunner>();

            mockRunner
                .Setup( r => r.ActivateDatePicker( It.IsAny<string>(), It.IsAny<string>() ) )
                .Callback( ( string id, string f ) => this.OnActivateDatePicker( id, f ) );

            base.JSRunner = mockRunner.Object;

            var mockIdGenerator = new Mock<IIdGenerator>();

            mockIdGenerator
                .Setup( r => r.Generate )
                .Returns( Guid.NewGuid().ToString() );

            base.IdGenerator = mockIdGenerator.Object;

            base.ParentValidation = validation;
            base.DateExpression = dateExpression;

            this.OnInitialized();
        }

        public string TextValue
        {
            get { return base.CurrentValueAsString; }
        }

        public string ClickedId { get; private set; }

        public async Task<ParseValue<T>> ParseValueAsync( string value )
        {
            return await base.ParseValueFromStringAsync( value );
        }

        public Task Click()
        {
            return OnClickHandler( new MouseEventArgs() );
        }

        public void OnChange( ChangeEventArgs e )
        {
            base.OnChangeHandler( e );
        }

        private bool OnActivateDatePicker( string elementId, string formatString )
        {
            this.ClickedId = elementId;
            return true;
        }

    }
}
