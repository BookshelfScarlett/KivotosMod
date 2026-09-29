using Terraria;
using Terraria.ID;

namespace KivotosMod.Cores.ParticleSystem
{
    /// <summary>
    ///<para>该粒子系统面向对象，主要适合一些需要单独做效果的粒子</para>
    ///<para>如果你需要一些粒子上Shader或者之类的，就该使用这种</para>
    ///<para>比ECSP粒子系统用起来更加简易且自由，但性能很低，不建议大量生成j</para>
    /// </summary>
    public abstract class BaseParticle
    {
        #region 基础属性
        public bool Important = false;
        /// <summary>
        /// 使用材质
        /// </summary>
        public virtual string Texture => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');

        /// <summary>
        /// 该粒子存在了多少帧，一般不需要手动修改这个值
        /// </summary>
        public int Time = 0;

        /// <summary>
        /// 粒子的存在时间上限
        /// </summary>
        public int Lifetime = 0;

        /// <summary>
        /// 位置与向量
        /// </summary>
        public Vector2 Position;
        public Vector2 Velocity;

        public Vector2 Origin;
        public Color DrawColor;
        public float Rotation;
        public float Scale = 1f;

        /// <summary>
        /// 粒子的透明度
        /// </summary>
        public float Opacity = 1f;

        /// <summary>
        /// 生命周期的进度，介于0到1之间。
        /// 0表示粒子刚生成，1表示粒子消失。
        /// </summary>
        public float LifetimeRatio => Time / (float)Lifetime;

        /// <summary>
        /// 渲染的混合模式，默认为<see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        public virtual BlendState UseBlendState => BlendState.AlphaBlend;
        #endregion

        /// <summary>
        /// 在世界内生成粒子
        /// </summary>
        /// <returns></returns>
        public BaseParticle Spawn()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            if (UseBlendState == BlendState.AlphaBlend)
            {
                if (!Important && BaseParticleManager.ActiveParticlesAlpha.Count > ParticleUtilities.MaxParticles)
                    BaseParticleManager.ActiveParticlesAlpha.RemoveAt(0);
                BaseParticleManager.ActiveParticlesAlpha.Add(this);
            }
            else if (UseBlendState == BlendState.Additive)
            {
                if (!Important && BaseParticleManager.ActiveParticlesAdditive.Count > ParticleUtilities.MaxParticles)
                    BaseParticleManager.ActiveParticlesAdditive.RemoveAt(0);
                BaseParticleManager.ActiveParticlesAdditive.Add(this);
            }
            else
            {
                if (!Important && BaseParticleManager.ActiveParticlesNonPremultiplied.Count > ParticleUtilities.MaxParticles)
                    BaseParticleManager.ActiveParticlesNonPremultiplied.RemoveAt(0);
                BaseParticleManager.ActiveParticlesNonPremultiplied.Add(this);
            }
            OnSpawn();
            return this;
        }
        /// <summary>     
        /// 在世界内生成粒子   
        /// </summary>
        /// <returns></returns>
        public BaseParticle SpawnToPriority()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            if (UseBlendState == BlendState.AlphaBlend)
            {
                if (!Important && BaseParticleManager.PriorityActiveParticlesAlpha.Count > ParticleUtilities.MaxParticles)
                    BaseParticleManager.PriorityActiveParticlesAlpha.RemoveAt(0);
                BaseParticleManager.PriorityActiveParticlesAlpha.Add(this);
            }
            else if (UseBlendState == BlendState.Additive)
            {
                if (!Important && BaseParticleManager.PriorityActiveParticlesAdditive.Count > ParticleUtilities.MaxParticles)
                    BaseParticleManager.PriorityActiveParticlesAdditive.RemoveAt(0);
                BaseParticleManager.PriorityActiveParticlesAdditive.Add(this);
            }
            else
            {
                if (!Important && BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Count > ParticleUtilities.MaxParticles)
                    BaseParticleManager.PriorityActiveParticlesNonPremultiplied.RemoveAt(0);
                BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Add(this);
            }
            OnSpawn();
            return this;
        }
        public BaseParticle SpawnToPriorityNonPreMult()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            // 初始化时间
            Time = 0;
            if (!Important && BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Count > ParticleUtilities.MaxParticles)
                BaseParticleManager.PriorityActiveParticlesNonPremultiplied.RemoveAt(0);
            BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Add(this);
            OnSpawn();
            return this;
        }
        public BaseParticle SpawnToNonPreMult()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            // 初始化时间
            Time = 0;
            if (!Important && BaseParticleManager.ActiveParticlesNonPremultiplied.Count > ParticleUtilities.MaxParticles)
                BaseParticleManager.ActiveParticlesNonPremultiplied.RemoveAt(0);
            BaseParticleManager.ActiveParticlesNonPremultiplied.Add(this);
            OnSpawn();
            return this;
        }
        public virtual void OnSpawn() { }

        /// <summary>
        /// 粒子的更新，默认不做任何操作
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// 立刻清除粒子
        /// </summary>
        public void Kill()
        {
            Time = Lifetime;
        }

        public virtual void OnKill() { }

        /// <summary>
        /// 覆写这个就可以自定义绘制
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
