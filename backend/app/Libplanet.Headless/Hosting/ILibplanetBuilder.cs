namespace Libplanet.Headless.Hosting;

using System.Collections.Immutable;
using Libplanet.Action.Loader;
using Libplanet.Blockchain.Renderers;
using Libplanet.Assets;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.Net;

public interface ILibplanetBuilder
{
    ILibplanetBuilder UseConfiguration(Configuration configuration);

    ILibplanetBuilder UseBlockPolicy(IBlockPolicy blockPolicy);
    ILibplanetBuilder UseActionLoader(IActionLoader actionLoader);
    ILibplanetBuilder UseRenderers(IEnumerable<IRenderer> renderers);

    ILibplanetBuilder OnDifferentAppProtocolVersionEncountered(
        DifferentAppProtocolVersionEncountered differentApvEncountered);

    ILibplanetBuilder UseNativeTokens(IImmutableSet<Currency> nativeTokens);

    ILibplanetBuilder UseValidator(PrivateKey privateKey);

    InstantiatedNodeComponents Build();
}
