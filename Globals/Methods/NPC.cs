using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace KivotosMod.Globals.Methods
{
    public static partial class KivotosMethods
    {
        /// <summary>
        /// 给你的目标来一拳
        /// <br>用于强制移动你的目标，你可以用这个去强制击退</br>
        /// </summary>
        public static void PunchTarget(this NPC target, Vector2 punchDirection, float punchStrength)
        {
            if (!target.IsLegal())
                return;
            Vector2 punchVel = punchDirection.ToSafeNormalize() * punchStrength;
            target.velocity = punchVel;
            //代办：给一拳的击退是需要做多人同步的
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
        }
        /// <summary>
        /// 给你的目标挑飞
        /// <br>击退方向从"远离玩家的水平方向"插值到"垂直向上"</br>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="owner"></param>
        /// <param name="heightOffset"></param>
        /// <param name="minSpeed"></param>
        /// <param name="maxSpeed"></param>
        public static void PunchTargetUp(this NPC target, Player owner, float heightOffset, float minSpeed = 10, float maxSpeed = 14)
        {
            if (!target.IsLegal())
                return;
            Vector2 punchDir = owner.Center.GetNormalVector2(target.Center);
            float mouseHeightOffset = target.Center.Y - Main.MouseWorld.Y;
            float t = Utils.GetLerpValue(0, heightOffset, mouseHeightOffset, true);
            Vector2 upDir = -Vector2.UnitY;
            Vector2 finalPuncDir = Vector2.Lerp(punchDir, upDir, t).ToSafeNormalize();
            float punchSpeed = Lerp(minSpeed, maxSpeed, t);
            if (target.type == NPCID.DungeonGuardian)
                punchSpeed *= 120;
            target.velocity = finalPuncDir * punchSpeed;
        }
    }
}
