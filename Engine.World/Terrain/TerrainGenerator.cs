using System.Numerics;

namespace Engine.World.Terrain;

public sealed class TerrainGenerator
{
    private readonly int _seed;
    private readonly float _frequency;
    private readonly float _threshold;

    public TerrainGenerator(int seed, float frequency = 0.08f, float threshold = 0.0f)
    {
        _seed = seed;
        _frequency = frequency;
        _threshold = threshold;
    }

    public TerrainMap Generate(int width, int height)
    {
        var noise = new PerlinNoise(_seed);
        var map = new TerrainMap(width, height);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var value = noise.Noise(x * _frequency, y * _frequency);
                var type = value > _threshold ? TileType.Ground : TileType.Blocked;
                var tile = new Tile(type, value);
                map.SetTile(x, y, tile);
            }
        }

        return map;
    }

    /// <summary>
    /// Simple 2D Perlin-like noise implementation for reproducible terrain seeds.
    /// </summary>
    private sealed class PerlinNoise
    {
        private readonly Vector2[] _gradients;
        private readonly int[] _permutation;

        public PerlinNoise(int seed)
        {
            var rand = new Random(seed);
            _gradients = Enumerable.Range(0, 256)
                .Select(_ => RandomUnitVector(rand))
                .ToArray();
            _permutation = Enumerable.Range(0, 256).ToArray();
            for (var i = 255; i > 0; i--)
            {
                var swap = rand.Next(i + 1);
                (_permutation[i], _permutation[swap]) = (_permutation[swap], _permutation[i]);
            }
        }

        public float Noise(float x, float y)
        {
            var xi = (int)MathF.Floor(x) & 255;
            var yi = (int)MathF.Floor(y) & 255;
            var xf = x - MathF.Floor(x);
            var yf = y - MathF.Floor(y);

            var topRight = GradientDot( xi + 1, yi + 1, xf - 1, yf - 1);
            var topLeft = GradientDot( xi, yi + 1, xf, yf - 1);
            var bottomRight = GradientDot( xi + 1, yi, xf - 1, yf);
            var bottomLeft = GradientDot( xi, yi, xf, yf);

            var u = Fade(xf);
            var v = Fade(yf);

            var x1 = Lerp(bottomLeft, bottomRight, u);
            var x2 = Lerp(topLeft, topRight, u);
            var result = Lerp(x1, x2, v);
            return result;
        }

        private float GradientDot(int xi, int yi, float x, float y)
        {
            var g = _gradients[Hash(xi, yi)];
            return g.X * x + g.Y * y;
        }

        private int Hash(int x, int y)
        {
            return _permutation[(x + _permutation[y & 255]) & 255];
        }

        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        private static float Lerp(float a, float b, float t) => a + t * (b - a);

        private static Vector2 RandomUnitVector(Random rand)
        {
            var angle = (float)(rand.NextDouble() * Math.PI * 2);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }
    }
}
