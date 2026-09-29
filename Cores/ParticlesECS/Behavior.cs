using Terraria.ModLoader;

namespace KivotosMod.Cores.ParticlesECS
{
    public class ECSParticleBehavior : ModType
    {
        public int Type;
        protected override void Register()
        {
            Type = ECSParticleDataManager.PAICollection.Count;
            if (!ECSParticleDataManager.PAICollection.Contains(this))
                ECSParticleDataManager.PAICollection.Add(this);
        }
        public virtual void OnSpawn(ref ECSParticleData data) { }
        /// <summary>
        /// 粒子的更新，默认不做任何操作
        /// </summary>
        public virtual void Update(ref ECSParticleData data) { }
        public virtual void OnKill(ref ECSParticleData data) { }
        /// <summary>
        /// 覆写这个就可以自定义绘制
        /// </summary>
        public virtual void Draw(ref ECSParticleData data) { }
    }
}
