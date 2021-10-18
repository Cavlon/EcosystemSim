using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    
    float height = 0.65f;
    int segments = 5;
    public float initialRegisterTime;
    [SerializeField] Vector3 offset;
    [SerializeField] Material meshColour;

    Mesh mesh;

    public void CreateFOV(float distance, float angle, EntityBase entity)
    {       
        mesh = CreateWedgeMesh(distance, angle);
        GameObject meshObject = new GameObject("FOV", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(FOVTrigger), typeof(Rigidbody));
        Rigidbody rb = meshObject.GetComponent<Rigidbody>();
        MeshCollider meshCollider = meshObject.GetComponent<MeshCollider>();
        FOVTrigger fovTrig = meshObject.GetComponent<FOVTrigger>();
        meshObject.transform.parent = transform;
        if (transform.CompareTag("Deer"))
        {
            fovTrig.isDeer = true;
            fovTrig.deer = GetComponent<Deer>();
        }
        meshObject.transform.position = transform.position + offset;
        meshObject.layer = 2;
        rb.useGravity = false;
        rb.isKinematic = true;
        meshObject.GetComponent<MeshFilter>().mesh = mesh;
        meshObject.GetComponent<MeshRenderer>().material = meshColour;
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        fovTrig.initialRegisterTime = initialRegisterTime;
        entity.fovTrig = fovTrig;
    }

    Mesh CreateWedgeMesh(float distance, float angle)
    {
        Mesh mesh = new Mesh();

        int triangleNum = (segments * 4) + 2 + 2;
        int verticesNum = triangleNum * 3;

        Vector3[] vertices = new Vector3[verticesNum];
        int[] triangles = new int[verticesNum];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        //left
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; i++)
        {           
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
             
            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            //far
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < verticesNum; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
