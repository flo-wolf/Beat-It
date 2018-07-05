using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public static Teleporter instance;

    //The TeleporterDot that will be put as a child to the selected GridDot
    public GameObject teleporterDotPrefab;

    [Header("GridDots to teleport")]
    //Define dot0 of the Teleporter
    public GridDot tele0;
    //Define dot1 of the Teleporter
    public GridDot tele1;

    // Use this for initialization
    void Start()
    {
        instance = this;

        InitialTeleDotPositions();
    }

    //Takes the chosen GridDots in the Inspector and places the prefabs there
    void InitialTeleDotPositions()
    {
        GameObject teleDot0 = GameObject.Instantiate(teleporterDotPrefab, Vector2.zero, Quaternion.identity);
        GameObject teleDot1 = GameObject.Instantiate(teleporterDotPrefab, Vector2.zero, Quaternion.identity);

        teleDot0.transform.parent = tele0.transform;
        teleDot0.transform.localPosition = Vector3.zero;

        teleDot1.transform.parent = tele1.transform;
        teleDot1.transform.localPosition = Vector3.zero;
    }
}
