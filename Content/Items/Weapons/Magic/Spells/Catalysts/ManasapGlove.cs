using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class ManasapGlove : BaseCatalyst
{
    static readonly float magicDamageBoost = 10;
    static readonly float magicSpeedBoost = 25;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicDamageBoost, magicSpeedBoost);

    static Asset<Texture2D> inventoryTexture;

    public override void SetStaticDefaults()
    {
        inventoryTexture = Request<Texture2D>("NeoParacosm/Content/Items/Weapons/Magic/Spells/Catalysts/ManasapGlove_Inventory");
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 12;
        Item.width = 14;
        Item.height = 16;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Blue;
        Item.autoReuse = true;
        Item.mana = 8;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Pure] += magicDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Pure] += magicSpeedBoost / 100f;
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
        recipe.AddIngredient(ItemID.ManaCrystal, 2);
        recipe.AddIngredient(ItemID.Leather, 3);
        recipe.AddRecipeGroup("NeoParacosm:AnyEvilMaterial", 10);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}