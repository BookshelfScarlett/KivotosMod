
using KivotosMod.Globals.Database.Enums;

namespace KivotosMod.Cores.PixelatedRender
{
    /// <summary>
    /// 只支持BeforePlayers与BeforeDusts图层
    /// </summary>
    public interface IPixelatedRenderer
    {
        BlendState BlendState => BlendState.AlphaBlend;
        KivotosDrawLayer LayerToRenderTo => KivotosDrawLayer.BeforeDusts;
        void RenderPixelated(SpriteBatch spriteBatch);
    }
}
