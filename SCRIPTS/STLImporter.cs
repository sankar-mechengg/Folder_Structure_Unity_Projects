using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class STLImporter : MonoBehaviour
{
    // Path to the STL file
    public string filename = "Bike.stl";
    private string filePath;

    //-----------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //The filename is located at streamingAssets folder
        filePath = Path.Combine(Application.streamingAssetsPath, filename);

        StartCoroutine(LoadSTLAsync(filePath));
    }

    //-----------------------------------------------------------------------------------

    // Load an STL file asynchronously
    IEnumerator LoadSTLAsync(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found at " + path);
            yield break;
        }

        Mesh mesh = new Mesh(); // Create a new mesh
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Allows for more vertices than the default 65535

        // Determine if the file is ASCII or binary
        bool isAscii = IsAsciiStl(path);

        // Load the file
        if (isAscii)
        {
            yield return StartCoroutine(LoadAsciiSTL(path, mesh));
        }
        else
        {
            yield return StartCoroutine(LoadBinarySTL(path, mesh));
        }

        // After the mesh is loaded, create the GameObject
        CreateMeshGameObject(mesh);
    }

    //-----------------------------------------------------------------------------------


    // Check if the STL file is ASCII or binary
    bool IsAsciiStl(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            string header = reader.ReadLine();
            return header != null
                && header.Trim().StartsWith("solid", StringComparison.OrdinalIgnoreCase);
        }
    }

    //-----------------------------------------------------------------------------------


    // Load an ASCII STL file
    IEnumerator LoadAsciiSTL(string path, Mesh mesh)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().StartsWith("facet normal"))
            {
                string[] vertex1 = lines[i + 2].Trim().Split(' ');
                string[] vertex2 = lines[i + 3].Trim().Split(' ');
                string[] vertex3 = lines[i + 4].Trim().Split(' ');

                Vector3 v1 = new Vector3(
                    float.Parse(vertex1[1], CultureInfo.InvariantCulture),
                    float.Parse(vertex1[2], CultureInfo.InvariantCulture),
                    float.Parse(vertex1[3], CultureInfo.InvariantCulture)
                );

                Vector3 v2 = new Vector3(
                    float.Parse(vertex2[1], CultureInfo.InvariantCulture),
                    float.Parse(vertex2[2], CultureInfo.InvariantCulture),
                    float.Parse(vertex2[3], CultureInfo.InvariantCulture)
                );

                Vector3 v3 = new Vector3(
                    float.Parse(vertex3[1], CultureInfo.InvariantCulture),
                    float.Parse(vertex3[2], CultureInfo.InvariantCulture),
                    float.Parse(vertex3[3], CultureInfo.InvariantCulture)
                );

                int startIndex = vertices.Count;
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                triangles.Add(startIndex);
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);
            }

            if (i % 1000 == 0) // Yield every 1000 lines to keep the editor responsive
            {
                yield return null;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    // Load a binary STL file
    IEnumerator LoadBinarySTL(string path, Mesh mesh)
    {
        byte[] bytes = File.ReadAllBytes(path);
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int offset = 84; // Skip the header and number of triangles
        int count = BitConverter.ToInt32(bytes, 80);

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < 3; j++) // Read each vertex of the triangle
            {
                Vector3 vertex = new Vector3(
                    BitConverter.ToSingle(bytes, offset + 12 + (j * 12)),
                    BitConverter.ToSingle(bytes, offset + 16 + (j * 12)),
                    BitConverter.ToSingle(bytes, offset + 20 + (j * 12))
                );
                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);
            }

            offset += 50; // Move to the next facet

            if (i % 1000 == 0) // Yield every 1000 facets to keep the editor responsive
            {
                yield return null;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    //-----------------------------------------------------------------------------------

    // Create a GameObject with the imported mesh
    private void CreateMeshGameObject(Mesh mesh)
    {
        GameObject meshObject = new GameObject("ImportedSTL");
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        // Add a material that will be rendered by URP (Universal Render Pipeline)
        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        // Center the mesh
        meshObject.transform.position = -mesh.bounds.center;

        // Scale the mesh to fit within the screen size
        //Get Screen Height
        float screenHeight = Camera.main.orthographicSize * 2.0f;
        //Get Screen Width
        float screenWidth = screenHeight * Camera.main.aspect;
        //Get the mesh size
        float meshSize = Mathf.Max(mesh.bounds.size.x, mesh.bounds.size.y, mesh.bounds.size.z);
        //Scale the mesh to fit within the screen size
        float scale = Mathf.Min(screenWidth, screenHeight) / meshSize;
        meshObject.transform.localScale = new Vector3(scale, scale, scale);

        // Add a mesh collider for physics interactions
        meshObject.AddComponent<MeshCollider>();

        // Add the mesh object to the scene
        meshObject.transform.SetParent(transform);

        //Reset the object position to 0,0,0
        meshObject.transform.position = Vector3.zero;

        // Set the camera to look at the mesh
        Camera.main.transform.LookAt(meshObject.transform.position);
    }

    //-----------------------------------------------------------------------------------
}
