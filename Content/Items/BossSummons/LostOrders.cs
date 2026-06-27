using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Bosses.DeathKnightCaptain;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.BossSummons;

public class LostOrders : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 3;
        ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
    }

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 52;
        Item.maxStack = 1;
        Item.rare = ItemRarityID.Yellow;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        //player.itemLocation += new Vector2(-16 * player.direction, 16);
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
    }

    public override bool CanUseItem(Player player)
    {
        return !NPC.AnyNPCs(NPCType<DeathKnightCaptain>()) && player.InModBiome<DeadForestBiome>();
    }

    public override bool? UseItem(Player player)
    {
        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (0.2f, 0.5f) }, player.Center);

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC.SpawnBoss((int)player.MountedCenter.X, (int)player.MountedCenter.Y - 300, NPCType<DeathKnightCaptain>(), player.whoAmI);
        }
        else
        {
            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: Type);
        }
        return true;
    }
}

public class LostOrdersDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCType<SpearKnight>()
            || entity.type == NPCType<ShieldKnight>()
            || entity.type == NPCType<StaffKnight>()
            || entity.type == NPCType<BombKnight>();
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<LostOrders>(), 50));
    }
}
