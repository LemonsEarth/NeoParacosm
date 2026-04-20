using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Data;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace NeoParacosm.Content.NPCs.Friendly.Town;

[AutoloadHead]
public class DragonWorshipper : ModNPC
{
    public const string ShopName = "Shop";

    private static int ShimmerHeadIndex;
    private static Profiles.StackedNPCProfile NPCProfile;

    public override LocalizedText DeathMessage => this.GetLocalization("DeathMessage");

    public override void Load()
    {
        ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.ShimmerTownTransform[Type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.

        // Influences how the NPC looks in the Bestiary
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {

        };

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        NPC.Happiness
            .SetBiomeAffection<OceanBiome>(AffectionLevel.Like)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
            .SetBiomeAffection<JungleBiome>(AffectionLevel.Love)
            .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Love)
            .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Like)
            .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Hate)
        ;

        // This creates a "profile" for ExamplePerson, which allows for different textures during a party and/or while the NPC is shimmered.
        NPCProfile = new Profiles.StackedNPCProfile(
            new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
            new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer")
        );

        ContentSamples.NpcBestiaryRarityStars[Type] = 5;
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 50;
        NPC.height = 64;
        NPC.aiStyle = -1;
        NPC.damage = 120;
        NPC.defense = 25;
        NPC.lifeMax = 1000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
    }

    public override void AI()
    {

    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary. (use localization keys)
				new FlavorTextBestiaryInfoElement("Mods.NeoParacosm.NPCs.DragonWorshipper.Bestiary"),
        ]);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        int num = NPC.life > 0 ? 1 : 5;

        for (int k = 0; k < num; k++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Granite);
        }

        if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
        {
            for (int i = 0; i < 4; i++)
            {
                LemonUtils.SmokeGore(NPC.GetSource_FromThis(), NPC.RandomPos(), 2, 4);
            }
        }
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_SpawnNPC)
        {
            // A TownNPC is "unlocked" once it successfully spawns into the world.
            TownNPCRespawnSystem.UnlockedDragonWorshipperSpawn = true;
        }
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
        if (TownNPCRespawnSystem.UnlockedDragonWorshipperSpawn)
        {
            return true;
        }

        // Player has for sure interacted with researcher, meaning they for sure saw the dragon remains
        if (ResearcherQuest.Progress >= ResearcherQuest.ProgressState.CollectedData && Main.hardMode)
        {
            return true;
        }

        return false;
    }

    public override ITownNPCProfile TownNPCProfile()
    {
        return NPCProfile;
    }

    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
                "Sellot",
                "Davoti",
                "Foroer",
                "Teyst"
            };
    }

    public override void FindFrame(int frameHeight)
    {

    }

    public override string GetChat()
    {
        WeightedRandom<string> chat = new WeightedRandom<string>();

        if (ResearcherQuest.Progress >= ResearcherQuest.ProgressState.DownedResearcher && DarkCataclysmSystem.DarkCataclysmActive)
        {
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T1"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T2"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T3"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T4"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T5"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T6"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.DCDialogue.T7"));
        }
        else
        {
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.StandardDialogue.T1"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.StandardDialogue.T2"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.StandardDialogue.T3"));
            chat.Add(Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.StandardDialogue.T4"));
        }

        string chosenChat = chat; // chat is implicitly cast to a string. This is where the random choice is made.

        return chosenChat;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Language.GetTextValue("Mods.NeoParacosm.NPCs.DragonWorshipper.TradeButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (firstButton)
        {

            shop = ShopName; // Name of the shop tab we want to open.
        }
    }

    // Not completely finished, but below is what the NPC will sell
    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, ShopName);
        //.Add<ExampleItem>()
        //.Add<EquipMaterial>()
        //.Add<BossItem>()
        //.Add(new Item(ModContent.ItemType<Items.Placeable.Furniture.ExampleWorkbench>()) { shopCustomPrice = Item.buyPrice(copper: 15) }) // This example sets a custom price, ExampleNPCShop.cs has more info on custom prices and currency.

        npcShop.Register(); // Name of this shop tab
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {

    }

    // Make this Town NPC teleport to the King and/or Queen statue when triggered. Return toKingStatue for only King Statues. Return !toKingStatue for only Queen Statues. Return true for both.
    public override bool CanGoToStatue(bool toKingStatue) => true;

    // Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
    public override void OnGoToStatue(bool toKingStatue)
    {
        StatueTeleport();
    }

    // Create a square of pixels around the NPC on teleport.
    public void StatueTeleport()
    {
        LemonUtils.DustBurst(16, NPC.Center, DustID.GemTopaz, 5, 6, 1.2f, 2f);
    }

    public override void LoadData(TagCompound tag)
    {

    }

    public override void SaveData(TagCompound tag)
    {

    }
}
