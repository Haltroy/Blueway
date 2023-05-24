using System;
using System.IO;
using System.Reflection;

namespace Blueway;

internal static class Tools
{
    public static string FormatVersion(this Version ver)
    => "" + (ver.Major > 0 ? ver.Major : "") + (ver.Minor > 0 ? "." + ver.Minor : "") + (ver.Build > 0 ? "." + ver.Build : "") + (ver.Revision > 0 ? "." + ver.Revision : "");

    public static string ReadResource(string name)
    {
        // Determine path
        var assembly = Assembly.GetExecutingAssembly();
        string resourcePath = name;

        using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}