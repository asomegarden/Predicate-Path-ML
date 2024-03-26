using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ThiefAgent : Agent
{
    public int[] boldnesses = { 0, 10, 30, 40 };
    private int boldness = 10;

    private int defaultPenalty = -20;

    public int currentPoint = 0;

    public WayPoint[] wayPoints;
    private WayPoint destinationWaypoint;

    public float moveSpeed = 4f;

    private void Awake()
    {
        Application.runInBackground = true;
    }

    public override void Initialize()
    {

    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(boldness); //대담성 추가

        foreach(var wayPoint in wayPoints) //방문지 정보 추가
        {
            Vector3 position = wayPoint.transform.localPosition;
            sensor.AddObservation(position.x);
            sensor.AddObservation(position.y);

            sensor.AddObservation(wayPoint.point);
            sensor.AddObservation(wayPoint.risk);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int wayPointIndex = actions.DiscreteActions[0];
        destinationWaypoint = wayPoints[wayPointIndex];

        StartCoroutine(MovePathCoroutine());
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-1, -1, 0);
        RequestDecision();
    }

    private IEnumerator MovePathCoroutine()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        Vector3 targetPosition = Vector3.zero;
        float remainDistance = 0f;

        targetPosition = destinationWaypoint.transform.localPosition;
        remainDistance = (targetPosition - this.transform.localPosition).sqrMagnitude;

        while (remainDistance > 0.01f)
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetPosition, moveSpeed * Time.fixedDeltaTime);
            remainDistance = (targetPosition - this.transform.localPosition).sqrMagnitude;
            yield return waitForFixedUpdate;
        }

        UseWayPoint();
    }

    private void UseWayPoint()
    {
        // 보상 계산
        AddReward(-destinationWaypoint.risk);
        AddReward(destinationWaypoint.point);

        RequestDecision();


        // 에피소드 종료
        if (destinationWaypoint == wayPoints[wayPoints.Length - 1])
        {
            AddReward(100);
            EndEpisode();
        }
    }
}
