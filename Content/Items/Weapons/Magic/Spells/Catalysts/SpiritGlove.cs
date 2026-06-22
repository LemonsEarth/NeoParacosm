using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class SpiritGlove : BaseCatalyst
{
    static readonly float magicDamageBoost = 20;
    static readonly float holySpeedBoost = 25;
    static readonly float darkSpeedBoost = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicDamageBoost, holySpeedBoost, darkSpeedBoost);

    static Asset<Texture2D> inventoryTexture;

    public override void SetStaticDefaults()
    {
        inventoryTexture = Request<Texture2D>("NeoParacosm/Content/Items/Weapons/Magic/Spells/Catalysts/SpiritGlove_Inventory");
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 40;
        Item.width = 14;
        Item.height = 16;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Yellow;
        Item.autoReuse = true;
        Item.mana = 6;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.AddElementalDamageBoost(SpellElement.Pure, magicDamageBoost / 100f);
            player.AddElementalExpertiseBoost(SpellElement.Holy, holySpeedBoost / 100f);
            player.AddElementalExpertiseBoost(SpellElement.Dark, darkSpeedBoost / 100f);
        }
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
        recipe.AddIngredient(ItemType<ManasapGlove>(), 1);
        recipe.AddIngredient(ItemID.SpectreBar, 8);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}