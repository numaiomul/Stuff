using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Swarm
{

	private Elem[] _swarmOpt;
	private bool _swarmIsDirty = false;
	private List<Elem> _swarmList = new List<Elem>();
	private List<GameObject> _swarmGO = new List<GameObject>();


	private Vector2 target;
	private void SetTarget() {
		if (MainManager.instance.target == null) {
			target = Vector2.zero;
		} else {
			target = new Vector2(MainManager.instance.target.position.x, MainManager.instance.target.position.y);
		}
	}

	private float centerInfluence;
	private void SetCenterInfluence() {
		centerInfluence = MainManager.instance.centerInfluence;
	}

	private float separationInfluence;
	private void SetSeparationInfluence() {
		separationInfluence = MainManager.instance.separationInfluence;
	}

	private float matchInfluence;
	private void SetMatchInfluence() {
		matchInfluence = MainManager.instance.matchInfluence;
	}

	private float boundInfluence;
	private void SetBoundInfluence() {
		boundInfluence = MainManager.instance.boundInfluence;
	}

	private float targetInfluence;
	private void SetTargetInfluence() {
		targetInfluence = MainManager.instance.targetInfluence;
	}

	public void AddToSwarm(GameObject _elem) {
		Elem arrayRef = new Elem();
		arrayRef.Init(_elem);
		_swarmGO.Add(_elem);
		_swarmList.Add(arrayRef);
		_swarmIsDirty = true;
	}

	//field for swarm Calculations
	private Vector2 elemCenter = new Vector2();
	private Vector2 elemCenterRuleFour = new Vector2();
	private Vector2 elemOffset = new Vector2();
	private Vector2 elemShy = new Vector2();
	private Vector2 diff;
	private int counter = 0;
	private int counterRule2 = 0;
	private int arrayLength;

#if !UNITY_WEBPLAYER
	private unsafe Elem* arrayRef;
	private unsafe Elem* arrayRefRuleTwo;
	public unsafe void Update()
#else
	private Elem[] arrayRef;
	private Elem[] arrayRefRuleTwo;
	public void Update() 
#endif
	{
		if (_swarmIsDirty) {
			arrayLength = _swarmList.Count;
			_swarmOpt = _swarmList.ToArray();
			_swarmIsDirty = false;
		}
		if (arrayLength <= 1) {
			return;
		}
		SetBoundInfluence();
		SetCenterInfluence();
		SetMatchInfluence();
		SetSeparationInfluence();
		SetTargetInfluence();
		SetTarget();
		elemCenter = new Vector2();
		elemOffset = new Vector2();
		diff = new Vector2();
#if !UNITY_WEBPLAYER
		fixed (Elem* swarmList = _swarmOpt)
#endif
		{

#if !UNITY_WEBPLAYER
			arrayRef = swarmList;
#else
			arrayRef = _swarmOpt;
#endif

			for (counter = 0; counter < arrayLength; counter++) {
#if !UNITY_WEBPLAYER
				elemCenter += (arrayRef->position);
				elemOffset += (arrayRef->offSet);
				arrayRef++;
#else
				elemCenter += arrayRef[counter].position;
				elemCenter += arrayRef[counter].offSet;
#endif
			}

#if !UNITY_WEBPLAYER
			arrayRef = swarmList;
#endif

			for (counter = 0; counter < arrayLength; counter++) {
				diff.x = 0; diff.y = 0;
#if !UNITY_WEBPLAYER
				diff += Rule1(arrayRef) * centerInfluence;
				diff += Rule2(arrayRef) * separationInfluence;
				diff += Rule3(arrayRef) * matchInfluence;
				diff += Rule4(arrayRef) * boundInfluence;
				diff += Rule5(arrayRef) * targetInfluence;
				(arrayRef->offSet) += (diff * Time.deltaTime);
				arrayRef->MoveElem(_swarmGO[counter]);
				arrayRef++;
#else
				diff += Rule1(arrayRef[counter]) * centerInfluence;
				diff += Rule2(arrayRef[counter]) * separationInfluence;
				diff += Rule3(arrayRef[counter]) * matchInfluence;
				diff += Rule4(arrayRef[counter]) * boundInfluence;
				diff += Rule5(arrayRef[counter]) * targetInfluence;
				arrayRef[counter].offSet += diff * Time.deltaTime;
				arrayRef[counter].MoveElem(_swarmGO[counter]);
#endif
			}
			arrayRef = null;
		}
	}


	// Boids try to fly towards the centre of mass of neighbouring boids.
#if !UNITY_WEBPLAYER
	private unsafe Vector2 Rule1(Elem* elem) {
		return (((elemCenter - (elem->position)) / (arrayLength - 1)) - (elem->position));
	}
#else
	private Vector2 Rule1(Elem elem) {
		return (((elemCenter - (elem.position))/(arrayLength-1)) - (elem.position));
	}
#endif
	//Boids try to keep a small distance away from other objects (including other boids).
#if !UNITY_WEBPLAYER
	private unsafe Vector2 Rule2(Elem* elem) {
		elemShy = Vector2.zero;
		fixed (Elem* swarmList = _swarmOpt) {
			arrayRefRuleTwo = swarmList;
			for (counterRule2 = 0; counterRule2 < arrayLength; counterRule2++) {
				if ((arrayRefRuleTwo->id) != (elem->id)) {
					//we're not using Vector2.Distance because it does (a-b).magnitude which adds Mathf.Sqrt to the process
					if (((arrayRefRuleTwo->position)-(elem->position)).sqrMagnitude	< 12f) {
						elemShy -= ((arrayRefRuleTwo->position) - (elem->position));
					}
				}
				arrayRefRuleTwo++;
			}
			arrayRefRuleTwo = null;
		}
		return elemShy;
	}
#else
	private Vector2 Rule2(Elem elem) {
		elemShy =  Vector2.zero;
		arrayRefRuleTwo = _swarmOpt;
		for (counterRule2 = 0; counterRule2 < arrayLength; counterRule2++) {
			if ((arrayRefRuleTwo[counter].id) != (elem.id)) {
				//we're not using Vector2.Distance because it does (a-b).magnitude which adds Mathf.Sqrt to the process
				if ((arrayRefRuleTwo[counter].position - elem.position).sqrMagnitude < 12f) {
					elemShy -= ((arrayRefRuleTwo[counter].position) - (elem.position));
				}
			}
		}
		return elemShy;
	}
#endif

	//Boids try to match velocity with near boids.
#if !UNITY_WEBPLAYER
	private unsafe Vector2 Rule3(Elem* elem) {
		return ((elemOffset - (elem->offSet)) / (arrayLength - 1) - (elem->offSet));
	}
#else
	private Vector2 Rule3(Elem elem) {
		return ((elemOffset-(elem.offSet))/(arrayLength - 1) - (elem.offSet));
	}
#endif

	//Boids try to stay withing edged (soft-edges)
#if !UNITY_WEBPLAYER
	private unsafe Vector2 Rule4(Elem* elem) {
		elemCenterRuleFour = Vector2.zero;
		if ((elem->position).x < -50f) {
			elemCenterRuleFour.x = 1;
		} else if ((elem->position).x > 50f) {
			elemCenterRuleFour.x = -1;
		}

		if ((elem->position).y < -50f) {
			elemCenterRuleFour.y = 1;
		} else if ((elem->position).y > 50f) {
			elemCenterRuleFour.y = -1;
		}

		return elemCenterRuleFour;
	}
#else
	private Vector2 Rule4(Elem elem) {
		elemCenterRuleFour = Vector2.zero;
		if ((elem.position).x < -50f) {
			elemCenterRuleFour.x = 1;
		}
		else if ((elem.position).x > 50f) {
			elemCenterRuleFour.x = -1;
		}
		
		if ((elem.position).y < -50f) {
			elemCenterRuleFour.y = 1;
		}
		else if ((elem.position).y > 50f) {
			elemCenterRuleFour.y = -1;
		}
		return elemCenterRuleFour;
	}
#endif
	//Boids try to go toward the target
#if !UNITY_WEBPLAYER
	private unsafe Vector2 Rule5(Elem* elem) {
		return ((target - (elem->position)) / 100);
	}
#else
	private Vector2 Rule5(Elem elem) {
		return ((target - (elem.position)) / 100);
	}
#endif
}
