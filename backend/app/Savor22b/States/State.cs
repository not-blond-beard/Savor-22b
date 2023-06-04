namespace Savor22b.States;

using Bencodex.Types;


public interface State
{
    IValue Serialize();
}

