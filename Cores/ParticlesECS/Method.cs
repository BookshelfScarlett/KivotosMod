namespace KivotosMod.Cores.ParticlesECS
{
    public static class ECSMethod
    {
        public static int NewParticle(int Type, int timeLeft, Vector2 position, Vector2 velocity, Color drawColor, float opacity = 1, float rotation = 0, float scale = 0, BlendState blendstate = null, float ai0 = 0, float ai1 = 0, float ai2 = 0, bool aibool0 = false, bool aibool1 = false, bool aibool2 = false, int aiint0 = 0, int aiint1 = 0, int aiint2 = 0)
        {
            ECSParticleBehavior p = ECSParticleDataManager.PAICollection[Type];
            BlendState curState = blendstate is null ? BlendState.AlphaBlend : blendstate;
            if (curState == BlendState.AlphaBlend)
            {
                if (ECSParticleDataManager.activePoint_alpha < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_alpha[ECSParticleDataManager.activePoint_alpha];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, Vector2.Zero, opacity,
                        rotation, scale, ai0, ai1, ai2, aibool0, aibool1, aibool2, aiint0, aiint1, aiint2);
                    ECSParticleDataManager.activePoint_alpha++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_alpha - 1;
                }
            }
            else if (curState == BlendState.Additive)
            {
                if (ECSParticleDataManager.activePoint_add < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_add[ECSParticleDataManager.activePoint_add];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, Vector2.Zero, opacity,
                        rotation, scale, ai0, ai1, ai2, aibool0, aibool1, aibool2, aiint0, aiint1, aiint2);
                    ECSParticleDataManager.activePoint_add++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_add - 1;
                }
            }
            else
            {
                if (ECSParticleDataManager.activePoint_Nonmult < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_nopremult[ECSParticleDataManager.activePoint_Nonmult];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, Vector2.Zero, opacity,
                        rotation, scale, ai0, ai1, ai2, aibool0, aibool1, aibool2, aiint0, aiint1, aiint2);
                    ECSParticleDataManager.activePoint_Nonmult++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_Nonmult - 1;
                }
            }
            return -1;
        }
        public static int NewParticle(int Type, int timeLeft, Vector2 position, Vector2 velocity, Color drawColor, Vector2 scale,
            float opacity = 1, float rotation = 0, BlendState blendstate = null, float ai0 = 0, float ai1 = 0, float ai2 = 0,
            bool aibool0 = false, bool aibool1 = false, bool aibool2 = false, int aiint0 = 0, int aiint1 = 0, int aiint2 = 0)
        {
            ECSParticleBehavior p = ECSParticleDataManager.PAICollection[Type];
            BlendState curState = blendstate is null ? BlendState.AlphaBlend : blendstate;
            if (curState == BlendState.AlphaBlend)
            {
                if (ECSParticleDataManager.activePoint_alpha < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_alpha[ECSParticleDataManager.activePoint_alpha];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, scale, opacity, rotation, 0, ai0, ai1, ai2, aibool0, aibool1, aibool2, aiint0, aiint1, aiint2);
                    ECSParticleDataManager.activePoint_alpha++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_alpha - 1;
                }
            }
            else if (curState == BlendState.Additive)
            {
                if (ECSParticleDataManager.activePoint_add < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_add[ECSParticleDataManager.activePoint_add];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, scale, opacity, rotation, 0, ai0, ai1, ai2, aibool0, aibool1, aibool2, aiint0, aiint1, aiint2);
                    ECSParticleDataManager.activePoint_add++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_add - 1;
                }
            }
            else
            {
                if (ECSParticleDataManager.activePoint_Nonmult < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_nopremult[ECSParticleDataManager.activePoint_Nonmult];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, scale, opacity, rotation, 0, ai0, ai1, ai2, aibool0, aibool1, aibool2, aiint0, aiint1, aiint2);
                    ECSParticleDataManager.activePoint_Nonmult++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_Nonmult - 1;
                }
            }
            return -1;
        }
        /// <summary>
        /// 你只是怕了
        /// </summary>
        public static void ReSetParticleInfo(ref ECSParticleData particleDate, int Type, int timeLeft, Vector2 position, Vector2 velocity, Color drawColor, Vector2 scale2,
            float opacity = 1f, float rotation = 0, float scale = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0,
            bool aibool0 = false, bool aibool1 = false, bool aibool2 = false, int aiint0 = 0, int aiint1 = 0, int aiint2 = 0)
        {
            particleDate = ECSParticleDataManager.TempleParticle;

            particleDate.Lifetime = timeLeft;
            particleDate.Active = true;
            particleDate.Type = Type;
            particleDate.whoAmI = ECSParticleDataManager.activePoint_Nonmult;
            particleDate.Position = position;
            particleDate.Velocity = velocity;
            particleDate.DrawColor = drawColor;
            particleDate.Rotation = rotation;
            particleDate.Scale = scale;
            particleDate.Scale2 = scale2;
            particleDate.aifloat0 = ai0;
            particleDate.aifloat1 = ai1;
            particleDate.aifloat2 = ai2;
            particleDate.aibool0 = aibool0;
            particleDate.aibool1 = aibool1;
            particleDate.aibool2 = aibool2;
            particleDate.aiint0 = aiint0;
            particleDate.aiint1 = aiint1;
            particleDate.aiint2 = aiint2;
            particleDate.Opacity = opacity;
        }
    }
}
