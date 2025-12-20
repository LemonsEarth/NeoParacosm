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

public class LightningRod : BaseCatalyst
{
    static readonly float lightningDamageBoost = 15;
    static readonly float lightningSpeedBoost = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(lightningDamageBoost, lightningSpeedBoost);
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 36;
        Item.width = 64;
        Item.height = 64;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.LightRed;
        Item.autoReuse = true;
        Item.mana = 16;
        Item.noMelee = false;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Lightning] += lightningDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Lightning] += lightningSpeedBoost / 100f;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.CopperBar, 24);
        recipe.AddIngredient(ItemID.PalladiumBar, 15);
        recipe.AddIngredient(ItemID.SoulofFlight, 5);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}