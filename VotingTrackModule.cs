using AssettoServer.Server.Plugin;
using Autofac;
using nvrlift.AssettoServer.ContentManager;
using nvrlift.AssettoServer.Restart;
using nvrlift.AssettoServer.Track;

namespace VotingTrackPlugin;

public class VotingTrackModule : AssettoServerModule<VotingTrackConfiguration>
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register Base Stuff
        builder.RegisterType<WindowsFileRestartImplementation>().As<IRestartImplementation>().SingleInstance();
        builder.RegisterType<TrackImplementation>().AsSelf().SingleInstance();
        builder.RegisterType<TrackManager>().AsSelf().SingleInstance();
        builder.RegisterType<ContentManagerImplementation>().AsSelf().SingleInstance();
        
        builder.RegisterType<VotingTrackPlugin>().AsSelf().As<IAssettoServerAutostart>().SingleInstance();
    }
}
