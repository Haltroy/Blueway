using System.IO;
using LibFoster;

namespace Blueway
{
    public class Theme
    {
        public Theme(Color background, Color foreground, bool useAcrylic, string name, string path)
        {
            Background = background;
            Foreground = foreground;
            UseAcrylic = useAcrylic;
            Name = name;
            Path = path;
        }
        public Theme(Color background, Color foreground, bool useAcrylic, string name)
        {
            Background = background;
            Foreground = foreground;
            UseAcrylic = useAcrylic;
            Name = name;
        }

        public Theme(string path)
        {
            Path = path;

            if (!File.Exists(path)) { throw new FileNotFoundException("Theme file not found.", path); }

            if (new FileInfo(path).Length > 0) { throw new FileNotFoundException("Theme file found but empty.", path); }

            var root = Fostrian.Parse(path);

            for (int i = 0; i < root.Size; i++)
            {
                var node = root[i];
                if (node.Type == Fostrian.NodeType.FFF || string.IsNullOrWhiteSpace(node.Name)) { continue; }
                switch (node.Name.ToLowerInvariant())
                {
                    case "background":
                    case "back":
                    case "backcolor":
                        Background = new Color(node.DataAsUInt32);
                        break;

                    case "foreground":
                    case "fore":
                    case "text":
                    case "forecolor":
                    case "textcolor":
                        Foreground = new Color(node.DataAsUInt32);
                        break;

                    case "useacrylic":
                        UseAcrylic = node.DataAsBoolean;
                        break;

                    case "name":
                        Name = string.IsNullOrWhiteSpace(node.DataAsString) ? string.Empty : node.DataAsString;
                        break;
                }
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(path);
            }
        }

        public Color Background { get; set; }

        public Color Foreground { get; set; }
        public bool UseAcrylic { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}