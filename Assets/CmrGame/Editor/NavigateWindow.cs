using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class UINavWindow : EditorWindow
{
    private class Node
    {
        public Rect rect;
        public string title;
        public Dictionary<Direction, Node> links = new();

        public Node(Rect rect, string title)
        {
            this.rect = rect;
            this.title = title;
        }
    }

    private enum Direction { Up, Down, Left, Right }

    private List<Node> nodes = new();
    private Node linkingFrom;
    private Direction linkingDir;

    private Vector2 panOffset = Vector2.zero;
    private float zoom = 1f;

    private Rect leftPanel;
    private Rect rightPanel;

    [MenuItem("Window/UINav Editor")]
    private static void OpenWindow()
    {
        var window = GetWindow<UINavWindow>();
        window.titleContent = new GUIContent("UI Navigation");
    }

    private void OnGUI()
    {
        leftPanel = new Rect(0, 0, 200, position.height);
        rightPanel = new Rect(leftPanel.width, 0, position.width - leftPanel.width, position.height);

        DrawLeftPanel();
        DrawRightPanel();
    }

    private void DrawLeftPanel()
    {
        GUILayout.BeginArea(leftPanel, EditorStyles.helpBox);
        GUILayout.Label("操作区", EditorStyles.boldLabel);

        if (GUILayout.Button("新增节点"))
        {
            nodes.Add(new Node(new Rect(50, 50, 100, 60), "Node " + nodes.Count));
        }

        if (GUILayout.Button("应用到 UIFormView"))
        {
            ApplyToUI();
        }

        GUILayout.EndArea();
    }

    private void DrawRightPanel()
    {
        GUI.BeginGroup(rightPanel);
        {
            Rect canvasRect = new Rect(0, 0, rightPanel.width, rightPanel.height);

            DrawGrid(20, 0.2f, Color.gray, canvasRect);

            Vector2 localMouse = Event.current.mousePosition;

            if (canvasRect.Contains(localMouse))
            {
                ProcessEvents(Event.current, localMouse);
            }

            BeginWindows();
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                Rect transformedRect = Transform(node.rect);
                node.rect = GUI.Window(i, transformedRect, id => DrawNodeWindow(id), node.title);
                node.rect.position = InverseTransform(node.rect.position);
                node.rect.size /= zoom;
            }
            EndWindows();

            DrawConnections();
        }
        GUI.EndGroup();
    }

    private void DrawNodeWindow(int id)
    {
        Event e = Event.current;

        // 右键弹菜单
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            ShowContextMenu(nodes[id]);
            e.Use();
        }

        // 左键拖动，仅当不是拖线状态
        if (linkingFrom == null)
        {
            GUI.DragWindow();
        }
    }

    private void ShowContextMenu(Node node)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("连线/Up"), false, () => BeginLinking(node, Direction.Up));
        menu.AddItem(new GUIContent("连线/Down"), false, () => BeginLinking(node, Direction.Down));
        menu.AddItem(new GUIContent("连线/Left"), false, () => BeginLinking(node, Direction.Left));
        menu.AddItem(new GUIContent("连线/Right"), false, () => BeginLinking(node, Direction.Right));
        menu.ShowAsContext();
    }

    private void BeginLinking(Node node, Direction dir)
    {
        linkingFrom = node;
        linkingDir = dir;
    }

    private void ProcessEvents(Event e, Vector2 localMouse)
    {
        // 中键拖动
        if (e.type == EventType.MouseDrag && e.button == 2)
        {
            panOffset += e.delta;
            e.Use();
        }

        // 滚轮缩放
        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.1f;
            float oldZoom = zoom;
            zoom = Mathf.Clamp(zoom + zoomDelta, 0.3f, 2f);

            panOffset = (panOffset - localMouse) * (zoom / oldZoom) + localMouse;
            e.Use();
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, Rect area)
    {
        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        for (float y = panOffset.y % (gridSpacing * zoom); y < area.height; y += gridSpacing * zoom)
            Handles.DrawLine(new Vector3(0, y, 0), new Vector3(area.width, y, 0));

        for (float x = panOffset.x % (gridSpacing * zoom); x < area.width; x += gridSpacing * zoom)
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, area.height, 0));

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private Rect Transform(Rect rect)
    {
        return new Rect(Transform(rect.position), rect.size * zoom);
    }

    private Vector2 Transform(Vector2 pos)
    {
        return (pos * zoom) + panOffset;
    }

    private Vector2 InverseTransform(Vector2 pos)
    {
        return (pos - panOffset) / zoom;
    }

    private void DrawConnections()
    {
        foreach (var node in nodes)
        {
            foreach (var kv in node.links)
            {
                if (kv.Value != null)
                {
                    var color = GetColor(kv.Key);
                    var start = Transform(GetAnchor(node.rect, kv.Key));
                    var end = Transform(GetAnchor(kv.Value.rect, Opposite(kv.Key)));

                    Handles.DrawBezier(
                        start, end,
                        start + GetTangent(kv.Key) * 40,
                        end + GetTangent(Opposite(kv.Key)) * 40,
                        color, null, 3f
                    );
                }
            }
        }

        if (linkingFrom != null)
        {
            var color = GetColor(linkingDir);
            var start = Transform(GetAnchor(linkingFrom.rect, linkingDir));
            Vector2 mousePos = Event.current.mousePosition;

            Handles.DrawBezier(
                start, mousePos,
                start + GetTangent(linkingDir) * 40,
                mousePos - GetTangent(linkingDir) * 40,
                color, null, 2f
            );
            Repaint();

            // 点击目标节点完成连线
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                foreach (var node in nodes)
                {
                    if (Transform(node.rect).Contains(mousePos))
                    {
                        linkingFrom.links[linkingDir] = node;
                        break;
                    }
                }
                linkingFrom = null;
                Event.current.Use();
            }
        }
    }

    private static Color GetColor(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Color.blue,
            Direction.Down => Color.red,
            Direction.Left => new Color(1f, 0.5f, 0f),
            Direction.Right => Color.green,
            _ => Color.white
        };
    }

    private static Vector2 GetAnchor(Rect rect, Direction dir)
    {
        return dir switch
        {
            Direction.Up => new Vector2(rect.center.x, rect.yMin),
            Direction.Down => new Vector2(rect.center.x, rect.yMax),
            Direction.Left => new Vector2(rect.xMin, rect.center.y),
            Direction.Right => new Vector2(rect.xMax, rect.center.y),
            _ => rect.center
        };
    }

    private static Vector2 GetTangent(Direction dir)
    {
        return dir switch
        {
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            _ => Vector2.zero
        };
    }

    private static Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => dir
        };
    }

    private void ApplyToUI()
    {
        Debug.Log("应用到 UIFormView，这里可以写序列化逻辑");
    }
}
