using System;

namespace Blueway;

internal static class Tools 
{
    public static string FormatVersion(this Version ver) 
    => "" + (ver.Major > 0 ? ver.Major : "") + (ver.Minor > 0 ? "." + ver.Minor : "") + (ver.Build > 0 ? "." + ver.Build : "") + (ver.Revision > 0? "." + ver.Revision : "");
}