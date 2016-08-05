using UnityEngine;
using System.Collections;

public class EnemyMnMEminemAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
      name = "enemy";
      gameObject.AddComponent<Rigidbody>();
      gameObject.AddComponent<BoxCollider>();
	}

	// Update is called once per frame
	void Update () {

	}
}
