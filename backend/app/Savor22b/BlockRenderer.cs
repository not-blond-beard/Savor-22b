
namespace Savor22b;

using Libplanet.Blockchain.Renderers;
using System.Reactive.Subjects;
using SVRBlock = Libplanet.Blocks.Block;

public class BlockRenderer : IRenderer
{
    public Subject<(SVRBlock OldTip, SVRBlock NewTip)> BlockSubject =
        new Subject<(SVRBlock OldTip, SVRBlock NewTip)>();

    public void RenderBlock(SVRBlock oldTip, SVRBlock newTip) =>
        BlockSubject.OnNext((oldTip, newTip));
}
