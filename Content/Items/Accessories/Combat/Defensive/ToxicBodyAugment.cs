namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class ToxicBodyAugment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ToxicBodyAugmentPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Stinger, 12)
            .AddRecipeGroup("NeoParacosm:AnyGoldBar", 15)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class ToxicBodyAugmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active && Main.rand.NextBool(6))
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.DistanceSQ(Player.Center) < 250 * 250 && npc.CanBeChasedBy())
                {
                    npc.AddBuff(BuffID.Poisoned, 300);
                }
            }
        }
    }
}
