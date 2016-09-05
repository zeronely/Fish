using UnityEngine;
using System.Collections;

public class FishController : MonoBehaviour
{
    public GameObject[] FishList;
    public GameObject FishRoot; //鱼模型的挂载路径
    public GameObject BubblePs; //气泡粒子特效
    public int MaxBubblesEmitted = 5; //气泡粒子发射器的最大数量

    public float AreaWith = 0.96f;
    public float AreaHeigth= 0.67f;
    public float AreaDepth = 0.04f;
    public float ChildSpeedMultipler = 2;

    public float FishCounter = 30; //鱼的最大数量
    public float MinSpeed = 0.01f;
    public float MaxSpeed = 1;
    public AnimationCurve SpeedCurveMultiplier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1)); //鱼的动画曲线
    public float FishAcceleration;//鱼的加速度
    public float FishBrakePower;//鱼骤停能力

    public float MinAnimitonSpeed = 1.5f;
    public float MaxAnimationSpeed = 3.0f;

    public float PositionSphereWidth = 25;
    public float PositionSphereDepth = 5;
    public float PositionSphereHeigth = 5;

    public float MinRandomPositionTimer = 4;
    public float MaxRandomPositionTimer = 10;

    private float ChildSpeed;
    private int ActiveChildren;

    private Vector3 ChildBufferPos;

    private void AddFish()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void RandowWayPointTime()
    {

    }

    private void AutoRandowWayPointPosition()
    {

    }

    private void SetRandowWayPointPosition()
    {
        ChildSpeed = Random.Range(1, ChildSpeedMultipler);
        Vector3 vt = Vector3.zero;
        vt.x = Random.Range(-PositionSphereWidth, PositionSphereWidth) + transform.position.x;
        vt.z = Random.Range(-PositionSphereDepth, PositionSphereDepth) + transform.position.z;
        vt.y = Random.Range(-PositionSphereHeigth, PositionSphereHeigth) + transform.position.y;

        ChildBufferPos = vt;
    }

    private float RandomWaypointTime()
    {
        return Random.Range(MinRandomPositionTimer, MaxRandomPositionTimer);
    }




}
