using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using KivotosMod.Runtime.Spine38;
using SpineBlendMode = KivotosMod.Runtime.Spine38.BlendMode;

namespace KivotosMod.Content.Tablet;

// This UI bridge is version-specific; the existing Spine 4.2 renderer stays separate.
internal sealed class TabletSpine38Scene : IDisposable
{
    private readonly TabletSpine38Animation _animation;
    private Skeleton _skeleton => _animation.Skeleton;
    private readonly BasicEffect _effect;
    private readonly RasterizerState _rasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };
    private readonly BlendState _additive = new() { ColorSourceBlend = Blend.One, ColorDestinationBlend = Blend.One,
        AlphaSourceBlend = Blend.One, AlphaDestinationBlend = Blend.One };
    private readonly BlendState _multiply = new() { ColorSourceBlend = Blend.DestinationColor, ColorDestinationBlend = Blend.InverseSourceAlpha,
        AlphaSourceBlend = Blend.One, AlphaDestinationBlend = Blend.InverseSourceAlpha };
    private readonly BlendState _screen = new() { ColorSourceBlend = Blend.One, ColorDestinationBlend = Blend.InverseSourceColor,
        AlphaSourceBlend = Blend.One, AlphaDestinationBlend = Blend.InverseSourceAlpha };
    private readonly SkeletonClipping _clipper = new();
    private static readonly int[] Quad = { 0, 1, 2, 2, 3, 0 };
    private float[] _vertices = new float[1024];
    private VertexPositionColorTexture[] _drawVertices = new VertexPositionColorTexture[512];
    public Rectangle Bounds { get; }

    public TabletSpine38Scene(Mod mod, bool sora = false)
    {
        string assetRoot = sora ? "Assets/Tablet/Home/Sora38/" : "Assets/Tablet/Home/Arona38/";
        string stem = sora ? "sora" : "arona";
        var atlas = new Atlas(new StringReader(System.Text.Encoding.UTF8.GetString(mod.GetFileBytes(assetRoot + stem + ".atlas.bin"))),
            assetRoot, new ModTextureLoader(mod));
        // Opt-in is limited to this inspected asset. No editor/version header is altered.
        var binary = new SkeletonBinary(atlas) { AllowLegacy3875 = true };
        using (var stream = new MemoryStream(mod.GetFileBytes(assetRoot + stem + ".skel.bin")))
            _animation = new TabletSpine38Animation(binary.ReadSkeletonData(stream), sora: sora);
        float[] buffer = null;
        _skeleton.GetBounds(out float x, out float y, out float w, out float h, ref buffer);
        Bounds = new Rectangle((int)MathF.Floor(x), (int)MathF.Floor(y), (int)MathF.Ceiling(w), (int)MathF.Ceiling(h));
        _effect = new BasicEffect(Main.instance.GraphicsDevice) { TextureEnabled = true, VertexColorEnabled = true,
            LightingEnabled = false, View = Matrix.Identity };
    }

    public void Update(float seconds) => _animation.Update(seconds);
    public void LookAt(float x, float y) => _animation.LookAt(x, y);
    public void Blink() => _animation.Blink();

    // Positions are in the same UI coordinate space as the surrounding SpriteBatch.
    // Restore the batch after immediate drawing; clip the skeleton to the tablet glass.
    public void Draw(SpriteBatch batch, Vector2 anchor, float scale, Rectangle clip)
    {
        GraphicsDevice device = Main.instance.GraphicsDevice;
        batch.End();
        Rectangle previousScissor = device.ScissorRectangle;
        BlendState previousBlend = device.BlendState;
        RasterizerState previousRasterizer = device.RasterizerState;
        DepthStencilState previousDepth = device.DepthStencilState;
        SamplerState previousSampler = device.SamplerStates[0];
        try
        {
            Vector2 tl = Vector2.Transform(new Vector2(clip.Left, clip.Top), Main.UIScaleMatrix);
            Vector2 br = Vector2.Transform(new Vector2(clip.Right, clip.Bottom), Main.UIScaleMatrix);
            device.ScissorRectangle = Rectangle.Intersect(new Rectangle((int)tl.X, (int)tl.Y,
                Math.Max(1, (int)MathF.Ceiling(br.X - tl.X)), Math.Max(1, (int)MathF.Ceiling(br.Y - tl.Y))), device.Viewport.Bounds);
            device.RasterizerState = _rasterizer;
            device.DepthStencilState = DepthStencilState.None;
            device.SamplerStates[0] = SamplerState.LinearClamp;
            _effect.World = Main.UIScaleMatrix;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 0, 1);
            foreach (Slot slot in _skeleton.DrawOrder)
            {
                Attachment attachment = slot.Attachment;
                if (attachment is ClippingAttachment clipping) { _clipper.ClipStart(slot, clipping); continue; }
                Texture2D texture;
                float[] uvs;
                int[] triangles;
                int count;
                float r, g, b, a;
                if (attachment is RegionAttachment region)
                {
                    texture = ((AtlasRegion)region.RendererObject).page.rendererObject as Texture2D;
                    count = 8;
                    region.ComputeWorldVertices(slot.Bone, _vertices, 0, 2);
                    uvs = region.UVs; triangles = Quad;
                    r = region.R; g = region.G; b = region.B; a = region.A;
                }
                else if (attachment is MeshAttachment mesh)
                {
                    texture = ((AtlasRegion)mesh.RendererObject).page.rendererObject as Texture2D;
                    count = mesh.WorldVerticesLength;
                    if (_vertices.Length < count) Array.Resize(ref _vertices, count * 2);
                    mesh.ComputeWorldVertices(slot, 0, count, _vertices, 0, 2);
                    uvs = mesh.UVs; triangles = mesh.Triangles;
                    r = mesh.R; g = mesh.G; b = mesh.B; a = mesh.A;
                }
                else { _clipper.ClipEnd(slot); continue; }
                _animation.ApplyEyeTracking(attachment, _vertices, count);
                float[] vertices = _vertices;
                int indexCount = triangles.Length;
                if (_clipper.IsClipping)
                {
                    _clipper.ClipTriangles(vertices, count, triangles, indexCount, uvs);
                    vertices = _clipper.ClippedVertices.Items; uvs = _clipper.ClippedUVs.Items;
                    triangles = _clipper.ClippedTriangles.Items; count = _clipper.ClippedVertices.Count;
                    indexCount = _clipper.ClippedTriangles.Count;
                }
                int vertexCount = count / 2;
                if (texture != null && vertexCount > 0 && indexCount > 0)
                {
                    if (_drawVertices.Length < vertexCount) Array.Resize(ref _drawVertices, vertexCount * 2);
                    float alpha = a * slot.A * _skeleton.A * _animation.EyeOpacity(attachment);
                    Color color = new Color(r * slot.R * _skeleton.R * alpha, g * slot.G * _skeleton.G * alpha,
                        b * slot.B * _skeleton.B * alpha, alpha);
                    for (int v = 0, p = 0; v < vertexCount; v++, p += 2)
                        _drawVertices[v] = new VertexPositionColorTexture(new Vector3(anchor.X + vertices[p] * scale,
                            anchor.Y - vertices[p + 1] * scale, 0), color, new Vector2(uvs[p], uvs[p + 1]));
                    BlendState blend = slot.Data.BlendMode switch {
                        SpineBlendMode.Additive => _additive, SpineBlendMode.Multiply => _multiply,
                        SpineBlendMode.Screen => _screen, _ => BlendState.AlphaBlend };
                    _effect.Texture = texture;
                    foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        device.BlendState = blend;
                        device.SamplerStates[0] = SamplerState.LinearClamp;
                        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _drawVertices, 0, vertexCount, triangles, 0, indexCount / 3);
                    }
                }
                _clipper.ClipEnd(slot);
            }
        }
        finally
        {
            _clipper.ClipEnd();
            device.ScissorRectangle = previousScissor;
            device.BlendState = previousBlend; device.RasterizerState = previousRasterizer;
            device.DepthStencilState = previousDepth; device.SamplerStates[0] = previousSampler;
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        }
    }

    public void Dispose()
    {
        _effect.Dispose(); _rasterizer.Dispose(); _additive.Dispose(); _multiply.Dispose(); _screen.Dispose();
    }

    private sealed class ModTextureLoader : TextureLoader
    {
        private readonly Mod _mod;
        public ModTextureLoader(Mod mod) => _mod = mod;
        public void Load(AtlasPage page, string path)
        {
            path = path.Replace('\\', '/');
            // These atlas PNGs are premultiplied by prepare_textures.py before
            // packaging. tML's rawimg conversion/reader preserves their RGB;
            // unlike direct PngReader loading, it does not premultiply for us.
            Texture2D texture = _mod.Assets.Request<Texture2D>(path[..^4], AssetRequestMode.ImmediateLoad).Value;
            page.rendererObject = texture; page.width = texture.Width; page.height = texture.Height;
        }
        public void Unload(object texture) { } // Textures are owned by Mod.Assets.
    }
}
