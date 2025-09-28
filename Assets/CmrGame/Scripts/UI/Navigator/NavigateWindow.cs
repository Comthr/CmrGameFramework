using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class UINavWindow : EditorWindow
{
    private class Node
    {
        public Rect rect;
        public string name;
        public object target; // 对应 ISelectable
        public Dictionary<Direction, Node> links = new();

        public Node(string name, Vector2 pos, object target)
        {
            this.name = name;
            this.target = target;
            rect = new Rect(pos.x, pos.y, 100, 40);
        }
    }

    private enum Direction { Up, Down, Left, Right }

    private List<Node> nodes = new();
    private Node draggingNode;
    private Vector2 dragOffset;

    private Node linkingFrom;
    private Direction linkingDir;

    [MenuItem("Window/Custom/UI Navigation")]
    private static void ShowWindow()
    {
        GetWindow<UINavWindow>("UI Navigation");
    }

    private void OnEnable()
    {
        // 🔹这里可以改成自动扫描场景里的 ISelectable
        nodes.Clear();
        nodes.Add(new Node("ButtonA", new Vector2(50, 100), null));
        nodes.Add(new Node("ButtonB", new Vector2(300, 200), null));
        nodes.Add(new Node("ButtonC", new Vector2(200, 400), null));
    }

    private void OnGUI()
    {
        DrawConnections();
        DrawNodes();
        ProcessEvents(Event.current);

        if (GUILayout.Button("Apply to UIFormView"))
        {
            ApplyNavigation();
        }

        if (GUI.changed) Repaint();
    }

    private void DrawNodes()
    {
        foreach (var node in nodes)
        {
            GUI.Box(node.rect, node.name, EditorStyles.helpBox);
        }
    }

    private void DrawConnections()
    {
        Handles.color = Color.green;
        foreach (var node in nodes)
        {
            foreach (var kv in node.links)
            {
                if (kv.Value != null)
                {
                    Handles.DrawBezier(
                        node.rect.center,
                        kv.Value.rect.center,
                        node.rect.center + Vector2.up * 50,
                        kv.Value.rect.center + Vector2.down * 50,
                        Color.green,
                        null,
                        2f
                    );
                }
            }
        }

        // 正在连线中的情况（跟随鼠标）
        if (linkingFrom != null)
        {
            Handles.DrawBezier(
                linkingFrom.rect.center,
                Event.current.mousePosition,
                linkingFrom.rect.center + Vector2.up * 50,
                Event.current.mousePosition + Vector2.down * 50,
                Color.yellow,
                null,
                2f
            );
            Repaint();
        }
    }

    private void ProcessEvents(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 0) // 左键
        {
            foreach (var node in nodes)
            {
                if (node.rect.Contains(e.mousePosition))
                {
                    if (linkingFrom != null && node != linkingFrom) // 完成连线
                    {
                        linkingFrom.links[linkingDir] = node;
                        linkingFrom = null;
                        GUI.changed = true;
                    }
                    else
                    {
                        draggingNode = node;
                        dragOffset = e.mousePosition - node.rect.position;
                    }
                    return;
                }
            }
        }

        if (e.type == EventType.MouseDown && e.button == 1) // 右键
        {
            foreach (var node in nodes)
            {
                if (node.rect.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
                    {
                        menu.AddItem(new GUIContent("Link/" + dir), false, () =>
                        {
                            linkingFrom = node;
                            linkingDir = dir;
                        });
                    }
                    menu.ShowAsContext();
                    e.Use();
                    return;
                }
            }
        }

        if (e.type == EventType.MouseDrag && draggingNode != null)
        {
            draggingNode.rect.position = e.mousePosition - dragOffset;
            GUI.changed = true;
        }

        if (e.type == EventType.MouseUp && draggingNode != null)
        {
            draggingNode = null;
        }
    }

    private void ApplyNavigation()
    {
        foreach (var node in nodes)
        {
            Debug.Log($"[{node.name}] → Up:{node.links.GetValueOrDefault(Direction.Up)?.name}, " +
                      $"Down:{node.links.GetValueOrDefault(Direction.Down)?.name}, " +
                      $"Left:{node.links.GetValueOrDefault(Direction.Left)?.name}, " +
                      $"Right:{node.links.GetValueOrDefault(Direction.Right)?.name}");
        }

        // 🔹这里写回到 UIFormView 的 ISelectable 配置
    }
}

