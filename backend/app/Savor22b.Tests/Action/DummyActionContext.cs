namespace Savor22b.Tests.Action;

using System.Security.Cryptography;
using Libplanet;
using Libplanet.Action;
using Libplanet.State;
using Libplanet.Tx;


public class DummyActionContext : IActionContext
{
    public Address Signer { get; set; }

    public TxId? TxId { get; set; }

    public Address Miner { get; set; }

    public long BlockIndex { get; set; }

    public bool Rehearsal { get; set; }

    public IAccountStateDelta PreviousStates { get; set; }

    public IRandom Random { get; set; }

    public HashDigest<SHA256>? PreviousStateRootHash { get; set; }

    public bool BlockAction { get; }

    public long GasUsed() => 0;

    public long GasLimit() => 0;

    public IActionContext GetUnconsumedContext()
    {
        return new DummyActionContext
        {
            Signer = Signer,
            TxId = TxId,
            Miner = Miner,
            BlockIndex = BlockIndex,
            Rehearsal = Rehearsal,
            PreviousStates = PreviousStates,
            Random = Random,
            PreviousStateRootHash = PreviousStateRootHash,
        };
    }

    public void PutLog(string log)
    {
    }
}
