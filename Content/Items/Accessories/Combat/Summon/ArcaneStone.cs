using NeoParacosm.Content.Projectiles.Friendly.Summon;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class ArcaneStone : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 46;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ArcaneStonePlayer>().Active = true;
    }
}

public class ArcaneStonePlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (Main.myPlayer == Player.whoAmI)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner == Player.whoAmI && proj.minion)
                {
                    Projectile.NewProjectileDirect(
                        proj.GetSource_FromThis(),
                        proj.Center,
                        Vector2.Zero,
                        ProjectileType<ArcaneStoneProjectile>(),
                        proj.damage * 4,
                        proj.knockBack,
                        proj.owner,
                        ai0: 1f,
                        ai1: proj.width,
                        ai2: proj.height
                        );
                    proj.Kill();
                }
            }
        }
    }
}

public class ArcaneStoneMinionProjectile : GlobalProjectile
{
    public override void PostAI(Projectile projectile)
    {
        if (projectile.minion && projectile.GetOwner().GetModPlayer<ArcaneStonePlayer>().Active)
        {
            if (Main.rand.NextBool(300))
            {
                Projectile.NewProjectileDirect(
                    projectile.GetSource_FromThis(),
                    projectile.Center,
                    Vector2.Zero,
                    ProjectileType<ArcaneStoneProjectile>(),
                    projectile.damage * 4,
                    projectile.knockBack,
                    projectile.owner,
                    ai0: 1f,
                    ai1: projectile.width,
                    ai2: projectile.height
                    );
                projectile.Kill();
            }
        }
    }
}

public class ArcaneStoneDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Drippler;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<ArcaneStone>(), 50, 1, 1));
    }
}
