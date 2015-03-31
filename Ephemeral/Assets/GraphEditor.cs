using UnityEngine;
using System.Collections;

public class GraphEditor : MonoBehaviour {

	[SerializeField]
	GameObject _nodePrefab;
	[SerializeField]
	GameObject _connectionPrefab;

	Camera _camera;

	void Awake () {
		_camera = GetComponent<Camera>();
	}

	void Start () {
		Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width, cursorTexture.height)/2f, cursorMode);
	}

	Node _sourceNode;
	Connection _tempConnection;

	void Update () {

		if (Input.GetKeyDown(KeyCode.C)) {
			Application.LoadLevel(0);
		}

		var mousePos = Input.mousePosition;
		mousePos.z = 10.0f;
		mousePos = _camera.ScreenToWorldPoint(mousePos);

		if (_tempConnection != null && _sourceNode != null) {
			_tempConnection.SetTemp(_sourceNode, mousePos);
			Collider2D res = Physics2D.OverlapCircle((Vector2)mousePos,0.5f);
 			if (res != null) {
 				var node = res.GetComponent<Node>();
 				if (node != null) {
 					Vector3 dest = (_sourceNode.transform.position - node.transform.position).normalized * node.ActivationRadius + node.transform.position;
 					_tempConnection.SetTemp(_sourceNode, dest);
 				}
 			}
		}

		if (Input.GetMouseButtonDown(0)) {
			Collider2D res = Physics2D.OverlapCircle((Vector2)mousePos,0.5f);
 			if (res != null) {
 				var node = res.GetComponent<Node>();
 				if (node != null) {
 					_sourceNode = node;
 				}
 			} else {
 				var nodeObj = Instantiate(_nodePrefab, mousePos, Quaternion.identity) as GameObject;
 				var node = nodeObj.GetComponent<Node>();
 				_sourceNode = node;
 			}
			var connectionObj = Instantiate(_connectionPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			_tempConnection = connectionObj.GetComponent<Connection>();
			_tempConnection.SetTemp(_sourceNode, mousePos);
		}

		if (Input.GetMouseButtonUp(0) && _tempConnection != null) {
			if (_sourceNode == null) { return; }
 			Collider2D res = Physics2D.OverlapCircle((Vector2)mousePos,0.5f);
 			if (res != null) {
 				var node = res.GetComponent<Node>();
 				if (node != null) {
 					_tempConnection.SetNodes(_sourceNode, node);
 					node.AddNodeConnection(_sourceNode);
 					_sourceNode.AddNodeConnection(node);
 				}
 			} else {
 				var nodeObj = Instantiate(_nodePrefab, mousePos, Quaternion.identity) as GameObject;
 				var node = nodeObj.GetComponent<Node>();
				_tempConnection.SetNodes(_sourceNode, node);
				node.AddNodeConnection(_sourceNode);
				_sourceNode.AddNodeConnection(node);
 				// Destroy(_tempConnection.gameObject);
 			}
 			_tempConnection = null;
 			_sourceNode = null;
		}
	}

	[SerializeField]
	Texture2D cursorTexture;
	CursorMode cursorMode = CursorMode.Auto;
	Vector2 hotSpot = Vector2.zero;
}
