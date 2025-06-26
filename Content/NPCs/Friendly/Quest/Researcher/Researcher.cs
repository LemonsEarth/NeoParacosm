using NeoParacosm.Content.Items.Armor.Summoner;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Core.UI.ResearcherUI;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using Terraria.Utilities;
using static NeoParacosm.Core.Systems.WorldDataSystem;

namespace NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;

[AutoloadHead]
public class Researcher : ModNPC
{
    int AITimer = 0;

    int talkAmount = 0; // Amount of times talk has been pressed
    Dictionary<int, int> progressTextAmount = new Dictionary<int, int>()
    {
        {0, 1},
        {1, 4},
        {2, 1},
    };
    public static Dictionary<int, int> AscendableItems { get; private set; }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
        AscendableItems = new Dictionary<int, int>()
        {
            { ItemID.BloodButcherer, ModContent.ItemType<AscendedBloodButcherer>() },
            { ItemID.CrimsonRod, ModContent.ItemType<AscendedCrimsonRod>() },
            { ItemID.TheUndertaker, ModContent.ItemType<AscendedUndertaker>() },
            { ItemID.TheRottedFork, ModContent.ItemType<AscendedRottedFork>() },
            { ItemID.CrimsonHelmet, ModContent.ItemType<AscendedCrimsonHelmet>() },
            { ItemID.CrimsonScalemail, ModContent.ItemType<AscendedCrimsonScalemail>() },
            { ItemID.CrimsonGreaves, ModContent.ItemType<AscendedCrimsonGreaves>() },
        };
    }

    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 50;
        NPC.lifeMax = 250;
        NPC.defense = 15;
        NPC.damage = 20;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 500;
        NPC.knockBackResist = 0.3f;
        NPC.dontTakeDamage = true;
        NPC.aiStyle = -1;
        NPC.townNPC = true;
        NPC.friendly = true;
    }

    public override List<string> SetNPCNameList()
    {
        return new List<string>()
        {
            "Max",
            "Sax",
            "Gavo",
            "Ryuien",
            "Lage"
        };
    }

    public override string GetChat()
    {
        NPC.direction = Math.Sign(NPC.DirectionTo(Main.LocalPlayer.Center).X);
        NPC.spriteDirection = NPC.direction;
        WeightedRandom<string> chat = new WeightedRandom<string>();

        int msgCount = 4;
        for (int i = 0; i < msgCount; i++)
        {
            chat.Add(this.GetLocalization("ChatInteractDialogue.T" + i).Value);
        }

        string chosenChat = chat;

        return chosenChat;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = "Talk";
        if (ResearcherQuestProgress < ResearcherQuestProgressState.CollectedData)
        {
            button2 = "";
        }
        else
        {
            button2 = "Ascend";
        }
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (talkAmount >= progressTextAmount[(int)ResearcherQuestProgress])
        {
            talkAmount = 0;
        }
        if (firstButton)
        {
            Main.npcChatText = this.GetLocalization($"TalkDialogue.Progress.P{(int)ResearcherQuestProgress}.T{talkAmount}").Format(NPC.GivenName);
            Main.npcChatText += "\n\n" + $"{talkAmount + 1}/{progressTextAmount[(int)ResearcherQuestProgress]}"; // dialogue left
            talkAmount++;
        }
        else
        {
            ResearcherUISystem UISystem = ModContent.GetInstance<ResearcherUISystem>();
            if (UISystem.userInterface.CurrentState == null)
            {
                UISystem.ShowUI();
            }
            Main.CloseNPCChatOrSign();
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }


    public override void AI()
    {

        AITimer++;
    }

    public override void OnKill()
    {

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }
}
