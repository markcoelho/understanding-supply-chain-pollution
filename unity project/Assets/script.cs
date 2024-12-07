using UnityEngine;

public class script : MonoBehaviour
{
    public string targetName = "Character1_LeftHand"; // Name of the target object
    public float speed = 5.0f; // The speed of the attraction
    public string modelPath = "box big"; // Path to the model in Resources (without extension)

    private Transform target; // The object that the cube will be attracted to

    void Start()
    {
        // Load the model from the Resources folder
        Mesh modelMesh = Resources.Load<Mesh>(modelPath);
        Material modelMaterial = Resources.Load<Material>(modelPath);
        
        if (modelMesh != null && modelMaterial != null)
        {
            // Create a new GameObject
            GameObject cube = new GameObject("Cube");

            // Set the position of the new GameObject
            cube.transform.position = transform.position;

            // Add MeshFilter and MeshRenderer components
            MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = cube.AddComponent<MeshRenderer>();

            // Assign the mesh and material
            meshFilter.mesh = modelMesh;
            meshRenderer.material = modelMaterial;

            Debug.Log("Model loaded and instantiated.");
        }
        else
        {
            Debug.LogError("Model mesh or material not found: " + modelPath);
        }

        // Find the target object by name
        target = GameObject.Find(targetName)?.transform;

        if (target == null)
        {
            Debug.LogError("Target object not found!");
        }
        else
        {
            Debug.Log("Target object found: " + target.name);
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Move the cube towards the target at a constant speed
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
