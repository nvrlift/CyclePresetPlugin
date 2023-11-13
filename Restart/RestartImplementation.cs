using System.Text;
using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using Serilog;

namespace CyclePresetPlugin.Restart;

public class RestartImplementation
{
    private readonly ACServerConfiguration _acServerConfiguration;
    private readonly EntryCarManager _entryCarManager;
    private readonly CyclePresetConfiguration _cyclePresetConfiguration;

    public RestartImplementation(ACServerConfiguration acServerConfiguration, CyclePresetConfiguration cyclePresetConfiguration, EntryCarManager entryCarManager)
    {
        _acServerConfiguration = acServerConfiguration;
        _cyclePresetConfiguration = cyclePresetConfiguration;
        _entryCarManager = entryCarManager;
    }

    public void InitiateRestart(string presetPath)
    {
        // Reconnect clients
        Log.Information("Reconnecting all clients for preset change.");
        if (_acServerConfiguration.Extra.EnableClientMessages && _cyclePresetConfiguration.ReconnectEnabled)
        {
            foreach (var client in _entryCarManager.EntryCars.Select(c => c.Client))
            {
                if (client == null) continue;
            
                Log.Information($"Reconnecting {client.Name} for Server reset.");
                client.SendPacket(new ReconnectClientPacket
                {
                    Time = 10,
                });
            }
        
            Thread.Sleep(2000);
        }
        else
        {
            foreach (var client in _entryCarManager.EntryCars.Select(c => c.Client))
            {
                if (client == null) continue;
            
                _entryCarManager.KickAsync(client, "SERVER RESTART FOR TRACK CHANGE (won't take long)").GetAwaiter().GetResult();
                Log.Information($"Kicking {client.Name} for track change server restart.");
            }
        }
        
        var preset = new DirectoryInfo(presetPath).Name;

        // Restart the server
        var restartPath = Path.Join(_acServerConfiguration.BaseFolder, "restart", $"{Environment.ProcessId}.asrestart");
        Log.Information($"Trying to create restart file: {restartPath}");
        var restartFile = File.Create(restartPath);
        byte[] content = new UTF8Encoding(true).GetBytes(preset);
        restartFile.Write(content, 0, content.Length);
        restartFile.Close();
    }
}
