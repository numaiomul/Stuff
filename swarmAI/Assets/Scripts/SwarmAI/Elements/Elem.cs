using UnityEngine;
using System.Collections;

public struct Elem {

	
	private static int _id = 0;
	
	public int id;
	
	//private GameObject gameRef;
	
	private static float maxSpeed = 50f;
	public static float speed
	{
		get {
			return maxSpeed * Time.deltaTime;
		}
	}

	private Vector2 _elemPosition;
	public Vector2 position {
		get {
			return _elemPosition;
		}
	}

	private Vector2 _offSet;
	public Vector2 offSet
	{
		get { 
			return _offSet; 
		}
		set {
			_offSet = value;	
		}
	}
	public void Init(GameObject gameRef) {
		_elemPosition = new Vector2(gameRef.transform.position.x,gameRef.transform.position.y);
		_offSet = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f));
		id = _id++;
	}
	
	public void MoveElem (GameObject gameRef) {
		if (_offSet.magnitude > speed) {
			_offSet = (_offSet / _offSet.magnitude) * speed;	
		}
		_elemPosition += _offSet;
		
		gameRef.transform.position = new Vector3(_elemPosition.x, _elemPosition.y,0);
	}
	
	public override string ToString ()
	{
		return string.Format ("[Elem: position={0}, offSet={1}]", position, offSet);
	}
}