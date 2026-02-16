using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using NeoParacosm.Core.Players;
using NeoParacosm.Content.Projectiles.Friendly.Melee;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class SpiritCoating : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 5);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SpiritCoatingPlayer>().Active = true;
    }
}

public class SpiritCoatingPlayer : CoatingPlayer
{
    public override int BaseCD => 30;
    public override void OnHitEffect(NPC npc, NPC.HitInfo hit)
    {
        int projType = Main.rand.NextBool(10) ? ProjectileType<SpiritProjHealing>() : ProjectileType<SpiritProjDamage>();
        Vector2 pos = npc.Center + LemonUtils.RandomVector2Circular(300, 150, 100, 100);
        Projectile.NewProjectileDirect(
            Player.GetSource_FromThis(),
            pos,
            Vector2.Zero,
            projType,
            (int)(hit.Damage * 0.5f),
            hit.Knockback * 0.5f,
            Player.whoAmI,
            ai0: 60,
            ai1: Main.rand.NextFloat(1.75f, 2.25f)
            );
    }
}

public class DungeonSpiritNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.DungeonSpirit || entity.type == NPCID.DungeonGuardian;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.DungeonSpirit)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<SpiritCoating>(), 20, 1, 1));
        }
        else if (npc.type == NPCID.DungeonGuardian)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<SpiritCoating>(), 1, 1, 1));
        }
    }
}
