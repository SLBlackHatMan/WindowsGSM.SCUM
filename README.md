SCUM Dedicated Server Plugin for WindowsGSM

This plugin adds support for running a SCUM Dedicated Server using WindowsGSM on Windows systems.

It is designed to be stable, minimal, and compatible across different Windows environments.

✅ Tested Environment

WindowsGSM v1.23.1

SCUM Dedicated Server (Steam AppID: 3792580)

Windows Server 2016 / 2019 / 2022

Windows 10 / 11 (64-bit)

✨ Features

Install & update SCUM Dedicated Server via SteamCMD

Start / Stop / Restart server from WindowsGSM

Live server console output inside WindowsGSM

Player and server status monitoring

Uses minimal launch parameters for maximum compatibility

Safe for clean Windows installations

🧩 Prerequisites

Latest version of WindowsGSM
👉 https://windowsgsm.com/

64-bit Windows operating system

System meets SCUM Dedicated Server minimum requirements

⚠️ REQUIRED RUNTIMES (VERY IMPORTANT)

SCUM is built on Unreal Engine, which requires several Microsoft runtimes.
On fresh Windows installs (especially Windows Server) the server will not start without these.

🔹 You MUST install ALL of the following
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

🔁 IMPORTANT STEP (DO NOT SKIP)

After installing all runtimes:

Reboot the system

Run SCUMServer.exe once manually

Path:

SCUM\Binaries\Win64\SCUMServer.exe


Let it run for 10–20 seconds, then close it.

This initializes Unreal Engine files and prevents silent crashes when starting via WindowsGSM.

📦 Installation

Download the latest release from:

https://github.com/SLBlackHatMan/WindowsGSM.SCUM/releases/latest


Open WindowsGSM

Go to Plugins → Import Plugin

Import the downloaded plugin

Restart WindowsGSM

Create a new SCUM server

Install & start the server

⚙️ Configuration
Ports

Game Port: Set in WindowsGSM

Query Port: Handled automatically by the server

Beacon Port: Game Port + 1

Make sure the selected ports are allowed through your firewall.

❌ Common Error: Exit Code -1073741515
Exit Code: -1073741515

Meaning

A required runtime DLL is missing.

Fix

Install all required runtimes listed above, reboot, and start the server again.

This is not a plugin bug.

🌐 Server Not Showing / Direct Connect Issues

Before opening an issue, verify:

✔ Server starts and stays running

✔ Required runtimes are installed

✔ Correct ports are used

✔ Firewall allows UDP traffic

✔ Server has been running for a few minutes (first registration may take time)

🛑 What This Plugin Intentionally Does NOT Do

Does not force -MultiHome

Does not force -QueryPort

Does not overwrite server configuration files

Does not hard-kill the server process

These choices are intentional to ensure maximum compatibility and stability.

🐛 Issues & Support

When opening an issue, please include:

Windows version

WindowsGSM version

SCUM server start log

Crash log (if any)

Confirmation that all required runtimes are installed

Incomplete reports may delay support.

📜 License

MIT License
Free to use, modify, and distribute.
