# SCUM Dedicated Server Plugin for WindowsGSM

![GitHub release (latest by date)](https://img.shields.io/github/v/release/your-username/your-repo-name?style=for-the-badge)


This plugin adds support for managing a **SCUM Dedicated Server** within the WindowsGSM application.

---

## Features

* **Install/Update:** Download and update the SCUM server using SteamCMD.
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

1.  **Download:** Click [here](https://github.com/SLBlackHatMan/WindowsGSM.SCUM) to download the latest release (`SCUM.cs`).
2.  **Move:** Place the downloaded `SCUM.cs` file into the `plugins` folder of your WindowsGSM installation directory.
    > Example Path: `C:\WindowsGSM\plugins`
3.  **Restart:** Close and restart the WindowsGSM application. The plugin will be loaded automatically.

---

## Usage

1.  Open WindowsGSM.
2.  Navigate to the **Servers** menu and click **Install New Server**.
3.  Scroll down to find **SCUM Dedicated Server** in the list.
4.  Click **Install**. WindowsGSM will handle the download and setup.
5.  Once installed, you can start your server and manage it from the main UI.

---

## Configuration

This plugin creates a standard `ServerSettings.ini` file.

* **Config File Location:** `\servers\{YourServerID}\serverfiles\SCUM\Saved\Config\WindowsServer\ServerSettings.ini`
* You can edit this file to change your server name, password, max players, and other game-specific settings.

---

## Common Issues

### Server not showing in the in-game list?

This is almost always a firewall or port forwarding issue. For your server to be visible to the public, you must open the following **UDP** ports on **both** your VPS provider's firewall (e.g., AWS, Vultr) and the Windows Firewall on the server itself.

* **Game Port:** The port you set in WGS M (default `7042`) - Protocol **UDP**
* **Query Port:** The query port you set in WGS M (default `27015`) - Protocol **UDP**
* **Beacon Port:** This is your Game Port + 1 (e.g., `7043`) - Protocol **UDP**

---

## Contributing

Contributions are welcome! If you find a bug or have a suggestion for improvement, please open an issue or submit a pull request.

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
