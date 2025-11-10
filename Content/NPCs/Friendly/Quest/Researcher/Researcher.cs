using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Armor.Ranged;
using NeoParacosm.Content.Items.Armor.Summoner;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Core.UI.ResearcherUI.Dialogue;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using static NeoParacosm.Core.Systems.Data.ResearcherQuest;

namespace NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;

[AutoloadHead]
public class Researcher : ModNPC
{
    int AITimer = 0;

    public static Dictionary<int, int> AscendableItems { get; private set; }
    public static Dictionary<int, int> AscendableItems2 { get; private set; }

    public override bool NeedSaving()
    {
        return true;
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;

        AscendableItems = new Dictionary<int, int>()
        {
            { ItemID.BloodButcherer, ItemType<AscendedBloodButcherer>() },
            { ItemID.CrimsonRod, ItemType<AscendedCrimsonRod>() },
            { ItemID.TheUndertaker, ItemType<AscendedUndertaker>() },
            { ItemID.TheRottedFork, ItemType<AscendedRottedFork>() },
            { ItemID.CrimsonHelmet, ItemType<AscendedCrimsonHelmet>() },
            { ItemID.CrimsonScalemail, ItemType<AscendedCrimsonScalemail>() },
            { ItemID.CrimsonGreaves, ItemType<AscendedCrimsonGreaves>() },

            { ItemID.Vilethorn, ItemType<AscendedVilethorn>() },
            { ItemID.Musket, ItemType<AscendedMusket>() },
            { ItemID.LightsBane, ItemType<AscendedLightsBane>() },
            { ItemID.BallOHurt, ItemType<AscendedBallOHurt>() },
            { ItemID.ShadowHelmet, ItemType<AscendedShadowHelmet>() },
            { ItemID.ShadowScalemail, ItemType<AscendedShadowScalemail>() },
            { ItemID.ShadowGreaves, ItemType<AscendedShadowGreaves>() },
        };

        AscendableItems2 = new Dictionary<int, int>()
        {
            { ItemType<AscendedCrimsonRod>() , ItemType<SupremeCrimsonRod>() },
            { ItemType<AscendedBloodButcherer>() , ItemType<SupremeBloodButcherer>() },
            { ItemType<AscendedRottedFork>() , ItemType<SupremeRottedFork>() },
            { ItemType<AscendedUndertaker>() , ItemType<SupremeUndertaker>() },

            { ItemType<AscendedBallOHurt>() , ItemType<SupremeBallOHurt>() },
            { ItemType<AscendedLightsBane>() , ItemType<SupremeLightsBane>() },
            { ItemType<AscendedVilethorn>() , ItemType<SupremeVilethorn>() },
            { ItemType<AscendedMusket>() , ItemType<SupremeMusket>() },

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
        //NPC.townNPC = true;
        NPC.friendly = true;
    }

    /*public override List<string> SetNPCNameList()
    {
        return new List<string>()
        {
            "Max",
            "Sax",
            "Gavo",
            "Ryuien",
            "Lage"
        };
    }*/

    /*public override string GetChat()
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
    }*/

    /*public override void SetChatButtons(ref string button, ref string button2)
    {
        button = "Talk";
        if (ResearcherQuestProgress < ResearcherQuestProgressState.TalkedAfterCollectingData)
        {
            button2 = "";
        }
        else
        {
            button2 = "Ascend";
        }
    }*/

    /*public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (ResearcherQuestProgress == ResearcherQuestProgressState.CollectedData && talkAmount == 3)
        {
            ResearcherQuestProgress = ResearcherQuestProgressState.TalkedAfterCollectingData;
            SoundEngine.PlaySound(SoundID.Chat with { Pitch = 1f });
            talkAmount = 0;
        }
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
            AscensionUISystem UISystem = GetInstance<AscensionUISystem>();
            if (UISystem.userInterface.CurrentState == null)
            {
                UISystem.ShowUI();
            }
        }
    }*/

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
    }

    float talkDistance = 150;
    public override void AI()
    {
        ResearcherDialogueUISystem dialogueSystem = GetInstance<ResearcherDialogueUISystem>();
        if (LemonUtils.CanTalkToNPC(NPC, talkDistance))
        {
            NPC.direction = Math.Sign(NPC.DirectionTo(Main.LocalPlayer.Center).X);
            NPC.spriteDirection = NPC.direction;
            if (dialogueSystem.userInterface.CurrentState == null)
            {
                dialogueSystem.ShowUI();
            }
            else
            {
                dialogueSystem.HideUI();
            }
        }

        if (Progress == ProgressState.DownedMechBoss)
        {
            if (LemonUtils.NotClient())
            {
                NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, NPCType<ResearcherNote>());
            }
            NPC.active = false;
        }

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
