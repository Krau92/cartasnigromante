using System;
using System.Collections.Generic;

[Serializable]
public abstract class Passives
{
    public abstract string name { get; }
    public abstract void PassiveEffect();
}

