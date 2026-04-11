namespace NeoParacosm.Content.Items.Accessories.Combat.Ranged;

public class ObsidianArrowhead : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1, 50);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ObsidianArrowheadPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<SharpenedArrowhead>())
            .AddIngredient(ItemID.Hellstone, 3)
            .AddIngredient(ItemID.Obsidian, 20)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class ObsidianArrowheadPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.type == ProjectileID.WoodenArrowFriendly)
        {
            modifiers.ArmorPenetration += 10;
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.type == ProjectileID.WoodenArrowFriendly)
        {
            for (int i = 0; i < 6; i++)
            {
                Projectile.NewProjectileDirect(
                    Player.GetSource_FromThis(),
                    proj.Center,
                    Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(3f, 5f),
                    ProjectileID.CrystalShard,
                    proj.damage / 6,
                    3f,
                    Player.whoAmI
                    );
            }
        }
    }
}
