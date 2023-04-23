using System;

namespace Blueway
{
    public class Color
    {
        /// <summary>
        /// Creates a new <see cref="Color"/>.
        /// </summary>
        /// <param name="a">Alpha Channel</param>
        /// <param name="r">Red Channel</param>
        /// <param name="g">Green Channel</param>
        /// <param name="b">Blue Channel</param>
        public Color(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a new <see cref="Color"/>.
        /// </summary>
        /// <param name="n">Decimal value of the hexadecimal color (#FE0254 -> 16646740)</param>
        public Color(uint n) : this(ConvertUIntToHex(n)) { }

        private static string ConvertUIntToHex(uint n)
        {
            string hex = n.ToString("X");
            if (hex.Length > 6 && hex.Length < 8) { hex = "0" + hex; }
            if (hex.Length < 6)
            {
                for (int i = hex.Length; i < 6; i++)
                {
                    hex = "0" + hex;
                }
            }
            return hex;
        }

        /// <summary>
        /// Creates a new <see cref="Color"/>.
        /// </summary>
        /// <param name="r">Red Channel</param>
        /// <param name="g">Green Channel</param>
        /// <param name="b">Blue Channel</param>
        public Color(byte r, byte g, byte b) : this(byte.MaxValue, r, g, b)
        {
        }

        /// <summary>
        /// Create a new <see cref="Color"/>.
        /// </summary>
        /// <param name="all">Value of all channels.</param>
        /// <param name="includesAlpha">Determines if the value of <paramref name="all"/> should include the <see cref="A"/> channel or not.</param>
        public Color(byte all, bool includesAlpha = false) : this(includesAlpha ? all : byte.MaxValue, all, all, all)
        {
        }

        /// <summary>
        /// Create a new <see cref="Color"/>.
        /// </summary>
        /// <param name="all">Value of all channels.</param>
        /// <param name="includesAlpha">Determines if the value of <paramref name="all"/> should include the <see cref="A"/> channel or not.</param>
        public Color(int all, bool includesAlpha = false) : this(includesAlpha ? all : 255, all, all, all)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Color"/>.
        /// </summary>
        /// <param name="r">Red Channel</param>
        /// <param name="g">Green Channel</param>
        /// <param name="b">Blue Channel</param>
        public Color(int r, int g, int b) : this(255, r, g, b)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Color"/>.
        /// </summary>
        /// <param name="a">Alpha Channel</param>
        /// <param name="r">Red Channel</param>
        /// <param name="g">Green Channel</param>
        /// <param name="b">Blue Channel</param>
        public Color(int a, int r, int g, int b)
        {
            A = (byte)a;
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }

        /// <summary>
        /// Creates a new <see cref="Color"/> from <paramref name="hex"/>.
        /// </summary>
        /// <param name="hex">Hexadecimal of color. Examples: #a #ee #fff #0080ff #ff0080ff</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Color(string hex)
        {
            hex = hex.StartsWith("#") ? hex.Substring(1, hex.Length - 1) : hex;
            if (hex.Length == 1 || hex.Length == 2)
            {
                int all = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                R = (byte)all;
                G = (byte)all;
                B = (byte)all;
            }
            else if (hex.Length == 3)
            {
                R = (byte)int.Parse(hex.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
                G = (byte)int.Parse(hex.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
                B = (byte)int.Parse(hex.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length > 3 && hex.Length <= 6)
            {
                if (hex.Length < 6)
                {
                    int addL = 6 - hex.Length;
                    for (int i = 0; i < addL; i++)
                    {
                        hex += hex[hex.Length - 1];
                    }
                }
                var _r = hex.Substring(0, 2);
                var _g = hex.Substring(2, 2);
                var _b = hex.Substring(4, 2);
                R = (byte)int.Parse(_r, System.Globalization.NumberStyles.HexNumber);
                G = (byte)int.Parse(_g, System.Globalization.NumberStyles.HexNumber);
                B = (byte)int.Parse(_b, System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length > 6 && hex.Length <= 8)
            {
                if (hex.Length < 8)
                {
                    int addL = 8 - hex.Length;
                    for (int i = 0; i < addL; i++)
                    {
                        hex += hex[hex.Length - 1];
                    }
                }
                var _a = hex.Substring(0, 2);
                var _r = hex.Substring(2, 2);
                var _g = hex.Substring(4, 2);
                var _b = hex.Substring(6, 2);
                A = (byte)int.Parse(_a, System.Globalization.NumberStyles.HexNumber);
                R = (byte)int.Parse(_r, System.Globalization.NumberStyles.HexNumber);
                G = (byte)int.Parse(_g, System.Globalization.NumberStyles.HexNumber);
                B = (byte)int.Parse(_b, System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the color information as hexadecimal format.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public string ToHex(bool ignoreAlpha = true)
        {
            return "#" + (!ignoreAlpha ? A.ToString("X2") : "") + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }

        /// <summary>
        /// Shifts brightness of color.
        /// </summary>
        /// <param name="value">Value to increase/decrease.</param>
        /// <param name="shiftAlpha">Determines if the <see cref="A"/> should be shifted too.</param>
        /// <returns><see cref="Color"/></returns>
        public Color ShiftBrightness(int value, bool shiftAlpha = false)
        {
            var color = new Color(
                shiftAlpha ? (!IsTransparencyHigh ? (byte)HTAlt.Tools.AddIfNeeded(A, value, byte.MaxValue) : (byte)HTAlt.Tools.SubtractIfNeeded(A, value, 0)) : A,
                !IsBright ? (byte)HTAlt.Tools.AddIfNeeded(R, value, byte.MaxValue) : (byte)HTAlt.Tools.SubtractIfNeeded(R, value, 0),
                !IsBright ? (byte)HTAlt.Tools.AddIfNeeded(G, value, byte.MaxValue) : (byte)HTAlt.Tools.SubtractIfNeeded(G, value, 0),
                !IsBright ? (byte)HTAlt.Tools.AddIfNeeded(B, value, byte.MaxValue) : (byte)HTAlt.Tools.SubtractIfNeeded(B, value, 0));
            return color;
        }

        /// <summary>
        /// Gets Brightness level between 0-255.
        /// </summary>
        /// <param name="c">Color for checking brightness.</param>
        /// <returns>Level of brightness between 0-255</returns>
        public int Brightness => (int)Math.Sqrt(
               R * R * .241 +
               G * G * .691 +
               B * B * .068);

        /// <summary>
        /// Gets Transparency level between 0-255.
        /// </summary>
        /// <param name="c">Color for checking transparency.</param>
        /// <returns>Level of transparency between 0-255</returns>
        public int Transparency => A;

        /// <summary>
        /// Returns true if the color is not so opaque, otherwise false.
        /// </summary>
        /// <param name="c">Color for checking transparency.</param>
        /// <returns>Returns true if the color is not so opaque, otherwise false.</returns>
        public bool IsTransparencyHigh => A < 130;

        /// <summary>
        /// Returns true if the color is opaque, otherwise false.
        /// </summary>
        /// <param name="c">Color for checking opacity.</param>
        /// <returns>Returns true if the color is opaque, otherwise false.</returns>
        public bool IsOpaque => A == 255;

        /// <summary>
        /// Returns true if the color is invisible due to high transparency.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Returns true if the color is invisible.</returns>
        public bool IsInvisible => A == 0;

        /// <summary>
        /// Determines which color (Black or White) to use for foreground of the color.
        /// </summary>
        /// <param name="c">Color to work on.</param>
        /// <returns>Returns Black if color is bright, otherwise White.</returns>
        public Color AutoWhiteBlack => IsBright ? Color.Black : Color.White;

        /// <summary>
        /// Determines which color (Black or White) is closer to the color.
        /// </summary>
        /// <param name="c">Color to work on.</param>
        /// <returns>Returns White if color is bright, otherwise Black.</returns>
        public Color WhiteOrBlack => IsBright ? White : Black;

        /// <summary>
        /// Returns <sse cref=true"/> if the color is bright.
        /// </summary>
        /// <param name="c">Color to work on.</param>
        /// <returns><sse cref=true"/> if color is bright, otherwise <sse cref=false"/></returns>
        public bool IsBright => Brightness > 130;

        /// <summary>
        /// Reverses a color.
        /// </summary>
        /// <param name="c">Color to work on.</param>
        /// <param name="reverseAlpha"><sse cref=true"/> to also reverse Alpha (Transparency) channel.</param>
        /// <returns>Opposite of the color.</returns>
        public Color ReverseColor(bool reverseAlpha) => new Color(reverseAlpha ? (255 - A) : A,
                                  255 - R,
                                  255 - G,
                                  255 - B);

        /// <summary>
        /// Alpha Channel
        /// </summary>
        public byte A { get; set; } = 255;

        /// <summary>
        /// Red Channel
        /// </summary>
        public byte R { get; set; } = 0;

        /// <summary>
        /// Green Channel
        /// </summary>
        public byte G { get; set; } = 0;

        /// <summary>
        /// Blue Channel
        /// </summary>
        public byte B { get; set; } = 0;

        /// <summary>
        /// Generates a random color.
        /// </summary>
        /// <param name="Transparency">Value of random generated color's alpha channel. This parameter is ignored if <paramref name="RandomTransparency"/> is set to true.</param>
        /// <param name="RandomTransparency">True to randomize Alpha channel, otherwise use <paramref name="Transparency"/>.</param>
        /// <param name="Seed">Seed of random generator. Default is 172703.</param>
        /// <returns>Random color.</returns>
        public static Color RandomColor(int Transparency = 255, bool RandomTransparency = false, int Seed = 172703)
        {
            Random rand = new Random(Seed);
            int max = 256;
            int a = Transparency;
            if (RandomTransparency)
            {
                a = rand.Next(max);
            }
            int r = rand.Next(max);
            int g = rand.Next(max);
            int b = rand.Next(max);
            return new Color(a, r, g, b);
        }

        #region Colors

        public static Color White => new Color(255, 255, 255, 255);
        public static Color Gray => new Color(255, 128, 128, 128);
        public static Color Black => new Color(255, 0, 0, 0);
        public static Color Transparent => new Color(0, 0, 0, 0);
        public static Color Red => new Color(255, 255, 0, 0);
        public static Color Green => new Color(255, 0, 255, 0);
        public static Color Blue => new Color(255, 0, 0, 255);

        #endregion Colors
    }
}