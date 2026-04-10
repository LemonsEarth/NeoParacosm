using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Items.Accessories.Combat.Magic;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Items.BossSummons;

public class DecrepitBirdWhistle : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 3;
        ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
    }

    public override void SetDefaults()
    {
        Item.width = 66;
        Item.height = 48;
        Item.maxStack = 1;
        Item.rare = ItemRarityID.Green;
        Item.useAnimation = 60;
        Item.useTime = 60;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
    }

    public override bool CanUseItem(Player player)
    {
        return !NPC.AnyNPCs(NPCType<Deathbird>()) && player.InModBiome<DeadForestBiome>();
    }

    public override bool? UseItem(Player player)
    {  
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0f, 1f) }, player.Center);
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.2f, 0.4f) }, player.Center);
        SoundEngine.PlaySound(SoundID.DD2_WyvernScream with { PitchRange = (0.5f, 0.7f) }, player.Center);
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC.SpawnBoss((int)player.MountedCenter.X, (int)player.MountedCenter.Y - 300, NPCType<Deathbird>(), player.whoAmI);
        }
        else
        {
            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: Type);
        }
        return true;
    }
}

public class DecrepitBirdWhistleDropNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCType<SpearKnight>()
            || entity.type == NPCType<ShieldKnight>()
            || entity.type == NPCType<StaffKnight>();
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<DecrepitBirdWhistle>(), 25));
    }
}
