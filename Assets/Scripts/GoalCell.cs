using UnityEngine;

public class GoalCell : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            if (GameManager.isGameEnd == false)
            {
                GameManager.isGameEnd = true;
            }
        }
    }
}
