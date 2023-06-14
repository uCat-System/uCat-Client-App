using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    public class MText_SampleScene_ListTest : MonoBehaviour
    {
        [SerializeField] List<Transform> lists = new List<Transform>();
        int selectedList = 0;
        [SerializeField] int speed = 1;
        float startTime = 0;
        float distance = 0;
        Vector3 targetPos = Vector3.zero;
        Vector3 startPos = Vector3.zero;

        void Update()
        {
            if (distance == 0)
                return;

            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / distance;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);
        }

        public void Next()
        {
            selectedList++;
            if (selectedList >= lists.Count)
                selectedList = 0;

            GetPosition();
        }
        public void Previous()
        {
            selectedList--;
            if (selectedList < 0)
                selectedList = lists.Count - 1;

            GetPosition();
        }
        void GetPosition()
        {
            targetPos = new Vector3(lists[selectedList].position.x, 0, 0);
            startPos = transform.position;

            // Keep a note of the time the movement started.
            startTime = Time.time;

            // Calculate the journey length.
            distance = Vector3.Distance(startPos, targetPos);
        }
    }
}