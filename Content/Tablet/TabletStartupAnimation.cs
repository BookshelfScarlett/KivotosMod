using System;

namespace KivotosMod.Content.Tablet;

// Adapted from the legacy UnlockLayer: 70 triangles, seven active at once,
// followed by a one-second fade after tapping the display.
internal sealed class TabletStartupAnimation
{
    private readonly Random _random;
    private float _fadeRemaining;
    public bool Visible { get; private set; }
    public bool Fading { get; private set; }
    public float Opacity => !Visible ? 0 : Fading ? _fadeRemaining : 1;
    public Triangle[] Triangles { get; } = new Triangle[70];

    public TabletStartupAnimation(Random random = null)
    {
        _random = random ?? Random.Shared;
        for (int row = 0; row < 7; row++)
            for (int col = 0; col < 10; col++)
                Triangles[row * 10 + col] = new Triangle(col * 56 + 7, row * 56 + 30, (row + col) % 2 == 0);
    }

    public void Start()
    {
        Reset();
        Visible = true;
        Update(0);
    }

    public void Reset()
    {
        Visible = Fading = false;
        _fadeRemaining = 0;
        foreach (Triangle triangle in Triangles) triangle.Remaining = 0;
    }

    public void Tap()
    {
        if (!Visible || Fading) return;
        Fading = true;
        _fadeRemaining = 1;
    }

    public void Update(float seconds)
    {
        if (!Visible) return;
        seconds = Math.Clamp(seconds, 0, 0.1f);
        int active = 0;
        foreach (Triangle triangle in Triangles)
        {
            triangle.Remaining = Math.Max(0, triangle.Remaining - seconds);
            if (triangle.Remaining > 0) active++;
        }
        while (active < 7)
        {
            Triangle triangle = Triangles[_random.Next(Triangles.Length)];
            if (triangle.Remaining > 0) continue;
            triangle.Remaining = triangle.Duration = _random.Next(90, 240) / 60f;
            active++;
        }
        if (Fading)
        {
            _fadeRemaining = Math.Max(0, _fadeRemaining - seconds);
            if (_fadeRemaining <= 0.00001f) Reset();
        }
    }

    internal sealed class Triangle
    {
        public readonly float X, Y;
        public readonly bool Flipped;
        public float Remaining, Duration;
        public float Opacity
        {
            get
            {
                if (Remaining <= 0) return 0;
                float t = 1 - Math.Abs(2 * Remaining / Duration - 1);
                float eased = t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
                return eased * 0.25f;
            }
        }

        public Triangle(float x, float y, bool flipped) { X = x; Y = y; Flipped = flipped; }
    }
}
