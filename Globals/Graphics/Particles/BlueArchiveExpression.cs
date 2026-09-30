using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticleSystem;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.Particles
{
    public class BlueArchiveExpression : BaseParticle
    {
        public override BlendState UseBlendState => BlendState.AlphaBlend;
        public float RandSeedValue = 0;
        public int PlayerIndex;
        public int ExpressionType = 0;
        public Vector2 TargetOffset = Vector2.Zero;
        /// <summary>
        /// <paramref name="playerIndex"/>表示这个表情符号跟随的玩家索引，若为<see langword="-1"/>则不跟随玩家<br></br>
        /// <paramref name="type"/>为表情的种类。其中：<br></br>
        /// <br><see langword="0"/>：感叹号，<see langword="1"/>：问号</br>
        /// <br><see langword="2"/>：感叹号+问号，<see langword="3"/>：生气</br>
        /// <br><see langword="4"/>：汗，<see langword="5"/>：累</br>
        /// <br>默认为<see langword="0"/></br>
        /// </summary>
        /// <param name="playerIndex">跟随的玩家索引。如果你选择跟随玩家，那么position会被固定位玩家的中心点，而velocity实际上会变为相对于玩家中心的向量差</param>
        public BlueArchiveExpression(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, int playerIndex = -1, int type = 0)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            PlayerIndex = playerIndex;
            ExpressionType = type;
        }
        public override void OnSpawn()
        {
            RandSeedValue = Main.rand.NextFloat(ToRadians(-25f), ToRadians(25f));
            Opacity = 0f;
            if (PlayerIndex != -1)
            {
                Player player = Main.player[PlayerIndex];
                Position = player.Center + Velocity;
                TargetOffset = Velocity;
                Velocity = Vector2.Zero;
            }
        }

        public override void Update()
        {
            if (PlayerIndex != -1)
            {
                Player player = Main.player[PlayerIndex];
                if (player.dead || !player.active)
                {
                    Kill();
                    return;
                }
                Position = player.Center + TargetOffset;
            }
            if (LifetimeRatio > 0.85f)
            {
                Opacity = Lerp(Opacity, 0, 0.21f);
            }
            else
                Opacity = Lerp(Opacity, 1, 0.1f);
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {

            Texture2D texture = ExpressionType switch
            {
                1 => KivotosTextureAssets.Particle_ExpressionQuestion.Value,
                2 => KivotosTextureAssets.Particle_ExpressionQuestionShock.Value,
                3 => KivotosTextureAssets.Particle_ExpressionAngry.Value,
                4 => KivotosTextureAssets.Particle_ExpressionDrop.Value,
                5 => KivotosTextureAssets.Particle_ExpressionTired.Value,
                _ => KivotosTextureAssets.Particle_ExpressionAngry.Value,
            };
            float rotOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5) * .45f;
            //最终旋转角度
            float finalRotation = Rotation * RandSeedValue + rotOffset;
            Color edgeC = Color.Lerp(Color.Transparent, Color.White, Opacity);
            //for(int i =0; i <8;i++)
            //spriteBatch.Draw(texture, Position + (TwoPi/8f*i).ToRotationVector2()*2*Opacity - Main.screenPosition, null, edgeC.ToAddColor(), finalRotation, Vector2.Zero, Scale, 0, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, finalRotation, Vector2.Zero, Scale, 0, 0f);
        }
    }
}
