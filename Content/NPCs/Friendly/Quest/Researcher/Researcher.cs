using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Armor.Summoner;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using NeoParacosm.Core.UI.ResearcherUI.Dialogue;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.Utilities;
using static NeoParacosm.Core.Systems.WorldDataSystem;

namespace NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;

[AutoloadHead]
public class Researcher : ModNPC
{
    int AITimer = 0;

    public static Dictionary<int, int> AscendableItems { get; private set; }

    public override bool NeedSaving()
    {
        return true;
    }

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

            { ItemID.Vilethorn, ModContent.ItemType<AscendedVilethorn>() },
            { ItemID.Musket, ModContent.ItemType<AscendedMusket>() },
            { ItemID.LightsBane, ModContent.ItemType<AscendedLightsBane>() },
            { ItemID.BallOHurt, ModContent.ItemType<AscendedBallOHurt>() },
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
            AscensionUISystem UISystem = ModContent.GetInstance<AscensionUISystem>();
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
        ResearcherDialogueUISystem dialogueSystem = ModContent.GetInstance<ResearcherDialogueUISystem>();
        if (Main.LocalPlayer.Alive() && NPC.Hitbox.Contains(Main.MouseWorld.ToPoint()) && Main.LocalPlayer.Distance(NPC.Center) < talkDistance 
            && Main.mouseRight && Main.mouseRightRelease)
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
