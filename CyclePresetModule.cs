using AssettoServer.Server.Plugin;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using nvrlift.AssettoServer.Preset;
using nvrlift.AssettoServer.Restart;
using nvrlift.AssettoServer.Track;

namespace CyclePresetPlugin;

public class CyclePresetModule : AssettoServerModule<CyclePresetConfiguration>
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register Base Stuff
        builder.RegisterType<PresetConfigurationManager>().AsSelf().SingleInstance();
        builder.RegisterType<RestartImplementation>().AsSelf().SingleInstance();
        builder.RegisterType<TrackImplementation>().AsSelf().SingleInstance();
        builder.RegisterType<TrackManager>().AsSelf().SingleInstance();
        
        builder.RegisterType<CyclePresetPlugin>().AsSelf().As<IAssettoServerAutostart>().SingleInstance();
    }
}
