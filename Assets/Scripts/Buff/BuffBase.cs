using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase
{
    public virtual void OnTurnEnd() { }
    public virtual void OnAdd() { }
    public virtual void OnMerge() { }
}
