namespace NeoParacosm.Core.Systems.Particles.Renderers;

public class BeforeDustParticleRenderer : ParticleRenderer
{
    public override void Load()
    {
        On_Main.DrawDust += On_Main_DrawDust;
    }

    private void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
    {
        DrawParticles();
        orig(self);
    }
}

public class AfterDustParticleRenderer : ParticleRenderer
{
    public override void Load()
    {
        On_Main.DrawDust += On_Main_DrawDust;
    }

    private void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        DrawParticles();
    }
}
