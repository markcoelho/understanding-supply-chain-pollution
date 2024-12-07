using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class gravity : MonoBehaviour
{
    public float speed = 0f;
    public string[] prefabNames = { "bottle", "box", "plastic", "plasticbag", "cup", "paper"};
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, float> weights = new Dictionary<string, float> {
        { "bottle", 0.03f }, { "box", 0.2f }, { "plastic", 0.02f }, { "plasticbag", 0.02f },
        { "shoebox", 0.6f }, { "smallbox", 0.7f }, { "cup", 0.24f }, { "paper", 0.01f }
    };
    
    private GameObject unityChan;
    private Camera mainCamera;
    private List<AttractableBox> instances = new List<AttractableBox>();
    private string lastFileContent = "";
    private float totalWeight = 0f;
    private Text uiText;

    // Targets
    private Transform[] targets;

    void Start()
    {
        // Load prefabs and initialize targets
        foreach (string name in prefabNames)
            prefabs[name] = Resources.Load<GameObject>(name);

        unityChan = GameObject.Find("unitychan_dynamic");
        mainCamera = Camera.main;
        InitializeTargets();
        InitializeUI();

        // Start file change watcher
        StartCoroutine(WatchFileChanges());
    }

    private void InitializeTargets()
    {
        string[] targetNames = {
            "Character1_LeftHand", "Character1_RightHand", "Character1_LeftFoot", "Character1_RightFoot",
            "Character1_LeftUpLeg", "Character1_RightUpLeg", "Character1_LeftShoulder", "Character1_RightShoulder",
            "Character1_Head", "Character1_LeftForeArm", "Character1_RightForeArm", "Character1_RightLeg", "Character1_LeftLeg"
        };
        targets = new Transform[targetNames.Length];
        for (int i = 0; i < targetNames.Length; i++)
            targets[i] = GameObject.Find(targetNames[i])?.transform;
    }

    private void InitializeUI()
    {
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO != null)
        {
            uiText = new GameObject("DynamicText").AddComponent<Text>();
            uiText.transform.SetParent(canvasGO.transform);
            uiText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            uiText.fontSize = 24;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.rectTransform.sizeDelta = new Vector2(400, 100);
            uiText.rectTransform.anchoredPosition = new Vector2(0, -200);
            uiText.text = "0 kg";
        }
        else
            Debug.LogError("Canvas not found");
    }

    void Update()
    {
        foreach (AttractableBox box in instances)
        {
            if (box.Instance && box.Target)
                MoveObjectTowardsTarget(box);
        }
    }

        public float attractionForce = 1f;
        public float inertiaFactor = 0.000001f;
    

    private void MoveObjectTowardsTarget(AttractableBox box)
            {
                Vector3 direction = (box.Target.position - box.Instance.transform.position).normalized;
                Rigidbody rb = box.Instance.GetComponent<Rigidbody>();
                attractionForce = 3f;
                inertiaFactor = 0.000000000000000000000000000000000001f;
                if (rb != null)
                {
                    // Aplicar uma força de atração na direção desejada
                    Vector3 force = direction * attractionForce;

                    // Adicionar uma força de inércia baseada na velocidade atual
                    force += rb.velocity * inertiaFactor;

                    // Aplicar a força resultante ao Rigidbody
                    rb.AddForce(force);
                }
            }


    IEnumerator WatchFileChanges()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "quiz", "answer.txt");

        while (true)
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                if (content != lastFileContent)
                {
                    lastFileContent = content;
                    ProcessFileContent(content);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void ProcessFileContent(string content)
    {
        string[] lines = content.Split('\n');
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] parts = line.Split(' ');
                if (parts.Length == 2 && int.TryParse(parts[0], out int quantity))
                    HandlePrefabInstantiation(parts[1], quantity);
            }
        }
        UpdateText(totalWeight);
    }

    private void HandlePrefabInstantiation(string type, int quantity)
    {
        type = type.ToLower();
        if (prefabs.ContainsKey(type))
        {
            GameObject prefab = prefabs[type];
            float weight = weights.ContainsKey(type) ? weights[type] : 0f;
            totalWeight += quantity * weight;
            for (int i = 0; i < quantity/30; i++)
                InstantiatePrefab(prefab);
        }
    }

    void InstantiatePrefab(GameObject prefab)
    {
        Vector3 randomPosition = unityChan.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        GameObject instance = Instantiate(prefab, randomPosition, transform.rotation);
        Transform target = GetRandomTarget();
        instances.Add(new AttractableBox(instance, target));

        // Adjust prefab scale and add physics components
        AdjustPrefab(instance, prefab);
        AddPhysics(instance);

        if (target != null)
            Debug.Log($"Prefab {instance.name} attracted to: {target.name}");
    }

    private void AdjustPrefab(GameObject instance, GameObject prefab)
    {
        Vector3 scale = prefab.name switch
        {
            "box" => new Vector3(0.8f, 0.8f, 0.8f),
            "plasticbag" => new Vector3(0.4f, 0.4f, 0.4f),
            "bottle" => new Vector3(0.6f, 0.6f, 0.6f),
            "cup" => new Vector3(0.4f, 0.4f, 0.4f),
            "plastic" => new Vector3(0.4f, 0.4f, 0.4f),
            "paper" => new Vector3(0.4f, 0.4f, 0.4f),
            _ => Vector3.one
        };
        
        instance.transform.localScale = scale;
    }

    private void AddPhysics(GameObject instance)
    {
        if (!instance.GetComponent<Collider>())
            instance.AddComponent<BoxCollider>();
        if (!instance.GetComponent<Rigidbody>())
        {
            Rigidbody rb = instance.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = false;
        }
    }

    private Transform GetRandomTarget() =>
        targets[Random.Range(0, targets.Length)];

    private void UpdateText(float weight) =>
        uiText.text = $"{weight:F2} kg";

    private class AttractableBox
    {
        public GameObject Instance { get; }
        public Transform Target { get; }

        public AttractableBox(GameObject instance, Transform target)
        {
            Instance = instance;
            Target = target;
        }
    }
}
