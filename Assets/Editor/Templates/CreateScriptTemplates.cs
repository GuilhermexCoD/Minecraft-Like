using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public static class CreateScriptTemplates
{
    private const string kBaseEditorDirectory = "/Editor/BaseEditor";
    private const string kBaseEditorTemplate = "/ScriptTemplates/BaseEditor.cs.txt";

    [Shortcut("Base Editor", KeyCode.E, ShortcutModifiers.Control)]
    [MenuItem("Assets/Create/Custom C# Script/Base Editor")]
    public static void CreateEditorTemplate()
    {
        string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Path.GetExtension(selectionPath) == ".cs")
        {
            string saveDirectory = $"{Application.dataPath}{kBaseEditorDirectory}";

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string fileName = $"{Selection.activeObject.name}Editor.cs";

            string templatePath = $"{Application.dataPath}{kBaseEditorTemplate}";

            string filePath = $"{saveDirectory}/{fileName}";

            string content = File.ReadAllText(templatePath);

            content = PreprocessScriptAssetTemplate(filePath, content, new Dictionary<string, string>() { { "#SELECTEDSCRIPTNAME#", Selection.activeObject.name } });

            File.WriteAllText(filePath, content);

            AssetDatabase.ImportAsset($"Assets{kBaseEditorDirectory}/{fileName}");
        }
    }

    public static string PreprocessScriptAssetTemplate(string pathName, string resourceContent, Dictionary<string, string> replaceValues)
    {
        string rootNamespace = null;
        if (Path.GetExtension(pathName) == ".cs")
        {
            rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
        }

        string text = resourceContent;
        text = text.Replace("#NOTRIM#", "");
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = text.Replace("#NAME#", fileNameWithoutExtension);
        string text2 = fileNameWithoutExtension.Replace(" ", "");
        text = text.Replace("#SCRIPTNAME#", text2);
        foreach (var keyValue in replaceValues)
        {
            text = text.Replace(keyValue.Key, keyValue.Value);
        }
        text = RemoveOrInsertNamespace(text, rootNamespace);
        if (char.IsUpper(text2, 0))
        {
            text2 = char.ToLower(text2[0]) + text2.Substring(1);
            return text.Replace("#SCRIPTNAME_LOWER#", text2);
        }

        text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
        return text.Replace("#SCRIPTNAME_LOWER#", text2);
    }

    internal static string RemoveOrInsertNamespace(string content, string rootNamespace)
    {
        string text = "#ROOTNAMESPACEBEGIN#";
        string text2 = "#ROOTNAMESPACEEND#";
        if (!content.Contains(text) || !content.Contains(text2))
        {
            return content;
        }

        if (string.IsNullOrEmpty(rootNamespace))
        {
            content = Regex.Replace(content, "((\\r\\n)|\\n)[ \\t]*" + text + "[ \\t]*", string.Empty);
            content = Regex.Replace(content, "((\\r\\n)|\\n)[ \\t]*" + text2 + "[ \\t]*", string.Empty);
            return content;
        }

        string separator = (content.Contains("\r\n") ? "\r\n" : "\n");
        List<string> list = new List<string>(content.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
        int i;
        for (i = 0; i < list.Count && !list[i].Contains(text); i++)
        {
        }

        string text3 = list[i];
        string text4 = text3.Substring(0, text3.IndexOf("#"));
        list[i] = "namespace " + rootNamespace;
        list.Insert(i + 1, "{");
        for (i += 2; i < list.Count; i++)
        {
            string text5 = list[i];
            if (!string.IsNullOrEmpty(text5) && text5.Trim().Length != 0)
            {
                if (text5.Contains(text2))
                {
                    list[i] = "}";
                    break;
                }

                list[i] = text4 + text5;
            }
        }

        return string.Join(separator, list.ToArray());
    }
}
