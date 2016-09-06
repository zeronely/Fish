using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishController : MonoBehaviour
{
    public Fish[] childPrefabs;
    public Transform groupTransform;
    public bool groupChildToNewTransform;
    public string groupName = "";              // Name of group (if no name, the school name will be used)
    public bool groupChildToSchool;            // Parents fish transform to school transform
    public int childAmount = 30;              // Number of objects
    public float spawnSphere = 3;              // Range around the spawner waypoints will created //changed to box
    public float spawnSphereDepth = 3;
    public float spawnSphereHeight = 1.5f;
    public float childSpeedMultipler = 2;     // Adjust speed of entire school
    public float minSpeed = 6;                 // minimum random speed 
    public float maxSpeed = 10;
    public AnimationCurve SpeedCurveMultiplier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1)); //鱼的动画曲线
    public float minScale = 0.7f;                // minimum random size
    public float maxScale = 1;                 // maximum random size
    public float minDamping = 1;               // Rotation tween damping, lower number = smooth/slow rotation (if this get stuck in a loop, increase this value)
    public float maxDamping = 2;
    public float waypointDistance = 1;         // How close this can get to waypoint before creating a new waypoint (also fixes stuck in a loop)
    public float minAnimationSpeed = 2;
    public float maxAnimationSpeed = 4;
    public float randomPositionTimerMax = 10;  // When _autoRandomPosition is enabled
    public float randomPositionTimerMin = 4;
    public float acceleration = 0.025f;            // How fast child speeds up
    public float brake = 0.01f;                    // How fast child slows down 
    public float positionSphere = 25;          // If _randomPositionTimer is bigger than zero the controller will be moved to a random position within this sphere
    public float positionSphereDepth = 5;      // Overides height of sphere for more controll
    public float positionSphereHeight = 5;     // Overides height of sphere for more controll
    public bool childTriggerPos;           // Runs the random position function when a child reaches the controller
    public bool forceChildWaypoints;       // Forces all children to change waypoints when this changes position
    public bool autoRandomPosition;            // Automaticly positions waypoint based on random values (_randomPositionTimerMin, _randomPositionTimerMax)
    public float forcedRandomDelay = 1.5f;     // Random delay added before forcing new waypoint
    public float schoolSpeed;

    public List<Fish> roamers;
    public Vector3 posBuffer;

    ///AVOIDANCE
    public bool avoidance;              //Enable/disable avoidance
    public float avoidAngle = 0.35f;        //Angle of the rays used to avoid obstacles left and right
    public float avoidDistance = 1;     //How far avoid rays travel
    public float avoidSpeed = 75;           //How fast this turns around when avoiding	
    public float stopDistance = 0.5f;       //How close this can be to objects directly in front of it before stopping and backing up. This will also rotate slightly, to avoid "robotic" behaviour
    public float stopSpeedMultiplier = 2;

    ///PUSH
    public bool push;
    public float pushDistance;              //How far away obstacles can be before starting to push away	
    public float pushForce = 5;

    private Vector3 ChildBufferPos;

    //气泡特效
    public Bubbles bubbles;

    //FRAME SKIP
    public int updateDivisor = 1;
    public float newDelta;
    public int updateCounter;
    public int activeChildren;



    void Start()
    {
        posBuffer = transform.position;
        schoolSpeed = Random.Range(1, childSpeedMultipler);
        AddFish(childAmount);
        Invoke("AutoRandomWaypointPosition", RandomWaypointTime());
    }

    void Update()
    {
        if (activeChildren > 0)
        {
            if (updateDivisor > 1)
            {
                updateCounter++;
                updateCounter = updateCounter % updateDivisor;
                newDelta = Time.deltaTime * updateDivisor;
            }
            else
            {
                newDelta = Time.deltaTime;
            }
            UpdateFishAmount();
        }
    }

    void InstantiateGroup()
    {
        if (groupTransform) return;
        GameObject g = new GameObject();
        groupTransform = g.transform;
        groupTransform.position = transform.position;
        if (groupName != "")
        {
            g.name = groupName;
            return;
        }
        g.name = transform.name + " Fish Container";
    }

    void AddFish(int amount)
    {
        if (groupChildToNewTransform) InstantiateGroup();
        for (int i = 0; i < amount; i++)
        {
            int child = Random.Range(0, childPrefabs.Length);
            Fish obj = Instantiate(childPrefabs[child]);
            obj.spawner = this;
            roamers.Add(obj);
            AddChildToParent(obj.transform);
        }
    }

    void AddChildToParent(Transform obj)
    {
        if (groupChildToSchool)
        {
            obj.parent = transform;
            return;
        }
        if (groupChildToNewTransform)
        {
            obj.parent = groupTransform;
            return;
        }
    }

    void RemoveFish(int amount)
    {
        Fish dObj = roamers[roamers.Count - 1];
        roamers.RemoveAt(roamers.Count - 1);
        Destroy(dObj.gameObject);
    }

    void UpdateFishAmount()
    {
        if (childAmount >= 0 && childAmount < roamers.Count)
        {
            RemoveFish(1);
            return;
        }
        if (childAmount > roamers.Count)
        {
            AddFish(1);
            return;
        }
    }

    //Set waypoint randomly inside box
    public void SetRandomWaypointPosition()
    {
        schoolSpeed = Random.Range(1, childSpeedMultipler);
        Vector3 t;
        t.x = Random.Range(-positionSphere, positionSphere) + transform.position.x;
        t.z = Random.Range(-positionSphereDepth, positionSphereDepth) + transform.position.z;
        t.y = Random.Range(-positionSphereHeight, positionSphereHeight) + transform.position.y;
        posBuffer = t;
        if (forceChildWaypoints)
        {
            for (int i = 0; i < roamers.Count; i++)
            {
                (roamers[i] as Fish).Wander(Random.value * forcedRandomDelay);
            }
        }
    }

    void AutoRandomWaypointPosition()
    {
        if (autoRandomPosition && activeChildren > 0)
        {
            SetRandomWaypointPosition();
        }
        CancelInvoke("AutoRandomWaypointPosition");
        Invoke("AutoRandomWaypointPosition", RandomWaypointTime());
    }

    float RandomWaypointTime()
    {
        return Random.Range(randomPositionTimerMin, randomPositionTimerMax);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying && posBuffer != transform.position) posBuffer = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(posBuffer, new Vector3(spawnSphere * 2, spawnSphereHeight * 2, spawnSphereDepth * 2));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3((positionSphere * 2) + spawnSphere * 2, (positionSphereHeight * 2) + spawnSphereHeight * 2, (positionSphereDepth * 2) + spawnSphereDepth * 2));
    }




}
