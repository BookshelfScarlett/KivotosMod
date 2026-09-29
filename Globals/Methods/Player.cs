using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Methods
{
    public static partial class KivotosMethods
    {
        /// <summary>
        /// 判断玩家是否正在手持指定类型的物品。
        /// </summary>
        public static bool IsHolding<T>(this Player player) where T : ModItem => IsHolding(player, ItemType<T>());
        /// <summary>
        /// 判断玩家是否正在手持指定物品ID的物品。
        /// </summary>
        public static bool IsHolding(this Player player, int itemID) => player.HeldItem.type == itemID;
        /// <summary>
        /// 控制玩家的手臂旋转，可选择前臂、后臂或双臂。
        /// </summary>
        /// <param name="player">目标玩家。</param>
        /// <param name="armRot">手臂的目标旋转角度（世界坐标系下的弧度，通常为武器/物品的方向）。</param>
        /// <param name="specificArm">
        /// 指定要控制的手臂：<br/>
        /// -  <c>0</c>（默认）：同时控制前臂和后臂。<br/>
        /// -  <c>&gt;0</c>（正数）：仅控制前臂（CompositeArmFront）。<br/>
        /// -  <c>&lt;0</c>（负数）：仅控制后臂（CompositeArmBack）。
        /// </param>
        /// <param name="customArmRot">
        /// 手臂的本地基础偏移角度（弧度），用于调整手臂默认朝向。实际最终旋转角度 = <paramref name="armRot"/> - <paramref name="customArmRot"/>。<br/>
        /// 默认值为 <see cref="PiOver2"/>（90°），表示手臂默认指向正右方时，需要减去此偏移以获得正确的贴图旋转。
        /// </param>
        public static void ControlPlayerArm(this Player player, float armRot, int specificArm = 0, float customArmRot = MathHelper.PiOver2)
        {
            float armType = Math.Sign(specificArm);
            switch (armType)
            {
                case 1:
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    break;
                case -1:
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    break;
                default:
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    break;
            }
        }
        /// <summary>
        /// 获取玩家指向鼠标的单位向量。
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector2 ToMouseVector2(this Player player) => (Main.MouseWorld - player.Center).ToSafeNormalize();
        /// <summary>
        /// 应用武器攻击速度，返回实际攻击间隔
        /// </summary>
        public static int ApplyWeaponAttackSpeed(this Player player, Item item, int time, int Min)
        {
            float a = player.GetWeaponAttackSpeed(item);
            float Mult = 1f / a;
            int RealAttack = (int)(time * Mult);
            if (RealAttack < Min)
                return Min;
            else
                return RealAttack;
        }
        public static bool HasProj<T>(this Player player) where T : ModProjectile => HasProj(player, ProjectileType<T>());
        public static bool HasProj(this Player player, int projID) => player.ownedProjectileCounts[projID] > 0;

        /// <summary>
        /// 重载一个out传参，输出你判定的拥有的proj的ID以方便后续可能需要的计算，或者别的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="player"></param>
        /// <param name="ProjID"></param>
        /// <returns></returns>
        public static bool HasProj<T>(this Player player, out int ProjID) where T : ModProjectile
        {
            ProjID = ProjectileType<T>();
            return HasProj<T>(player);
        }

    }
}
