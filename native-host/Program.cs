using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DashboardZenBridge;

internal static class Program
{
    private const string ConfigFileName = ".zen_config.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [STAThread]
    private static int Main()
    {
        try
        {
            using Stream input = Console.OpenStandardInput();
            using Stream output = Console.OpenStandardOutput();
            JsonObject? request = ReadMessage(input);
            if (request is null) return 0;

            JsonObject response = HandleRequest(request);
            WriteMessage(output, response);
            return 0;
        }
        catch (Exception ex)
        {
            try
            {
                using Stream output = Console.OpenStandardOutput();
                WriteMessage(output, Error(ex.Message));
            }
            catch
            {
                // Chrome will report the native host failure.
            }

            return 1;
        }
    }

    private static JsonObject HandleRequest(JsonObject request)
    {
        string action = request["action"]?.GetValue<string>() ?? "";

        return action switch
        {
            "status" => Ok(new JsonObject { ["status"] = "online" }),
            "pickFolder" => PickFolder(),
            "openPath" => OpenPath(GetRequiredString(request, "path")),
            "getOpenPaths" => GetOpenPaths(),
            "saveConfig" => SaveConfig(request),
            "loadConfig" => LoadConfig(GetRequiredString(request, "path")),
            _ => Error($"Unknown action: {action}")
        };
    }

    private static JsonObject PickFolder()
    {
        Application.EnableVisualStyles();
        using FolderBrowserDialog dialog = new()
        {
            Description = "프로젝트 폴더를 선택하세요",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        DialogResult result = dialog.ShowDialog();
        if (result != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
        {
            return Ok(new JsonObject { ["path"] = "" });
        }

        return Ok(new JsonObject { ["path"] = dialog.SelectedPath });
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_RESTORE = 9;

    private static JsonObject OpenPath(string targetPath)
    {
        try
        {
            string fullPath = NormalizeWindowsPath(targetPath);

            foreach (var window in EnumerateExplorerWindows())
            {
                if (NormalizeWindowsPath(window.Path) == fullPath)
                {
                    ActivateWindow(window.Hwnd);
                    return Ok(new JsonObject { ["path"] = targetPath, ["activated"] = true });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Explorer activation check failed: {ex.Message}");
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = "explorer.exe",
            Arguments = Quote(targetPath),
            UseShellExecute = true
        };

        Process.Start(startInfo);
        return Ok(new JsonObject { ["path"] = targetPath, ["activated"] = false });
    }

    private static void ActivateWindow(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero) return;

        ShowWindow(hwnd, SW_RESTORE);
        SetForegroundWindow(hwnd);
    }

    private static JsonObject SaveConfig(JsonObject request)
    {
        JsonNode? projectNode = request["project"];
        if (projectNode is null) return Error("Project payload missing");

        JsonObject? project = projectNode.AsObject();
        string absPath = project["absPath"]?.GetValue<string>() ?? "";
        if (string.IsNullOrWhiteSpace(absPath)) return Error("Project absPath missing");

        Directory.CreateDirectory(absPath);
        string targetPath = Path.Combine(absPath, ConfigFileName);
        File.WriteAllText(targetPath, project.ToJsonString(JsonOptions), Encoding.UTF8);

        return Ok(new JsonObject { ["path"] = targetPath });
    }

    private static JsonObject LoadConfig(string targetDirectory)
    {
        string targetPath = Path.Combine(targetDirectory, ConfigFileName);
        if (!File.Exists(targetPath))
        {
            return Error("No config found", "not_found");
        }

        JsonNode? config = JsonNode.Parse(File.ReadAllText(targetPath, Encoding.UTF8));
        return Ok(config);
    }

    private static JsonObject GetOpenPaths()
    {
        var paths = new List<string>();
        try
        {
            paths.AddRange(EnumerateExplorerWindows().Select(window => NormalizeWindowsPath(window.Path)));
        }
        catch { }

        var array = new JsonArray();
        foreach (var p in paths.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct()) array.Add(p);
        return Ok(array);
    }

    private static IEnumerable<(string Path, IntPtr Hwnd)> EnumerateExplorerWindows()
    {
        Type? shellAppType = Type.GetTypeFromProgID("Shell.Application");
        if (shellAppType == null) yield break;

        dynamic? shellApp = Activator.CreateInstance(shellAppType);
        if (shellApp == null) yield break;

        dynamic? windows = shellApp.Windows();
        if (windows == null) yield break;

        foreach (dynamic? window in windows)
        {
            if (window == null) continue;

            string? path = TryGetExplorerWindowPath(window);
            if (string.IsNullOrWhiteSpace(path)) continue;

            IntPtr hwnd = IntPtr.Zero;
            try { hwnd = (IntPtr)window.HWND; } catch { }
            yield return (path, hwnd);
        }
    }

    private static string? TryGetExplorerWindowPath(dynamic window)
    {
        try
        {
            string? fullName = Convert.ToString(window.FullName);
            if (!string.IsNullOrWhiteSpace(fullName) &&
                !Path.GetFileName(fullName).Equals("explorer.exe", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
        }
        catch { }

        try
        {
            string? folderPath = Convert.ToString(window.Document.Folder.Self.Path);
            if (!string.IsNullOrWhiteSpace(folderPath)) return folderPath;
        }
        catch { }

        try
        {
            string? locationUrl = Convert.ToString(window.LocationURL);
            if (!string.IsNullOrWhiteSpace(locationUrl) && Uri.TryCreate(locationUrl, UriKind.Absolute, out Uri? uri) && uri.IsFile)
            {
                return uri.LocalPath;
            }
        }
        catch { }

        return null;
    }

    private static string NormalizeWindowsPath(string path)
    {
        return Path.GetFullPath(path).TrimEnd('\\', '/').ToLowerInvariant();
    }

    private static JsonObject? ReadMessage(Stream input)
    {
        Span<byte> lengthBytes = stackalloc byte[4];
        if (input.Read(lengthBytes) != 4) return null;

        int length = BitConverter.ToInt32(lengthBytes);
        if (length <= 0) return null;

        byte[] buffer = new byte[length];
        int offset = 0;
        while (offset < length)
        {
            int read = input.Read(buffer, offset, length - offset);
            if (read == 0) throw new EndOfStreamException("Unexpected end of native message");
            offset += read;
        }

        return JsonNode.Parse(Encoding.UTF8.GetString(buffer))?.AsObject();
    }

    private static void WriteMessage(Stream output, JsonObject message)
    {
        byte[] payload = Encoding.UTF8.GetBytes(message.ToJsonString());
        output.Write(BitConverter.GetBytes(payload.Length));
        output.Write(payload);
        output.Flush();
    }

    private static JsonObject Ok(JsonNode? data = null)
    {
        return new JsonObject
        {
            ["ok"] = true,
            ["data"] = data
        };
    }

    private static JsonObject Error(string message, string? code = null)
    {
        JsonObject response = new()
        {
            ["ok"] = false,
            ["error"] = message
        };
        if (!string.IsNullOrWhiteSpace(code)) response["code"] = code;
        return response;
    }

    private static string GetRequiredString(JsonObject request, string property)
    {
        string value = request[property]?.GetValue<string>() ?? "";
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{property} missing");
        return value;
    }

    private static string Quote(string value)
    {
        return $"\"{value.Replace("\"", "\\\"")}\"";
    }
}

