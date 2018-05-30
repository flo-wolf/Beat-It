using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotSpawner : MonoBehaviour {

	//public stuff
	public Dot dot;
	public Dot dotStart;
    public int MaxDots = 5;

    public int rythmSpawnSkip = 4;

    private int rythmSpawnSkipCount = 4;
    private int DotCount = 0;

    //private stuff
    private List<Dot> DotList = new List<Dot>();
	private LineRenderer connection;
    private List<LineRenderer> connections = new List<LineRenderer>();

    //random values used for calculating a suitable spawn position
    private float randomRadius;
    private float randomAngle;

	// Use this for initialization
	void Start ()
    {
		DotList.Add (dotStart);
		connection = dotStart.GetComponent<LineRenderer>();

        RythmManager.onRythm.AddListener(Spawn);
    }
		
	//Method to connect 2 points with a linerenderer
	void Connect (Dot currentDot , Dot nextDot)
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
        int randomDot = Random.Range(0, DotList.Count - 1);

		//get the random Dot from the list
		Dot currentDot = DotList[randomDot];

		// Find a suitable spawn location
		Vector3 spawnVector = CastRays(currentDot);

		bool isGoodLocation = true;
		foreach (Dot node in DotList) {
			if (spawnVector == node.transform.position)
				isGoodLocation = false;
		}

        if (isGoodLocation && DotCount < MaxDots - 1)
		{

            Debug.Log("1111 ");
			//suck in outgoing connection
			SuckIn(randomDot , currentDot);

			//cast rays
            //this.castRays( currentDot );

			//create new node
			Dot nextDot = Instantiate (dot, spawnVector, Quaternion.Euler( new Vector3 (-180,0,0)));
            nextDot.transform.parent = transform;

            DotCount++;

            nextDot.name = "LevelDot " + DotCount.ToString();
			//add to list
			DotList.Insert (randomDot + 1, nextDot);
			//name it ( i cant change the name of the child of the new Dots :C )
			nextDot.gameObject.transform.GetChild(0).name = System.Convert.ToString ("Blockradius" + DotCount);

			//connect the old point with the new one
			Connect (currentDot, nextDot);

			//reconnect the new Dot with the one wich has no incoming connection
			Reconnect (randomDot + 1, nextDot);
			isGoodLocation = true;
		}
	}

    // Find a suitable spawn vector and set spawnVector to that found vector
	Vector2 CastRays(Dot node)
    {
		//the ray has hit the level boundaries (block radius)
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
		for (int i = 0; i < hitArray.Length-1; i++) {
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

        // the first try failed. Now we iterate around the node and try to find a suitable spawn location.
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
			for (int j = 0; j < hitArray.Length - 1; i++)
            {
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
            return hit[randomVector];
        else
            return node.transform.position;
    }

	void SuckIn(int Index , Dot currentDot)
	{
		//if the Index is bigger than 0/ not the start of the list
		//basically set end and start point of linerenderer to the point so it's not visible anymore
		if (Index > 0)
        {
			Vector3[] position = new Vector3[] 
            {
				DotList [Index - 1].transform.position,
				DotList [Index - 1].transform.position
			};
			DotList [Index + 1].hasIC = false;
			currentDot._OC.SetPositions (position);
		}
		//If it's the start of the list
		else if ( Index == 0 )
		{
			Vector3[] position = new Vector3[] 
            {
				DotList [0].transform.position ,
				DotList [0].transform.position
			};
			DotList [0].hasIC = false;
		}
	}

	//throw new point
	void ThrowPoint()
	{

	}

	//checks if it has IC ( pretty useless :P )
	bool GetIC(Dot currentDot)
	{
		return currentDot.hasIC;
	}

	//reconnects the missing connections
	void Reconnect(int Index , Dot nextDot )
	{
		//set the linerenderers endposition to the next Dot's position
		if (Index < DotList.Count - 1)
        {
			Vector3[] positions = new Vector3[] { nextDot.transform.position, DotList [Index + 1].transform.position };
			nextDot._OC = nextDot.GetComponent<LineRenderer> ();
			nextDot._OC.SetPositions (positions);
			Debug.Log ("Reconnected to " + DotList [Index + 1].name);
		} else if (Index == DotList.Count - 1 || Index >= DotList.Count - 1)
        {
			Vector3[] positions = new Vector3[] { nextDot.transform.position , DotList [ 0 ].transform.position };
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
