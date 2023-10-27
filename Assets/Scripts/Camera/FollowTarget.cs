using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;  // Reference to the target GameObject (set in the Unity Inspector)
    public float speed = 5.0f;  // Speed at which the follower moves

    private Vector3 startPos;

    public bool moving;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moving = !moving;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = startPos;
        }

        if (target != null && moving)
        {
            // Calculate the direction from the follower to the target
            Vector3 direction = target.position - transform.position;

            // Normalize the direction vector to get a unit vector
            direction.Normalize();

            // Move the follower towards the target
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
