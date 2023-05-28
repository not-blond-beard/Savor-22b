namespace Libplanet.Headless.Hosting;

using System.Collections.Immutable;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Blockchain.Renderers;
using Libplanet.Assets;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.Net;

public interface ILibplanetBuilder<T>
    where T : IAction, new()
{
    ILibplanetBuilder<T> UseConfiguration(Configuration configuration);

    ILibplanetBuilder<T> UseBlockPolicy(IBlockPolicy blockPolicy);
    ILibplanetBuilder<T> UseActionLoader(IActionLoader actionLoader);
    ILibplanetBuilder<T> UseRenderers(IEnumerable<IRenderer> renderers);

    ILibplanetBuilder<T> OnDifferentAppProtocolVersionEncountered(
        DifferentAppProtocolVersionEncountered differentApvEncountered);

    ILibplanetBuilder<T> UseNativeTokens(IImmutableSet<Currency> nativeTokens);

    ILibplanetBuilder<T> UseValidator(PrivateKey privateKey);

    InstantiatedNodeComponents<T> Build();
}
