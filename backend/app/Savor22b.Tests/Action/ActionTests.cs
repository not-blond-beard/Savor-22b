namespace Savor22b.Tests.Action;

using Libplanet;
using Libplanet.Crypto;

public class ActionTests
{
    private readonly PrivateKey _signer = new PrivateKey();

    public ActionTests()
    {
    }

    public PrivateKey Signer()
    {
        return _signer;
    }

    public Address SignerAddress()
    {
        return _signer.PublicKey.ToAddress();
    }
}
