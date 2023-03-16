using Serilog;
using System.Text.RegularExpressions;

namespace PineMigration
{
    public static class Program
    {
        public static void Main()
        {
            SetupLogger();

            try
            {
                //find and read package config
                var packageConfig_content = GetPackageConfigContent();

                //make sure csproj can be found
                var csprojPath = GetCSProjectFile();

                //run algorithm on package config content
                var itemGroup = ConfigToReferences(packageConfig_content);

                //enhance csproj content
                var csprojContent = GetEnhancedCsProject(csprojPath, itemGroup);

                //write result into csproj file
                Log.Information("Overwriting {@csprojPath}", csprojPath);
                File.WriteAllLines(csprojPath, csprojContent);
                Log.Information("Program executed sucessfully!");
            }
            catch (Exception ex)
            {
                Log.Error($"Exception: {ex.Message}", ex);
            }
        }

        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs\\PineMigration.log", rollingInterval: RollingInterval.Minute)
            .CreateLogger();
        }

        private static string GetPackageConfigContent()
        {
            var currDir = Directory.GetCurrentDirectory();
            var filename = "packages.config";
            var path = Path.Combine(currDir, filename);

            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException($"{path} was expected and could not be found");
            }
            Log.Information("File {@filename} found at {@path}", filename, path);
            return File.ReadAllText(path);
        }

        private static string GetCSProjectFile()
        {
            var currDir = Directory.GetCurrentDirectory();
            var csproj = Directory.GetFiles(currDir).First(f => new FileInfo(f).Extension == ".csproj");
            var filename = new FileInfo(csproj).Name;
            if (string.IsNullOrWhiteSpace(csproj))
            {
                throw new FileNotFoundException($"A *.csproj file could not be found in {currDir}");
            }
            Log.Information("File {@filename} found at {@csproj}", filename, csproj);

            return csproj;
        }

        private static string[] ConfigToReferences(string packageConfig)
        {
            const string pattern = "id=\"(.*)\"\\sversion=\"(.*)\"\\stargetFramework";
            var regex = new Regex(pattern); //<package id="Antlr" version="3.5.0.2" targetFramework="net472" />
            var matches = regex.Matches(packageConfig);//Finds all occurences

            List<string> contentResult = new List<string>();
            contentResult.Add("<ItemGroup>");

            foreach (Match match in matches)
            {
                //match.Group[0] contains the string searched on
                var name = match.Groups[1];     //Antlr
                var version = match.Groups[2];  //3.5.0.2

                contentResult.Add($"\t<PackageReference Include=\"{name}\" Version=\"{version}\"/>");
            }

            contentResult.Add("</ItemGroup>");

            return contentResult.ToArray();
        }

        private static string[] GetEnhancedCsProject(string path, string[] itemGroup)
        {
            var csprojContent = File.ReadAllLines(path).ToList();
            Log.Information("File {@path} content read successfully", path);

            var endIndex = csprojContent.IndexOf("</Project>");
            csprojContent.InsertRange(endIndex, itemGroup);
            Log.Information("Prepares insert of new ItemGroup at line {@endIndex}", endIndex);
            return csprojContent.ToArray();
        }
    }
}