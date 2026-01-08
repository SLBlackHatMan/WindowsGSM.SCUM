using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class SCUM : SteamCMDAgent
    {
        /* =========================================================
         * PLUGIN METADATA
         * ========================================================= */
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCUM",
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for supporting SCUM Dedicated Server",
            version = "1.7.0",
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#34c9eb"
        };

        /* =========================================================
         * STEAMCMD SETTINGS
         * ========================================================= */
        public override bool loginAnonymous => true;
        public override string AppId => "3792580";

        /* =========================================================
         * CONSTRUCTOR
         * ========================================================= */
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        public SCUM(ServerConfig serverData) : base(serverData)
            => base.serverData = _serverData = serverData;

        /* =========================================================
         * SERVER INFO
         * ========================================================= */
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe";
        public string FullName = "SCUM Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 2;

        /* =========================================================
         * QUERY
         * ========================================================= */
        public object QueryMethod = new A2S();

        /* =========================================================
         * REQUIRED PUBLIC FIELDS (WindowsGSM reflection)
         * ========================================================= */
        public string Port = "27042";
        public string QueryPort = "27043";
        public string Maxplayers = "32";
        public string Defaultmap = "Dedicated";
        public string Additional = "-log";

        /* =========================================================
         * CONFIG (INTENTIONALLY EMPTY)
         * ========================================================= */
        public async void CreateServerCFG() { }

        /* =========================================================
         * START SERVER
         * ========================================================= */
        public async Task<Process> Start()
        {
            string exePath = Functions.ServerPath.GetServersServerFiles(
                _serverData.ServerID, StartPath);

            if (!File.Exists(exePath))
            {
                Error = $"SCUMServer.exe not found ({exePath})";
                return null;
            }

            // Minimal & safe parameters (ZAP compatible)
            string args = $" {_serverData.ServerParam} ";
            args += $"-port={_serverData.ServerPort} ";
            args += $"-MaxPlayers={_serverData.ServerMaxPlayer} ";
            args += $"{Additional}";

            Process p = new Process
            {
                StartInfo =
                {
                    FileName = exePath,
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
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

        /* =========================================================
         * STOP SERVER (GRACEFUL)
         * ========================================================= */
        public async Task Stop(Process p)
        {
            await Task.Run(async () =>
            {
                if (p == null || p.HasExited)
                    return;

                try
                {
                    p.CloseMainWindow();
                    await Task.Delay(2000);
                }
                catch { }
            });
        }

        /* =========================================================
         * INSTALL / UPDATE
         * ========================================================= */
        public async Task<Process> Install()
        {
            var steamCMD = new Installer.SteamCMD();
            Process p = await steamCMD.Install(
                _serverData.ServerID, string.Empty, AppId, true, loginAnonymous);

            Error = steamCMD.Error;
            return p;
        }

        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var (p, error) = await Installer.SteamCMD.UpdateEx(
                _serverData.ServerID,
                AppId,
                validate,
                custom: custom,
                loginAnonymous: loginAnonymous);

            Error = error;
            return p;
        }

        /* =========================================================
         * VALIDATION / BUILD INFO
         * ========================================================= */
        public bool IsInstallValid()
            => File.Exists(Functions.ServerPath.GetServersServerFiles(
                _serverData.ServerID, StartPath));

        public bool IsImportValid(string path)
            => File.Exists(Path.Combine(path, StartPath));

        public string GetLocalBuild()
            => new Installer.SteamCMD().GetLocalBuild(
                _serverData.ServerID, AppId);

        public async Task<string> GetRemoteBuild()
            => await new Installer.SteamCMD().GetRemoteBuild(AppId);
    }
}
