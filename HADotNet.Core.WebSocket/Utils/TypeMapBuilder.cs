using System.Reflection;

namespace HADotNet.Core.WebSocket.Utils;

public static class TypeMapBuilder
{

    /// <summary>
    /// Builds a <see cref="IReadOnlyDictionary{TKey, TValue}"/> with
    /// the MessageType as key and the Type as value
    /// from the given <see cref="Assembly"/>
    /// </summary>
    /// <param name="assemblies">Assemblies</param>
    /// <returns>Type Map</returns>
    public static IReadOnlyDictionary<string, Type> BuildTypeMap(
        IEnumerable<Assembly> assemblies
    )
    {
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(a => !a.IsAbstract
                && a.IsClass
            )
            .Select(a => (a, a.GetCustomAttribute<MessageTypeAttribute>()))
            .Where(a => a.Item2 != null);

        return types.ToDictionary(
            k => k.Item2.TypeValue,
            v => v.a
        );
    }

}