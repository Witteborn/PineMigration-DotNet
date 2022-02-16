using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PineMigration
{
    public static class Program
    {
        public static void Main()
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
            File.WriteAllLines(csprojPath, csprojContent);
        }

        private static string GetPackageConfigContent()
        {
            var currDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(currDir, "packages.config");

            if (File.Exists(configPath) == false)
            {
                throw new FileNotFoundException($"{configPath} was expected and could not be found");
            }

            return File.ReadAllText(configPath);
        }
        private static string GetCSProjectFile()
        {
            var currDir = Directory.GetCurrentDirectory();
            var csproj = Directory.GetFiles(currDir).First(f => new FileInfo(f).Extension == ".csproj");

            if (string.IsNullOrWhiteSpace(csproj))
            {
                throw new FileNotFoundException($"A *.csproj file could not be found in {currDir}");
            }

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

            var endIndex = csprojContent.IndexOf("</Project>");
            csprojContent.InsertRange(endIndex, itemGroup);

            return csprojContent.ToArray();
        }

    }
}
