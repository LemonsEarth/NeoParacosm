namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class UltimateAugment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.sellPrice(1, 3, 3, 1);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<UltimateAugmentPlayer>().Active = true;
        if (player.HasBuff(BuffID.Ironskin) && player.HasBuff(BuffID.Endurance))
        {
            player.noKnockback = true;
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<ArmoredBodyAugment>())
            .AddIngredient(ItemType<HauntedBodyAugment>())
            .AddIngredient(ItemType<FlamingBodyAugment>())
            .AddIngredient(ItemType<ToxicBodyAugment>())
            .AddIngredient(ItemType<CyborgsBodyAugment>())
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class UltimateAugmentPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Active && info.Damage > Player.statLifeMax2 * 0.1f)
        {
            int roll = Main.rand.Next(0, 5);
            switch (roll)
            {
                case 0:
                    foreach (var npc in Main.ActiveNPCs)
                    {
                        if (npc.DistanceSQ(Player.Center) < 250 * 250 && npc.CanBeChasedBy())
                        {
                            npc.AddBuff(BuffID.Venom, 480);
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            Player.Center,
                            -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-60, 60))) * 6,
                            ProjectileID.BallofFire,
                            150, 6f, Player.whoAmI);
                    }
                    break;
                case 2:
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            Player.Center,
                            Vector2.UnitY.RotatedByRandom(6.28f) * 3,
                            ProjectileID.InsanityShadowFriendly,
                            125, 6f,
                            Player.whoAmI,
                            ai0: 1); // ai0 > 0 mutes it for some reason
                    }
                    break;
                case 3:
                    Player.AddBuff(BuffID.Ironskin, 60 * 30);
                    Player.AddBuff(BuffID.Endurance, 60 * 30);
                    break;
                case 4:
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(),
                            Player.Center,
                            -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30))) * 10,
                            ProjectileID.ClusterMineI, 150, 10f, Player.whoAmI);
                    }
                    break;
            }
        }
    }
}
