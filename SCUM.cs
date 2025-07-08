using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class SCUM
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCUM",
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for SCUM Dedicated Server",
            version = "1.0",
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#f38b00"
        };

        // - Server configuration
        public SCUM(ServerConfig serverData) => _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        // - Game server setup
        public string StartPath => "SCUMServer.exe";
        public string FullName = "SCUM Dedicated Server";
        public bool AllowsEmbedConsole = false;
        public int PortIncrements = 2;
        public object QueryMethod = new UT3();

        public string Port = "7777";
        public string QueryPort = "27015";
        public string Defaultmap = "";
        public string Maxplayers = "64";
        public string Additional = "-log"; // Default additional args

        // - Start server
        public async Task<Process> Start()
        {
            string exePath = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);
            if (!File.Exists(exePath))
            {
                Error = $"{StartPath} not found!";
                return null;
            }

            string param = $"-MULTIHOME=0.0.0.0 -PORT={_serverData.ServerPort} -QUERYPORT={_serverData.ServerQueryPort} {Additional}";

            Process p = new Process();
            p.StartInfo.WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID);
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = param;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;

            try
            {
                p.Start();
                return await Task.FromResult(p);
            }
            catch (Exception ex)
            {
                Error = $"Failed to start server: {ex.Message}";
                return null;
            }
        }

        // - Stop server
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                try
                {
                    foreach (var process in Process.GetProcessesByName("SCUMServer"))
                    {
                        if (process.MainModule?.FileName?.StartsWith(ServerPath.GetServersServerFiles(_serverData.ServerID)) == true)
                        {
                            process.Kill();
                        }
                    }
                }
                catch (Exception e)
                {
                    Error = $"Failed to stop server: {e.Message}";
                }
            });
        }

        // - Install SCUM server via SteamCMD
        public async Task<Process> Install()
        {
            string appId = "911460";
            return await SteamCMD.Install(_serverData.ServerID, appId, "anonymous", string.Empty);
        }

        // - Update (same as install for SteamCMD)
        public async Task<Process> Update()
        {
            return await Install();
        }

        // - Is server installed?
        public bool IsInstallValid()
        {
            return File.Exists(ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath));
        }

        // - Is import valid?
        public bool IsImportValid(string path)
        {
            string exePath = Path.Combine(path, StartPath);
            Error = $"Invalid Path! Could not find {StartPath}";
            return File.Exists(exePath);
        }

        // - Local version check
        public string GetLocalBuild()
        {
            return SteamCMD.GetInstalledBuild(_serverData.ServerID, "911460");
        }

        // - Remote version check
        public async Task<string> GetRemoteBuild()
        {
            return SteamCMD.GetRemoteBuild("911460");
        }
    }
}
