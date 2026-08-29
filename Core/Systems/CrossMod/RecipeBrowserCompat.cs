using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

namespace NeoParacosm.Core.Systems.CrossMod;

public class RecipeBrowserCompat : ModSystem
{
    public override void PostSetupContent()
    {
        if (ModLoader.TryGetMod("RecipeBrowser", out Mod rb) && !Main.dedServ)
        {
            rb.Call(
                "AddItemCategory",
                "Catalysts",
                "Weapons",
                Mod.Assets.Request<Texture2D>("Core/Systems/CrossMod/CatalystIconSmall"),
                (Predicate<Item>)((Item item) =>
                {
                    if (item.ModItem != null && item.ModItem is BaseCatalyst)
                    {
                        return true;
                    }
                    return false;
                })
            );

            rb.Call(
                "AddItemCategory",
                "Spells",
                "Weapons",
                Mod.Assets.Request<Texture2D>("Core/Systems/CrossMod/SpellIconSmall"),
                (Predicate<Item>)((Item item) =>
                {
                    if (item.ModItem != null && item.ModItem is BaseSpell)
                    {
                        return true;
                    }
                    return false;
                })
            );
        }
    }
}
