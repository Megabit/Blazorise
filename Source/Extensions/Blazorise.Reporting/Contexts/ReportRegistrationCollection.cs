#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

internal sealed class ReportRegistrationCollection<T>
{
    private readonly List<(object Owner, T Value)> registrations = [];

    public void Set( object owner, T value )
    {
        int index = registrations.FindIndex( registration => ReferenceEquals( registration.Owner, owner ) );

        if ( index >= 0 )
            registrations[index] = ( owner, value );
        else
            registrations.Add( ( owner, value ) );
    }

    public bool Remove( object owner )
    {
        int index = registrations.FindIndex( registration => ReferenceEquals( registration.Owner, owner ) );

        if ( index < 0 )
            return false;

        registrations.RemoveAt( index );
        return true;
    }

    public bool TryGetValue( object owner, out T value )
    {
        int index = registrations.FindIndex( registration => ReferenceEquals( registration.Owner, owner ) );

        if ( index >= 0 )
        {
            value = registrations[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    public int IndexOf( object owner )
    {
        return registrations.FindIndex( registration => ReferenceEquals( registration.Owner, owner ) );
    }

    public IEnumerable<T> Values
    {
        get
        {
            foreach ( (object Owner, T Value) registration in registrations )
                yield return registration.Value;
        }
    }

    public T LastOrDefault => registrations.Count > 0 ? registrations[^1].Value : default;

    public int Count => registrations.Count;
}