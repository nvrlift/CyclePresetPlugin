using AssettoServer.Server.Plugin;
using Autofac;
using CyclePresetPlugin.Preset;
using CyclePresetPlugin.Restart;
using Microsoft.Extensions.DependencyInjection;

namespace CyclePresetPlugin;

public class CyclePresetModule : AssettoServerModule<CyclePresetConfiguration>
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register Base Stuff
        builder.RegisterType<PresetConfigurationManager>().AsSelf().SingleInstance();
        builder.RegisterType<RestartImplementation>().AsSelf().SingleInstance();
        builder.RegisterType<PresetImplementation>().AsSelf().SingleInstance();
        builder.RegisterType<PresetManager>().AsSelf().SingleInstance();
        
        builder.RegisterType<CyclePresetPlugin>().AsSelf().As<IAssettoServerAutostart>().SingleInstance();
    }
}
