using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float minRadius = 0.3f;
	public float maxRadius = 0.8f;
    public float radius;
    public bool startNode = false;

    public GameObject fillCircleGo;
    public NodeSegment nodeSegment;
    public SpriteRenderer sr;
    public NodeSpawner nodeSpawner; // was spawned by this spawner

    // get and set handled by underscore variables
    private LineRenderer incomingConnection;
    private LineRenderer outgoingConnection;

    private Vector2 startPos;
    private Vector2 endPos;

    [HideInInspector]
    public bool hasIC = false;
    [HideInInspector]
    public bool hasOC = false;

    private float defaultLocalScale;            // default player dot size
    private float defaultFillCircleSize;        // default fill indicator size (inside player dot)
    private float scale = 0f;
    private float opacity = 0f;

    

    // outgoing connection
    public LineRenderer _OC
	{ 
		get { return outgoingConnection; }
		set { 
			hasOC = true;
			outgoingConnection = value; }
	}

    // incoming connection
	public LineRenderer _IC
	{
		get { return this.incomingConnection; }
		set { 
			hasIC = true;
			incomingConnection = value; }
	}


// Use this for initialization
	void Start () {
		_OC = gameObject.GetComponent<LineRenderer> ();
        RandomRadius();

        defaultLocalScale = transform.localScale.x;
        defaultFillCircleSize = fillCircleGo.transform.localScale.x;
        transform.localScale = Vector3.zero;

        // the starting node needs to be faded in right away, all other nodes get faded during the spawning coroutine in NodeSpawner.cs
        if (startNode)
        {
            StartCoroutine(Fade(true));
        }
    }

    void Update()
    {
        // update fill amount
        if(incomingConnection != null)
        {
            Color c = incomingConnection.startColor;
            c.a = nodeSpawner.opacity;
            incomingConnection.startColor = c;
            incomingConnection.endColor = c;
        }
        if (outgoingConnection != null)
        {
            Color c = outgoingConnection.startColor;
            c.a = nodeSpawner.opacity;
            outgoingConnection.startColor = c;
            outgoingConnection.endColor = c;
        }

    }

    void RandomRadius()
	{
		radius = Random.Range (minRadius, maxRadius);
	}


    /// interpolates the dots opacity as well as its size
    public IEnumerator Fade(bool fadeIn)
    {
        float elapsedTime = 0f;
        float startScale = scale;
        float startOpacity = opacity;

        while (elapsedTime <= fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color c = sr.color;
            if (fadeIn)
            {
                scale = Mathf.SmoothStep(startScale, defaultLocalScale, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale, scale, 1);

                opacity = Mathf.SmoothStep(startOpacity, 1, (elapsedTime / fadeDuration));
            }
            else
            {
                scale = Mathf.SmoothStep(startScale, 0, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale, scale, 1);
                opacity = Mathf.SmoothStep(startOpacity, 0, (elapsedTime / fadeDuration));
            }

            c.a = opacity;
            sr.color = c;

            yield return null;
        }
        if (!fadeIn)
        {
            GameObject.Destroy(gameObject);
        }
        yield return null;
    }
}
