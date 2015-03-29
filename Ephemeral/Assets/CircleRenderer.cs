using UnityEngine;
using System.Collections;

public class CircleRenderer : MonoBehaviour {

	[SerializeField]
	int _resolution = 100;
	[SerializeField]
	float _radius = 1f;
	public float Radius {
		set {
			_radius = value;
			if (_vertices == null || !_meshFilter) { return; }
			PopulateCircleVerts(_radius);
			UpdateMesh();
		}
	}
	[SerializeField]
	float _outerStrokeRadius = 2f;
	public float OuterStrokeRadius {
		set {
			_outerStrokeRadius = value;
			if (_vertices == null || !_meshFilter) { return; }
			PopulateStrokeVerts(_innerStrokeRadius, _outerStrokeRadius);
			UpdateMesh();
		}
	}
	[SerializeField]
	float _innerStrokeRadius = 1.2f;
	public float InnerStrokeRadius {
		set {
			_innerStrokeRadius = value;
			if (_vertices == null || !_meshFilter) { return; }
			PopulateStrokeVerts(_innerStrokeRadius, _outerStrokeRadius);
			UpdateMesh();
		}
	}

	MeshFilter _meshFilter;
	Vector3[] _vertices = null;

	// Use this for initialization
	void Awake () {
		_meshFilter = GetComponent<MeshFilter>();
		MakeMesh();
	}

	private void UpdateMesh () {
		Mesh mesh = _meshFilter.mesh;
		mesh.vertices = _vertices;
		_meshFilter.mesh = mesh;
	}

	void OnValidate () {
		if (!_meshFilter) { return; }
		PopulateStrokeVerts(_innerStrokeRadius, _outerStrokeRadius);
		PopulateCircleVerts(_radius);
		UpdateMesh();
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(transform.position, _radius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _innerStrokeRadius);
		Gizmos.DrawWireSphere(transform.position, _outerStrokeRadius);
	}

	private void PopulateStrokeVerts (float innerRadius, float outerRadius) {
		float theta = 2 * Mathf.PI / ((float)_resolution);
		float c = Mathf.Cos(theta);//precalculate the sine and cosine
		float s = Mathf.Sin(theta);

		// outer stroke
		float t;
		float xInner = innerRadius;
		float xOuter = outerRadius;
		float yInner = 0f;
		float yOuter = 0f;
		for(int i = 0; i < _resolution * 2; i+=2) {
			_vertices[(_resolution + 1) + i] = new Vector3(xInner + 0f, yInner + 0f);
			_vertices[(_resolution + 1) + i + 1] = new Vector3(xOuter + 0f, yOuter + 0f);

			//apply the rotation matrix
			t = xInner;
			xInner = c * xInner - s * yInner;
			yInner = s * t + c * yInner;

			t = xOuter;
			xOuter = c * xOuter - s * yOuter;
			yOuter = s * t + c * yOuter;
		}
	}

	private void PopulateCircleVerts (float radius) {
		float theta = 2 * Mathf.PI / ((float)_resolution);
		float c = Mathf.Cos(theta);//precalculate the sine and cosine
		float s = Mathf.Sin(theta);

		float t;
		float x = radius;//we start at angle = 0
		float y = 0f;
		_vertices[_resolution] = new Vector3(0f,0f); // Center vert
		for(int i = 0; i < _resolution; i++) {
			_vertices[i] = new Vector3(x + 0f, y + 0f);
			//apply the rotation matrix
			t = x;
			x = c * x - s * y;
			y = s * t + c * y;
		}
	}

	private void MakeMesh () {
		Mesh mesh = new Mesh();

		// Central circle
		_vertices = new Vector3[(_resolution + 1) + (_resolution*2)];
		int[] fillTriangles = new int[_resolution*3];
		int[] strokeTriangles = new int[_resolution*2*3];
		for(int i = 0; i < _resolution; i++) {
			fillTriangles[i*3 + 0] = i;
			fillTriangles[i*3 + 1] = _resolution;
			fillTriangles[i*3 + 2] = (i + 1) % _resolution;

			strokeTriangles[i*3 + 0] = i * 2 + 0 + (_resolution + 1);
			strokeTriangles[i*3 + 1] = (i * 2 + 2)%(_resolution*2) + (_resolution + 1);
			strokeTriangles[i*3 + 2] = (i * 2 + 1)%(_resolution*2) + (_resolution + 1);

			strokeTriangles[_resolution*3 + i*3 + 0] = (i * 2 + 1 + 0)%(_resolution*2) + (_resolution + 1);
			strokeTriangles[_resolution*3 + i*3 + 1] = (i * 2 + 1 + 1)%(_resolution*2) + (_resolution + 1);
			strokeTriangles[_resolution*3 + i*3 + 2] = (i * 2 + 1 + 2)%(_resolution*2) + (_resolution + 1);
		}

		PopulateStrokeVerts(_innerStrokeRadius, _outerStrokeRadius);
		PopulateCircleVerts(_radius);

		mesh.subMeshCount = 2;
		mesh.vertices = _vertices;
		mesh.SetTriangles(fillTriangles, 0);
		mesh.SetTriangles(strokeTriangles, 1);
		mesh.MarkDynamic();
		_meshFilter.mesh = mesh;
	}

}