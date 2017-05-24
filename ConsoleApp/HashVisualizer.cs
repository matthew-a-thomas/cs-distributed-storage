namespace ConsoleApp
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Security.Cryptography;

    internal class HashVisualizer
    {
        private readonly Func<HashAlgorithm> _hashFactory;

        public HashVisualizer(Func<HashAlgorithm> hashFactory)
        {
            _hashFactory = hashFactory;
        }

        public Bitmap MakeBitmap(byte[] input, int imageSize)
        {
            if (imageSize <= 0)
                imageSize = 1;
            var bitmap = new Bitmap(imageSize, imageSize);

            byte[] hash;
            using (var hasher = _hashFactory())
                hash = hasher.ComputeHash(input);
            const int groupSize = 8;
            var data =
                hash
                    .Select((x, i) => (x, i))
                    .GroupBy(x => x.Item2 / groupSize, x => x.Item1)
                    .Select(grouping => grouping.ToList())
                    .Select(group =>
                    {
                        while (group.Count < groupSize)
                            group.Add(0);
                        var color1 = Color.FromArgb(group[0], group[1], group[2]);
                        var color2 = Color.FromArgb(group[3], group[4], group[5]);
                        var rotation = group[6] * 90f / byte.MaxValue;
                        var size = (float)group[7] / byte.MaxValue;
                        return new
                        {
                            Color1 = color1,
                            Color2 = color2,
                            Rotation = rotation,
                            Size = size
                        };
                    })
                    .ToList();

            var numRowsAndColumns = (int)Math.Ceiling(Math.Sqrt(data.Count));
            var scale = imageSize / (numRowsAndColumns + 1f);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                foreach (var thing in data.Select((x, i) => new
                {
                    x.Color1,
                    x.Color2,
                    x.Rotation,
                    x.Size,
                    Row = i / numRowsAndColumns,
                    Column = i % numRowsAndColumns
                }))
                {
                    var size = (thing.Size * 0.5f + 0.5f) * scale;
                    var transformation = new Matrix();
                    transformation.Translate(thing.Column * scale + scale, thing.Row * scale + scale);
                    transformation.Rotate(thing.Rotation);
                    transformation.Scale(size, size);
                    var points = new[]
                    {
                        new PointF(-0.5f, 0.5f),
                        new PointF(0.5f, 0.5f),
                        new PointF(0.5f, -0.5f),
                        new PointF(-0.5f, -0.5f)
                    };
                    transformation.TransformPoints(points);
                    graphics.FillPolygon(
                        new SolidBrush(thing.Color1),
                        points
                    );
                    size *= 0.75f;
                    graphics.FillEllipse(
                        new SolidBrush(thing.Color2),
                        thing.Column * scale + scale - size / 2,
                        thing.Row * scale + scale - size / 2,
                        size,
                        size
                    );
                }
                graphics.DrawString(
                    string.Concat<string>(hash.Select(x => x.ToString("X2"))),
                    new Font("Consolas", 32),
                    new SolidBrush(Color.Black),
                    new RectangleF(0, 0, imageSize, imageSize)
                );
            }

            return bitmap;
        }
    }
}
