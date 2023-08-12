#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Docs.Domain
{
    public enum ResultType
    {
        Ok,

        Created,

        Updated,
        Upserted,
        Replaced,

        Deleted
    }

    public enum ErrorType
    {
        None,

        // User related
        NotFound,
        WrongArguments,
        NotValid,

        // Auth
        NoAuthentication,
        NotAuthorized,
        EmailNotConfirmed,

        // Logic related
        Unknown,
        ConfigurationError,
        NetworkError,
        Duplicate,

        SendEmail,

        ///// <summary>
        ///// Specific error code
        ///// </summary>
        //E311 = 311,

    }

    /// <summary>
    /// The Result itself.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Is set to true if the method ran correcty.
        /// </summary>
        public bool Succeeded { get; private set; }


        /// <summary>
        /// ResultType of the method.
        /// </summary>
        public ResultType? ResultType { get; private set; }

        /// <summary>
        /// Description of the error. Can be null.
        /// </summary>
        public ErrorDescription ErrorDescription { get; private set; }


        #region Constructors

        /// <summary>
        /// Positive result. Conclusion is true and result type is Ok.
        /// </summary>
        public Result()
        {
            Succeeded = true;
            this.ResultType = Domain.ResultType.Ok;
        }

        /// <summary>
        /// Positive result. Conclusion is true and result type is given as argument.
        /// </summary>
        /// <param name="resultType"></param>
        public Result( ResultType resultType )
        {
            Succeeded = true;
            this.ResultType = resultType;
        }



        /// <summary>
        /// Negative result. Conclusion is false.
        /// </summary>
        /// <param name="errorType"></param>
        public Result( ErrorType errorType )
        {
            Succeeded = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorType = errorType;
        }

        public Result( ErrorType errorType, Exception exception )
        {
            Succeeded = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = exception.Message;
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result( ErrorType errorType, string errorMessage, Exception exception = null )
        {
            Succeeded = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = errorMessage;
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }

        public Result( ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null )
        {
            Succeeded = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = string.Join( "\n", errorMessages );
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }



        #endregion

        /// <summary>
        /// Will throw SafeRunException if Conclusion is false.
        /// </summary>
        public Result EnsureSuccess()
        {
            if ( !Succeeded )
                throw ErrorDescription.AsException();

            return this;
        }

        public void Set( ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null )
        {
            Succeeded = false;
            ErrorDescription = new ErrorDescription();
            ErrorDescription.ErrorMessage = string.Join( "\n", errorMessages );
            ErrorDescription.Exception = exception;
            ErrorDescription.ErrorType = errorType;
        }
    }

    /// <summary>
    /// Generic Result, which is able to return the desired object. So instead of return typ "Customer", it would be Result<Customer>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// The desired output.
        /// </summary>
        public T Output { get; private set; }


        public Result() : base() { }
        public Result( ResultType resultType ) : base( resultType ) { }


        public Result( ErrorType errorType ) : base( errorType ) { }
        public Result( ErrorType errorType, Exception exception ) : base( errorType, exception ) { }
        public Result( ErrorType errorType, string errorMessage, Exception exception = null ) : base( errorType, errorMessage, exception ) { }
        public Result( ErrorType errorType, IEnumerable<string> errorMessages, Exception exception = null ) : base( errorType, errorMessages, exception ) { }

        public Result( ErrorDescription errorDescription ) : base( errorDescription.ErrorType, errorDescription.ErrorMessage, errorDescription.Exception ) { }


        public Result<T> AddMethodInfo( params string[] infos )
        {
            // Prepend error message with additional info
            base.ErrorDescription.ErrorMessage = $"{typeof( T ).Name} {string.Join( ", ", infos )}, {ErrorDescription.ErrorMessage}";
            return this;
        }


        public Result<T> SetOutput( T value )
        {
            Output = value;
            return this;
        }

        /// <summary>
        /// Makes sure that Result is positive and the Output is not null. Otherwise Exception is thrown.
        /// Not null Output is returned.
        /// </summary>
        /// <returns></returns>
        public T EnsureOutput()
        {
            if ( Succeeded && Output != null )
                return Output;

            throw ErrorDescription.AsException();
        }
    }

    /// <summary>
    /// Detailed error description.
    /// </summary>
    public class ErrorDescription
    {

        //[JsonConverter(typeof(StringEnumConverter))]
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }

        private Exception exception;
        internal Exception Exception
        {
            get => exception;
            set
            {
                exception = value;
                StackTrace = exception?.StackTrace;
            }
        }

        public Guid Guid { get; private set; }

        public ErrorDescription()
        {
            Guid = Guid.NewGuid();
        }

        public AppException AsException()
        {
            return new AppException( ErrorType, ErrorMessage, Exception );
        }
    }

    /// <summary>
    /// The exception which is able to hold informations about the Result.
    /// </summary>
    public class AppException : Exception
    {
        public ErrorType ErrorType { get; private set; }

        public AppException( ErrorType errorType ) : base( errorType.ToString() )
        {
            ErrorType = errorType;
        }

        public AppException( ErrorType errorType, string errorMessage, Exception exception ) : base( errorMessage, exception )
        {
            ErrorType = errorType;
        }

        public Result AsResultObject()
        {
            return new Result( ErrorType, Message, InnerException );
        }

    }
}
