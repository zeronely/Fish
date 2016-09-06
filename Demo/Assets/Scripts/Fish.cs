using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour
{
    [HideInInspector]
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
    public Transform scanner;
    private bool isScanner;
    private bool isInstantiated;
    private static int updateNextSeed = 0;
    private int updateSeed = -1;

#if UNITYEDITOR
    static bool isWarning;
#endif

    void Start()
    {
        //Check if there is a controller attached
        if(scanner == null)
        {
            scanner = transform.FindChild("Scanner");
        }

        if (spawner)
        {
            SetRandomScale();
            LocateRequiredChildren();
            RandomizeStartAnimationFrame();
            SkewModelForLessUniformedMovement();
            speed = Random.Range(spawner.minSpeed, spawner.maxSpeed);
            Wander(0);
            SetRandomWaypoint();
            CheckForBubblesThenInvoke();
            isInstantiated = true;
            GetStartPos();
            FrameSkipSeedInit();
            spawner.activeChildren++;
            return;
        }

        this.enabled = false;
        Debug.Log(gameObject + " found no school to swim in: " + this + " disabled... Standalone fish not supported, please use the SchoolController");
    }

    void Update()
    {
        //Skip frames
        if (spawner.updateDivisor <= 1 || spawner.updateCounter == updateSeed)
        {
            CheckForDistanceToWaypoint();
            RotationBasedOnWaypointOrAvoidance();
            ForwardMovement();
            RayCastToPushAwayFromObstacles();
            SetAnimationSpeed();
        }
    }

    void FrameSkipSeedInit()
    {
        if (spawner.updateDivisor > 1)
        {
            int updateSeedCap = spawner.updateDivisor - 1;
            updateNextSeed++;
            this.updateSeed = updateNextSeed;
            updateNextSeed = updateNextSeed % updateSeedCap;
        }
    }

    void CheckForBubblesThenInvoke()
    {
        if (spawner.bubbles)
            InvokeRepeating("EmitBubbles", (spawner.bubbles.emitEverySecond * Random.value) + 1, spawner.bubbles.emitEverySecond);
    }

    void EmitBubbles()
    {
        spawner.bubbles.EmitBubbles(transform.position, speed);
    }

    void OnDisable()
    {
        CancelInvoke();
        spawner.activeChildren--;
    }

    void OnEnable()
    {
        if (isInstantiated)
        {
            CheckForBubblesThenInvoke();
            spawner.activeChildren++;
        }
    }

    void LocateRequiredChildren()
    {
        if (!model) model = transform.FindChild("Model");
        if (!scanner)
        {
            scanner = new GameObject().transform;
            scanner.transform.parent = this.transform;
            scanner.transform.localRotation = Quaternion.identity;
            scanner.transform.localPosition = Vector3.zero;
#if UNITYEDITOR
            if (!isWarning)
            {
                Debug.Log("No scanner assigned: creating... (Increase instantiate performance by manually creating a scanner object)");
                isWarning = true;
            }
#endif
        }
    }

    void SkewModelForLessUniformedMovement()
    {
        // Adds a slight rotation to the model so that the fish get a little less uniformed movement	
        Quaternion rx = new Quaternion();
        rx.eulerAngles = new Vector3(0, 0, Random.Range(-25, 25));
        model.transform.rotation = rx;
    }

    void SetRandomScale()
    {
        float sc = Random.Range(spawner.minScale, spawner.maxScale);
        transform.localScale = Vector3.one * sc;
    }

    void RandomizeStartAnimationFrame()
    {
        //
        Animation animtion = model.GetComponent<Animation>();
        if (animtion != null)
        {
            foreach (AnimationState state in animtion)
            {
                state.speed = Random.value * state.length;
            }
        }
    }

    void GetStartPos()
    {
        RaycastHit hit;
        if (Physics.Raycast(spawner.transform.position, wayPoint, out hit, (int)Vector3.Distance(transform.position, wayPoint)))
        {
            transform.position = hit.point;
            return;
        }
        //-Vector is to avoid zero rotation warning
        transform.position = wayPoint - new Vector3(0.1f, 0.1f, 0.1f);
    }

    Vector3 findWaypoint()
    {
        Vector3 t;
        t.x = Random.Range(-spawner.spawnSphere, spawner.spawnSphere) + spawner.posBuffer.x;
        t.z = Random.Range(-spawner.spawnSphereDepth, spawner.spawnSphereDepth) + spawner.posBuffer.z;
        t.y = Random.Range(-spawner.spawnSphereHeight, spawner.spawnSphereHeight) + spawner.posBuffer.y;
        return t;
    }

    //Uses scanner to push away from obstacles
    void RayCastToPushAwayFromObstacles()
    {
        if (spawner.push)
        {
            RotateScanner();
            RayCastToPushAwayFromObstaclesCheckForCollision();
        }
    }

    void RayCastToPushAwayFromObstaclesCheckForCollision()
    {
        RaycastHit hit;
        float d;
        Vector3 cacheForward = scanner.forward;
        if (Physics.Raycast(transform.position, cacheForward, out hit, (int)spawner.pushDistance))
        {
            Fish s;
            s = hit.transform.GetComponent<Fish>();
            d = (spawner.pushDistance - hit.distance) / spawner.pushDistance;   // Equals zero to one. One is close, zero is far	
            if (s)
            {
                transform.position -= cacheForward * spawner.newDelta * d * spawner.pushForce;
            }
            else
            {
                speed -= 0.01f * spawner.newDelta;
                if (speed < 0.1f)
                    speed = 0.1f;
                transform.position -= cacheForward * spawner.newDelta * d * spawner.pushForce * 2;
                //Tell scanner to rotate slowly
                isScanner = false;
            }
        }
        else
        {
            //Tell scanner to rotate randomly
            isScanner = true;
        }
    }

    void RotateScanner()
    {
        //Scan random if not pushing
        if (isScanner)
        {
            scanner.rotation = Random.rotation;
            return;
        }
        //Scan slow if pushing
        scanner.Rotate(new Vector3(150 * spawner.newDelta, 0, 0));
    }

    bool Avoidance()
    {
        //Avoidance () - Returns true if there is an obstacle in the way
        if (!spawner.avoidance)
            return false;
        RaycastHit hit;
        float d;
        Quaternion rx = transform.rotation;
        Vector3 ex = transform.rotation.eulerAngles;
        Vector3 cacheForward = transform.forward;
        Vector3 cacheRight = transform.right;
        //Up / Down avoidance
        if (Physics.Raycast(transform.position, -Vector3.up + (cacheForward * 0.1f), out hit, (int)spawner.avoidDistance))
        {
            //Debug.DrawLine(transform.position,hit.point);
            d = (spawner.avoidDistance - hit.distance) / spawner.avoidDistance;
            ex.x -= spawner.avoidSpeed * d * spawner.newDelta * (speed + 1);
            rx.eulerAngles = ex;
            transform.rotation = rx;
        }
        if (Physics.Raycast(transform.position, Vector3.up + (cacheForward * 0.1f), out hit, (int)spawner.avoidDistance))
        {
            //Debug.DrawLine(transform.position,hit.point);
            d = (spawner.avoidDistance - hit.distance) / spawner.avoidDistance;
            ex.x += spawner.avoidSpeed * d * spawner.newDelta * (speed + 1);
            rx.eulerAngles = ex;
            transform.rotation = rx;
        }

        //Crash avoidance //Checks for obstacles forward
        if (Physics.Raycast(transform.position, cacheForward + (cacheRight * Random.Range(-0.1f, 0.1f)), out hit, (int)spawner.stopDistance))
        {
            //					Debug.DrawLine(transform.position,hit.point);
            d = (spawner.stopDistance - hit.distance) / spawner.stopDistance;
            ex.y -= spawner.avoidSpeed * d * spawner.newDelta * (targetSpeed + 3);
            rx.eulerAngles = ex;
            transform.rotation = rx;
            speed -= d * spawner.newDelta * spawner.stopSpeedMultiplier * speed;
            if (speed < 0.01f)
            {
                speed = 0.01f;
            }
            return true;
        }
        else if (Physics.Raycast(transform.position, cacheForward + (cacheRight * (spawner.avoidAngle + rotateCounterL)), out hit, (int)spawner.avoidDistance))
        {
            //				Debug.DrawLine(transform.position,hit.point);
            d = (spawner.avoidDistance - hit.distance) / spawner.avoidDistance;
            rotateCounterL += 0.1f;
            ex.y -= spawner.avoidSpeed * d * spawner.newDelta * rotateCounterL * (speed + 1);
            rx.eulerAngles = ex;
            transform.rotation = rx;
            if (rotateCounterL > 1.5f)
                rotateCounterL = 1.5f;
            rotateCounterR = 0;
            return true;
        }
        else if (Physics.Raycast(transform.position, cacheForward + (cacheRight * -(spawner.avoidAngle + rotateCounterR)), out hit, (int)spawner.avoidDistance))
        {
            //			Debug.DrawLine(transform.position,hit.point);
            d = (spawner.avoidDistance - hit.distance) / spawner.avoidDistance;
            if (hit.point.y < transform.position.y)
            {
                ex.y -= spawner.avoidSpeed * d * spawner.newDelta * (speed + 1);
            }
            else
            {
                ex.x += spawner.avoidSpeed * d * spawner.newDelta * (speed + 1);
            }
            rotateCounterR += 0.1f;
            ex.y += spawner.avoidSpeed * d * spawner.newDelta * rotateCounterR * (speed + 1);
            rx.eulerAngles = ex;
            transform.rotation = rx;
            if (rotateCounterR > 1.5)
                rotateCounterR = 1.5f;
            rotateCounterL = 0;
            return true;
        }
        else
        {
            rotateCounterL = 0;
            rotateCounterR = 0;
        }
        return false;
    }

    void ForwardMovement()
    {
        transform.position += transform.TransformDirection(Vector3.forward) * speed * spawner.newDelta;
        if (param < 1)
        {
            if (speed > targetSpeed)
            {
                param += spawner.newDelta * spawner.acceleration;
            }
            else
            {
                param += spawner.newDelta * spawner.brake;
            }
            speed = Mathf.Lerp(speed, targetSpeed, param);
        }
    }

    void RotationBasedOnWaypointOrAvoidance()
    {
        Quaternion rotation;
        rotation = Quaternion.LookRotation(wayPoint - transform.position);
        if (!Avoidance())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, spawner.newDelta * damping);
        }
        //Limit rotation up and down to avoid freaky behavior
        float angle = transform.localEulerAngles.x;
        angle = (angle > 180) ? angle - 360 : angle;
        Quaternion rx = transform.rotation;
        Vector3 rxea = rx.eulerAngles;
        rxea.x = ClampAngle(angle, -50.0f, 50.0f);
        rx.eulerAngles = rxea;
        transform.rotation = rx;
    }

    void CheckForDistanceToWaypoint()
    {
        if ((transform.position - wayPoint).magnitude < spawner.waypointDistance + stuckCounter)
        {
            Wander(0);  //create a new waypoint
            stuckCounter = 0;
            CheckIfThisShouldTriggerNewFlockWaypoint();
            return;
        }
        stuckCounter += spawner.newDelta * (spawner.waypointDistance * 0.25f);
    }

    void CheckIfThisShouldTriggerNewFlockWaypoint()
    {
        if (spawner.childTriggerPos)
        {
            spawner.SetRandomWaypointPosition();
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    void SetAnimationSpeed()
    {
        Animator animtor = model.GetComponent<Animator>();
        if (animtor != null)
        {
            animtor.SetTrigger("Swim");
            animtor.speed = (Random.Range(spawner.minAnimationSpeed, spawner.maxAnimationSpeed) * spawner.schoolSpeed * this.speed);
        }
        //

        Animation animtion = model.GetComponent<Animation>();
        if(animtion != null)
        {
            foreach (AnimationState state in model.GetComponent<Animation>())
            {
                state.speed = (Random.Range(spawner.minAnimationSpeed, spawner.maxAnimationSpeed) * spawner.schoolSpeed * this.speed) + 0.1f;
            }
        }
    }

    public void Wander(float delay)
    {
        damping = Random.Range(spawner.minDamping, spawner.maxDamping);
        targetSpeed = Random.Range(spawner.minSpeed, spawner.maxSpeed) * spawner.SpeedCurveMultiplier.Evaluate(Random.value) * spawner.schoolSpeed;
        Invoke("SetRandomWaypoint", delay);
    }

    void SetRandomWaypoint()
    {
        param = 0;
        wayPoint = findWaypoint();
    }
}
