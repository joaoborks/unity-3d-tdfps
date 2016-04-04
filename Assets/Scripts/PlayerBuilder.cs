using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class PlayerBuilder : MonoBehaviour
{
    // The placeholders must have the same name as the enum items
    enum Tool
    {
        None,
        Base,
        Blaster,
        Tumper,
        Laser
    }
    Tool tool = Tool.Base;
    Tool nextTool = Tool.None;

    // Placeholders
    Dictionary<string, GameObject> placeHolders;
    Dictionary<string, List<MeshRenderer>> phRenderers;
    GameObject ph;
    Material defMaterial;
    Material invMaterial;

    // To be removed
    public Text tCredits;
    public Text tBases;
    // End

    // Object Prefabs
    Dictionary<string, GameObject> prefabs;

    // Currency Variables
    public Dictionary<string, int> costs { get; private set; }
    int credits = 3000;
    int bases = 10;
    bool valid;

    // Raycasting
    const float MAX_RANGE = 6f;
    Camera cam;
    Vector3 target;
    Vector3 dir;
    Vector3 screenPoint;    
    Ray ray;
    RaycastHit hit;
    Collider hitCol;
    LayerMask mask;
    int floorLayer;
    int baseLayer;
    int turretLayer;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        // To be removed
        tBases.text = bases.ToString();
        tCredits.text = credits.ToString();
        // End

        // Get turret names and total turret count
        string[] turretNames = Enum.GetNames(typeof(Tool)).Skip(1).ToArray();
        int types = turretNames.Length;

        // Sets each turret cost
        costs = new Dictionary<string, int>(types);
        costs[Tool.Base.ToString()] = 1;
        costs[Tool.Blaster.ToString()] = 300;
        costs[Tool.Tumper.ToString()] = 500;
        costs[Tool.Laser.ToString()] = 2000;

        // Initializes the prefabs Dictionary
        prefabs = new Dictionary<string, GameObject>(types);
        // Gets references to the placeholder objects
        placeHolders = new Dictionary<string, GameObject>(types);
        phRenderers = new Dictionary<string, List<MeshRenderer>>(types);
        defMaterial = Resources.Load<Material>("Materials/Placeholder/Default");
        invMaterial = Resources.Load<Material>("Materials/Placeholder/Invalid");
        Transform phParent = GameObject.Find("Placeholders").transform;
        string name;
        for (int i = 0; i < types; i++)
        {
            name = turretNames[i];
            placeHolders[name] = phParent.FindChild(name).gameObject;
            placeHolders[name].SetActive(false);
            prefabs[name] = Resources.Load<GameObject>("Prefabs/Buildables/" + name);
            phRenderers[name] = placeHolders[name].GetComponentsInChildren<MeshRenderer>().ToList();
            phRenderers[name].RemoveAll(HasDetail);
            phRenderers[name].TrimExcess();
        }

        // Set initial tool
        ph = placeHolders[tool.ToString()];

        // Set properties for raycasting
        cam = Camera.main;
        screenPoint = new Vector3(Screen.width / 2, Screen.height / 2, MAX_RANGE);
        floorLayer = LayerMask.NameToLayer("Floor");
        baseLayer = LayerMask.NameToLayer("Base");
        turretLayer = LayerMask.NameToLayer("Turret");        
    }

    void Update()
    {
        // Change tool on input
        if (Input.GetButtonDown("Base"))
            nextTool = Tool.Base;
        else if (Input.GetButtonDown("Blaster"))
            nextTool = Tool.Blaster;
        else if (Input.GetButtonDown("Tumper"))
            nextTool = Tool.Tumper;
        else if (Input.GetButtonDown("Laser"))
            nextTool = Tool.Laser;

        if (nextTool != Tool.None)
        {
            tool = nextTool;
            nextTool = Tool.None;
            ph.SetActive(false);
            ph = placeHolders[tool.ToString()];
        }

        // Update active tool based on input
        if (valid && Input.GetButtonDown("Fire1"))
            Build();
    }

    void FixedUpdate()
    {
        ph.SetActive(false);
        // Checks buildable place
        valid = false;
        target = cam.ScreenToWorldPoint(screenPoint);
        dir = target - cam.transform.position;
        ray = new Ray(cam.transform.position, dir.normalized);
        mask = (1 << baseLayer) | (tool.ToString() == "Base" ? (1 << floorLayer) : (1 << turretLayer));
        if (Physics.Raycast(ray, out hit, MAX_RANGE, mask))
        {
            hitCol = hit.collider;
            // For Base Placement
            if (hitCol.gameObject.layer == floorLayer)
            {
                // Checks if this floor already has a base attached
                if (!Physics.Raycast(hitCol.transform.position + Vector3.down, Vector3.up, 50, 1 << baseLayer))
                    valid = Affordable();

                DisplayPlaceholder();
            }
            else if (hitCol.gameObject.layer == baseLayer)
            {
                if (tool.ToString() == "Base")
                {
                    // Upgrade
                }
                else
                {
                    if (!Physics.Raycast(hitCol.transform.position, Vector3.up, 50, 1 << turretLayer))
                        valid = Affordable();

                    DisplayPlaceholder();
                }
                print("Targeting: " + hitCol.gameObject.name);
            }
            else if (hitCol.gameObject.layer == turretLayer)
            {
                // Upgrade
                print("Targeting: " + hitCol.gameObject.name);
            }
        }            
    }

    void Build()
    {
        Instantiate(prefabs[tool.ToString()], hitCol.transform.position, Quaternion.identity);
        valid = false;
        hitCol = null;
        if (tool.ToString() == "Base")
        {
            bases -= costs[tool.ToString()];
            tBases.text = bases.ToString();
        }
        else
        {
            credits -= costs[tool.ToString()];
            tCredits.text = credits.ToString();
        }
    }    

    bool Affordable()
    {
        int cost = costs[tool.ToString()];
        return tool.ToString() == "Base" ? bases >= cost : credits >= cost;
    }

    void ColorValidation(MeshRenderer mr)
    {
        mr.material = valid ? defMaterial : invMaterial;
    }

    void DisplayPlaceholder()
    {
        ph.SetActive(true);
        ph.transform.position = hitCol.transform.position;
        phRenderers[tool.ToString()].ForEach(ColorValidation);
    }

    bool HasDetail(MeshRenderer mr)
    {
        return mr.material.color != defMaterial.color;
    }
}