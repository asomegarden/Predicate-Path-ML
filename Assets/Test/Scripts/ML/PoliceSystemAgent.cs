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
        // �⺻ �г�Ƽ
        // �Ÿ��� ���� �г�Ƽ
        // �� �������� ������ �г�Ƽ
        // 1:1:1 ������ �Ǳ⸦ ����
        // ��ΰ� �ִ� 18������ �����Ǿ��ְ�, �Ÿ��� �ִ� 20�̱� ������, ���� ������ ����.
        // �⺻ �г�Ƽ�� �� ���������� 10���� ����
        // �׷��� �� ���ļ� 50������ �ִ� �г�Ƽ
        // �׷��� ������ �� 5�迡�� 10�������� �����ؼ� 250~500���� ����
        // ����� 300���� ����
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
