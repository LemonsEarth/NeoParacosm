
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Misc;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.NPCs.Friendly.Special;

public class FriendlyGuardian : ModNPC
{
    ref float AITimer => ref NPC.ai[0];
    ref float DamageMultiplier => ref NPC.ai[1];
    ref float LifeMultiplier => ref NPC.ai[2];
    ref float Owner => ref NPC.ai[3];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        NPCID.Sets.CannotDropSouls[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return true;
    }

    public override bool NeedSaving() => false;

    public override void SetDefaults()
    {
        NPC.width = 102;
        NPC.height = 102;
        NPC.damage = 1000;
        NPC.SuperArmor = true;
        NPC.lifeMax = 20;
        NPC.value = 0;
        NPC.friendly = true;
        NPC.noTileCollide = true;
        NPC.ShowNameOnHover = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * LifeMultiplier);
            NPC.life = NPC.lifeMax;
            NPC.damage = (int)(NPC.damage * DamageMultiplier);

        }
        if (AITimer % 1000 == 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(NPC, NPC.position, Vector2.Zero, ProjectileType<InvisibleProjectileFriendly>(), NPC.damage, (1 - NPC.knockBackResist) * 6, -1, NPC.whoAmI, NPC.width, NPC.height);
            }
        }

        Player player = Main.player[(int)Owner];
        if (player == null)
        {
            NPC.SimpleStrikeNPC(NPC.life, 1);
            return;
        }
        Vector2 targetPos = player.Center;
        NPC closestNPC = LemonUtils.GetClosestNPC(NPC.Center, 800);
        if (closestNPC != null && closestNPC.active)
        {
            targetPos = closestNPC.Center;
        }
        NPC.MoveToPos(targetPos, 0.2f, 0.2f, 0.6f, 0.6f);
        NPC.rotation = MathHelper.ToRadians(AITimer * 12);
        if (AITimer > 0 && AITimer % 180 == 0)
        {
            if (closestNPC == null || !closestNPC.active)
            {
                NPC.life -= 4;
            }
            else
            {
                NPC.life -= 2;
            }
        }
        NPC.friendlyRegen = 0;
        AITimer++;
    }


    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemType<ResearcherNoteItem>()));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return true;
    }
}
