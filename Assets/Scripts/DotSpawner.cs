using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotSpawner : MonoBehaviour {
	//public stuff
	public Dot Dot;
	public Dot DotStart;
    public int LevelInstanceSize = 5;

    public int rythmSpawnSkip = 4;

    private int rythmSpawnSkipCount = 4;
    private int LevelCounter = 0;
    //counter for naming the Dots
    private int counter = 2;

	//private stuff
	List<Dot> DotList = null;
	LineRenderer connection;
	List<LineRenderer> connections = null;
	//spawn delay
	//ADD IN THE GLOBAL BEAT THINGY
	public float delay;
	float timer;
	//random values
	float randomRadius;
	float randomAngle;
	//Spawnvector
	Vector3 spawnVector;
	// Use this for initialization
	void Start () {
		DotStart._OC = DotStart.GetComponent<LineRenderer> ();
		DotList = new List<Dot> ();
		DotList.Add (this.DotStart);
		connection = this.DotStart.GetComponent<LineRenderer> ();
		connections = new List<LineRenderer> ();

        RythmManager.onRythm.AddListener(Spawn);
    }
		
	//Method to connect 2 points with a linerenderer
	void connect (Dot currentDot , Dot nextDot)
	{
		Vector3[] positions = new Vector3[] {currentDot.transform.position , nextDot.transform.position};
		//OC = Outgoing Connection
		currentDot._OC = currentDot.GetComponent<LineRenderer> ();
		currentDot._OC.SetPositions (positions);
		//IC = Incoming Connection
		nextDot.hasIC = true;
	}


	void Spawn(float f)
	{
        if (rythmSpawnSkipCount < rythmSpawnSkip)
        {
            rythmSpawnSkipCount++;
            return;
        }
        else
            rythmSpawnSkipCount = 0;

            //choose random Dot from list
            int randomDot = Random.Range(0,this.DotList.Count - 1);
		//get the random Dot from the list
		Dot currentDot = DotList[randomDot];

		// GLOBAL RYTHM
		this.castRays( currentDot );
		bool isGoodLocation = true;
		foreach (Dot node in this.DotList) {
			if (this.spawnVector == node.transform.position)
				isGoodLocation = false;
		}

        if (isGoodLocation && this.LevelCounter < this.LevelInstanceSize - 1)
		{

            Debug.Log("1111 ");
			//suck in outgoing connection
			this.suckIn(randomDot , currentDot);

			//cast rays
            //this.castRays( currentDot );

			//create new node

			Dot nextDot = Instantiate (this.Dot, this.spawnVector, Quaternion.Euler( new Vector3 (-180,0,0)));
            nextDot.transform.parent = this.transform;
            nextDot.name = "LevelDot " + counter.ToString();
            this.LevelCounter++;
			//add to list
			DotList.Insert (randomDot + 1, nextDot);
			//name it ( i cant change the name of the child of the new Dots :C )
			nextDot.gameObject.transform.GetChild(0).name = System.Convert.ToString ("Blockradius" + this.counter);

			this.counter++;

			//connect the old point with the new one
			this.connect (currentDot, nextDot);
            
			timer = 0;
			//reconnect the new Dot with the one wich has no incoming connection
			this.reconnect (randomDot + 1, nextDot);
			isGoodLocation = true;
		}


	}

	void castRays(Dot node)
    {
		//node list - ignore the current node and its blockradius!!!
		bool hitBlockRadius = false;
		//default local direction
		Vector3 direction = new Vector3(0,1,0);
		//hit list stores all free positions
		List<Vector2> hit = new List<Vector2> ();
		//default angle
        float turnAngle = 0;
		//convert the angles to radians
		const float degToRad = Mathf.PI/180;
		//default spawnpoint
		Vector3 spawnpoint = new Vector3(0,0,0);

		//first ray needs to be shot seperatly
		//store all objects in the array
		RaycastHit2D[] hitArray = Physics2D.RaycastAll (node.transform.position, direction, node.radius);
		//check if the hitarray DOES NOT CONTAIN A BLOCKRADIUS OR A WALL
		for (int i = 0; i < hitArray.Length - 1; i++) {
			if ((hitArray[i].collider.tag == "blockradius") || (hitArray[i].collider.tag == "wall"))
        	{
				//If there is a BLOCKRADIUS OR A WALL then flag the array as not spawnable
				hitBlockRadius = true;
				//Jump out of loop directly (Performance-friendly)
				goto NEXT;
        	}
		}
		//adding the possible spawnpoint if it's a good one
		if (!hitBlockRadius)
			hit.Add (spawnpoint + direction.normalized * node.radius);
		NEXT:
		//increase angle and set hitBlockRadius to false
		turnAngle += 10f;
		hitBlockRadius = false;
		//all the other rays are checked here
        for (int i = 1; i < 36; i++)
        {
            //rotate the vector
			direction.x = node.transform.position.x + (Mathf.Cos(turnAngle * degToRad) * node.transform.position.x - Mathf.Sin(turnAngle * degToRad) * node.transform.position.y);
			direction.y = node.transform.position.y + (Mathf.Sin(turnAngle * degToRad) * node.transform.position.x + Mathf.Cos(turnAngle * degToRad) * node.transform.position.y);
			//get all objects in the ray's way 
			hitArray = Physics2D.RaycastAll (node.transform.position, direction, node.radius);
			spawnpoint = node.transform.position + direction.normalized * node.radius;
			//check all the objects if there is a blockradius or a wall
			for (int j = 0; j < hitArray.Length - 1; i++) {
				if ((hitArray[j].collider.tag == "blockradius") || (hitArray[j].collider.tag == "wall"))
				{
					//If so flag it as not usable and goto JUMP
					hitBlockRadius = true;
					goto JUMP;
				}
			}

			JUMP:
			if (!hitBlockRadius)
				hit.Add (spawnpoint);
			turnAngle += 10f;
			hitBlockRadius = false;
        }
			
		//choose random vector from the list
		int randomVector = Random.Range(0, hit.Count - 1);
		//override internal spawnvector
		//there are cases where the list is empty
		if (hit.Count != 0)
			this.spawnVector = hit [randomVector];
    }

	void suckIn(int Index , Dot currentDot)
	{
		//if the Index is bigger than 0/ not the start of the list
		//basically set end and start point of linerenderer to the point so it's not visible anymore
		if (Index > 0) {
			Vector3[] position = new Vector3[] {
				this.DotList [Index - 1].transform.position,
				this.DotList [Index - 1].transform.position
			};
			DotList [Index + 1].hasIC = false;
			currentDot._OC.SetPositions (position);
		}
		//If it's the start of the list
		else if ( Index == 0 )
		{
			Vector3[] position = new Vector3[] {
				this.DotList [0].transform.position ,
				this.DotList [0].transform.position
			};
			this.DotList [0].hasIC = false;
		}
	}

	//throw new point
	void throwPoint()
	{

	}

	//checks if it has IC ( pretty useless :P )
	bool getIC(Dot currentDot)
	{
		return currentDot.hasIC;
	}
	//reconnects the missing connections
	void reconnect(int Index , Dot nextDot )
	{
		//set the linerenderers endposition to the next Dot's position
		if (Index < this.DotList.Count - 1) {
			Vector3[] positions = new Vector3[] { nextDot.transform.position, this.DotList [Index + 1].transform.position };
			nextDot._OC = nextDot.GetComponent<LineRenderer> ();
			nextDot._OC.SetPositions (positions);
			Debug.Log ("Reconnected to " + DotList [Index + 1].name);
		} else if (Index == this.DotList.Count - 1 || Index >= this.DotList.Count - 1) {
			Vector3[] positions = new Vector3[] { nextDot.transform.position , this.DotList [ 0 ].transform.position };
			nextDot._OC = nextDot.GetComponent<LineRenderer> ();
			nextDot._OC.SetPositions (positions);
			Debug.Log ("Reconnected to " + DotList [0].name);
		}

	}

	// Update is called once per frame
	void Update ()
    {
	}
}
