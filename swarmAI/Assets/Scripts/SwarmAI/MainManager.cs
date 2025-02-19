using UnityEngine;
using System.Collections;

public class MainManager : MonoBehaviour {

	private static MainManager _instance;
	public static MainManager instance
	{
		get {
			if (_instance == null) Debug.LogException(new System.Exception("This SHOULD not be null"));
			return _instance;
		}
	}

	public GameObject elemPrefab;

	public Transform target;
	public Swarm swarm;
	#region MoveInfluence
	public float centerInfluence = 0.05f;
	public float separationInfluence = 0.4f;
	public float matchInfluence = 0.25f;
	public float boundInfluence = 1f;
	public float targetInfluence = 0f;
	#endregion
	
	
	
	#region Constants field
	const float xOffset = 1;
	const float yOffset = 1;
	Vector3 randomSpawn {
		get {
			return new Vector3(Random.Range(-xOffset,xOffset),Random.Range(-yOffset,yOffset),0f);
		}
	}
	#endregion

	#region Mono methods
	void Awake() {
		_instance = this;
		swarm = new Swarm();
	}

	void Update(){
		if (swarm != null) {
			swarm.Update();
		}
		else {
			Debug.LogError("problems");	
		}
	}
	#endregion

	#region Acces methods
	public void AddElement() {
		CreateElement();
	}
	public void AddTarget() {
		CreateTarget();
	}
	#endregion

	#region InternalLogic methods
	private void CreateElement() {
		Debug.Log("100x");
		for (int i = 0; i < 1000; i++) {
			swarm.AddToSwarm((GameObject)Instantiate(elemPrefab,randomSpawn,Quaternion.identity));
		}
	}
	private void CreateTarget() {
		//swarm.target = new Vector2 (Random.Range(-50f,50f),Random.Range(-50f,50f));
	}
	#endregion
}
