using UnityEngine;
using System.Threading;
public abstract class State
{
    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}
