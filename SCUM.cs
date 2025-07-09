using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace WindowsGSM.Plugins
{
    public class SCUM : SteamCMDAgent // CLASS NAME MUST BE SCUM
    {
        [DllImport("kernel32.dll")]
        public static extern bool WritePrivateProfileString(string strSection, string strKeyName, string strValue, string strFilePath);

        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SCUM",
            author = "SLBlackHatMan",
            description = "WindowsGSM plugin for SCUM Dedicated Server",
            version = "1.4", // Incremented version
            url = "https://github.com/SLBlackHatMan/WindowsGSM.SCUM",
            color = "#86d1ff" // A SCUM-themed color
        };

        // - Standard Constructor
        public SCUM(ServerConfig serverData) : base(serverData) { }
        private readonly ServerConfig _serverData;

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "3792580"; // CORRECT AppId for SCUM Dedicated Server

        // - Game server Fixed variables
        public override string StartPath => @"SCUM\Binaries\Win64\SCUMServer.exe"; // This path was correct
        public string FullName = "SCUM Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 2; // SCUM uses multiple ports, typically GamePort and QueryPort are sequential.
        public object QueryMethod = new A2S(); // MUST NOT BE NULL. Use A2S for querying.

        // - Game server default values
        public string Port = "7777"; // Common default SCUM port
        public string QueryPort = "7779"; // Common default SCUM query port
        public string Defaultmap = "SCUM"; // Map is always SCUM island
        public string Maxplayers = "120";
        public string Additional = "";

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            string configPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, @"SCUM\Saved\Config\WindowsServer\ServerSettings.ini");
            // Your URL points to your repo, which is fine if that's what you intend.
            string SCUMCFGURL = "https://raw.githubusercontent.com/SLBlackHatMan/WindowsGSM.SCUM/refs/heads/main/Serverconfig.ini";

            try
            {
                // Create directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));

                // Download a base config file
                using (WebClient ScumCFG = new WebClient())
                {
                    await ScumCFG.DownloadFileTaskAsync(SCUMCFGURL, configPath);
                }

                // Overwrite the MaxPlayers setting in the INI file using the value from WindowsGSM
                // The Section for SCUM is [SCUM]
                WritePrivateProfileString("SCUM", "MaxPlayers", _serverData.ServerMaxPlayer, configPath);
            }
            catch (Exception e)
            {
                Error = "CFG Error: " + e.Message;
            }
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
            // Remove -NOSTEAM. It prevents the server from registering with Steam.
            string param = "-log";
            param += $" -server"; // Identifies it as a server process
            param += $" -ip={_serverData.ServerIP}";
            param += $" -port={_serverData.ServerPort}";
            param += $" -QueryPort={_serverData.ServerQueryPort}";
            param += $" -MaxPlayers={_serverData.ServerMaxPlayer}";
            param += $" {_serverData.ServerParam}"; // For other params like -multihome

            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = shipExePath,
                    Arguments = param,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            var serverConsole = new ServerConsole(_serverData.ServerID);
            p.OutputDataReceived += serverConsole.AddOutput;
            p.ErrorDataReceived += serverConsole.AddOutput;

            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                return p;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null;
            }
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c"); // Send Ctrl+C to gracefully shut down
            });
            await Task.Delay(20000); // Wait for server to shut down
        }
    }
}
