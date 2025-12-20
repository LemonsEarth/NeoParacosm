using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class HeadstoneRing : BaseCatalyst
{
    static readonly float darkDamageBoost = 15;
    static readonly float darkSpeedBoost = 20;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(darkDamageBoost, darkSpeedBoost);

    static Asset<Texture2D> inventoryTexture;

    public override void SetStaticDefaults()
    {
        inventoryTexture = Request<Texture2D>("NeoParacosm/Content/Items/Weapons/Magic/Spells/Catalysts/HeadstoneRing_Inventory");
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 8;
        Item.width = 28;
        Item.height = 28;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.mana = 21;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Dark] += darkDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Dark] += darkSpeedBoost / 100f;
        }
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.Draw(inventoryTexture.Value, position, null, Color.White, 0f, inventoryTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Vector2 drawPos = Item.Center - Main.screenPosition;
        spriteBatch.Draw(inventoryTexture.Value, drawPos, null, Color.White, rotation, inventoryTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);

        return false;
    }
}