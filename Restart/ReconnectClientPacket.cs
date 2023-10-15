using System.Text;
using AssettoServer.Shared.Network.Packets;
using AssettoServer.Shared.Network.Packets.Outgoing;
using AssettoServer.Shared.Network.Packets.Shared;

namespace CyclePresetPlugin.Restart;

public class ReconnectClientPacket : IOutgoingNetworkPacket
{
    public ushort Time = 0;
    public void ToWriter(ref PacketWriter writer)
    {
        writer.Write((byte)ACServerProtocol.Extended);
        writer.Write((byte)CSPMessageTypeTcp.ClientMessage);
        writer.Write<byte>(255);
        writer.Write((ushort)CSPClientMessageType.LuaMessage);
        writer.Write(0xC7A5384A);
        writer.Write(Time);
    }
}
