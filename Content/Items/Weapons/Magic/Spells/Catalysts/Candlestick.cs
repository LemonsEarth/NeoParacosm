using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class Candlestick : BaseCatalyst
{
    static readonly float fireDamageBoost = 15;
    static readonly float fireSpeedBoost = 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(fireDamageBoost, fireSpeedBoost);

    static Asset<Texture2D> inventoryTexture;

    public override void SetStaticDefaults()
    {
        inventoryTexture = Request<Texture2D>("NeoParacosm/Content/Items/Weapons/Magic/Spells/Catalysts/Candlestick_Inventory");
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 15;
        Item.width = 16;
        Item.height = 16;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.mana = 12;
    }

    public override void UpdateInventory(Player player)
    {     
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Fire] += fireDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Fire] += fireSpeedBoost / 100f;
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
        recipe.AddIngredient(ItemID.Torch, 1);
        recipe.AddIngredient(ItemID.Hellstone, 10);
        recipe.AddIngredient(ItemID.ManaCrystal, 1);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}