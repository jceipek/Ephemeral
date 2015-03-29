using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Packet : MonoBehaviour {
	CircleRenderer _circleRenderer;

	float _lifetime = 1f;
	Node _target;
	Vector3 _initLoc;
	float _energy = 0f;

	public void InitWith (float energy, float lifetime, Node target, Vector3 initLoc) {
		_target = target;
		_lifetime = lifetime;
		_initLoc = initLoc;
		_energy = energy;
	}

	float _timer = 0f;
	void FixedUpdate () {
		Vector3 unnormalizedDir = (_target.transform.position - _initLoc);
		Vector3 dir = unnormalizedDir.normalized;
		transform.position = _initLoc + (unnormalizedDir - (dir * _target.Radius)) * _timer/_lifetime;
		if (_timer >= _lifetime) {
			_target.RecieveEnergy(_energy);
			Destroy(gameObject);
		}
		_timer += Time.fixedDeltaTime;
	}

	void Awake () {
		_circleRenderer = GetComponent<CircleRenderer>();
	}
}