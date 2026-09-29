using KivotosMod.Content.Projs.StudentWeapons;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivotosMod.Content.Items.StudentWeapons.TrinityGeneralSchool
{
    public class KoharuWeapon: StudentWeaponClass
    {
        protected override string Owner => "Koharu";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.HoshinoPink);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 35;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<HoshinoWeaponHeldProj>();
        }
    }
}
