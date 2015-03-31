using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	[SerializeField]
	bool _alwaysEmit = false;

	[SerializeField]
	float _activationEnergy = 0.3f;
	float _visualActivationEnergy = 0f;

	[SerializeField]
	float _energy = 0.3f;
	float _visualEnergy = 0f;
	public float Energy {
		get { return _energy; }
	}
	public float Radius {
		get { return Mathf.Sqrt(_visualEnergy + 0.0f); }
	}
	public float ActivationRadius {
		get { return Mathf.Sqrt(_visualActivationEnergy + 0.0f); }
	}

	float _transmitSpeed = 1f;

	[SerializeField]
	GameObject _packetPrefab;
	CircleRenderer _circleRenderer;
	CircleCollider2D _circlecollider;
	AudioSource _audioSource;

	[SerializeField]
	List<Node> _connectedNodes = new List<Node>();

	public void AddNodeConnection (Node n) {
		_connectedNodes.Add(n);
	}

	public void RemoveNodeConnection (Node n) {
		_connectedNodes.Remove(n);
	}

	void Awake () {
		_circleRenderer = GetComponent<CircleRenderer>();
		_circlecollider = GetComponent<CircleCollider2D>();
		_audioSource = GetComponent<AudioSource>();
	}

	void OnValidate () {
		_circleRenderer = GetComponent<CircleRenderer>();
		_circleRenderer.Radius = Radius;
		_circleRenderer.InnerStrokeRadius = ActivationRadius;
		_circleRenderer.OuterStrokeRadius = ActivationRadius + 0.1f;
	}

	void Start () {
		_circleRenderer.Radius = Radius;
		_circlecollider.radius = ActivationRadius + 0.1f;
		_circleRenderer.InnerStrokeRadius = ActivationRadius;
		_circleRenderer.OuterStrokeRadius = ActivationRadius + 0.1f;
		if (_alwaysEmit) {
			_circleRenderer.Radius = 0.5f;
			_circleRenderer.InnerStrokeRadius = 0f;
			_circleRenderer.OuterStrokeRadius = 0f;
		}
	}

	float _timer = 0f;
	void FixedUpdate () {
		if (_alwaysEmit) {
			if (_timer > _transmitSpeed) {
				SendPackets(_transmitSpeed);
				_timer = 0f;
			}
			_timer += Time.deltaTime;
		} else if (_energy >= _activationEnergy && _visualEnergy >= _activationEnergy) {
			SendPackets(_transmitSpeed);
			// _energy -= _activationEnergy;
			_energy = 0f;
			_audioSource.pitch = _connectedNodes.Count;
			_audioSource.Play();
		}

		float activationStep = _visualEnergySpeed * Time.deltaTime;
		if (_visualActivationEnergy < _activationEnergy) {
			_visualActivationEnergy += activationStep;
			if (ActivationRadius > 0f) {
				_circleRenderer.InnerStrokeRadius = ActivationRadius;
				_circleRenderer.OuterStrokeRadius = ActivationRadius + 0.1f;
			}
		} else {
			_visualActivationEnergy = _activationEnergy;
			if (ActivationRadius > 0f) {
				_circleRenderer.InnerStrokeRadius = ActivationRadius;
				_circleRenderer.OuterStrokeRadius = ActivationRadius + 0.1f;
			}
		}

		float stepSize = _visualEnergySpeed * Time.deltaTime;
		if (_visualEnergy + stepSize < _energy) {
			_visualEnergy += stepSize;
			_circleRenderer.Radius = Radius;
		} else if (_visualEnergy - stepSize > _energy) {
			_visualEnergy -= stepSize;
			_circleRenderer.Radius = Radius;
		} else if (_visualEnergy != _energy) {
			_visualEnergy = _energy;
			_circleRenderer.Radius = Radius;
		}
	}

	float _visualEnergySpeed = 0.3f;
	void Update () {

	}

	void SendPackets (float duration) {
		for (int i = 0; i < _connectedNodes.Count; i++) {
			Vector3 toOther = (_connectedNodes[i].transform.position - transform.position);
			float angle = Mathf.Atan2(toOther.y, toOther.x) * Mathf.Rad2Deg;
            Quaternion dir = Quaternion.AngleAxis(angle, Vector3.forward);
			GameObject packetObj = Instantiate(_packetPrefab, transform.position, dir) as GameObject;
			Packet packet = packetObj.GetComponent<Packet>();
			packet.InitWith(energy: 0.05f,
							// energy: _activationEnergy/_connectedNodes.Count, // TODO: Think about dividing by node count
				lifetime: _transmitSpeed,
							target: _connectedNodes[i],
							initLoc: transform.position);
		}
	}

	public void RecieveEnergy (float e) {
		if (_alwaysEmit) { return; }
		_energy += e;
	}



	// [SerializeField]
	// bool _isSource = false;
	// bool IsSource {
	// 	get { return _isSource; }
	// }
	// [SerializeField]
	// bool _isSink = false;
	// bool IsSink {
	// 	get { return _isSink; }
	// }

	// public void IncreaseValue (float amount) {
	// 	if (_isSink || _isSource) { return; }
	// 	_value += amount;
	// 	_circleRenderer.Radius = Radius;
	// }
	// private float DecreaseValue (float amount) {
	// 	if (_isSource) { return amount; }
	// 	if (_isSink) { return 0f; }
	// 	if (_value > amount) {
	// 		_value -= amount;
	// 		_circleRenderer.Radius = Radius;
	// 		return amount;
	// 	}
	// 	return 0f;
	// }

	// float _emissionInterval = 0.3f;
	// float _quantityPerEmission = 0.1f;



	// void Start () {
	// 	_circleRenderer.Radius = Radius;
	// }

	// void OnValidate () {
	// 	_circleRenderer = GetComponent<CircleRenderer>();
	// 	_circleRenderer.Radius = Radius;
	// }

	// float _timer = 0f;
	// void FixedUpdate () {
	// 	_timer += Time.fixedDeltaTime;
	// 	// _fastTimer = 0f;
	// 	while (_timer > _emissionInterval) {
	// 		_timer -= _emissionInterval;
	// 		for (int i = 0; i < _connectedNodes.Count; i++) {
	// 			if (!_connectedNodes[i].IsSource && !_isSink && (_isSource || _connectedNodes[i].IsSink || Value - _connectedNodes[i].Value > 0.01f)) {
	// 				float a = DecreaseValue(_quantityPerEmission);
	// 				_connectedNodes[i].IncreaseValue(a);

	// 				Vector3 toOther = (_connectedNodes[i].transform.position - transform.position);
	// 				float angle = Mathf.Atan2(toOther.y, toOther.x) * Mathf.Rad2Deg;
	//                 Quaternion dir = Quaternion.AngleAxis(angle, Vector3.forward);
	// 				// Quaternion dir = Quaternion.LookRotation(toOther.normalized, Vector3.up);
	// 				GameObject packetObj = Instantiate(_packetPrefab, transform.position, dir) as GameObject;
	// 				Packet packet = packetObj.GetComponent<Packet>();
	// 				packet.InitWith(lifetime: _emissionInterval,
	// 								target: _connectedNodes[i],
	// 								initLoc: transform.position);
	// 			}
	// 		}
	// 	}
	// }


}