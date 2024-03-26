using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ThiefAgent : Agent
{
    public int[] boldnesses = { 0, 5, 10, 15 };
    public int boldness = 10;

    public int currentPoint = 0;

    public WayPoint[] wayPoints;
    private WayPoint destinationWaypoint;
    private int wayPointIndex;

    public Text blodnessText;

    public float moveSpeed = 4f;

    public List<WayPoint> prevWayPoints = new List<WayPoint>();

    private void Awake()
    {
        Application.runInBackground = true;
    }

    public override void Initialize()
    {
        blodnessText.text = $"blodness {boldness}";
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(boldness); //대담성 추가
        sensor.AddObservation(currentPoint);
        sensor.AddObservation(wayPointIndex);

        foreach (var wayPoint in wayPoints) //방문지 정보 추가
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
        wayPointIndex = actions.DiscreteActions[0];
        destinationWaypoint = wayPoints[wayPointIndex];

        if (prevWayPoints.Contains(destinationWaypoint))
        {
            AddReward(-50); // 중복 방문지 패널티: 큼
            RequestDecision();
        }
        else
        {
            prevWayPoints.Add(destinationWaypoint);
            StartCoroutine(MovePathCoroutine());
        }
    }

    public override void OnEpisodeBegin()
    {
        prevWayPoints.Clear();
        //boldness = boldnesses[Random.Range(0, boldnesses.Length)];
        
        currentPoint = 0;
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
        // 보상 계산\
        // 고려할 파라미터 목록
        // 현재 위치와의 거리: 멀수록 패널티 (0~20)
        // 획득할 수 있는 포인트 (0~10) //이건 나중에 방문지 도착할 때 한번에 제공하는걸로.
        // 방문지 리스크 (0~20)
        // 방문지 리스크는 boldness에 따라 달라짐. 이때 boldness에 따라 선택이 크게 달라질 수 있어야함

        Vector3 wayPointPosition = destinationWaypoint.transform.localPosition;

        float xDistance = Mathf.Abs(wayPointPosition.x - transform.localPosition.x);
        float yDistance = Mathf.Abs(wayPointPosition.y - transform.localPosition.y);
        float distancePenalty = (xDistance + yDistance);

        float reward = 0f;
        reward -= distancePenalty;
        reward -= (boldness - destinationWaypoint.risk); //리스크를 인지하는 정도 완화

        currentPoint += destinationWaypoint.point;

        // 에피소드 종료
        if (destinationWaypoint == wayPoints[wayPoints.Length - 1])
        {
            AddReward(currentPoint);
            EndEpisode();
        }
        else
        {
            AddReward(reward);
            RequestDecision();
        }
    }
}
