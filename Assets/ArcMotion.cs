using UnityEngine;

public class ArcMotion : MonoBehaviour
{
    public Transform target;     // The target object to move toward.
    public float arcHeight = 5f; // Height of the arc.
    public float speed = 5f;     // Speed of the projectile.

    private Vector3 startPos;    // Initial position of the projectile.
    private Vector3 targetPos;   // Target position.
    private float journeyLength; // Total distance to the target.
    private float startTime;     // Time when the motion started.

    public bool moving;


    void Start()
    {
        startPos = transform.position;
        targetPos = target.position;
        journeyLength = Vector3.Distance(startPos, targetPos);
        startTime = Time.time;
    }

    void Update()
    {

         if (Input.GetKeyDown(KeyCode.Space))
        {
            moving = !moving;
        }

         if (target != null && moving)
        {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
        currentPos.y += Mathf.Sin(fractionOfJourney * Mathf.PI) * arcHeight;

        transform.position = currentPos;
           
        }

    }
}
