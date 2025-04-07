using UnityEngine;

public class PolygonMeshGenerator : MonoBehaviour
{
    private Mesh myMesh = null;

    void Start()
    {
        // get the polygon collider 2d component and turn it into a mesh
        // the mesh should be rendered, and then also shown in the editor via gizmos
        var polygonCollider = GetComponent<PolygonCollider2D>();
        myMesh = polygonCollider.CreateMesh(false, false);
        myMesh.RecalculateNormals();
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;
    }

    // draw the mesh in the editor via gizmos
    void OnDrawGizmos()
    {
        if (myMesh == null || true)
        {
			var polygonCollider = GetComponent<PolygonCollider2D>();
			myMesh = polygonCollider.CreateMesh(false, false);
			myMesh.RecalculateNormals();
		}
		
		Gizmos.color = Color.black;
        Gizmos.DrawMesh(myMesh, transform.position, transform.rotation);
    }
}
