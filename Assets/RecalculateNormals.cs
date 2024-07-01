using UnityEngine;

public class RecalculateNormals : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            mesh.RecalculateNormals();
        }
    }
}
