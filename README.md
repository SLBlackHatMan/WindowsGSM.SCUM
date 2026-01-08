SCUM Dedicated Server Plugin for WindowsGSM

This plugin adds support for running a SCUM Dedicated Server using WindowsGSM on Windows systems.

Tested with:

WindowsGSM v1.23.1

SCUM Dedicated Server (Steam AppID: 3792580)

Windows Server 2016 / 2019 / 2022

Windows 10 / 11

ZAP-Hosting (Game Server & Rootserver)

VPS / Dedicated Servers

✨ Features

Install & Update SCUM Dedicated Server via SteamCMD

Start / Stop / Restart server from WindowsGSM

Live Console Output inside WindowsGSM

Player & Status Monitoring

WindowsGSM Backup Support

Hosting-provider friendly (no forced MultiHome or QueryPort)

⚠️ This plugin is intentionally minimal to ensure maximum compatibility with managed hosting providers.

🧩 Prerequisites

Latest version of WindowsGSM
👉 https://windowsgsm.com/

64-bit Windows operating system

System meets SCUM Dedicated Server minimum requirements

⚠️ REQUIRED RUNTIMES (VERY IMPORTANT)

SCUM uses Unreal Engine, which requires several Microsoft runtimes.
On fresh Windows installs (especially Windows Server 2016) the server will NOT start without these.

✅ You MUST install ALL of the following
1️⃣ .NET Framework 4.8

Required by WindowsGSM

👉 https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48

2️⃣ Visual C++ Redistributable 2015–2022 (x64)

Required by SCUMServer.exe

👉 https://aka.ms/vs/17/release/vc_redist.x64.exe

3️⃣ Visual C++ Redistributable 2013 (x64)

Required by Unreal Engine components

👉 https://www.microsoft.com/en-us/download/details.aspx?id=40784

4️⃣ DirectX End-User Runtime (June 2010)

Required even on headless servers

👉 https://www.microsoft.com/en-us/download/details.aspx?id=8109

🔁 IMPORTANT

After installing all dependencies:

Reboot the server

Run SCUMServer.exe once manually before starting via WindowsGSM

Path:

SCUM\Binaries\Win64\SCUMServer.exe


This initializes Unreal Engine files and prevents silent crashes.

📦 Installation

Download the latest release from:
👉 https://github.com/SLBlackHatMan/WindowsGSM.SCUM/releases/latest

Import Plugin in WindowsGSM:

WindowsGSM → Plugins → Import Plugin


Restart WindowsGSM

Create a new SCUM server

Install & start the server

⚙️ Configuration
Ports

Game Port: Set in WindowsGSM

Query Port: Managed automatically

Beacon Port: Game Port + 1

⚠️ ZAP-Hosting Users

Use ONLY the ports assigned in the ZAP panel

Do NOT invent custom ports

Do NOT force -MultiHome

NAT & port routing are handled by ZAP

🚫 Common Error: Exit Code -1073741515
Exit Code: -1073741515


This means:

❌ Required runtime DLLs are missing

✅ Fix

Install all runtimes listed above, then reboot.

This is not a plugin bug.

🌐 Server Not Showing / Direct Connect Fails

Checklist:

✔ Server is running

✔ Correct ports are used

✔ Required runtimes installed

✔ UDP allowed in firewall / provider panel

✔ Host-assigned ports used (ZAP)

⏳ Server list registration may take a few minutes after first start.

🛑 What This Plugin Intentionally Does NOT Do

Does not force -MultiHome

Does not force -QueryPort

Does not overwrite server config files

Does not hard-kill the server process

This is intentional for maximum compatibility with managed hosting providers.

🧪 Compatibility
Environment	Supported
WindowsGSM v1.23.1	✅
Windows Server 2016	✅ (runtimes required)
Windows Server 2019+	✅
Windows 10 / 11	✅
ZAP-Hosting Game Server	✅
ZAP Rootserver / VPS	✅
Dedicated Server	✅
🐛 Issues & Support

When opening an issue, please include:

Windows version

WindowsGSM version

Hosting provider (ZAP, VPS, Dedicated)

Crash log (if any)

Confirmation that required runtimes are installed

📜 License

MIT License
Free to use, modify, and distribute.
