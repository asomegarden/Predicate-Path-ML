
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    public PoliceSystemAgent agent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("thief"))
        {
            agent.OnThiefCaught();
        }else if (collision.CompareTag("cheese"))
        {
            Cheese cheese = collision.GetComponent<Cheese>();
            if (cheese.isActive)
            {
                cheese.isActive = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("cheese"))
        {
            Cheese cheese = collision.GetComponent<Cheese>();
            if (!cheese.isActive)
            {
                cheese.isActive = true;
            }
        }
    }
}
