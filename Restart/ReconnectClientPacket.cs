using System.Text;
using AssettoServer.Shared.Network.Packets;
using AssettoServer.Shared.Network.Packets.Outgoing;
using AssettoServer.Shared.Network.Packets.Shared;

namespace CyclePresetPlugin.Restart;

public class ReconnectClientPacket : IOutgoingNetworkPacket
{
    public ushort Time;
    private readonly byte[] _reconnectBytes = Encoding.ASCII.GetBytes("ReconnectClients");
    public void ToWriter(ref PacketWriter writer)
    {
        writer.Write((byte)ACServerProtocol.Extended);
        writer.Write((byte)CSPMessageTypeTcp.ClientMessage);
        writer.Write<byte>(255);
        writer.Write((ushort)CSPClientMessageType.LuaMessage);
        writer.Write(0xC9F693DA);
        writer.Write(Time);
        foreach (byte reconnect in _reconnectBytes)
        {
            writer.Write(reconnect);
        }
        // writer.WriteUTF8String("ReconnectClients");
    }
}
