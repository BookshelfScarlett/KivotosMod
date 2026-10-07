using KivotosMod.Cores.ParticleSystem;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics.Particles;
using KivotosMod.Globals.Players.Vanitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Cores.Halos
{
    public abstract class BaseHalo
    {
        public int Type;
        public virtual string Texture => KivotosContent.GetAsset(KivotosContent.Items + "Vanity/" + HaloOwner, "Halo");
        public bool Important = false;
        /// <summary>
        /// 光环跟随的角色
        /// <br>需要与本地化键值的名字相同</br>
        /// <br>这个键值名也用于处理玩家切换实装的处死</br>
        /// </summary>
        public virtual string HaloOwner => "Kei";
        /// <summary>
        /// 这个光环需要跟随的玩家
        /// <br>主要用于时装</br>
        /// </summary>
        public int PlayerIndex = -1;
        /// <summary>
        /// 该光环存在了多少帧，一般不需要手动修改这个值
        /// </summary>
        public int Time = 0;
        /// <summary>
        /// 光环的存在时间上限
        /// </summary>
        public int Lifetime = 0;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Origin;
        public Color DrawColor;
        public float Rotation;
        public float Scale = 1;
        public float Opcacity;
        public float LifetimeRatios => Time / (float)Lifetime;
        public virtual BlendState UseBlendState => BlendState.AlphaBlend;
        public virtual void OnSpawn() 
        {
            Opcacity = 0;
        }
        /// <summary>
        /// 光环更新之前的逻辑
        /// <br>正常情况下复写这个来实现你需要的逻辑</br>
        /// <br>返回否时会阻止<see cref="Update()"/></br>
        /// </summary>
        public virtual bool PreUdate()
        {
            return true; 
        }
        /// <summary>
        /// 光环更新
        /// <br>默认处理了与玩家有关的生命周期和运动，如果你需要做别的，推荐复写PreUpdate()</br>
        /// </summary>
        public virtual void Update()
        {
            if (!PreUdate())
                return;
            if (PlayerIndex == -1)
            {
                Kill();
                return;
            }
            Player player = Main.player[PlayerIndex];
            bool illegalPlayer =player.dead||!player.active;
            KivotosVanitysPlayer kivotosVanitysPlayer = player.GetModPlayer<KivotosVanitysPlayer>();
            if(kivotosVanitysPlayer.VanityName != HaloOwner)
            {
            }
        }
        /// <summary>
        /// 立刻清除光环
        /// </summary>
        public void Kill()
        {
            Time = Lifetime;
            OnKill();

        }
        public virtual void OnKill()
        {

        }
        public BaseHalo Spawn()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            BaseHalowMethod.AddHalo(this);
            return this;
        }
        public virtual void Draw(SpriteBatch sb)
        {
            Texture2D halo = Request<Texture2D>(Texture).Value;
            sb.Draw(halo, Position - Main.screenPosition, null, DrawColor * Opcacity, Rotation, halo.Size() / 2f, Scale, 0, 0);
        }

    }
}
