using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSpawner : MonoBehaviour {

	//public stuff
	public Node node;
	public Node nodeStart;
    public int maxNodes = 5;
    public float opacityFadeDuration = 1f;

    public int rythmSpawnSkip = 4;

    private int rythmSpawnSkipCount = 4;
    private int nodeCount = 0;

    //private stuff
    private List<Node> nodeList = new List<Node>();
	private LineRenderer connection;
    private List<LineRenderer> connections = new List<LineRenderer>();

    //random values used for calculating a suitable spawn position
    private float randomRadius;
    private float randomAngle;

    // opacity value for all connections
    [HideInInspector]
    
    public float opacity = 0f;

	// Use this for initialization
	void Start ()
    {
		nodeList.Add (nodeStart);
		connection = nodeStart.GetComponent<LineRenderer>();

        RythmManager.onRythm.AddListener(Spawn);
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

        //choose random Node from list
        int randomNode = Random.Range(0, nodeList.Count - 1);

		//get the random Node from the list
		Node currentNode = nodeList[randomNode];

		// Find a suitable spawn location
		Vector3 spawnVector = CastRays(currentNode);

		bool isGoodLocation = true;
		foreach (Node node in nodeList) {
			if (spawnVector == node.transform.position)
				isGoodLocation = false;
		}

        if (isGoodLocation && nodeCount < maxNodes - 1)
		{
			//suck in outgoing connection
			//SuckIn(randomNode , currentNode);

			//create new node
			Node nextNode = Instantiate (node, spawnVector, Quaternion.Euler( new Vector3 (-180,0,0)));
            nextNode.transform.parent = transform;
            nextNode.nodeSpawner = this;

            nodeCount++;

            nextNode.name = "LevelNode " + nodeCount.ToString();
			//add to list
			nodeList.Insert (randomNode + 1, nextNode);
			//name it ( i cant change the name of the child of the new Nodes :C )
			nextNode.gameObject.transform.GetChild(0).name = System.Convert.ToString ("Blockradius" + nodeCount);

			//connect the old point with the new one
			Connect (currentNode, nextNode);

			//reconnect the new Node with the one wich has no incoming connection
			Reconnect (randomNode + 1, nextNode);
			isGoodLocation = true;

            StopAllCoroutines();
            StartCoroutine(FadeConnections());
		}
	}

    IEnumerator FadeConnections()
    {
        // fadeout
        float elapsedTime = 0f;
        float startOpacity = 0f;

        while (elapsedTime <= opacityFadeDuration)
        {
            elapsedTime += Time.deltaTime;

            opacity = Mathf.SmoothStep(startOpacity, 1, (elapsedTime / (opacityFadeDuration)));
            yield return null;
        }


        yield return null;
    }

    // connect 2 points with a linerenderer
    void Connect(Node currentNode, Node nextNode)
    {
        Vector3[] positions = new Vector3[] { currentNode.transform.position, nextNode.transform.position };
        //OC = Outgoing Connection
        currentNode._OC = currentNode.GetComponent<LineRenderer>();
        currentNode._OC.SetPositions(positions);
        //IC = Incoming Connection
        nextNode.hasIC = true;
    }

    // reconnects the missing connections
    void Reconnect(int Index, Node nextNode)
    {
        //set the linerenderers endposition to the next Node's position
        if (Index < nodeList.Count - 1)
        {
            Vector3[] positions = new Vector3[] { nextNode.transform.position, nodeList[Index + 1].transform.position };
            nextNode._OC = nextNode.GetComponent<LineRenderer>();
            nextNode._OC.SetPositions(positions);
            Debug.Log("Reconnected to " + nodeList[Index + 1].name);
        }
        else if (Index == nodeList.Count - 1 || Index >= nodeList.Count - 1)
        {
            Vector3[] positions = new Vector3[] { nextNode.transform.position, nodeList[0].transform.position };
            nextNode._OC = nextNode.GetComponent<LineRenderer>();
            nextNode._OC.SetPositions(positions);
            Debug.Log("Reconnected to " + nodeList[0].name);
        }
    }

    // Find a suitable spawn vector and set spawnVector to that found vector
    Vector2 CastRays(Node node)
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

    /*
	void SuckIn(int Index , Node currentNode)
	{
		//if the Index is bigger than 0/ not the start of the list
		//basically set end and start point of linerenderer to the point so it's not visible anymore
		if (Index > 0)
        {
			Vector3[] position = new Vector3[] 
            {
				nodeList [Index - 1].transform.position,
				nodeList [Index - 1].transform.position
			};
			nodeList [Index + 1].hasIC = false;
			currentNode._OC.SetPositions (position);
		}
		//If it's the start of the list
		else if ( Index == 0 )
		{
			Vector3[] position = new Vector3[] 
            {
				nodeList [0].transform.position ,
				nodeList [0].transform.position
			};
			nodeList [0].hasIC = false;
		}
	}

	//throw new point
	void ThrowPoint()
	{

	}
    */

    // Update is called once per frame
    void Update ()
    {
	}
}
