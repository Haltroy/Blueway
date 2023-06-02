using System;

namespace Blueway
{
    [Obsolete]
    internal static class DefaultThemes
    {
        public static Theme Light => new Theme(new Color(255, 255, 255, 255), new Color(255, 0, 0, 0), new Color(255, 0, 128, 255), true, "Light");
        public static Theme Dark => new Theme(new Color(255, 0, 0, 0), new Color(255, 255, 255, 255), new Color(255, 0, 128, 255), true, "Dark");
        public static Theme Breeze => new Theme(new Color(255, 0, 180, 216), new Color(255, 0, 0, 0), new Color(255, 0, 128, 255), true, "Breeze");
        public static Theme Breath => new Theme(new Color(255, 0, 216, 198), new Color(255, 0, 0, 0), new Color(255, 0, 128, 255), true, "Breath");
        public static Theme Backupster => new Theme(new Color(255, 44, 22, 43), new Color(255, 255, 255, 255), new Color(255, 0, 128, 255), false, "Backupster");
    }
}