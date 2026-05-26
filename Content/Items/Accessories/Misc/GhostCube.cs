using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class GhostCube : ModItem
{
    int standingStillTimer = 0;

    public override void SetDefaults()
    {
        Item.width = 56;
        Item.height = 36;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Blue;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (MathF.Abs(player.velocity.X) <= 0.02f)
        {
            standingStillTimer++;
            if (standingStillTimer > 180)
            {
                player.AddBuff(BuffID.Invisibility, 120);
            }
        }
        else
        {
            standingStillTimer = 0;
        }
    }
}

public class GhostCubeDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Ghost;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<GhostCube>(), 10));
    }
}
