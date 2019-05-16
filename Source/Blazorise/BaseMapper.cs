#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    public interface IBaseMapper
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

        protected class Rule<T>
        {
            public Func<T> Class { get; set; }

            public Func<bool> Condition { get; set; }
        }

        protected bool dirty = true;

        protected List<Rule<string>> rules;

        protected List<Rule<IEnumerable<string>>> listRules;

        #endregion

        #region Methods

        protected IEnumerable<string> GetValidRules()
        {
            for ( int r = 0; r < rules.Count; ++r )
            {
                var rule = rules[r];

                if ( !rule.Condition() ) // skip false conditions
                    continue;

                var key = rule.Class();

                if ( key != null )
                    yield return key;
            }
        }

        protected IEnumerable<string> GetValidListRules()
        {
            for ( int r = 0; r < listRules.Count; ++r )
            {
                var listRule = listRules[r];

                if ( !listRule.Condition() )
                    continue;

                var key = listRule.Class();

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
                rules = new List<Rule<string>>() { new Rule<string> { Class = () => value, Condition = () => true } };
            else
                rules.Add( new Rule<string> { Class = () => value, Condition = () => true } );

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
                rules = new List<Rule<string>>() { new Rule<string> { Class = value, Condition = () => true } };
            else
                rules.Add( new Rule<string> { Class = value, Condition = () => true } );

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
                rules = new List<Rule<string>>() { new Rule<string> { Class = () => value, Condition = condition } };
            else
                rules.Add( new Rule<string> { Class = () => value, Condition = condition } );

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
                rules = new List<Rule<string>>() { new Rule<string> { Class = value, Condition = condition } };
            else
                rules.Add( new Rule<string> { Class = value, Condition = condition } );

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
                listRules = new List<Rule<IEnumerable<string>>>() { new Rule<IEnumerable<string>> { Class = values, Condition = condition } };
            else
                listRules.Add( new Rule<IEnumerable<string>> { Class = values, Condition = condition } );

            return this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the mapping is ready to rebuild.
        /// </summary>
        internal bool IsDirty => dirty;

        #endregion
    }
}
