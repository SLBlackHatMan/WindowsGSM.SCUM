using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class Scum : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.Scum",
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for supporting Scum Dedicated Server",
            version = "1.1.0",
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#1E8449"
        };

        // - Standard Constructor
        public Scum(ServerConfig serverData) : base(serverData) { }

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "3792580"; // SCUM Dedicated Server AppID

        // - Game server Fixed variables
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe";
        public string FullName = "Scum Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 2;
        public object QueryMethod = new A2S();

        // - Game server default values
        public string ServerName = "WindowsGSM Scum Server";
        public string Defaultmap = "SCUM";
        public string Maxplayers = "64";
        public string Port = "7777";
        public string QueryPort = "7778";
        public string Additional = "-log";

        // - Start server function (Corrected to fix async warning and _serverData errors)
        public Task<Process> Start()
        {
            string shipExePath = Functions.ServerPath.GetServersServerFiles(serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return Task.FromResult<Process>(null);
            }

            string param = $"{serverData.ServerMap}";
            param += $" -MultiHome={serverData.ServerIP}";
            param += $" -Port={serverData.ServerPort}";
            param += $" -QueryPort={serverData.ServerQueryPort}";
            param += $" -MaxPlayers={serverData.ServerMaxPlayer}";
            param += $" {serverData.ServerParam}";

            Process p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Path.GetDirectoryName(shipExePath),
                    FileName = shipExePath,
                    Arguments = param,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            var serverConsole = new ServerConsole(serverData.ServerID);
            p.OutputDataReceived += serverConsole.AddOutput;
            p.ErrorDataReceived += serverConsole.AddOutput;

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                return Task.FromResult(p);
            }
            catch (Exception e)
            {
                Error = e.Message;
                return Task.FromResult<Process>(null);
            }
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() => {
                if (p != null && !p.HasExited)
                {
                    p.Kill();
                }
            });
        }
    }
}