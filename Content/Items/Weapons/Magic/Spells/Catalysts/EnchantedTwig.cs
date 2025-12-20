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

public class EnchantedTwig : BaseCatalyst
{
    static readonly float magicDamageBoost = 5;
    static readonly float magicSpeedBoost = 12;
    static readonly float natureDamageBoost = 10;
    static readonly float natureSpeedBoost = 18;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicDamageBoost, magicSpeedBoost, natureDamageBoost, natureSpeedBoost);

    static Asset<Texture2D> inventoryTexture;

    public override void SetStaticDefaults()
    {
        inventoryTexture = Request<Texture2D>("NeoParacosm/Content/Items/Weapons/Magic/Spells/Catalysts/EnchantedTwig_Inventory");
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 8;
        Item.width = 14;
        Item.height = 18;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Blue;
        Item.autoReuse = true;
        Item.mana = 11;
    }

    public override void UpdateInventory(Player player)
    {
        player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Pure] += magicDamageBoost / 100f;
        player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Pure] += magicSpeedBoost / 100f;
        player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Nature] += natureDamageBoost / 100f;
        player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Nature] += natureSpeedBoost / 100f;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.Draw(inventoryTexture.Value, position, null, Color.White, 0f, inventoryTexture.Size() * 0.5f, 0.6f, SpriteEffects.None, 0);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Vector2 drawPos = Item.Center - Main.screenPosition;
        spriteBatch.Draw(inventoryTexture.Value, drawPos, null, Color.White, rotation, inventoryTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);

        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.LivingWoodWand, 1);
        recipe.AddIngredient(ItemID.ManaCrystal, 1);
        recipe.AddIngredient(ItemID.Wood, 10);
        recipe.AddIngredient(ItemID.BorealWood, 10);
        recipe.AddIngredient(ItemID.RichMahogany, 10);
        recipe.AddIngredient(ItemID.PalmWood, 10);
        recipe.AddTile(TileID.LivingLoom);
        recipe.Register();
    }
}