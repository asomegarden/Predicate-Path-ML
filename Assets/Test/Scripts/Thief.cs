using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thief : MonoBehaviour
{
    public Dictionary<int, List<Vector3>> pathDictionary;
    private List<Vector3> path;
    private int currentPointIndex = 0;
    public float moveSpeed = 4f;

    public Text pathText;
    public PoliceSystemAgent policeSystemAgent;

    private Coroutine movePathCoroutine = null;

    public void Initialize()
    {
        pathDictionary = new Dictionary<int, List<Vector3>>();
        pathDictionary.Add(0, new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 2, 0),
            new Vector3(2, 2, 0),
            new Vector3(2, 3, 0),
            new Vector3(3, 3, 0),
            new Vector3(3, 4, 0),
            new Vector3(4, 4, 0),
            new Vector3(4, 5, 0),
            new Vector3(5, 5, 0),
            new Vector3(5, 6, 0),
            new Vector3(6, 6, 0),
            new Vector3(6, 7, 0),
            new Vector3(7, 7, 0),
            new Vector3(7, 8, 0),
            new Vector3(8, 8, 0),
            new Vector3(8, 9, 0),
            new Vector3(9, 9, 0)
        });

        pathDictionary.Add(1, new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 2, 0),
            new Vector3(0, 3, 0),
            new Vector3(0, 4, 0),
            new Vector3(0, 5, 0),
            new Vector3(0, 6, 0),
            new Vector3(0, 7, 0),
            new Vector3(0, 8, 0),
            new Vector3(0, 9, 0),
            new Vector3(1, 9, 0),
            new Vector3(2, 9, 0),
            new Vector3(3, 9, 0),
            new Vector3(4, 9, 0),
            new Vector3(5, 9, 0),
            new Vector3(6, 9, 0),
            new Vector3(7, 9, 0),
            new Vector3(8, 9, 0),
            new Vector3(9, 9, 0)
        });

        pathDictionary.Add(2, new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(3, 0, 0),
            new Vector3(4, 0, 0),
            new Vector3(5, 0, 0),
            new Vector3(6, 0, 0),
            new Vector3(7, 0, 0),
            new Vector3(8, 0, 0),
            new Vector3(9, 0, 0),
            new Vector3(9, 1, 0),
            new Vector3(9, 2, 0),
            new Vector3(9, 3, 0),
            new Vector3(9, 4, 0),
            new Vector3(9, 5, 0),
            new Vector3(9, 6, 0),
            new Vector3(9, 7, 0),
            new Vector3(9, 8, 0),
            new Vector3(9, 9, 0)
        });

        this.transform.localPosition = new Vector3(-1, -1, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("cheese"))
        {
            Cheese cheese = collision.GetComponent<Cheese>();
            if (cheese.isActive)
            {
                policeSystemAgent.OnFeedEaten(collision.transform.localPosition);
            }
        }
    }

    // 에피소드가 시작될 때 호출되어야 하는 메서드
    public void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(-1, -1, 0);

        int pathId = Random.Range(0, pathDictionary.Keys.Count);
        pathText.text = $"Path {pathId + 1}";

        path = pathDictionary[pathId];
        currentPointIndex = 0; // 인덱스를 초기화하여 경로의 시작점부터 다시 이동

        if (movePathCoroutine != null) StopCoroutine(movePathCoroutine);
        movePathCoroutine = StartCoroutine(MovePathCoroutine());
    }

    private IEnumerator MovePathCoroutine()
    {
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        Vector3 targetPosition = Vector3.zero;
        float remainDistance = 0f;

        while (currentPointIndex < path.Count)
        {
            targetPosition = path[currentPointIndex++];
            remainDistance = (targetPosition - this.transform.localPosition).sqrMagnitude;

            while (remainDistance > 0.01f)
            {
                this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetPosition, moveSpeed * Time.fixedDeltaTime);
                remainDistance = (targetPosition - this.transform.localPosition).sqrMagnitude;
                yield return waitForFixedUpdate;
            }
        }

        policeSystemAgent.EndEpisode();
    }
}
