using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {
	//some strange things happen here xD 


	LineRenderer incomingConnection;
	LineRenderer outgoingConnection;

	public bool hasIC = false;
	public bool hasOC = false;

    public float minRadius = 0.3f;
	public float maxRadius = 0.8f;
	float radius1;

	public float radius
	{
		get { return this.radius1; }
	}

	public LineRenderer _OC
	{ 
		get { return this.outgoingConnection; }
		set { 
			this.hasOC = true;
			this.outgoingConnection = value; }
	}

	public LineRenderer _IC
	{
		get { return this.incomingConnection; }
		set { 
			this.hasIC = true;
			this.incomingConnection = value; }
	}



	// Use this for initialization
	void Start () {
		this._OC = gameObject.GetComponent<LineRenderer> ();
		this.randomRadius ();
	}

	void randomRadius()
	{
		this.radius1 = Random.Range (this.minRadius, this.maxRadius);
	}
		
	// Update is called once per frame
	void Update () {
		
	}
}
