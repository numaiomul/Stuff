using UnityEngine;
using System.Collections;

public class TestAnimation : MonoBehaviour {

	public int id;

	void OnEnable() {
		Debug.Log(id);
	}

	public void OnEvent() {
	Debug.Log ("here be event");
	}
}
