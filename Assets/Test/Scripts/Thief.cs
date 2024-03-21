using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    public List<Vector3> path; // thief가 이동할 경로를 저장하는 리스트
    private int currentPointIndex = 0; // 현재 이동 목표지점의 인덱스
    public float moveSpeed = 4f; // thief의 이동 속도

    public PoliceSystemAgent policeSystemAgent;

    private Coroutine movePathCoroutine = null;

    public void Initialize()
    {
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
