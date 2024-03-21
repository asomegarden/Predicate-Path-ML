using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class PoliceSystemAgent : Agent
{
    public GameObject policeObject;

    public float rewardForCatchingThief = 1.0f; // 도둑을 잡았을 때의 보상
    public float penaltyForMissingThief = -0.5f; // 먹이 먹힘 이벤트 발생 시 패널티

    public Thief thief;

    public int episodeCount = 1;

    private Vector3 eatenPosition = Vector3.zero;
    public void OnFeedEaten(Vector3 position)
    {
        // 월드에 존재하는 먹이가 먹히면 호출됨. 먹힌 먹이의 위치
        eatenPosition = position;
        SetReward(penaltyForMissingThief);
        RequestDecision();
        //RequestAction();
    }

    public void OnThiefCaught()
    {
        SetReward(rewardForCatchingThief);
        //EndEpisode(); 지속적으로 추적하는 학습을 할 수 있도록, 학습 종료 안함.
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
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int xPos = actions.DiscreteActions[0];
        int yPos = actions.DiscreteActions[1];

        policeObject.transform.localPosition = new Vector3(xPos, yPos, 0);
    }

    public override void OnEpisodeBegin()
    {
        thief.OnEpisodeBegin();
        eatenPosition = Vector3.zero;
        policeObject.transform.localPosition = new Vector3(-1, -1, 0);
    }
}
