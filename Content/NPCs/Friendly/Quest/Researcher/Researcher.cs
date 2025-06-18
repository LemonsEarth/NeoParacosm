using Microsoft.Xna.Framework;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Core.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;

[AutoloadHead]
public class Researcher : ModNPC
{
    int AITimer = 0;

    int talkAmount = 0; // Amount of times talk has been pressed
    Dictionary<int, int> progressTextAmount = new Dictionary<int, int>()
    {
        {0, 4},
    };

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
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
        Main.npcChatCornerItem = ItemID.TissueSample;
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
        button2 = "Quest";
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
        {
            Main.npcChatText = this.GetLocalization("TalkDialogue.Progress.P" + WorldDataSystem.ResearcherQuestProgress + ".T" + talkAmount).Format(NPC.GivenName);
            Main.npcChatText += "\n\n" + (talkAmount + 1) + "/" + progressTextAmount[WorldDataSystem.ResearcherQuestProgress];
            if (talkAmount < progressTextAmount[WorldDataSystem.ResearcherQuestProgress] - 1)
            {
                talkAmount++;
            }
            else
            {
                talkAmount = 0;
            }
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
