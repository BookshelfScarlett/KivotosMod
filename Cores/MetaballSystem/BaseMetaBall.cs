using KivotosMod.Assets.Register;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Cores.MetaballSystem
{
    public abstract class BaseMetaball : ModType
    {
        public int Type = 0;
        public virtual bool SetPority => false;
        public virtual int MetaballTimer => 1;
        // 这个元球对应的渲染目标
        public RenderTarget2D AlphaTexture;
        // 这个元球对应的背景
        public virtual Texture2D BackgroundTexture => KivotosTextureAssets.Metaball_ShadowNebula.Value;

        /// <summary>
        /// 描边颜色
        /// </summary>
        public virtual Color EdgeColor => Color.White;

        /// <summary>
        /// 是否更新
        /// </summary>
        public virtual bool Active()
        {
            return false;
        }

        /// <summary>
        /// 提供的更新方法
        /// </summary>
        public virtual void Update() { }

        protected sealed override void Register()
        {
            if (!MetaballManager.MetaballList.Contains(this))
                MetaballManager.MetaballList.Add(this);
            Type = MetaballManager.MetaballList.Count;
            if (Main.netMode == NetmodeID.Server)
                return;

            Main.QueueMainThreadAction(() =>
            {
                AlphaTexture?.Dispose();
                AlphaTexture = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }

        /// <summary>
        /// 提供的绘制方法
        /// </summary>
        public virtual void PrepareRenderTarget()
        {
            Main.spriteBatch.Draw(KivotosTextureAssets.Particle_WhiteCube.Value, new Vector2(960, 540), null, Color.White, 0, KivotosTextureAssets.Particle_WhiteCube.Value.Size() / 2f, 10, SpriteEffects.None, 0f);
        }

        public virtual bool PreDrawRT2D()
        {
            return true;
        }
        public virtual void PrepareShader()
        {

            Main.graphics.GraphicsDevice.Textures[0] = AlphaTexture;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            Main.graphics.GraphicsDevice.Textures[1] = BackgroundTexture;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            Effect shader = KivotosShaderAssets.MetaBallShader;
            shader.Parameters["renderTargetSize"].SetValue(AlphaTexture.Size());
            shader.Parameters["bakcGroundSize"].SetValue(BackgroundTexture.Size());
            shader.Parameters["edgeColor"].SetValue(EdgeColor.ToVector4());
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * MetaballTimer);
            shader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
