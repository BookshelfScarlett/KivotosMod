using System;

namespace KivotosMod.Globals.Database.Enums
{
    [Flags]
    public enum KivotosDrawLayer
    {
        BeforeTiles,
        BeforeNPCs,
        BeforeProjectiles,
        BeforePlayer,
        BeforeDusts,
        AfterDusts,
        AfterProjectiles,
        EndCapture
    }
}
