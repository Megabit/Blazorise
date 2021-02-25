using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace TodoApp
{
    public abstract class BaseTodoItems : ComponentBase
    {
        protected Validations validations;

        protected string description;

        protected Filter filter = Filter.All;

        protected List<Todo> todos = new List<Todo>()
        {
            new Todo { Description = "Buy milk" },
            new Todo { Description = "Call John regarding the meeting" },
            new Todo { Description = "Walk a dog" },
        };

        protected IEnumerable<Todo> Todos
        {
            get
            {
                var query = from t in todos select t;

                if ( filter == Filter.Active )
                    query = from q in query where !q.Completed select q;

                if ( filter == Filter.Completed )
                    query = from q in query where q.Completed select q;

                return query;
            }
        }

        protected void SetFilter( Filter filter )
        {
            this.filter = filter;
        }

        protected void OnCheckAll( bool isChecked )
        {
            todos.ForEach( x => x.Completed = isChecked );
        }

        protected void OnAddTodo()
        {
            if ( validations.ValidateAll() )
            {
                todos.Add( new Todo { Description = description } );
                description = null;

                validations.ClearAll();
            }
        }

        protected void OnClearCompleted()
        {
            todos.RemoveAll( x => x.Completed );
            filter = Filter.All;
        }

        protected void OnTodoStatusChanged( bool isChecked )
        {
            InvokeAsync( StateHasChanged );
        }
    }
}
