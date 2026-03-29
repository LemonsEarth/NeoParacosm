using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public class StarCrystal : BaseCatalyst
{
    static readonly float magicDamageBoost = 5;
    static readonly float magicSpeedBoost = 12;
    static readonly float earthDamageBoost = 10;
    static readonly float earthSpeedBoost = 18;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(magicDamageBoost, magicSpeedBoost, earthDamageBoost, earthSpeedBoost);
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 24;
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.LightRed;
        Item.autoReuse = true;
        Item.mana = 16;
        Item.noMelee = false;
    }

    public override void UpdateInventory(Player player)
    {
        if (BoostIsNotActive(player))
        {
            player.NPCatalystPlayer().CatalystBoostActive[Type] = true;
            player.NPCatalystPlayer().ElementalDamageBoosts[SpellElement.Pure] += magicDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Pure] += magicSpeedBoost / 100f;
            player.NPCatalystPlayer().ElementalDamageBoosts[SpellElement.Earth] += earthDamageBoost / 100f;
            player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Earth] += earthSpeedBoost / 100f;
        }
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<StarStone>(), 1);
        recipe.AddIngredient(ItemID.CrystalShard, 10);
        recipe.AddIngredient(ItemID.SoulofLight, 5);
        recipe.AddTile(TileID.CrystalBall);
        recipe.Register();
    }
}