# SCUM Dedicated Server Plugin for WindowsGSM
![GitHub release (latest by date)](https://img.shields.io/github/v/release/your-username/your-repo-name?style=for-the-badge)

This plugin adds support for managing a **SCUM Dedicated Server** within the WindowsGSM application.

---

## Features

* **Install/Update:** Download and update the SCUM server using the correct, official AppId.
* **Monitor:** View server status, player count, and other details directly from the WGS M UI.
* **Start/Stop/Restart:** Full control over the server process.
* **Auto-Configuration:** Automatically generates a default `ServerSettings.ini` on first install.
* **Embeddable Console:** View the live server console output within WindowsGSM.
* **Backup/Restore:** Utilize WindowsGSM's built-in backup functionality.

---

## Prerequisites

* You must have the latest version of [WindowsGSM](https://windowsgsm.com/) installed on your machine.
* Your machine must meet the minimum requirements to run a SCUM server.
* You are running a 64-bit version of Windows.

---

## Installation

1.  **Download:** Click [here](https://github.com/your-username/your-repo-name/releases/latest) to download the latest release (`SCUM.cs`).
2.  **Move:** Place the downloaded `SCUM.cs` file into the `plugins` folder of your WindowsGSM installation directory.
    > Example Path: `C:\WindowsGSM\plugins`
3.  **Restart:** Close and restart the WindowsGSM application. The plugin will be loaded automatically.

---

## Configuration

### Initial Setup

The plugin will create a default `ServerSettings.ini` file upon first installation. The settings used (Server Name, Port, etc.) are taken from the values you input in the WindowsGSM installation panel at that time.

### How to Change Server Settings (e.g., Server Name)

After installation, the settings in the WindowsGSM UI **do not** automatically update the game server's config file. You must edit the file directly.

1.  In WindowsGSM, select your SCUM server.
2.  Click the **EDIT CONFIG** button. This will open the `ServerSettings.ini` file in a text editor.
3.  Find the setting you want to change (e.g., `ServerName=YourOldName`) and modify it.
4.  Save the file and close the editor.
5.  **Restart** your server from the WindowsGSM panel for the changes to take effect.

---

## Troubleshooting

This section covers the most common issues users face.

### 1. Server Not Showing in the In-Game List

This is the most common problem and is almost always caused by one of the following issues:

* **Correct AppId:** `3792580` (This is an old, invalid ID that will download a broken server version).

> **Solution:** Ensure your `SCUM.cs` plugin file uses `AppId = "3792580"`. If you have installed the server using the wrong ID, you must **delete the server** in WindowsGSM and perform a clean installation with the corrected plugin.

#### B. Firewall or Ports are Blocked
For your server to be visible to the public, you must open the required ports. This must be done in **two places** if you are using a VPS:
1.  Your **VPS Provider's Control Panel** (e.g., AWS, Vultr, Google Cloud).
2.  The **Windows Defender Firewall** inside the server's operating system.

You must open the following ports with the **UDP** protocol:

* **Game Port:** The port you set in WGS M (default `7042`).
* **Query Port:** The query port you set in WGS M (default `27015`).
* **Beacon Port:** This is your Game Port + 1 (e.g., `7043`).

### 2. Server Log Shows It's Using the Wrong Port (e.g., 7777)

If you check your server's logs and see a line like `IpNetDriver listening on port 7777`, even though you configured a different port, this is a symptom of the `AppId` issue described above.

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
