namespace NeoParacosm.Content.Items.Accessories.Misc;

public class CorruptedLifeCrystal : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 26;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<CorruptedLifeCrystalPlayer>().corruptedLifeCrystal = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemID.LifeCrystal);
        recipe1.AddRecipeGroup("NeoParacosm:AnyEvilMaterial", 8);
        recipe1.AddTile(TileID.Anvils);
        recipe1.Register();
    }
}

public class CorruptedLifeCrystalPlayer : ModPlayer
{
    public bool corruptedLifeCrystal { get; set; } = false;

    public override void ResetEffects()
    {
        corruptedLifeCrystal = false;
    }

    public override void NaturalLifeRegen(ref float regen)
    {
        if (corruptedLifeCrystal)
        {
            regen *= 0;
        }
    }

    public override void PostUpdateEquips()
    {
        if (corruptedLifeCrystal)
        {
            Player.statLifeMax2 += 60;
        }
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (corruptedLifeCrystal)
        {
            healValue += 20;
        }
    }
}
