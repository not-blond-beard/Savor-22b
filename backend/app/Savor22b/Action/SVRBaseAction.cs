namespace Savor22b.Action;

using Bencodex.Types;
using Libplanet.Action;
using Libplanet.State;

public abstract class SVRBaseAction : IAction
{
    public abstract IValue PlainValue { get; }

    public abstract IAccountStateDelta Execute(IActionContext context);

    public abstract void LoadPlainValue(IValue plainValue);
}
