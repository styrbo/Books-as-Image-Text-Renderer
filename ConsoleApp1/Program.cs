using ConsoleApp1;

var resourcesFolder = $@"{Environment.CurrentDirectory}\Resources\";
var outputFolder = $@"{resourcesFolder}\Resources\Output\";
var configPath = $@"{Environment.CurrentDirectory}\Resources\config.json";

ConsoleDrawer.DrawText("Loading config...");
if (File.Exists(configPath) == false)
    return ConsoleDrawer.DrawError("Config file not found");
var configJson = File.ReadAllText(configPath);
var config = System.Text.Json.JsonSerializer.Deserialize<Config>(configJson);
if (config == null)
    return ConsoleDrawer.DrawError("Config file is invalid");

/////////////////////////////////////////////////////////////////
ConsoleDrawer.DrawText("Loading Font...");
if(string.IsNullOrEmpty(config.fontPath))
    return ConsoleDrawer.DrawError("Font path is empty");
var fontFilePath = Path.Combine(resourcesFolder, config.fontPath);
if(File.Exists(fontFilePath) == false)
    return ConsoleDrawer.DrawError("Font file not found");

var renderer = new ImageRenderer(config, fontFilePath);
ConsoleDrawer.DrawText("Finding text to render...");
var files = Directory.EnumerateFiles(resourcesFolder, "*.txt").ToArray();
ConsoleDrawer.DrawText("Found " + files.Length + " to render");
ConsoleDrawer.DrawText("Start Mapping...");
var stopwatch = System.Diagnostics.Stopwatch.StartNew();

stopwatch.Stop();
ConsoleDrawer.DrawText($"Mapping Finished in {stopwatch.Elapsed} Images Output Count: {0}");
if (ConsoleDrawer.AskPermissionToContinue("Start rendering?") == false) {
    return 1;
}

Directory.CreateDirectory(outputFolder);
foreach (var file in files) {
    var text = File.ReadAllText(file);
    var fileName = Path.GetFileNameWithoutExtension(file);
    renderer.Render(text, fileName);
}

Console.WriteLine("finish");
Console.ReadKey();

return 0;