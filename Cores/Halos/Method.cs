using KivotosMod.Globals.Players.Vanitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace KivotosMod.Cores.Halos
{
    public static class BaseHalowMethod
    {
        public static bool HasAnyHalo(this Player player)
        {
            foreach (var halo in BaseHaloManager.HaloCollection)
            {
                if (halo.PlayerIndex == player.whoAmI)
                    return true;
            }
            return false;
        }
        public static bool HasCertainHalo(this Player player, string haloNam)
        {
            foreach (var halo in BaseHaloManager.HaloCollection)
            {
                if (player.whoAmI != halo.PlayerIndex)
                    continue;
                if (halo.HaloOwner == haloNam)
                    return true;
            }
            return false;
        }
        public static void AddHalo(this Player player, BaseHalo haloType)
        {
            if (!haloType.Important && BaseHaloManager.HaloCollection.Count > BaseHaloManager.MaxHalows)
                BaseHaloManager.HaloCollection.RemoveAt(0);
            BaseHaloManager.HaloCollection.Add(haloType);
        }
        public static void AddHalo(BaseHalo haloType)
        {
            if (!haloType.Important && BaseHaloManager.HaloCollection.Count > BaseHaloManager.MaxHalows)
                BaseHaloManager.HaloCollection.RemoveAt(0);
            BaseHaloManager.HaloCollection.Add(haloType);
        }

    }
}
