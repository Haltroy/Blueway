namespace Blueway
{
    static class DefaultThemes
    {
        public static Theme Light => new Theme(new Color(128, 255, 255, 255), new Color(255, 0, 0, 0), true, "Light");
        public static Theme Dark => new Theme(new Color(128, 0, 0, 0), new Color(255, 255, 255, 255), true, "Dark");
        public static Theme Breeze => new Theme(new Color(128, 0, 180, 216), new Color(255, 0, 0, 0), true, "Breeze");
        public static Theme Breath => new Theme(new Color(128, 0, 216, 198), new Color(255, 0, 0, 0), true, "Breath");
        public static Theme Backupster => new Theme(new Color(128, 44, 22, 43), new Color(255, 255, 255, 255), true, "Backupster");
    }
}