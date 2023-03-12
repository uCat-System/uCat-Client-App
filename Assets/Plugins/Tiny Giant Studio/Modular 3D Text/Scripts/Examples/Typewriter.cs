using System.Collections;
using UnityEngine;

namespace MText
{
    [AddComponentMenu("Modular 3D Text/Extra/Typewriter")]
    public class Typewriter : MonoBehaviour
    {
        [TextArea]
        public string text = "Typewriter text";
        [Space(20)]

        public Modular3DText modular3DText = null;
        [SerializeField] bool startAutomatically = true;
        [SerializeField] float startDelay = 1;
        [Tooltip("Minimum and maximum possible speed.")]
        public Vector2 typeDelay = new Vector2(0.01f, 0.1f);
        [SerializeField] string typingSymbol = null;


        [Space(10)]
        [SerializeField] AudioClip typeSound = null;
        [Tooltip("Minimum and maximum possible volume. \nA variation of values makes it look natural.")]
        [SerializeField] Vector2 volume = Vector2.one;
        [SerializeField] AudioSource audioSource = null;

        ///If disabled while typing, this lets the script know it should start the routine again
        private bool typing = false;
        ///If typewriter is resumed, the effect resumes from this number letter.
        private int currentLetter;


        void Awake()
        {
            if (startAutomatically)
                modular3DText.Text = string.Empty;
        }

        void Start()
        {
            if (startAutomatically)
                StartTyping();
        }

        void OnEnable()
        {
            if (typing)
            {
                StopAllCoroutines();
                StartCoroutine(TypingRoutine());
            }
        }


        /// <summary>
        /// If gameobject is enabled, this starts a coroutine for the typewriter
        /// </summary>
        public void StartTyping()
        {
            StopAllCoroutines();
            StartCoroutine(FirstStart());
            typing = true;
        }

        IEnumerator FirstStart()
        {
            yield return null;
            yield return new WaitForSeconds(startDelay);
            StartCoroutine(TypingRoutine());
        }

        IEnumerator TypingRoutine()
        {
            if (modular3DText)
            {
                for (int i = currentLetter; i <= text.Length; i++)
                {
                    modular3DText.Text = (text.Substring(0, i) + typingSymbol);


                    yield return null;
                    yield return new WaitForSeconds(Random.Range(typeDelay.x, typeDelay.y));

                    if (audioSource && typeSound)
                    {
                        audioSource.pitch = Random.Range(0.9f, 1.1f);
                        audioSource.PlayOneShot(typeSound, Random.Range(volume.x, volume.y));
                    }
                    currentLetter = i;
                }
                typing = false;
            }
            else
            {
                Debug.Log("<color=red>No text object is selected on typewriter.</color> :" + gameObject.name, gameObject);
            }
        }
    }
}