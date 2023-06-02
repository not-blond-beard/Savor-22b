namespace Savor22b.Action;

using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;

public class SVRActionLoader : IActionLoader
{
    private readonly IActionLoader _actionLoader;

    public SVRActionLoader()
    {
        _actionLoader = TypedActionLoader.Create(typeof(SVRBaseAction).Assembly, typeof(SVRBaseAction));
    }

    /// <inheritdoc cref="IActionLoader.LoadAction"/>
    public IAction LoadAction(long index, IValue value) => _actionLoader.LoadAction(index, value);
}
