using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using NeoParacosm.Core.Players;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class CraterCoating : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<CraterCoatingPlayer>().Active = true;
    }
}

public class CraterCoatingPlayer : CoatingPlayer
{
    public override int BaseCD => 60;
    public override void OnHitEffect(NPC npc, NPC.HitInfo hit)
    {
        Vector2 pos = npc.Center + new Vector2(Main.rand.NextFloat(-300, 300), -800);
        float maxScale = Main.rand.NextBool(4) ? 6 : 2;
        Projectile.NewProjectileDirect(
            Player.GetSource_FromThis(),
            pos,
            pos.DirectionTo(npc.Center) * Main.rand.NextFloat(5, 10),
            Main.rand.Next(ProjectileID.Meteor1, ProjectileID.Meteor3 + 1),
            (int)(hit.Damage * 0.75f),
            hit.Knockback * 0.5f,
            Player.whoAmI,
            ai1: Main.rand.NextFloat(1f, maxScale) // Scale
            ).DamageType = DamageClass.Melee; // Changing to melee damage
    }
}

public class MeteorHeadNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.MeteorHead;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<CraterCoating>(), 20, 1, 1));
    }
}
