using System.Reflection;

namespace DemoComponentUse.Data;

internal static class DataReader
{
    public static string ReadResource(this string name)
    {
        using Stream? stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(Assembly.GetExecutingAssembly()
            .GetManifestResourceNames().Single(str => str.EndsWith(name)));
        using StreamReader reader = new(stream!);
        return reader.ReadToEnd();
    }

}
