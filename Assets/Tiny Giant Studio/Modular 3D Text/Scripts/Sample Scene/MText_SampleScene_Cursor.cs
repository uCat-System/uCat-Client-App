/// Created by Ferdowsur Asif @ Tiny Giant Studios
/// This code was made with the purpose of demonstration only for sample scene
/// Not optimized and not intended to be used with real projects
/// Like using camera.man and checking name string == are bad practices

using UnityEngine;

namespace MText
{
    public class MText_SampleScene_Cursor : MonoBehaviour
    {
        [SerializeField] Transform crosshair = null;
        [SerializeField] float rotationSpeed = 0.1f;
        [SerializeField] ParticleSystem hitEffect = null;
        [SerializeField] StatusToolTip statusToolTip = null;

        void Start()
        {
            Cursor.visible = false;
        }

        void Update()
        {
            if (!crosshair)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Note: Using camera.main in update is bad practice

            if (Physics.Raycast(ray, out RaycastHit hit, 1000))
            {
                crosshair.position = hit.point;

                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.transform.gameObject.name == "Target")
                    {
                        int damage = Random.Range(1, 50);
                        //reminder
                        //this is just for demonstration, in the sample scene. 
                        int style = 0;
                        if (hit.transform.position.x > 0) style = 1;
                        else if (hit.transform.position.x < 0) style = 2;
                        statusToolTip.ShowToolTip("-" + damage.ToString(), style, hit.point, Quaternion.Euler(0, 0, 0), true);

                        float currentHealth = hit.transform.GetChild(0).gameObject.GetComponent<MText_UI_Slider>().Value - damage;
                        if (currentHealth < 0) currentHealth = 0;
                        hit.transform.GetChild(0).gameObject.GetComponent<MText_UI_Slider>().UpdateValue(currentHealth);

                        hitEffect.transform.position = hit.point;
                        hitEffect.Play();
                    }
                }
            }

            crosshair.eulerAngles += new Vector3(0, rotationSpeed, 0);
        }
    }
}