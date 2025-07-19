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
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCUM",
            author = "SLBlackHatMan)",
            description = "WindowsGSM plugin for supporting SCUM Dedicated Server",
            version = "1.5.5",
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#34c9eb"
        };

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "3792580";

        // - Standard Constructor and properties
        public SCUM(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        // - Game server Fixed variables
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe";
        public string FullName = "SCUM Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 10;
        public object QueryMethod = new A2S();

        // - Game server default values
        public string Port = "27042";
        public string QueryPort = "27015";
        public string Defaultmap = "SCUM";
        public string Maxplayers = "32";
        public string Additional = "-nosteamclient -log";

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            string configPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, @"SCUM\Saved\Config\WindowsServer\ServerSettings.ini");
            string configFolder = Path.GetDirectoryName(configPath);

            if (configFolder != null && !Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            var settings = new string[]
            {
                "[/script/scum.serversettings]",
                $"MaxPlayers={_serverData.ServerMaxPlayer}",
                $"Port={_serverData.ServerPort}",
                $"QueryPort={_serverData.ServerQueryPort}",
                $"BeaconPort={int.Parse(_serverData.ServerPort) + 1}",
                $"ServerName={_serverData.ServerName}",
                "ServerPassword=",
                "TimeOfDaySpeed=3.84",
                "NightTimeDarkness=0.0",
                "RegisterToMasterServer=True"
            };

            File.WriteAllLines(configPath, settings);
        }

        // - Start server function
        public async Task<Process> Start()
        {
            string shipExePath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            string param = $"-MultiHome={_serverData.ServerIP} {_serverData.ServerParam}";

            Process p = new Process
            {
                StartInfo =
                {
                    FileName = shipExePath,
                    Arguments = param,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            var serverConsole = new ServerConsole(_serverData.ServerID);
            p.OutputDataReceived += serverConsole.AddOutput;
            p.ErrorDataReceived += serverConsole.AddOutput;

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            return p;
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                if (p != null && !p.HasExited)
                {
                    p.Kill();
                }
            });
        }

        // - Install server function
        public async Task<Process> Install()
        {
            var steamCMD = new Installer.SteamCMD();
            Process p = await steamCMD.Install(_serverData.ServerID, string.Empty, AppId, true, loginAnonymous);
            Error = steamCMD.Error;
            return p;
        }

        // - Update server function
        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var (p, error) = await Installer.SteamCMD.UpdateEx(_serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);
            Error = error;
            return p;
        }

        // - Validation functions
        public bool IsInstallValid() => File.Exists(Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath));
        public bool IsImportValid(string path) => File.Exists(Path.Combine(path, StartPath));
        public string GetLocalBuild() => new Installer.SteamCMD().GetLocalBuild(_serverData.ServerID, AppId);
        public async Task<string> GetRemoteBuild() => await new Installer.SteamCMD().GetRemoteBuild(AppId);
    }
}
