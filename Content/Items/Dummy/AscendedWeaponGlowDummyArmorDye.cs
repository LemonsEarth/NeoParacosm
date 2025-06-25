using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using Terraria.Graphics.Shaders;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Dummy
{
    public class AscendedWeaponGlowDummyArmorDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Avoid loading assets on dedicated servers. They don't use graphics cards.
            if (!Main.dedServ)
            {
                // The following code creates an effect (shader) reference and associates it with this item's type Id.
                GameShaders.Armor.BindShader(
                    Item.type,
                    new ArmorShaderData(Mod.Assets.Request<Effect>("Common/Assets/Shaders/Items/AscendedWeaponGlow"), "AscendedWeaponGlow") // Be sure to update the effect path and pass name here.
                );
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            int dye = Item.dye;
            Item.CloneDefaults(ItemID.GelDye);
            Item.dye = dye;
        }
    }
}
