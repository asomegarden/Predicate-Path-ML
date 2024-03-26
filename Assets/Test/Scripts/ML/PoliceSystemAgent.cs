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

    private float rewardForCatchingThief = 10f; // ?????? ?????? ???? ????
    private float penaltyForMissingThief = -1.0f; // ???? ???? ?????? ???? ?? ??????

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
        // ?????? ???????? ?????? ?????? ??????. ???? ?????? ????
        eatenPosition = position;
        float xDistance = Mathf.Abs((int)(eatenPosition.x - policeObject.transform.localPosition.x));
        float yDistance = Mathf.Abs((int)(eatenPosition.y - policeObject.transform.localPosition.y));
        float distanceReward = (xDistance + yDistance) / 20;
        float penalty = (penaltyForMissingThief - distanceReward) * 10;
        SetReward(penalty - ++step);
        RequestDecision();
        //RequestAction();
    }

    public void OnThiefCaught()
    {
        SetReward(rewardForCatchingThief);
        //EndEpisode(); ?????????? ???????? ?????? ?? ?? ??????, ???? ???? ????.
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
