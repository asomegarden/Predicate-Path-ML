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
    public Text logText;

    public GameObject[] policeObjects;

    private Vector2 prevPosition = Vector2.zero;

    int step = 0;

    private Vector3 eatenPosition = Vector3.zero;


    private void Awake()
    {
        Application.runInBackground = true;
    }

    public void OnFeedEaten(Vector3 position)
    {
        prevPosition = eatenPosition;
        eatenPosition = position;

            float xDistance = 0; 
            float yDistance = 0; 

            foreach (var policeObject in policeObjects)
            {
                xDistance += Mathf.Abs(eatenPosition.x - policeObject.transform.localPosition.x);
                yDistance += Mathf.Abs(eatenPosition.y - policeObject.transform.localPosition.y);
            }

            xDistance /= policeObjects.Length;
            yDistance /= policeObjects.Length;

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

        //RequestDecision();
    }

    public override void Initialize()
    {
        thief.Initialize();
        eatenPosition = Vector3.zero;
        prevPosition = Vector3.zero;
        foreach (var policeObject in policeObjects) policeObject.transform.localPosition = new Vector3(-2, -1, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 direction = Vector3.zero;

        if(step != 1) direction = (prevPosition - (Vector2)eatenPosition).normalized;

        sensor.AddObservation(direction.x);
        sensor.AddObservation(direction.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        for (int i = 0; i < policeObjects.Length; i++)
        {
            int xPos = actions.DiscreteActions[i];
            int yPos = actions.DiscreteActions[i+1];

            Vector2 movePosition = new Vector3(xPos - 5 + eatenPosition.x, yPos - 5 + eatenPosition.y, 0);

            //Clamp ����
            if (movePosition.x > 9)
            {
                movePosition.x = 9;
            }
            else if (movePosition.x < 0)
            {
                movePosition.x = 0;
            }

            if (movePosition.y > 9)
            {
                movePosition.y = 9;
            }
            else if (movePosition.y < 0)
            {
                movePosition.y = 0;
            }

            policeObjects[i].transform.localPosition = movePosition;
        }
    }

    public override void OnEpisodeBegin()
    {
        step = 0;

        thief.OnEpisodeBegin();
        ClearLog();
        PrintLog($"start episode {episodeCount}");
        episodeCount++;
        eatenPosition = Vector3.zero;

        foreach(var policeObject in policeObjects) 
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
