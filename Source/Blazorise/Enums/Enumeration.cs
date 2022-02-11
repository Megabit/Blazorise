#region Using directives
using System.Text;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for all complex enums.
    /// </summary>
    /// <typeparam name="T">Type of the complex enum.</typeparam>
    public record Enumeration<T>
        where T : Enumeration<T>
    {
        #region Members

        string name;

        string cachedName;

        #endregion

        #region Constructors

        /// <summary>
        /// A default enumeration contructor.
        /// </summary>
        /// <param name="name">Named value of the enum.</param>
        public Enumeration( string name )
        {
            Name = name;
            Parent = default;
        }

        /// <summary>
        /// A default enumeration contructor that accepts the parent object.
        /// </summary>
        /// <param name="parent">Parent enumeration.</param>
        /// <param name="name">Named value of the enum.</param>
        protected Enumeration( T parent, string name )
        {
            this.Name = name;
            this.Parent = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the new custom target based on the supplied enum value.
        /// </summary>
        /// <param name="name">Name value of the enum.</param>
        public static implicit operator Enumeration<T>( string name )
        {
            return new Enumeration<T>( name );
        }

        private string BuildName()
        {
            var sb = new StringBuilder();

            if ( Parent != null )
                sb.Append( Parent.Name ).Append( ' ' );

            sb.Append( name );

            return sb.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the enum name.
        /// </summary>
        public string Name
        {
            get
            {
                if ( cachedName == null )
                    cachedName = BuildName();

                return cachedName;
            }
            private set
            {
                if ( name == value )
                    return;

                name = value;
                cachedName = null;
            }
        }

        /// <summary>
        /// Gets the parent enumeration.
        /// </summary>
        public T Parent { get; private set; }

        #endregion
    }
}
