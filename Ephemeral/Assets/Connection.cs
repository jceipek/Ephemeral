using UnityEngine;
using System.Collections;

public class Connection : MonoBehaviour {
	[SerializeField]
	Node nodeA;
	Transform nodeAT;
	[SerializeField]
	Node nodeB;
	Transform nodeBT;

	Vector3 _tempDest;

	public void SetNodes (Node a, Node b) {
		nodeA = a;
		nodeB = b;
		nodeAT = nodeA.transform;
		nodeBT = nodeB.transform;
	}

	public void SetTemp (Node a, Vector3 tempDest) {
		nodeA = a;
		nodeAT = nodeA.transform;
		_tempDest = tempDest;
	}

	LineRenderer _lineRenderer;

	void Awake () {
		_lineRenderer = GetComponent<LineRenderer>();
	}

	void Start () {
		nodeAT = nodeA.transform;
		if (nodeB != null) {
			nodeBT = nodeB.transform;
		}
	}

	void OnValidate () {
		if (nodeA != null && nodeB != null) {
			nodeAT = nodeA.transform;
			nodeBT = nodeB.transform;
		}
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.cyan;
		if (nodeBT != null) {
			Gizmos.DrawLine(nodeAT.position, nodeBT.position);
		} else {
			Gizmos.DrawLine(nodeAT.position, _tempDest);
		}
	}

	void Update () {
		Vector3 dir;
		if (nodeBT == null) {
			dir = (nodeAT.position - _tempDest).normalized;
			_lineRenderer.SetPosition(0, nodeAT.position - dir * nodeA.ActivationRadius);
			_lineRenderer.SetPosition(1, _tempDest);
		} else {
			dir = (nodeAT.position - nodeBT.position).normalized;
			_lineRenderer.SetPosition(0, nodeAT.position - dir * nodeA.ActivationRadius);
			_lineRenderer.SetPosition(1, nodeBT.position + dir * nodeB.ActivationRadius);
		}
	}
}