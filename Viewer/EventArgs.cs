using System;

namespace Viewer;

public sealed class EventArgs<T> : EventArgs
{
    public EventArgs(T value) => Value = value;
    public static implicit operator EventArgs<T>(T value) => new(value);

    public T Value { get; }
}