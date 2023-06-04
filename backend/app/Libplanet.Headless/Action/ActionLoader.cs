namespace Libplanet.Headless.Action;

using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;

public class ActionLoader : IActionLoader
{
    private readonly IActionLoader _actionLoader;

    public ActionLoader()
    {
        _actionLoader = TypedActionLoader.Create(typeof(BaseAction).Assembly, typeof(BaseAction));
    }

    /// <inheritdoc cref="IActionLoader.LoadAction"/>
    public IAction LoadAction(long index, IValue value) => _actionLoader.LoadAction(index, value);
}
