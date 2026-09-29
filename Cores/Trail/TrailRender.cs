using KivotosMod.Cores.Trail;
using System.Collections.Generic;
using Terraria;

namespace KivotosMod.Cores.Trail
{
    public class TrailRender
    {

        public static void RenderTrail(TrailDrawDate[] drawDate, DrawSetting drawSetting)
        {
            if (drawDate.Length < 2)
                return;
            DrawTrail(drawDate, drawSetting);
        }

        public static void DrawTrail(TrailDrawDate[] DrawDate, DrawSetting drawSetting)
        {
            List<KivotosVertex> Vertexlist = new List<KivotosVertex>();

            for (int i = 0; i < DrawDate.Length; i++)
            {
                float progress = (float)i / DrawDate.Length;
                // 绘制位置
                Vector2 DrawPos = DrawDate[i].PosDate - (drawSetting.NotSkipMainScreenPosition ? Main.screenPosition : Vector2.Zero);

                // 每个片的高度与旋转
                Vector2 PrimitivesHeight = DrawDate[i].PrimitivesOffset;
                float PrimitivesHeightRot = DrawDate[i].PrimitivesHeightRot;
                Color DrawColor = DrawDate[i].DrawColor;

                Vertexlist.Add(new KivotosVertex(DrawPos - PrimitivesHeight.RotatedBy(PrimitivesHeightRot), DrawColor, new Vector3(progress, 0, 0)));
                Vertexlist.Add(new KivotosVertex(DrawPos + PrimitivesHeight.RotatedBy(PrimitivesHeightRot), DrawColor, new Vector3(progress, 1, 0)));
            }
            if (Vertexlist.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = drawSetting.Texture;
                Main.graphics.GraphicsDevice.SamplerStates[0] = drawSetting.SamplerState;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertexlist.ToArray(), 0, Vertexlist.Count - 2);
            }
        }
    }
}
