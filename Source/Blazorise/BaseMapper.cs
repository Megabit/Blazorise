#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public interface IBaseMapper : IDisposable
    {
        void Dirty();

        IBaseMapper Add( string value );

        IBaseMapper Add( Func<string> value );

        IBaseMapper If( string value, Func<bool> condition );

        IBaseMapper If( Func<string> value, Func<bool> condition );

        IBaseMapper If( Func<IEnumerable<string>> values, Func<bool> condition );
    }

    public abstract class BaseMapper : IBaseMapper
    {
        #region Members

        private bool disposed;

        protected bool dirty = true;

        protected Dictionary<Func<string>, Func<bool>> rules;

        protected Dictionary<Func<IEnumerable<string>>, Func<bool>> listRules;

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposed )
            {
                if ( disposing )
                {
                    if ( rules != null )
                    {
                        rules.Clear();
                        rules = null;
                    }

                    if ( listRules != null )
                    {
                        listRules.Clear();
                        listRules = null;
                    }
                }

                disposed = true;
            }
        }

        protected IEnumerable<string> GetValidRules()
        {
            foreach ( var rule in rules )
            {
                if ( !rule.Value() ) // skip false conditions
                    continue;

                var key = rule.Key();

                if ( key != null )
                    yield return key;
            }
        }

        protected IEnumerable<string> GetValidListRules()
        {
            foreach ( var listRule in listRules )
            {
                if ( !listRule.Value() )
                    continue;

                var key = listRule.Key();

                if ( key == null )
                    continue;

                foreach ( var value in key )
                    yield return value;
            }
        }

        /// <summary>
        /// Marks the builder as dirty to rebuild the values.
        /// </summary>
        public void Dirty()
        {
            dirty = true;
        }

        /// <summary>
        /// Adds a hardcoded value to the collection.
        /// </summary>
        /// <param name="value">Hardcoded classname.</param>
        /// <returns>Returns self.</returns>
        public IBaseMapper Add( string value )
        {
            if ( rules == null )
                rules = new Dictionary<Func<string>, Func<bool>> { { () => value, () => true } };
            else
                rules.Add( () => value, () => true );

            return this;
        }

        /// <summary>
        /// Adds a value getter to the collection.
        /// </summary>
        /// <param name="value">Value getter.</param>
        /// <returns>Returns self.</returns>
        public IBaseMapper Add( Func<string> value )
        {
            if ( rules == null )
                rules = new Dictionary<Func<string>, Func<bool>> { { value, () => true } };
            else
                rules.Add( value, () => true );

            return this;
        }

        /// <summary>
        ///  Adds a value to the collection but only if the condition is met.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="condition">Condition getter.</param>
        /// <returns></returns>
        public IBaseMapper If( string value, Func<bool> condition )
        {
            if ( rules == null )
                rules = new Dictionary<Func<string>, Func<bool>> { { () => value, condition } };
            else
                rules.Add( () => value, condition );

            return this;
        }

        /// <summary>
        ///  Adds a value getter to the collection but only if the condition is met.
        /// </summary>
        /// <param name="value">Value getter.</param>
        /// <param name="condition">Condition getter.</param>
        /// <returns>Returns self.</returns>
        public IBaseMapper If( Func<string> value, Func<bool> condition )
        {
            if ( rules == null )
                rules = new Dictionary<Func<string>, Func<bool>> { { value, condition } };
            else
                rules.Add( value, condition );

            return this;
        }

        /// <summary>
        /// Adds a list of values to the collection but only if the condition is met.
        /// </summary>
        /// <param name="values">Values getter.</param>
        /// <param name="condition">Condition getter.</param>
        /// <returns>Returns self.</returns>
        public IBaseMapper If( Func<IEnumerable<string>> values, Func<bool> condition )
        {
            if ( listRules == null )
                listRules = new Dictionary<Func<IEnumerable<string>>, Func<bool>> { { values, condition } };
            else
                listRules.Add( values, condition );

            return this;
        }

        #endregion
    }
}
