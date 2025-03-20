using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class gravity : MonoBehaviour
{
    public float speed = 0f;
    public string[] prefabNames = { "bottle", "box", "plastic", "plasticbag", "cup", "paper" };
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    private GameObject unityChan;
    private Camera mainCamera;
    private List<AttractableBox> instances = new List<AttractableBox>();
    private Text uiText;

    // Reference to the PipeServer script
    public PipeServer pipeServer;

    // Targets
    private Transform[] targets;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();


    public float cameraMoveRange = 2000f; // How far the camera can move left/right
    public float cameraSmoothness = 5f; // Smoothing factor for camera movement

    public float cameraSensitivity = 500f;

    public float groundLevel = 0f;

    void Start()
    {

        if (pipeServer == null)
        pipeServer = FindObjectOfType<PipeServer>();

        // Load prefabs and initialize targets
        foreach (string name in prefabNames)
            prefabs[name] = Resources.Load<GameObject>(name);

        unityChan = GameObject.Find("unitychan_dynamic");
        mainCamera = Camera.main;
        InitializeTargets();
        InitializeUI();
        InitializeSkeletonVisualization();

        // Generate 5 models of each type
        GenerateModels();
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
            uiText.fontSize = 18;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.rectTransform.sizeDelta = new Vector2(400, 100);
            uiText.rectTransform.anchoredPosition = new Vector2(0, -200);
            uiText.text = "User X Position: 0.00"; // Initialize with default text
        }
        else
            Debug.LogError("Canvas not found");
    }

    private void InitializeSkeletonVisualization()
    {
        // Define the connections between joints (targets)
        // Each pair represents a line to be drawn between two joints
        int[][] connections = {
            new int[] { 0, 9 },   // LeftHand to LeftForeArm
            new int[] { 9, 6 },   // LeftForeArm to LeftShoulder
            new int[] { 6, 7 },    // LeftShoulder to RightShoulder
            new int[] { 7, 10 },  // RightShoulder to RightForeArm
            new int[] { 10, 1 },   // RightForeArm to RightHand
            new int[] { 6, 4 },    // LeftShoulder to LeftUpLeg
            new int[] { 4, 12 },  // LeftUpLeg to LeftLeg
            new int[] { 12, 2 },   // LeftLeg to LeftFoot
            new int[] { 7, 5 },   // RightShoulder to RightUpLeg
            new int[] { 5, 11 },   // RightUpLeg to RightLeg
            new int[] { 11, 3 },   // RightLeg to RightFoot
            new int[] { 6, 8 },   // LeftShoulder to Head
            new int[] { 7, 8 }     // RightShoulder to Head
        };

        // Create LineRenderers for each connection
        foreach (var connection in connections)
        {
            GameObject lineObject = new GameObject("Line");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = 2;
            lineRenderers.Add(lineRenderer);
        }
    }

    private void GenerateModels()
    {
        foreach (string type in prefabNames)
        {
            for (int i = 0; i < 20; i++) // Generate 5 models of each type
            {
                HandlePrefabInstantiation(type, 1); // Instantiate one model at a time
            }
        }
    }

    void Update()
        {
            // Debug: Check if pipeServer is null
            if (pipeServer == null)
            {
                Debug.LogError("pipeServer is null! Assign it in the Inspector.");
                return;
            }

            // Get whether the user is in the frame
            bool userInFrame = pipeServer.GetUserInFrame();

            // Update object movement based on whether the user is in the frame
            foreach (AttractableBox box in instances)
                {
                    if (box.Instance && box.Target)
                    {
                        Rigidbody rb = box.Instance.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            if (userInFrame)
                            {
                                // User is in frame: Disable gravity and move objects towards targets
                                rb.useGravity = false;
                                MoveObjectTowardsTarget(box);
                            }
                            else
                            {
                                if(rb.position.y >= -5){
                                    rb.useGravity = true;
                                }else{
                                    rb.useGravity = false;
                                }
                            }
                        }
                    }
                }

            // Update skeleton visualization
            DrawSkeleton();

            // Get the user's X and Y position
            Vector2 userPosition = pipeServer.GetUserPosition();

            // Update the UI text with the user's X and Y position, and whether the user is in the frame
            if (uiText != null)
            {
                uiText.text = $"Pos: X = {userPosition.x:F2}, Y = {userPosition.y:F2}, In Frame: {userInFrame}";
            }

            // Move the camera based on the user's X position
            MoveCamera(userPosition.x);
        }

        private void MoveCamera(float userPositionX)
            {
                if (mainCamera == null)
                {
                    Debug.LogError("Main camera is not assigned!");
                    return;
                }

                // Map the user's X position (0 to 1) to the camera's movement range (non-inverted)
                float targetX = Mathf.Lerp(-cameraMoveRange, cameraMoveRange, userPositionX);

                // Apply sensitivity to the target position
                targetX *= cameraSensitivity;

                // Smoothly move the camera towards the target position
                Vector3 targetPosition = new Vector3(targetX, mainCamera.transform.position.y, mainCamera.transform.position.z);
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraSmoothness);

                // Clamp the camera's position to the movement range (optional)
                Vector3 clampedPosition = mainCamera.transform.position;
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, -cameraMoveRange * cameraSensitivity, cameraMoveRange * cameraSensitivity);
                mainCamera.transform.position = clampedPosition;
            }

    // Adjusted physics parameters
    public float attractionForce = 0.1f; // Reduced attraction force
    public float inertiaFactor = 0.1f; // Increased inertia factor
    public float maxVelocity = 5f; // Maximum velocity to prevent objects from moving too fast

    
        private void MoveObjectTowardsTarget(AttractableBox box)
        {
            Vector3 direction = (box.Target.position - box.Instance.transform.position).normalized;
            Rigidbody rb = box.Instance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply attraction force in the desired direction
                Vector3 force = direction * attractionForce;

                // Add inertia force based on current velocity
                force += rb.velocity * inertiaFactor;

                // Apply the resulting force to the Rigidbody
                rb.AddForce(force);

                // Limit the velocity to prevent objects from moving too fast
                if (rb.velocity.magnitude > maxVelocity)
                {
                    rb.velocity = rb.velocity.normalized * maxVelocity;
                }
            }
        }

    private void HandlePrefabInstantiation(string type, int quantity)
    {
        type = type.ToLower();
        if (prefabs.ContainsKey(type))
        {
            GameObject prefab = prefabs[type];
            for (int i = 0; i < quantity; i++)
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
            rb.drag = 2f; // Increased drag to slow down the object more effectively
            rb.angularDrag = 2f; // Increased angular drag to reduce spinning
            rb.mass = 0.5f; // Adjust mass for a more grounded feel
        }
    }

    private Transform GetRandomTarget() =>
        targets[Random.Range(0, targets.Length)];

    private void DrawSkeleton()
    {
        // Define the connections between joints (targets)
        int[][] connections = {
            new int[] { 0, 9 },   // LeftHand to LeftForeArm
            new int[] { 9, 6 },   // LeftForeArm to LeftShoulder
            new int[] { 6, 7 },   // LeftShoulder to RightShoulder
            new int[] { 7, 10 },  // RightShoulder to RightForeArm
            new int[] { 10, 1 },  // RightForeArm to RightHand
            new int[] { 6, 4 },   // LeftShoulder to LeftUpLeg
            new int[] { 4, 12 },  // LeftUpLeg to LeftLeg
            new int[] { 12, 2 },  // LeftLeg to LeftFoot
            new int[] { 7, 5 },   // RightShoulder to RightUpLeg
            new int[] { 5, 11 },  // RightUpLeg to RightLeg
            new int[] { 11, 3 },  // RightLeg to RightFoot
            new int[] { 6, 8 },  // LeftShoulder to Head
            new int[] { 7, 8 }    // RightShoulder to Head
        };

        // Update LineRenderer positions for each connection
        for (int i = 0; i < connections.Length; i++)
        {
            if (lineRenderers[i] != null && targets[connections[i][0]] != null && targets[connections[i][1]] != null)
            {
                lineRenderers[i].SetPosition(0, targets[connections[i][0]].position);
                lineRenderers[i].SetPosition(1, targets[connections[i][1]].position);
            }
        }
    }

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