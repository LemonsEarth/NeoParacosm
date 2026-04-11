namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class CyborgsBodyAugment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.buyPrice(1, 3, 3, 7);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<CyborgsBodyAugmentPlayer>().Active = true;
    }
}

public class CyborgsBodyAugmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active && Main.rand.NextBool(4))
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(),
                    Player.Center,
                    -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30))) * 10,
                    ProjectileID.ClusterMineI, 150, 10f, Player.whoAmI);
            }
        }
    }
}

public class CyborgsAugmentShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Cyborg;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<CyborgsBodyAugment>(), Condition.DownedPlantera);
    }
}
