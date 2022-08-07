using System;

namespace Hex.Logic;

public sealed class DeterministicRandomity : ARandomity
{
    readonly Random _random = new(1);
    public override float Float() => _random.NextSingle();
}