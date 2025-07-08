using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using System.Collections.Generic;

namespace WindowsGSM.Plugins
{
    public class Scum : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.Scum", // WindowsGSM.XXXX
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for supporting Scum Dedicated Server",
            version = "1.0.0",
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM", // Github repository link (Best practice)
            color = "#1E8449" // Color Hex
        };

        // - Standard Constructor and properties
        public Scum(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "1824980"; /* Correct AppID for SCUM Dedicated Server */

        // - Game server Fixed variables
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe"; // Game server start path
        public string FullName = "Scum Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect?
        public int PortIncrements = 2; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = new A2S(); // Query method should be use on current server type.

        // - Game server default values
        public string ServerName = "WindowsGSM Scum Server";
        public string Defaultmap = "SCUM"; // Map is always SCUM
        public string Maxplayers = "64"; // WGSM reads this as string but originally it is number or int (MaxPlayers)
        public string Port = "7777"; // WGSM reads this as string but originally it is number or int
        public string QueryPort = "7778"; // WGSM reads this as string but originally it is number or int (SteamQueryPort)
        public string Additional = "-log -SETTINGS"; // Additional startup parameters

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            // SCUM uses ServerSettings.ini, which is generated on first run.
            // This can be left empty as the server will create it.
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            string shipExePath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            // Correctly formatted start parameters for SCUM
            string param = $"{_serverData.ServerMap}";
            param += $" -MultiHome={_serverData.ServerIP}";
            param += $" -Port={_serverData.ServerPort}";
            param += $" -QueryPort={_serverData.ServerQueryPort}";
            param += $" -MaxPlayers={_serverData.ServerMaxPlayer}";
            param += $" {_serverData.ServerParam}";

            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Path.GetDirectoryName(shipExePath),
                    FileName = shipExePath,
                    Arguments = param,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Set up Redirect Input and Output to WindowsGSM Console if EmbedConsole is on
            if (AllowsEmbedConsole)
            {
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                var serverConsole = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
            }

            // Start Process
            try
            {
                p.Start();
                if (AllowsEmbedConsole)
                {
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                }

                return p;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null; // return null if fail to start
            }
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                if (p.Responding)
                {
                    p.Kill();
                }
            });
        }

        // - Update server function
        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var (p, error) = await Installer.SteamCMD.UpdateEx(serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);
            Error = error;
            if (p != null)
            {
                await Task.Run(p.WaitForExit);
            }
            return p;
        }

        public bool IsInstallValid()
        {
            return File.Exists(Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath));
        }

        public bool IsImportValid(string path)
        {
            string importPath = Path.Combine(path, StartPath);
            Error = $"Invalid Path! Fail to find {Path.GetFileName(StartPath)}";
            return File.Exists(importPath);
        }

        public string GetLocalBuild()
        {
            var steamCMD = new Installer.SteamCMD();
            return steamCMD.GetLocalBuild(_serverData.ServerID, AppId);
        }

        public async Task<string> GetRemoteBuild()
        {
            var steamCMD = new Installer.SteamCMD();
            return await steamCMD.GetRemoteBuild(AppId);
        }
    }
}