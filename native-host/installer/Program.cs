using System.IO.Compression;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DashboardZenBridgeSetup;

internal static class Program
{
    private const string HostName = "com.dashboard_zen.bridge";
    private const string ManifestFileName = HostName + ".json";
    private const string UninstallKeyName = "DashboardZenBridge";

    [STAThread]
    private static int Main(string[] args)
    {
        Application.EnableVisualStyles();

        try
        {
            string installDir = GetInstallDir();
            if (args.Any(arg => arg.Equals("--uninstall", StringComparison.OrdinalIgnoreCase) ||
                                arg.Equals("/uninstall", StringComparison.OrdinalIgnoreCase)))
            {
                Uninstall(installDir);
                return 0;
            }

            string extensionId = ResolveExtensionId(args);
            if (string.IsNullOrWhiteSpace(extensionId) || extensionId == "REPLACE_WITH_EXTENSION_ID")
            {
                MessageBox.Show(
                    "Chrome 확장 ID가 설정되지 않은 설치 파일입니다.\n배포 빌드 시 -ExtensionId 값을 넣어 다시 생성하세요.",
                    "TASHBOARD Bridge",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 2;
            }

            Directory.CreateDirectory(installDir);
            ExtractBridgePayload(installDir);

            string bridgeExe = Path.Combine(installDir, "DashboardZenBridge.exe");
            if (!File.Exists(bridgeExe))
            {
                throw new FileNotFoundException("DashboardZenBridge.exe was not installed.", bridgeExe);
            }

            string manifestPath = Path.Combine(installDir, ManifestFileName);
            WriteManifest(manifestPath, bridgeExe, extensionId);
            RegisterNativeHost(manifestPath);
            RegisterUninstaller(installDir);

            MessageBox.Show(
                "TASHBOARD Bridge 설치가 완료되었습니다.\nChrome 앱으로 돌아가면 ONLINE 상태로 전환됩니다.",
                "TASHBOARD Bridge",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "TASHBOARD Bridge 설치에 실패했습니다.\n\n" + ex.Message,
                "TASHBOARD Bridge",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return 1;
        }
    }

    private static string GetInstallDir()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TASHBOARD",
            "Bridge");
    }

    private static string ResolveExtensionId(string[] args)
    {
        string? fromArgs = args
            .Select(arg => arg.Trim())
            .FirstOrDefault(arg => arg.StartsWith("--extension-id=", StringComparison.OrdinalIgnoreCase) ||
                                   arg.StartsWith("/extension-id=", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(fromArgs))
        {
            int idx = fromArgs.IndexOf('=');
            if (idx >= 0 && idx + 1 < fromArgs.Length) return fromArgs[(idx + 1)..].Trim();
        }

        string? fromAssembly = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "BridgeExtensionId")
            ?.Value;

        return string.IsNullOrWhiteSpace(fromAssembly) ? InstallerConfig.ExtensionId : fromAssembly;
    }

    private static void ExtractBridgePayload(string installDir)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? payload = assembly.GetManifestResourceStream("bridge-payload.zip");
        if (payload is null) throw new InvalidOperationException("Embedded bridge payload was not found.");

        string tempZip = Path.Combine(Path.GetTempPath(), $"dashboard-zen-bridge-{Guid.NewGuid():N}.zip");
        try
        {
            using (FileStream fs = File.Create(tempZip))
            {
                payload.CopyTo(fs);
            }

            ZipFile.ExtractToDirectory(tempZip, installDir, overwriteFiles: true);
        }
        finally
        {
            if (File.Exists(tempZip)) File.Delete(tempZip);
        }
    }

    private static void WriteManifest(string manifestPath, string bridgeExe, string extensionId)
    {
        var manifest = new
        {
            name = HostName,
            description = "TASHBOARD Native Messaging Bridge",
            path = bridgeExe,
            type = "stdio",
            allowed_origins = new[] { $"chrome-extension://{extensionId}/" }
        };

        string json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(manifestPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static void RegisterNativeHost(string manifestPath)
    {
        using RegistryKey key = Registry.CurrentUser.CreateSubKey(
            $@"Software\Google\Chrome\NativeMessagingHosts\{HostName}",
            writable: true) ?? throw new InvalidOperationException("Could not open native messaging registry key.");

        key.SetValue(null, manifestPath, RegistryValueKind.String);
    }

    private static void RegisterUninstaller(string installDir)
    {
        string setupPath = Path.Combine(installDir, "TashboardBridgeSetup.exe");
        string? currentExe = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(currentExe) &&
            !currentExe.Equals(setupPath, StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(currentExe, setupPath, overwrite: true);
        }

        using RegistryKey key = Registry.CurrentUser.CreateSubKey(
            $@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{UninstallKeyName}",
            writable: true) ?? throw new InvalidOperationException("Could not open uninstall registry key.");

        key.SetValue("DisplayName", "TASHBOARD Bridge", RegistryValueKind.String);
        key.SetValue("DisplayVersion", "1.0.0", RegistryValueKind.String);
        key.SetValue("Publisher", "TASHBOARD", RegistryValueKind.String);
        key.SetValue("InstallLocation", installDir, RegistryValueKind.String);
        key.SetValue("UninstallString", $"\"{setupPath}\" --uninstall", RegistryValueKind.String);
        key.SetValue("NoModify", 1, RegistryValueKind.DWord);
        key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
    }

    private static void Uninstall(string installDir)
    {
        Registry.CurrentUser.DeleteSubKeyTree($@"Software\Google\Chrome\NativeMessagingHosts\{HostName}", throwOnMissingSubKey: false);
        Registry.CurrentUser.DeleteSubKeyTree($@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{UninstallKeyName}", throwOnMissingSubKey: false);

        if (Directory.Exists(installDir))
        {
            string? currentExe = Environment.ProcessPath;
            foreach (string file in Directory.EnumerateFiles(installDir, "*", SearchOption.AllDirectories))
            {
                if (!string.IsNullOrWhiteSpace(currentExe) &&
                    file.Equals(currentExe, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            string command = $"/c timeout /t 2 /nobreak > nul & rmdir /s /q \"{installDir}\"";
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        MessageBox.Show(
            "TASHBOARD Bridge가 제거되었습니다.",
            "TASHBOARD Bridge",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
