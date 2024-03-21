using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    public List<Vector3> path; // thief�� �̵��� ��θ� �����ϴ� ����Ʈ
    private int currentPointIndex = 0; // ���� �̵� ��ǥ������ �ε���
    public float moveSpeed = 4f; // thief�� �̵� �ӵ�

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

    // ���Ǽҵ尡 ���۵� �� ȣ��Ǿ�� �ϴ� �޼���
    public void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(-1, -1, 0);
        currentPointIndex = 0; // �ε����� �ʱ�ȭ�Ͽ� ����� ���������� �ٽ� �̵�

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
