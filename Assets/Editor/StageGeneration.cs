using UnityEngine;
using UnityEditor;

public class StageGeneration : EditorWindow
{
    Vector2 stSize;

    [MenuItem("Nucleum/Stage Generator")]
    static void Init()
    {
        StageGeneration sg = CreateInstance<StageGeneration>();
        sg.titleContent = new GUIContent("Stage Generator");
        sg.minSize = new Vector2(400, 300);
        sg.ShowUtility();
    }

    void OnGUI()
    {
        GameObject stage = GameObject.Find("Stage");
        stSize = EditorGUILayout.Vector2Field("Stage Size", stSize);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Stage"))
        {
            if (stSize.x > 0 && stSize.y > 0)
            {
                if (stage != null)
                    Clear();
                Generate();
            }
            else
                ShowNotification(new GUIContent("You must set a valid Stage Size!"));
        }
        if (GUILayout.Button("Clear Stage"))
        {
            if (stage != null)
                Clear();
            else
                ShowNotification(new GUIContent("There is no Stage to be cleared!"));
        }
        GUILayout.EndHorizontal();
    }

    void Generate()
    {
        GameObject tilePrefab = Resources.Load<GameObject>("Prefabs/Floor");
        float flSize = tilePrefab.GetComponentInChildren<BoxCollider>().size.x;

        GameObject stage = new GameObject("Stage");
        stage.transform.position = new Vector3(-stSize.x / 2 * flSize, 0, -stSize.y / 2 * flSize);
        stage.isStatic = true;

        // Generate Stage Tiles
        Transform row;
        GameObject tile;
        for (int y = 1; y <= stSize.y; y++)
        {
            row = new GameObject("Row " + y).transform;
            row.SetParent(stage.transform, false);
            row.localPosition = new Vector3(flSize / 2, 0, y * flSize - flSize / 2);
            row.gameObject.isStatic = true;
            for (int x = 0; x < stSize.x; x++)
            {
                tile = Instantiate(tilePrefab);
                tile.transform.SetParent(row, false);
                tile.transform.localPosition = new Vector3(x * flSize, 0);
                tile.name = "Tile_" + (x + 1);
                tile.isStatic = true;
            }
        }
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(stage, "Create " + stage.name);
        Selection.activeObject = stage;
    }

    void Clear()
    {
        GameObject stage = GameObject.Find("Stage");
        Undo.RegisterCompleteObjectUndo(stage, "Delete " + stage.name);
        DestroyImmediate(stage);
    }
}
