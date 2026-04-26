using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;

namespace Numerologia.UnitTests;

/// <summary>
/// Base class for bUnit tests that render MudBlazor components.
/// Registers all MudBlazor services and ensures MudPopoverProvider
/// is rendered before each component under test.
/// </summary>
public abstract class MudBlazorTestBase : TestContext
{
    private IRenderedComponent<MudPopoverProvider>? _popoverProvider;
    private bool _popoverProviderRendered;

    protected MudBlazorTestBase()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
    }

    /// <summary>
    /// Returns the markup of the entire render tree, including content
    /// portal-ed to MudPopoverProvider (e.g. MudMenu ChildContent).
    /// </summary>
    protected string FullMarkup => (_popoverProvider?.Markup ?? "") + "";

    /// <summary>
    /// Searches for components of type <typeparamref name="T"/> in both
    /// the component's own tree AND in the MudPopoverProvider tree (where
    /// MudMenu ChildContent is portal-ed in MudBlazor v9).
    /// </summary>
    protected IEnumerable<IRenderedComponent<T>> FindAllComponents<T>(IRenderedFragment cut)
        where T : IComponent
    {
        var fromCut = cut.FindComponents<T>();
        var fromPopover = _popoverProvider?.FindComponents<T>() ?? [];
        return fromCut.Concat(fromPopover);
    }

    protected new IRenderedComponent<TComponent> RenderComponent<TComponent>(
        Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder)
        where TComponent : IComponent
    {
        EnsurePopoverProvider();
        return base.RenderComponent<TComponent>(parameterBuilder);
    }

    protected IRenderedComponent<TComponent> RenderComponent<TComponent>()
        where TComponent : IComponent
    {
        EnsurePopoverProvider();
        return base.RenderComponent<TComponent>();
    }

    private void EnsurePopoverProvider()
    {
        if (_popoverProviderRendered) return;
        _popoverProvider = base.RenderComponent<MudPopoverProvider>();
        _popoverProviderRendered = true;
    }
}
