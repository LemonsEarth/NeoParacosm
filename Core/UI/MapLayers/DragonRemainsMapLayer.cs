using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Placeable.Tiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Map;
using Terraria.UI;

namespace NeoParacosm.Core.UI.MapLayers
{
    public class DragonRemainsMapLayer : ModMapLayer
    {
        public override Position GetDefaultPosition() => new Before(IMapLayer.Pings);
        Asset<Texture2D> texture;
        public override void Load()
        {
            texture = Request<Texture2D>("NeoParacosm/Core/UI/MapLayers/DragonRemainsMapIcon");
        }
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (!Main.mapFullscreen) return;
            if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.NotDownedEvilBoss) return; // only mark after evil boss
            const float scaleIfNotSelected = 1f;
            const float scaleIfSelected = scaleIfNotSelected * 1.25f;

            if (ResearcherQuest.Progress == ResearcherQuest.ProgressState.DownedPlantera) 
            {
                context.Draw(ParacosmTextures.GlowBallTexture.Value, ResearcherQuest.DragonRemainsTileEntityPos.ToVector2(), Color.White, new SpriteFrame(1, 1, 0, 0), 1f, 1f, Alignment.Center);
            }

            if (context.Draw(texture.Value, ResearcherQuest.DragonRemainsTileEntityPos.ToVector2(), Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
            {
                text = "Dragon Remains";
            }
        }
    }
}
