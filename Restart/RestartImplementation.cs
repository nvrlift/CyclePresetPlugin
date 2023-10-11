using System.Text;
using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using Serilog;

namespace CyclePresetPlugin.Restart;

public class RestartImplementation
{
    private readonly ACServerConfiguration _acServerConfiguration;
    private readonly EntryCarManager _entryCarManager;

    public RestartImplementation(ACServerConfiguration acServerConfiguration, EntryCarManager entryCarManager)
    {
        _acServerConfiguration = acServerConfiguration;
        _entryCarManager = entryCarManager;
    }

    public void InitiateRestart(string presetPath, RestartType type)
    {
        // Reconnect clients
        Log.Information("Reconnecting all clients for preset change.");
        if (_acServerConfiguration.Extra.EnableClientMessages)
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
            
                _entryCarManager.KickAsync(client, "SERVER RESTART").GetAwaiter().GetResult();
                Log.Information($"Kicking {client.Name} for Server reset.");
            }
        }
        
        var preset = new DirectoryInfo(presetPath).Name;

        switch (type)
        {
            case RestartType.WindowsFile:
            {
                var restartPath = Path.Join(_acServerConfiguration.BaseFolder, "restart", $"{Environment.ProcessId}.asrestart");
                Log.Information($"Trying to create restart file: {restartPath}");
                var restartFile = File.Create(restartPath);
                byte[] content = new UTF8Encoding(true).GetBytes(preset);
                restartFile.Write(content, 0, content.Length);
                restartFile.Close();
                break;
            }
            case RestartType.Docker:
            {
                throw new NotImplementedException();
            }
            default:
            {
                throw new NotImplementedException();
            }
        }
            
    }
}
