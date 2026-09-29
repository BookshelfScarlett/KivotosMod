namespace KivotosMod.Cores.Trail
{
    public struct TrailDrawDate(Vector2 drawPos, Color drawColor, Vector2 primitivesHeight, float primitivesHeightRot)
    {
        /// <summary>
        /// 传入的世界坐标
        /// </summary>
        public Vector2 PosDate = drawPos;
        /// <summary>
        /// 传入每个点的颜色
        /// </summary>
        public Color DrawColor = drawColor;
        /// <summary>
        /// 顶点的偏移
        /// </summary>
        public Vector2 PrimitivesOffset = primitivesHeight;
        /// <summary>
        /// 顶点偏移的整体旋转
        /// </summary>
        public float PrimitivesHeightRot = primitivesHeightRot;
    }
    /// <summary>
    /// 顶点信息的结构体
    /// </summary>
    public struct DrawSetting
    {
        public Texture2D Texture;
        public SamplerState SamplerState;
        public bool NotSkipMainScreenPosition;
        public DrawSetting(Texture2D texture, bool notSkipMainScreenPosition = true)
        {
            Texture = texture;
            SamplerState = SamplerState.PointClamp;
            NotSkipMainScreenPosition = notSkipMainScreenPosition;
        }
        public DrawSetting(Texture2D texture, SamplerState samplerState, bool notSkipMainScreenPosition = true)
        {
            Texture = texture;
            SamplerState = samplerState;
            NotSkipMainScreenPosition = notSkipMainScreenPosition;
        }
    }
}
