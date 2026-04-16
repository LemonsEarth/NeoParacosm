using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class CircletOfFocus : ModItem
{
    readonly float critBoost = 10f;
    readonly int manaBoost = 20;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(critBoost, manaBoost);
    public override void SetDefaults()
    {
        Item.width = 46;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetCritChance(DamageClass.Magic) += critBoost;
        player.statManaMax2 += manaBoost;
        player.GetModPlayer<CircletOfFocusPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.UnicornHorn, 3)
            .AddIngredient(ItemID.CrystalShard, 12)
            .AddIngredient(ItemID.PixieDust, 15)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}

public class CircletOfFocusPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public int CircletFocusNPCIndex { get; set; } = -1;
    public float CircletFocusDamageBoost { get; set; } = 0;
    public float CircletFocusTimer = 0;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic))
        {
            if (target.whoAmI != CircletFocusNPCIndex)
            {
                CircletFocusNPCIndex = target.whoAmI;
                CircletFocusDamageBoost = 0f;
            }
            else
            {
                CircletFocusTimer = 120;
            }
        }
    }

    public override void UpdateEquips()
    {
        if (CircletFocusTimer > 0)
        {
            CircletFocusDamageBoost += 5f / 60f;
            CircletFocusTimer--;
        }

        if (CircletFocusTimer <= 0)
        {
            CircletFocusDamageBoost -= 10f / 60f;
        }

        CircletFocusDamageBoost = MathHelper.Clamp(CircletFocusDamageBoost, 0, 25f);
        if (Active)
        {
            Player.GetDamage(DamageClass.Magic) += CircletFocusDamageBoost / 100f;
        }
    }
}
