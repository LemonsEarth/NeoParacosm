
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Misc;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;

public class ResearcherNote : ModNPC
{
    ref float AITimer => ref NPC.ai[0];
    Vector2 startPos = Vector2.Zero;

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
        return false;
    }
    public override bool NeedSaving() => true;

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 28;
        NPC.lifeMax = 128;
        NPC.friendly = true;
        NPC.dontTakeDamage = true;
        NPC.noTileCollide = true;
        NPC.ShowNameOnHover = false;
        NPC.noGravity = true;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            startPos = NPC.Center;
        }

        NPC.velocity = Vector2.Zero;
        NPC.Center = startPos + Vector2.UnitY * MathF.Sin(AITimer / 30f) * 16;

        if (LemonUtils.CanTalkToNPC(NPC, 150))
        {
            Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Death(), ItemType<ResearcherNoteItem>());
            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustPerfect(NPC.Bottom + Vector2.UnitX * Main.rand.Next(-NPC.width / 2, NPC.width / 2), DustID.GemDiamond, -Vector2.UnitY * Main.rand.NextFloat(1, 6), Scale: Main.rand.NextFloat(0.5f, 3f)).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item29, NPC.Center);
            NPC.active = false;
        }
        AITimer++;
    }


    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemType<ResearcherNoteItem>()));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        LemonUtils.DrawGlow(NPC.Center, Color.White, NPC.Opacity, 1f);
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, null, Color.White, 0, texture.Size() * 0.5f, 1f, SpriteEffects.None);
        return false;
    }
}
