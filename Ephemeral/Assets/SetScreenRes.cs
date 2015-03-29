using UnityEngine;
using System.Collections;

public class SetScreenRes : MonoBehaviour {

	void Awake () {
		ForceDefaultScreenRes();
	}

	void ForceDefaultScreenRes () {
		if (Application.isEditor) {
			Debug.Log("Will set sceen res to default in build!");
			return;
		}
		Resolution res = Screen.currentResolution;
		Screen.SetResolution(res.width, res.height, true, res.refreshRate);
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
