using System;

namespace Hex.Logic;

public interface IClock
{
    DateTime Now { get; }
}