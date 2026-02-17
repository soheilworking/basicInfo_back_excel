using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Formats.Png;

public static class ImageSharpCaptcha
{
    /// <summary>
    /// Render a CAPTCHA PNG as byte[].
    /// If fontPath is provided and exists it will load that font file; otherwise it picks a system font fallback.
    /// </summary>
    public static byte[] RenderPng(string code, int width = 220, int height = 80, string? fontPath = null)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("code required", nameof(code));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        // prepare font family (file or system fallback)
        FontFamily fontFamily;
        if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
        {
            var collection = new FontCollection();
            fontFamily = collection.Add(fontPath);
        }
        else
        {
            var sys = SystemFonts.Families.FirstOrDefault();
            if (sys == null) throw new InvalidOperationException("No system fonts available and no font file provided.");
            fontFamily = sys;
        }

        var rnd = new Random();
        using var image = new Image<Rgba32>(width, height);

        image.Mutate(ctx =>
        {
            // background
            ctx.Fill(new Rgba32(240, 240, 245));

            // draw simple wavy lines using small rotated rectangles (robust across ImageSharp versions)
            for (int i = 0; i < 6; i++)
            {
                var color = new Rgba32((byte)rnd.Next(70, 180), (byte)rnd.Next(70, 180), (byte)rnd.Next(70, 180), 200);
                float thickness = (float)(rnd.NextDouble() * 2.0 + 0.5);

                float prevX = 0f;
                float prevY = (float)(rnd.NextDouble() * height);
                int segments = 12;

                for (int s = 1; s <= segments; s++)
                {
                    float x = (float)(s * (width / (double)segments));
                    float y = (float)(rnd.NextDouble() * height);

                    // center between prev and current
                    float cx = (prevX + x) / 2f;
                    float cy = (prevY + y) / 2f;
                    float dx = x - prevX;
                    float dy = y - prevY;
                    float len = MathF.Max(1f, MathF.Sqrt(dx * dx + dy * dy));
                    float angleRad = MathF.Atan2(dy, dx);

                    // rectangle frame as RectangleF (centered at cx,cy) then convert to polygon
                    var rect = new RectangleF(cx - len / 2f, cy - thickness / 2f, len, thickness);
                    var poly = new RectangularPolygon(rect);

                    // rotate polygon around center
                    var matrix = Matrix3x2.CreateRotation(angleRad, new Vector2(cx, cy));
                    var rotated = poly.Transform(matrix);

                    ctx.Fill(Brushes.Solid(color), rotated);

                    prevX = x;
                    prevY = y;
                }
            }

            // draw characters with variable sizes and positions
            float baseSize = MathF.Max(24, height * 0.45f);
            float glyphSpacing = width / (float)(code.Length + 1);

            for (int i = 0; i < code.Length; i++)
            {
                string ch = code[i].ToString();
                float charSize = baseSize + rnd.Next(-6, 7);
                Font font = fontFamily.CreateFont(charSize, FontStyle.Bold);

                float x = glyphSpacing * (i + 0.5f) + rnd.Next(-8, 9);
                float y = height / 2f + rnd.Next(-10, 11);

                var color = new Rgba32((byte)rnd.Next(10, 120), (byte)rnd.Next(10, 120), (byte)rnd.Next(10, 120), 255);
                var brush = Brushes.Solid(color);

                // Use DrawText simple overload: (string, Font, IBrush, PointF)
                ctx.DrawText(ch, font, brush, new PointF(x, y - font.Size / 2f));
            }

            // salt noise (random pixels)
            int dots = width * height / 100;
            for (int i = 0; i < dots; i++)
            {
                int px = rnd.Next(width);
                int py = rnd.Next(height);
                image[px, py] = new Rgba32((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            }

            // semi-transparent rectangles for texture
            for (int i = 0; i < 6; i++)
            {
                int rectW = rnd.Next(10, Math.Max(10, width / 6));
                int rectH = rnd.Next(6, Math.Max(6, height / 6));
                int rectX = rnd.Next(-rectW / 2, width);
                int rectY = rnd.Next(-rectH / 2, height);
                var overlay = new Rgba32((byte)rnd.Next(200, 255), (byte)rnd.Next(200, 255), (byte)rnd.Next(200, 255), 60);
                ctx.Fill(overlay, new Rectangle(rectX, rectY, rectW, rectH));
            }
        });

        using var ms = new MemoryStream();
        var pngEncoder = new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression };
        image.Save(ms, pngEncoder);
        return ms.ToArray();
    }
}
