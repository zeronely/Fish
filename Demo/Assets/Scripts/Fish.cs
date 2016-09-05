using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour
{
    public FishController spawner;
    private Vector3 wayPoint;
    [HideInInspector]
    public float speed = 10;
    private float stuckCounter; //被迫
    private float damping;
    private Transform model;
    private float targetSpeed;
    private float param;
    private float rotateCounterR;
    private float rotateCounterL;
    public Transform scannerTf;
    private bool isScanner;
    private bool isInstantiated;
    private static int updateNextSeed = 0;
    private int updateSeed = -1;
    
    void Start()
    {

    }

    void Update()
    {

    }

}
