namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Tx;

public class GetUnsignedTransactionHex
{
    private readonly BlockChain _blockChain;
    private readonly Swarm? _swarm;

    public GetUnsignedTransactionHex(
        IAction action,
        PublicKey publicKey,
        BlockChain blockChain,
        Swarm? swarm
    )
    {
        Address signer = publicKey.ToAddress();
        TxActionList txActionList = new(new[] { action });

        long nonce = blockChain.GetNextTxNonce(signer);

        TxInvoice invoice = new TxInvoice(
            genesisHash: blockChain.Genesis.Hash,
            actions: txActionList
        );
        UnsignedTx unsignedTransaction = new UnsignedTx(
            invoice,
            new TxSigningMetadata(publicKey, nonce)
        );

        byte[] unsignedTransactionString = unsignedTransaction.SerializeUnsignedTx().ToArray();
        string unsignedTransactionHex = ByteUtil.Hex(unsignedTransactionString);

        UnsignedTransactionHex = unsignedTransactionHex;

        _blockChain = blockChain;
        _swarm = swarm;

// #if DEBUG
//         Signing(UnsignedTransactionHex);
// #endif
    }

    public string UnsignedTransactionHex { get; private set; }

    private static (string, string) SignTransaction(
        string unsignedTransactionHex,
        PrivateKey privateKey
    )
    {
        byte[] signature = privateKey.Sign(ByteUtil.ParseHex(unsignedTransactionHex));
        return (ByteUtil.Hex(signature), unsignedTransactionHex);
    }

    private void Signing(string unsignedTransactionHex)
    {
        var (signature, unsigned) = SignTransaction(
            unsignedTransactionHex,
            new PrivateKey("eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4")
        );

        UploadBlockChain(unsigned, signature);
    }

    private void UploadBlockChain(string _unsignedTransaction, string _signature)
    {
        byte[] signature = ByteUtil.ParseHex(_signature);
        IUnsignedTx unsignedTransaction = TxMarshaler.DeserializeUnsignedTx(
            ByteUtil.ParseHex(_unsignedTransaction)
        );

        Transaction signedTransaction = new Transaction(
            unsignedTransaction,
            signature.ToImmutableArray()
        );

        _blockChain.StageTransaction(signedTransaction);
        _swarm?.BroadcastTxs(new[] { signedTransaction });
    }
}
