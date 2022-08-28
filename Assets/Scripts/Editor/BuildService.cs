using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

public static class BuildService
{
    // [PostProcessBuildAttribute(0)]
    // public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    // {
    //     if (!buildTarget.Equals(BuildTarget.WebGL))
    //         return;
    //
    //     IncludeAxiosLibrary(buildPath);
    // }
    //
    // private static void IncludeAxiosLibrary(string buildPath)
    // {
    //     var indexFilePath = $"{buildPath}/index.html";
    //     var importText = "<script src=\"https://unpkg.com/axios/dist/axios.min.js\"></script>";
    //
    //     InsertAtFileLineIndex(importText, 119, indexFilePath);
    // }
    //
    // private static void InsertAtFileLineIndex(string textToInsert, int lineIndex, string pathToFile)
    // {
    //     var originalLines = File.ReadAllLines(pathToFile);
    //     var newLines = new List<string>();
    //
    //     for (var i = 0; i < lineIndex; i++)
    //     {
    //         newLines.Add(originalLines[i]);
    //     }
    //     
    //     newLines.Add(textToInsert);
    //
    //     for (int i = lineIndex, length = originalLines.Length; i < length; i++)
    //     {
    //         newLines.Add(originalLines[i]);
    //     }
    //     
    //     File.WriteAllLines(pathToFile, newLines);
    // }
}
