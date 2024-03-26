using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PoliceSystemAgent : Agent
{
    public int episodeCount = 1;

    private float rewardForCatchingThief = 300f;
    private float penaltyForMissingThief = -10;

    public Thief thief;
    public GameObject policeObject;
    public Text logText;

    int step = 0;

    private Vector3 eatenPosition = Vector3.zero;


    private void Awake()
    {
        Application.runInBackground = true;
    }

    public void OnFeedEaten(Vector3 position)
    {
        eatenPosition = position;
        float xDistance = Mathf.Abs(eatenPosition.x - policeObject.transform.localPosition.x);
        float yDistance = Mathf.Abs(eatenPosition.y - policeObject.transform.localPosition.y);
        float distancePenalty = (xDistance + yDistance);
        float penalty = (penaltyForMissingThief - distancePenalty);
        SetReward(penalty - ++step);
        // 기본 패널티
        // 거리에 따른 패널티
        // 뒤 스탭으로 갈수록 패널티
        // 1:1:1 비율이 되기를 원함
        // 경로가 최대 18정도로 구성되어있고, 거리도 최대 20이기 때문에, 같은 비율을 가짐.
        // 기본 패널티는 그 절반정도인 10으로 구성
        // 그러면 다 합쳐서 50정도가 최대 패널티
        // 그러면 보상은 그 5배에서 10배정도로 구성해서 250~500으로 구성
        // 현재는 300으로 설정
        RequestDecision();
    }

    public void OnThiefCaught()
    {
        SetReward(rewardForCatchingThief);
        PrintLog($"catch");
    }

    public override void Initialize()
    {
        thief.Initialize();
        eatenPosition = Vector3.zero;
        policeObject.transform.localPosition = new Vector3(-2, -1, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(eatenPosition.x);
        sensor.AddObservation(eatenPosition.y);
        sensor.AddObservation((policeObject.transform.localPosition - eatenPosition).sqrMagnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int xPos = actions.DiscreteActions[0];
        int yPos = actions.DiscreteActions[1];

        policeObject.transform.localPosition = new Vector3(xPos, yPos, 0);
    }

    public override void OnEpisodeBegin()
    {
        step = 0;
        thief.OnEpisodeBegin();
        ClearLog();
        PrintLog($"start episode {episodeCount}");
        episodeCount++;
        eatenPosition = Vector3.zero;
        policeObject.transform.localPosition = new Vector3(-2, -1, 0);
    }

    private void PrintLog(string content)
    {
        logText.text += $"> {content}\n";
    }
    private void ClearLog()
    {
        logText.text = string.Empty;
    }
}
