using UnityEngine;
using UnityEditor;

public class shaderLogger : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh cMesh = null; //Clone of starter mesh
    Quaternion lastRotation; //Used for update only on rotation

    public bool isCulling = true;
    public float cosineTolerance = 0f;

    private void GetMaxLength()
    {
        float zMax = transform.TransformPoint(meshFilter.mesh.vertices[0]).z;
        float zMin = transform.TransformPoint(meshFilter.mesh.vertices[0]).z;
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {
            float currentVert = transform.TransformPoint(meshFilter.mesh.vertices[i]).z;
            zMax = (currentVert > zMax) ? currentVert : zMax;
            zMin = (currentVert < zMin) ? currentVert : zMin;
        }
        Debug.Log(zMax + " " + zMin);

        transform.GetComponent<Renderer>().material.SetFloat("_MeshDepth", Mathf.Abs(zMin) + Mathf.Abs(zMax));
    }

    Mesh CullMesh(Mesh mesh, Vector3 cameraVector)
    {
        int[] newTriangles = new int[mesh.triangles.Length];

        int counter = 0;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 pointA = transform.TransformPoint(mesh.vertices[mesh.triangles[i]]);
            Vector3 pointB = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]);
            Vector3 pointC = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]);

            Vector3 triangleNormal = Vector3.Cross(pointB - pointA, pointC - pointA);
            float tempEqualation = Vector3.Dot(mesh.vertices[mesh.triangles[i]] - cameraVector, triangleNormal);
            if (tempEqualation >= cosineTolerance)
            {
                newTriangles[counter++] = mesh.triangles[i];
                newTriangles[counter++] = mesh.triangles[i + 1];
                newTriangles[counter++] = mesh.triangles[i + 2];
            }
        }
        mesh.triangles = newTriangles;
        MeshUtility.Optimize(mesh);
        return mesh;
    }

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        cMesh = new Mesh();
        cMesh.name = "meshClone";
        cMesh.vertices = meshFilter.sharedMesh.vertices;
        cMesh.triangles = meshFilter.sharedMesh.triangles;
    }


    void Update()
    {
        if (lastRotation != transform.rotation)
        {
            var newMesh = new Mesh();
            newMesh.vertices = cMesh.vertices;
            newMesh.triangles = cMesh.triangles;

            if (isCulling)
                newMesh = CullMesh(newMesh, Vector3.back);

            meshFilter.mesh = newMesh;  //3
            GetMaxLength();
            lastRotation = transform.rotation;
        }
    }
}
