using UnityEngine;

namespace MText
{
    public class MText_SampleScene_Announcement : MonoBehaviour
    {
        [SerializeField] string announcement = null;
        [SerializeField] Modular3DText modular3DText = null;
        [SerializeField] Animator animator = null;
        [SerializeField] ParticleSystem myParticleSystem = null;

        void Start()
        {
            animator.SetTrigger("Open");
            myParticleSystem.Play();
            Invoke(nameof(UpdateText), 1.5f);
        }

        void UpdateText()
        {
            modular3DText.UpdateText(announcement);
        }

    }
}