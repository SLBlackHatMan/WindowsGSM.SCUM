using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;
using WindowsGSM.GameServer.Engine;

namespace WindowsGSM.Plugins
{
    public class SCUM : SteamCMDAgent
    {
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCUM",
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for supporting SCUM Dedicated Server",
            version = "1.5.8",
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#34c9eb"
        };

        // SteamCMD
        public override bool loginAnonymous => true;
        public override string AppId => "3792580";

        private readonly ServerConfig _serverData;

        // Game server info
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe";
        public string FullName = "SCUM Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 10;
        public object QueryMethod = new A2S();

        // Constructor
        public SCUM(ServerConfig serverData) : base(serverData)
        {
            _serverData = serverData;
        }

        // Create server config (SYNC IO – REQUIRED)
        public Task CreateServerCFG()
        {
            try
            {
                string configPath = Functions.ServerPath.GetServersServerFiles(
                    _serverData.ServerID,
                    @"SCUM\Saved\Config\WindowsServer\ServerSettings.ini"
                );

                Directory.CreateDirectory(Path.GetDirectoryName(configPath));

                int basePort;
                if (!int.TryParse(_serverData.ServerPort, out basePort))
                    basePort = 27042;

                var settings = new[]
                {
                    "[/script/scum.serversettings]",
                    $"ServerName=\"{_serverData.ServerName}\"",
                    $"MaxPlayers={_serverData.ServerMaxPlayer}",
                    $"Port={basePort}",
                    $"QueryPort={_serverData.ServerQueryPort}",
                    $"BeaconPort={basePort + 1}",
                    "ServerPassword=",
                    "TimeOfDaySpeed=3.84",
                    "NightTimeDarkness=0.0",
                    "RegisterToMasterServer=True"
                };

                File.WriteAllLines(configPath, settings);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return Task.CompletedTask;
        }

        // Start server
        public async Task<Process> Start()
        {
            await CreateServerCFG();

            string exePath = Functions.ServerPath.GetServersServerFiles(
                _serverData.ServerID,
                StartPath
            );

            if (!File.Exists(exePath))
            {
                Error = "SCUMServer.exe not found";
                return null;
            }

            string param =
                "-MultiHome=" + _serverData.ServerIP + " " +
                "-port=" + _serverData.ServerPort + " " +
                "-QueryPort=" + _serverData.ServerQueryPort + " " +
                _serverData.ServerParam;

            var p = new Process
            {
                StartInfo =
                {
                    FileName = exePath,
                    Arguments = param,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            var console = new ServerConsole(_serverData.ServerID);
            p.OutputDataReceived += console.AddOutput;
            p.ErrorDataReceived += console.AddOutput;

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            return p;
        }

        // Stop server (Framework-safe)
        public Task Stop(Process p)
        {
            if (p != null && !p.HasExited)
            {
                try
                {
                    p.CloseMainWindow();
                }
                catch
                {
                    try { p.Kill(); } catch { }
                }
            }

            return Task.CompletedTask;
        }
    }
}
