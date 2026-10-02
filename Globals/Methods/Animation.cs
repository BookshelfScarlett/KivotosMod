using Terraria;

namespace KivotosMod.Globals.Methods
{
    public struct AnimationStruct(int slot)
    {
        public bool[] IsDone = new bool[slot];
        public int[] Progress = new int[slot];
        public int[] MaxProgress = new int[slot];
        public float[] Buffer = new float[slot];
    }
    public static class AniMethods
    {
        public static bool UpdateAniState(this AnimationStruct animationStruct, int slotID, float bufferLength = 0)
        {
            animationStruct.Progress[slotID]++;
            if (animationStruct.Progress[slotID] >= animationStruct.MaxProgress[slotID])
            {
                if (bufferLength > 0)
                {
                    animationStruct.Buffer[slotID]++;
                    if (animationStruct.Buffer[slotID] >= bufferLength)
                        animationStruct.IsDone[slotID] = true;
                }
                else
                    animationStruct.IsDone[slotID] = true;
            }
            return false;
        }
        public static float GetAniProgress(this AnimationStruct animationStruct, int slotID)
        {
            int id = slotID;
            float progress = animationStruct.Progress[slotID] / (float)animationStruct.MaxProgress[slotID];
            return Clamp(progress, 0f, 1f);
        }
        public static bool OnAnimationBegin(this AnimationStruct animationStruct, int slotID) => GetAniProgress(animationStruct, slotID) == 0;
        public static float UpdateAngle(this AnimationStruct animationHelper, float BeginAngle, float EndAngle, int Filp, float Progress, float PreFilpAdd = 0)
        {
            float startAngleOffset = ToRadians(BeginAngle);
            float endAngleOffset = ToRadians(EndAngle);
            float baseRotation = Lerp(startAngleOffset, endAngleOffset, Progress) + PreFilpAdd;
            if (Filp == -1)
                baseRotation = baseRotation * Filp;
            return baseRotation;
        }
        public static Vector2 ToTargetPosByMartix(this float rot, float tarScale, float width = 1, float height = 1)
        {
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(width, height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * tarScale;
            return tarPos;
        }
        public static float ToCurAnimationRot(this AnimationStruct helper, float beginAngle, float endAngle, int dir, bool Flip, float easedProgress, float preFlipAdd = 0)
        {
            return helper.UpdateAngle(beginAngle * Flip.ToDirectionInt(), endAngle * Flip.ToDirectionInt(), dir, easedProgress, preFlipAdd);
        }
    }
}
