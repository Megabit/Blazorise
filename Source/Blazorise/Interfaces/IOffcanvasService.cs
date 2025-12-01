#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Together with <see cref="OffcanvasProvider"/>, handles the instantiation of offcanvas panels with custom content.
/// </summary>
public interface IOffcanvasService
{
    /// <summary>
    /// Required <see cref="OffcanvasProvider"/> that manages the instantiation of offcanvas components with custom content.
    /// </summary>
    public OffcanvasProvider OffcanvasProvider { get; }

    /// <summary>
    /// Sets the required <see cref="OffcanvasProvider"/>.
    /// </summary>
    /// <param name="offcanvasProvider"></param>
    public void SetOffcanvasProvider( OffcanvasProvider offcanvasProvider );

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public Task<OffcanvasInstance> Show<TComponent>() where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="title"></param>
    public Task<OffcanvasInstance> Show<TComponent>( string title ) where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="title"></param>
    /// <param name="offcanvasInstanceOptions"></param>
    public Task<OffcanvasInstance> Show<TComponent>( string title, OffcanvasInstanceOptions offcanvasInstanceOptions ) where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="parameters"></param>
    public Task<OffcanvasInstance> Show<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="offcanvasInstanceOptions"></param>
    public Task<OffcanvasInstance> Show<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters, OffcanvasInstanceOptions offcanvasInstanceOptions ) where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="title"></param>
    /// <param name="parameters"></param>
    public Task<OffcanvasInstance> Show<TComponent>( string title, Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="title"></param>
    /// <param name="parameters"></param>
    /// <param name="offcanvasInstanceOptions"></param>
    public Task<OffcanvasInstance> Show<TComponent>( string title, Action<OffcanvasProviderParameterBuilder<TComponent>> parameters, OffcanvasInstanceOptions offcanvasInstanceOptions ) where TComponent : notnull, IComponent;

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is of <paramref name="componentType"/>.
    /// </summary>
    /// <param name="componentType"></param>
    public Task<OffcanvasInstance> Show( Type componentType );

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is of <paramref name="componentType"/>.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="componentType"></param>
    public Task<OffcanvasInstance> Show( string title, Type componentType );

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is a <see cref="RenderFragment"/>.
    /// </summary>
    /// <param name="content"></param>
    public Task<OffcanvasInstance> Show( RenderFragment content )
        => Show( string.Empty, content );

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is a <see cref="RenderFragment"/>.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public Task<OffcanvasInstance> Show( string title, RenderFragment content )
        => Show( title, content, null );

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is of <paramref name="componentType"/>.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="componentType"></param>
    /// <param name="componentParameters"></param>
    /// <param name="offcanvasInstanceOptions"></param>
    public Task<OffcanvasInstance> Show( string title, Type componentType, Dictionary<string, object> componentParameters = null, OffcanvasInstanceOptions offcanvasInstanceOptions = null );

    /// <summary>
    /// Shows an <see cref="Offcanvas"/> where the content is a <see cref="RenderFragment"/>.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="childContent"></param>
    /// <param name="offcanvasInstanceOptions"></param>
    public Task<OffcanvasInstance> Show( string title, RenderFragment childContent, OffcanvasInstanceOptions offcanvasInstanceOptions = null );

    /// <summary>
    /// Shows the offcanvas from a given offcanvas instance.
    /// </summary>
    public Task Show( OffcanvasInstance offcanvasInstance );

    /// <summary>
    /// Hides currently opened offcanvas.
    /// </summary>
    public Task Hide();

    /// <summary>
    /// Hides the offcanvas.
    /// </summary>
    public Task Hide( OffcanvasInstance offcanvasInstance );

    /// <summary>
    /// Returns all the offcanvas instances.
    /// </summary>
    public IEnumerable<OffcanvasInstance> GetInstances();

    /// <summary>
    /// Explicitly removes the offcanvas instance from the <see cref="OffcanvasProvider"/>.
    /// </summary>
    public Task Remove( OffcanvasInstance offcanvasInstance );

    /// <summary>
    /// Resets the state of the <see cref="OffcanvasProvider"/>.
    /// Any existing instances will be cleared.
    /// </summary>
    public Task Reset();
}