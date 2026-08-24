using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks; // Namespace necessário
using System.IO;
using System.IO.Compression;

public class BuildPostProcessor
{
    // O número 1 define a ordem de execução (útil se você tiver múltiplos post-processors)
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("calling post build script");
#if PROD
        Debug.Log("⚙️ Executando pós-build de Produção");
        ProcessPostBuild(true, pathToBuiltProject);

#endif

#if TEST
            Debug.Log("⚙️ Executando pós-build de Teste");
            ProcessPostBuild(false,pathToBuiltProject);
#endif

    }


    static void ProcessPostBuild(bool prod, string pathToBuiltProject)
    {
        string buildRootFolder = Path.GetDirectoryName(pathToBuiltProject);
        string buildAssetsFolder = Path.Combine(buildRootFolder, $"{UnityEngine.Application.productName}_Data", "DADOS SALVOS");
        Debug.Log(buildAssetsFolder);
        if (prod)
        {
            BuildZip(buildRootFolder);
        }

    }

    static void BuildZip(string buildPath)
    {
        string releaseRootDir = Path.Combine(Path.GetDirectoryName(buildPath), "release");
        Debug.Log($"release folder {releaseRootDir}");
        if (!Directory.Exists(releaseRootDir))
        {
            Debug.Log("release root dir does not exists creating...");
            Directory.CreateDirectory(releaseRootDir);
        }
        string releaseDir = Path.Combine(releaseRootDir, UnityEngine.Application.version, UnityEngine.Application.productName);
        if (Directory.Exists(releaseDir))
        {
            Debug.Log($"release dir exists removing {releaseDir}");
            Directory.Delete(releaseDir, true);
        }
        Debug.Log("copying to release folder...");
        string zipFolder = Path.GetDirectoryName(releaseDir);
        CopyDirectory(buildPath, releaseDir, true);
        string zipFileName = $"{UnityEngine.Application.productName.Replace(" ", "_")}_{UnityEngine.Application.version}.zip";
        Debug.Log("building zip");
        ZipFile.CreateFromDirectory(zipFolder, Path.Combine(releaseRootDir, zipFileName));
    }

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool ignoreMetaFiles = false)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            if (ignoreMetaFiles && file.Name.Contains(".meta"))
            {
                continue;
            }
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}