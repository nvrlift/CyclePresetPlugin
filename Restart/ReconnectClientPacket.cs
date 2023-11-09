using System.Text;
using AssettoServer.Network.ClientMessages;
using AssettoServer.Shared.Network.Packets;
using AssettoServer.Shared.Network.Packets.Outgoing;
using AssettoServer.Shared.Network.Packets.Shared;

namespace CyclePresetPlugin.Restart;

[OnlineEvent(Key = "reconnectClient")]
public class ReconnectClientPacket : OnlineEvent<ReconnectClientPacket>
{
    [OnlineEventField(Name = "time")]
    public ushort Time = 0;
}
