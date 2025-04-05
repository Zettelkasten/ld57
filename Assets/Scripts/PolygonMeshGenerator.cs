using UnityEngine;

public class PolygonMeshGenerator : MonoBehaviour
{
    void Start()
    {
        // get the polygon collider 2d component and turn it into a mesh
        // the mesh should be rendered, and then also shown in the editor via gizmos
        var polygonCollider = GetComponent<PolygonCollider2D>();
        var mesh = polygonCollider.CreateMesh(false, false);
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    // draw the mesh in the editor via gizmos
    void OnDrawGizmos()
    {
        var polygonCollider = GetComponent<PolygonCollider2D>();
        var mesh = polygonCollider.CreateMesh(false, false);
        mesh.RecalculateNormals();
        Gizmos.color = Color.black;
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
    }
}
