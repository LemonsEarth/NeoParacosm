using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.Subworlds.Gravefield;

public class Gravefield : Subworld
{
    public override int Width => 1200;
    public override int Height => 1200;

    public override bool ShouldSave => true;

    public override bool NoPlayerSaving => false;

    public override List<GenPass> Tasks => throw new NotImplementedException();

    static Asset<Texture2D> bg;

    public override void Load()
    {
        bg = Request<Texture2D>("NeoParacosm/Common/Assets/Textures/Backgrounds/thewholefuckingthing");
    }

    public override void Unload()
    {
        //bg.Dispose();
    }

    public override void OnLoad()
    {

    }

    public override void OnUnload()
    {

    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void DrawMenu(GameTime gameTime)
    {
        Main.spriteBatch.Draw(bg.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        base.DrawMenu(gameTime);
    }
}
