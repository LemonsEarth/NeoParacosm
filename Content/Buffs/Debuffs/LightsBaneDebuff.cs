﻿using NeoParacosm.Common.Utils;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Buffs.Debuffs;

public class LightsBaneDebuff : ModBuff
{
    int timer = 0;
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {

    }

    public override void Update(Player player, ref int buffIndex)
    {

    }
}
