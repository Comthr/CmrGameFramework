using CmrUnityGameFramework.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CmrGame
{
    public static class UIFormViewGenerator
    {
        public static void GenerateViewScript(GameObject prefab, string outputDir, string className, string namespaceName, bool addSuffix)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab 为空，无法生成脚本。");
                return;
            }
            // 确保输出目录存在
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            // 确定类名（默认为 UIFormLogic 组件类型名 + "View"）
            if (string.IsNullOrEmpty(className))
            {
                UIFormLogic logic = prefab.GetComponent<UIFormLogic>();
                className = (logic != null) ? (logic.GetType().Name + "View") : (prefab.name + "View");
            }

            // 处理后缀
            string fileName = className;
            if (addSuffix)
                fileName += "_Generated";
            string filePath = Path.Combine(outputDir, fileName + ".cs");

            // 获取所有 UIFormElement（及其子类）组件
            var elements = prefab.GetComponentsInChildren<UIFormElement>(true);
            // 分组键：类型名 + "_" + (ElementName 或 类型名)
            Dictionary<string, List<UIFormElement>> groups = new Dictionary<string, List<UIFormElement>>();
            foreach (var element in elements)
            {
                string typeName = element.GetType().Name;
                string nameKey = !string.IsNullOrEmpty(element.ElementName) ? element.ElementName : typeName;
                string key = typeName + "_" + nameKey;
                if (!groups.ContainsKey(key))
                    groups[key] = new List<UIFormElement>();
                groups[key].Add(element);
            }

            // 生成代码文本
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System.Collections.Generic;");
            if (!string.IsNullOrEmpty(namespaceName) && namespaceName != "CmrGame")
                sb.AppendLine("using CmrGame;");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(namespaceName))
            {
                sb.AppendLine("namespace " + namespaceName);
                sb.AppendLine("{");
            }
            sb.AppendLine("    public class " + fileName);
            sb.AppendLine("    {");

            //类内实现
            HashSet<string> usedNames = new HashSet<string>();
            foreach (var group in groups.Values)
            {
                bool isArray = (group.Count > 1) || (group.Count == 1 && group[0].IsGroup);
                if (isArray)
                {
                    string typeName = group[0].GetType().Name;
                    string elementName = group[0].ElementName;
                    // 字段名：ElementName或类型名
                    string fieldName = !string.IsNullOrEmpty(elementName) ? elementName : typeName;
                    fieldName = SanitizeName(fieldName);
                    if (char.IsDigit(fieldName[0]))
                        fieldName = "_" + fieldName;
                    // 避免重名
                    string baseName = fieldName;
                    int idx = 1;
                    while (usedNames.Contains(fieldName))
                        fieldName = baseName + (idx++);
                    usedNames.Add(fieldName);
                    // 生成数组字段
                    sb.AppendLine($"        public {typeName}[] {fieldName};");
                }
                else
                {
                    int index = 1;
                    foreach (var element in group)
                    {
                        string typeName = element.GetType().Name;
                        string fieldName = !string.IsNullOrEmpty(element.ElementName) ? element.ElementName : (typeName + index++);
                        fieldName = SanitizeName(fieldName);
                        if (char.IsDigit(fieldName[0]))
                            fieldName = "_" + fieldName;
                        string baseName = fieldName;
                        int idx = 1;
                        while (usedNames.Contains(fieldName))
                            fieldName = baseName + (idx++);
                        usedNames.Add(fieldName);
                        sb.AppendLine($"        public {typeName} {fieldName};");
                    }
                }
            }


            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(namespaceName))
                sb.AppendLine("}");

            // 写文件并刷新
            File.WriteAllText(filePath, sb.ToString());
            AssetDatabase.Refresh(); // 刷新资源数据库:contentReference[oaicite:11]{index=11}
            Debug.Log("生成视图脚本: " + filePath);
        }

        private static string SanitizeName(string name)
        {
            // 移除非法字符
            var sb = new StringBuilder();
            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
