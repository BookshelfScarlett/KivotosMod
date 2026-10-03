using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Cores.Halos
{
    public partial class BaseHaloManager : ModSystem
    {
        /// <summary>
        /// 光环正常情况下应该画不了那么多吧？
        /// </summary>
        public const int MaxHalows = 256;
        public static readonly List<BaseHalo> HaloCollection = [];
        public override void Load()
        {
        }
        public override void ClearWorld()
        {
            HaloCollection.Clear();
        }
        /// <summary>
        /// 在射弹里面更新而非在dust里更新
        /// <br>这样一定程度上可以确保更新层与绘制层能立刻接上</br>
        /// </summary>
        public override void PostUpdateProjectiles()
        {
            if (HaloCollection.Count == 0)
                return;
            for (int i = 0; i < HaloCollection.Count; i++)
            {
                HaloCollection[i].Update();
                HaloCollection[i].Position += HaloCollection[i].Velocity;
                HaloCollection[i].Time++;
            }
            HaloCollection.RemoveAll(j =>
            {
                if (j.Time >= j.Lifetime)
                {
                    j.OnKill();
                    return true;
                }
                return false;
            });
        }
        public static void DrawHalo(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);
            if (HaloCollection.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < HaloCollection.Count; i++)
                {
                    HaloCollection[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
        }
    }
}
