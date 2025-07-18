using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;
using WindowsGSM.GameServer.Engine;
using System.IO;
using Newtonsoft.Json;

namespace WindowsGSM.Plugins
{
    public class SCUM : SteamCMDAgent // CLASS NAME MUST BE SCUM
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCUM",
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for supporting SCUM Dedicated Server",
            version = "1.4.2", // Incremented version
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#86d1ff" // A SCUM-themed color
        };

       

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "3792580"; // CORRECT AppId for SCUM Dedicated Server
        
         // - Standard Constructor
        public SCUM(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;
        
        // - Game server Fixed variables
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe"; // This path was correct
        public string FullName = "SCUM dedicated server";
        public bool AllowsEmbedConsole = true; 
        public int PortIncrements = 2; // SCUM uses multiple ports, typically GamePort and QueryPort are sequential.

        // TODO: Undisclosed method
        public object QueryMethod = new A2S(); // MUST NOT BE NULL. Use A2S for querying.

        // - Game server default values
        public string Port = "10000";// Common default SCUM port
        public string QueryPort = "10000"; // Common default SCUM query port
        public string Defaultmap = "Dedicated"; // Map is always SCUM island
        public string Maxplayers = "32";
        public string Additional = "-log"; // Additional server start parameter

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
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

            // Prepare start parameter
            string param = $" {_serverData.ServerParam} ";
            param += $"-port={_serverData.ServerPort} ";
            param += $"-MaxPlayers={_serverData.ServerMaxPlayer} ";

            Process p;
            if (!AllowsEmbedConsole)
            {
                p = new Process
                {
                    StartInfo =
                    {
                        FileName = shipExePath,
                        Arguments = param.ToString(),
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = false
                    },
                    EnableRaisingEvents = true
                };
                p.Start();
            }
            else
            {
                p = new Process
                {
                    StartInfo =
                    {
                        FileName = shipExePath,
                        Arguments = param.ToString(),
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };
                var serverConsole = new Functions.ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }

            return p;
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(async () =>
            {
                if (p.StartInfo.CreateNoWindow)
                {
                    p.CloseMainWindow();
                }
                else
                {
                    Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                    Functions.ServerConsole.SendWaitToMainWindow("^c");
                    await Task.Delay(2000);
                }
            });
        }

        public async Task<Process> Install()
        {
            var steamCMD = new Installer.SteamCMD();
            Process p = await steamCMD.Install(_serverData.ServerID, string.Empty, AppId, true, loginAnonymous);
            Error = steamCMD.Error;

            return p;
        }

        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var (p, error) = await Installer.SteamCMD.UpdateEx(_serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);
            Error = error;
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
