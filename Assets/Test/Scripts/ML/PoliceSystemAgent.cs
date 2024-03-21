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

    public float rewardForCatchingThief = 1.0f; // ������ ����� ���� ����
    public float penaltyForMissingThief = -0.5f; // ���� ���� �̺�Ʈ �߻� �� �г�Ƽ

    public Thief thief;
    public GameObject policeObject;
    public Text logText;

    private Vector3 eatenPosition = Vector3.zero;

    public void OnFeedEaten(Vector3 position)
    {
        // ���忡 �����ϴ� ���̰� ������ ȣ���. ���� ������ ��ġ
        eatenPosition = position;
        SetReward(penaltyForMissingThief);
        RequestDecision();
        //RequestAction();
    }

    public void OnThiefCaught()
    {
        SetReward(rewardForCatchingThief);
        //EndEpisode(); ���������� �����ϴ� �н��� �� �� �ֵ���, �н� ���� ����.
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
