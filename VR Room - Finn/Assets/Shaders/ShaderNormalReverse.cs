using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class ReverseNormals : MonoBehaviour
{
    public MeshFilter filter;
    void Start()
    {
        if (filter != null)
        {
            Mesh mesh = Instantiate(filter.sharedMesh); // avoid editing sharedMesh
            filter.mesh = mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++) normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i];
                    triangles[i] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }
}
