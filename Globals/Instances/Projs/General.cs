using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Instances.Projs
{
    public partial class KivotosGlobalProjs : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool FirstFrame = false;
        public override void AI(Projectile projectile)
        {
            if (!FirstFrame)
            {
                FirstFrame = true;
            }
        }
    }
}
