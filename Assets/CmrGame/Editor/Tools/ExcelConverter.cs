using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class XlsxToCsvTool : EditorWindow
{
    private List<string> xlsxFiles = new List<string>();  // 存储文件路径
    private List<bool> fileSelections = new List<bool>();  // 存储文件选择状态
    private bool convertToTxt = false;  // 是否勾选转换为txt
    private string inputDirectory = "Assets/CmrGame/DataTable/Xlsx";  // 输入文件夹路径
    private string outputDirectory = "Assets/CmrGame/DataTable/Csv";  // 输出文件夹路径

    [MenuItem("Tools/Xlsx to Csv Tool")]
    public static void ShowWindow()
    {
        GetWindow<XlsxToCsvTool>("Xlsx to Csv Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Xlsx to Csv Tool", EditorStyles.boldLabel);

        // 输入文件夹选择
        GUILayout.BeginHorizontal();
        GUILayout.Label("Input Folder:", GUILayout.Width(100));
        inputDirectory = EditorGUILayout.TextField(inputDirectory, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder with Xlsx Files", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                inputDirectory = path;
                // 不再自动调用 LoadXlsxFiles(inputDirectory);
            }
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        // 输出文件夹选择
        GUILayout.BeginHorizontal();
        GUILayout.Label("Output Folder:", GUILayout.Width(100));
        outputDirectory = EditorGUILayout.TextField(outputDirectory, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            outputDirectory = EditorUtility.OpenFolderPanel("Select Output Folder", "", "");
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();

        // 新增：刷新文件列表按钮
        if (GUILayout.Button("刷新文件列表"))
        {
            LoadXlsxFiles(inputDirectory);
        }

        // 显示Xlsx文件列表
        if (xlsxFiles.Count > 0)
        {
            GUILayout.Label("Select Files to Convert:");
            for (int i = 0; i < xlsxFiles.Count; i++)
            {
                fileSelections[i] = EditorGUILayout.ToggleLeft(Path.GetFileName(xlsxFiles[i]), fileSelections[i]);
            }
        }
        else
        {
            GUILayout.Label("No .xlsx files found in the selected input folder.");
        }

        // 全选/全不选
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All"))
        {
            for (int i = 0; i < fileSelections.Count; i++)
                fileSelections[i] = true;
        }
        if (GUILayout.Button("Deselect All"))
        {
            for (int i = 0; i < fileSelections.Count; i++)
                fileSelections[i] = false;
        }
        GUILayout.EndHorizontal();

        // 是否导出为TXT
        convertToTxt = EditorGUILayout.Toggle("Convert to Txt", convertToTxt);

        // 转换按钮
        if (GUILayout.Button("Convert"))
        {
            ConvertSelectedFiles();
        }
    }

    // 加载输入文件夹下的所有 .xlsx 文件（递归）并尽可能保留原先的选中状态
    private void LoadXlsxFiles(string path)
    {
        // 保存旧的选中状态（使用绝对路径做 key，忽略大小写以兼容 Windows）
        var oldSelections = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < xlsxFiles.Count; i++)
        {
            try
            {
                string key = Path.GetFullPath(xlsxFiles[i]);
                oldSelections[key] = fileSelections[i];
            }
            catch
            {
                // 退路：如果 Path.GetFullPath 失败，用原始路径
                oldSelections[xlsxFiles[i]] = fileSelections[i];
            }
        }

        xlsxFiles.Clear();
        fileSelections.Clear();

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Input path is empty. Please select an input folder before refreshing.");
            Repaint();
            return;
        }

        if (!Directory.Exists(path))
        {
            Debug.LogWarning($"Input path does not exist: {path}");
            Repaint();
            return;
        }

        IEnumerable<string> filesEnum;
        try
        {
            // 递归查找所有子目录下的 .xlsx 文件
            filesEnum = Directory.EnumerateFiles(path, "*.xlsx", SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to enumerate .xlsx files under '{path}': {ex.Message}");
            Repaint();
            return;
        }

        // 将路径规范化并收集到列表（便于排序和后续处理）
        var fileList = new List<string>();
        foreach (var f in filesEnum)
        {
            try { fileList.Add(Path.GetFullPath(f)); }
            catch { fileList.Add(f); }
        }

        // 可选：排序以稳定显示顺序
        fileList.Sort(StringComparer.OrdinalIgnoreCase);

        // 填充 xlsxFiles 与 fileSelections（恢复原先的勾选状态）
        foreach (var fullPath in fileList)
        {
            if (Path.GetFileName(fullPath).StartsWith("~$"))
                continue;
            xlsxFiles.Add(fullPath);
            if (oldSelections.TryGetValue(fullPath, out bool wasSelected))
                fileSelections.Add(wasSelected);
            else
                fileSelections.Add(false);
        }

        if (xlsxFiles.Count == 0)
            Debug.LogWarning("No .xlsx files found in the selected folder.");

        Repaint(); // 刷新窗口显示
    }


    // 转换选中的文件
    private void ConvertSelectedFiles()
    {
        if (string.IsNullOrEmpty(outputDirectory))
        {
            Debug.LogError("Output directory is not set.");
            return;
        }

        for (int i = 0; i < xlsxFiles.Count; i++)
        {
            if (fileSelections[i])
            {
                string filePath = xlsxFiles[i];
                string relativePath = Path.GetRelativePath(inputDirectory, filePath);
                string outputFilePath = Path.Combine(outputDirectory, relativePath);

                // 确保输出目录存在
                string outputDir = Path.GetDirectoryName(outputFilePath);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // 如果勾选了txt格式，则只转换为txt文件
                if (convertToTxt)
                {
                    string txtFilePath = Path.ChangeExtension(outputFilePath, ".txt");

                    // 读取xlsx文件并转换为txt
                    string txtContent = ConvertXlsxToCsv(filePath);

                    // 保存为txt文件
                    try
                    {
                        File.WriteAllText(txtFilePath, txtContent);
                    }
                    catch
                    {
                        throw new Exception($"[XlsxToCsvTool] 转换失败！转换前请先关闭{txtFilePath}。");
                    }

                    Debug.Log($"Converted {filePath} to {txtFilePath}");
                }
                else
                {
                    // 否则转换为csv文件
                    string csvFilePath = Path.ChangeExtension(outputFilePath, ".csv");

                    // 读取xlsx文件并转换为csv
                    string csvContent = ConvertXlsxToCsv(filePath);

                    // 保存为CSV文件
                    try
                    {
                        File.WriteAllText(csvFilePath, csvContent);
                    }
                    catch
                    {
                        throw new Exception($"[XlsxToCsvTool] 转换失败！转换前请先关闭{csvFilePath}。");
                    }

                    Debug.Log($"Converted {filePath} to {csvFilePath}");
                }
            }
        }
        AssetDatabase.Refresh();  // 刷新资源
        Debug.Log("Conversion completed.");
    }

    // 将Xlsx文件转换为CSV格式
    private string ConvertXlsxToCsv(string xlsxFilePath)
    {
        string csvContent = "";
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(xlsxFilePath));

        // 解压 .xlsx 文件
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        Directory.CreateDirectory(tempDir);

        using (FileStream readFs = new FileStream(xlsxFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (ZipInputStream zipStream = new ZipInputStream(readFs))
        {
            ZipEntry entry;
            while ((entry = zipStream.GetNextEntry()) != null)
            {
                string filePath = Path.Combine(tempDir, entry.Name);
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 如果是目录，跳过
                if (entry.IsDirectory)
                    continue;

                using (FileStream fs = File.Create(filePath))
                {
                    byte[] buffer = new byte[2048];
                    int size;
                    while ((size = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, size);
                    }
                }
            }
        }

        // 先读取 sharedStrings.xml
        string sharedStringsPath = Path.Combine(tempDir, "xl", "sharedStrings.xml");
        List<string> sharedStrings = new List<string>();
        if (File.Exists(sharedStringsPath))
        {
            XmlDocument sharedDoc = new XmlDocument();
            sharedDoc.Load(sharedStringsPath);
            XmlNodeList stringItems = sharedDoc.GetElementsByTagName("si");
            foreach (XmlNode si in stringItems)
            {
                // <si> 里可能有多个 <t>，需要拼接
                StringBuilder sb = new StringBuilder();
                foreach (XmlNode node in si.ChildNodes)
                {
                    if (node.Name == "t")
                    {
                        sb.Append(node.InnerText);
                    }
                    else
                    {
                        foreach (XmlNode sub in node.ChildNodes)
                        {
                            if (sub.Name == "t")
                            {
                                sb.Append(sub.InnerText);
                            }
                        }
                    }
                }
                sharedStrings.Add(sb.ToString());
            }
        }

        // 再读取 sheet1.xml
        string sheetXmlPath = Path.Combine(tempDir, "xl", "worksheets", "sheet1.xml");
        if (File.Exists(sheetXmlPath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(sheetXmlPath);
            XmlNodeList rows = xmlDoc.GetElementsByTagName("row");

            StringBuilder csvBuilder = new StringBuilder();

            for(int i =0;i<rows.Count;i++)
            {
                XmlNode rNode = rows[i];
                int cnt = rNode.ChildNodes.Count;
                for (int j =0;j< cnt; j++)
                {
                    XmlNode cell = rNode.ChildNodes[j];
                    string cellValue = "";

                    // 判断是否是共享字符串
                    XmlAttribute typeAttr = cell.Attributes["t"];
                    if (typeAttr != null && typeAttr.Value == "s")
                    {
                        int sharedIndex;
                        if (int.TryParse(cell.InnerText, out sharedIndex) && sharedIndex < sharedStrings.Count)
                        {
                            cellValue = sharedStrings[sharedIndex];
                        }
                    }
                    else
                    {
                        cellValue = cell.InnerText;
                    }

                    csvBuilder.Append(cellValue);
                    if(j!=cnt-1)
                        csvBuilder.Append(",");
                }
                if(i!=rows.Count-1)
                    csvBuilder.AppendLine();
            }

            csvContent = csvBuilder.ToString();
        }

        // 删除临时文件夹
        Directory.Delete(tempDir, true);

        return csvContent;
    }


    private string FormatPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;

        string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);

        if (path.StartsWith(projectPath))
        {
            return path.Substring(projectPath.Length);
        }

        return path;
    }
}
