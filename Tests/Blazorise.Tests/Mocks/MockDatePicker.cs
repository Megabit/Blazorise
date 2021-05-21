﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Moq;

namespace Blazorise.Tests.Mocks
{
    internal class MockDatePicker<T> : DatePicker<T>
    {
        public MockDatePicker( Validation validation = null, Expression<Func<T>> dateExpression = null )
        {
            var mockRunner = new Mock<IJSRunner>();

            mockRunner
                .Setup( r => r.ActivateDatePicker( It.IsAny<ElementReference>(), It.IsAny<string>(), It.IsAny<object>() ) )
                .Callback( ( ElementReference reference, string id, object o ) => this.OnActivateDatePicker( reference, id, o ) );

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

        public void OnChange( ChangeEventArgs e )
        {
            base.OnChangeHandler( e );
        }

        private bool OnActivateDatePicker( ElementReference elementRef, string elementId, object options )
        {
            this.ClickedId = elementId;
            return true;
        }
    }
}
