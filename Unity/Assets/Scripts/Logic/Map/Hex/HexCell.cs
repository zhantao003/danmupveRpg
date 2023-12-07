using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection{
    NE,
    E,
    SE,
    SW,
    W,
    NW,
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]//, typeof(MeshCollider)
public class HexCell : MonoBehaviour
{
    public const float Size = 1f;
    public const float ScaleSize = 0.85f;
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private void Awake()
    {
        //GetComponent<MeshFilter>().mesh = mesh = GetComponent<MeshCollider>().sharedMesh = new Mesh();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshCollider>().sharedMesh = mesh;
        var collider = GetComponent<MeshCollider>();
        if(collider != null)
            GameObject.Destroy(collider);
        // Flat meshes such as quads or planes that are marked as convex will be modified by the physics engine to have a thickness (and therefore a volume) to satisfy this requirement.
        // The thickness of the resulting mesh is proportional to its size and can be up to 0.05 of its longest dimension in the plane of the mesh.
        //GetComponent<MeshCollider>().convex = true;

        mesh.name = "Hex Cell";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Draw();
        this.transform.localScale = Vector3.one * Size;
    }



    /// <summary>
    /// 绘制地形
    /// </summary>
    public void Draw()
    {
        Clear();
        Vector3 center = Vector3.zero;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            AddTriangle(center,
                    HexMetrics.corners[(int)dir],
                    HexMetrics.corners[(int)dir + 1]);
        }
        UpdateMesh();
    }

    /// <summary>
    /// 清空mesh数据
    /// </summary>
    private void Clear()
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
    }

    /// <summary>
    /// 绘制mesh数据
    /// </summary>
    private void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    /// <summary>
    /// 添加三角形。
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int count = triangles.Count;
        vertices.Add(v1);
        triangles.Add(count++);
        vertices.Add(v2);
        triangles.Add(count++);
        vertices.Add(v3);
        triangles.Add(count++);
    }

}