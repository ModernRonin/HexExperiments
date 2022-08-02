using System;

namespace Hex.Logic;

public static class TypeExtensions
{
    public static object DefaultValue(this Type self) =>
        self.IsValueType || Nullable.GetUnderlyingType(self) != null
            ? Activator.CreateInstance(self)
            : null;
}