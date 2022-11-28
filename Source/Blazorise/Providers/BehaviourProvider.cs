using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public abstract class BehaviourProvider : IBehaviourProvider
{
    #region DataGrid

    public abstract bool DataGridRowMultiSelectPreventClick { get; }

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member