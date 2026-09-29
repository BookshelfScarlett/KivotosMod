using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Methods.Textbox
{
    public class TextboxManager : ModSystem
    {
        public static float FirstLineY = 0;
        public static float LerpValue = 0;
        public static float EdgeValue = 0;
        public static float PrevLerpValue = 0;
        public static int CurIndex = 0;
        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.dedServ)
                return;

            Player p = Main.LocalPlayer;

            if (!Main.HoverItem.IsLegal())
            {
                LerpValue = 0;
                EdgeValue = 0;
                FirstLineY = 0;
                PrevLerpValue = 0;
                CurIndex = 0;
                return;
            }
            if (CurIndex != Main.HoverItem.type)
            {
                CurIndex = Main.HoverItem.type;
            }
            LerpValue = Lerp(LerpValue, 1.0f, 0.21f);
            if (LerpValue > 0.98f)
            {
                EdgeValue = Lerp(EdgeValue, 1f, 0.21f);
                LerpValue = 1f;
                PrevLerpValue = LerpValue;
            }
            else
            {
                PrevLerpValue = LerpValue;
            }

        }
    }
}
