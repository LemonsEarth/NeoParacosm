using NeoParacosm.Content.Items.Accessories.Combat.Summon;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class MysticCrystalBrooch : ModItem
{
    readonly float damageBoost = 10f;
    readonly float pureDamageBoost = 15f;
    readonly float pureExpertiseBoost = 15f;
    readonly float iceDamageBoost = 15f;
    readonly float iceExpertiseBoost = 15f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, pureDamageBoost, pureExpertiseBoost, iceDamageBoost, iceExpertiseBoost);
    public override void SetDefaults()
    {
        Item.width = 52;
        Item.height = 100;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetDamage(DamageClass.Magic) += damageBoost / 100f;
        player.GetDamage(DamageClass.Summon) += damageBoost / 100f;
        player.AddElementalDamageBoost(SpellElement.Pure, pureDamageBoost / 100f);
        player.AddElementalDamageBoost(SpellElement.Ice, iceDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, pureExpertiseBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, iceExpertiseBoost / 100f);
        player.GetModPlayer<MysticCrystalBroochPlayer>().Active = true;

        if (player.whoAmI == Main.myPlayer && NPPlayer.Timer % 60 == 0)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner == player.whoAmI && proj.minion)
                {
                    Projectile.NewProjectileDirect(
                        player.GetSource_FromThis(),
                        proj.Center,
                        -Vector2.UnitY * proj.velocity.Length() / 3f,
                        ProjectileID.NorthPoleSnowflake,
                        proj.damage / 2,
                        proj.knockBack / 2,
                        player.whoAmI,
                        ai1: Main.rand.Next(0, 3)
                    );
                }
            }
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<MysticCrystal>())
            .AddIngredient(ItemType<IcicleCollectorsBrooch>())
            .AddIngredient(ItemID.FrostCore, 1)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}

public class MysticCrystalBroochPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic))
        {
            if ((int)Player.GetCritChance(DamageClass.Magic) > 0 && Main.rand.NextBool(100 / (int)Player.GetCritChance(DamageClass.Magic))) // if crit
            {
                Player.AddBuff(BuffID.ManaRegeneration, 600);
                Player.AddBuff(BuffID.MagicPower, 600);
            }
            modifiers.DisableCrit();
        }
    }
}
