using UnityEngine;

namespace MText
{
    [DisallowMultipleComponent]
    public class MText_UI_Toggle : MonoBehaviour
    {
        [SerializeField]
        private bool _isOn = true;
        public bool IsOn
        {
            get { return _isOn; }
            set { _isOn = value; VisualUpdate(); }
        }
        public GameObject activeGraphic;
        public GameObject inactiveGraphic;

        /// <summary> 
        /// Sets the activate state according to the parameter passed.
        /// </summary>
        public void Set(bool set)
        {
            IsOn = set;
            VisualUpdate();
        }
        /// <summary> 
        /// Switches between on and off.
        /// </summary>
        public void Toggle()
        {
            IsOn = !IsOn;
            VisualUpdate();
        }

        /// <summary>
        /// Updates the visual of the Toggle to match the 'isOn' variable
        /// </summary>
        public void VisualUpdate()
        {
            if (IsOn) ActiveVisualUpdate();
            else InactiveVisualUpdate();
        }

        /// <summary> 
        /// Changes the graphic to activated.
        /// <para>This only changes the visual. Doesn't update the "active" bool</para>
        /// </summary>
        public void ActiveVisualUpdate()
        {
            ToggleGraphic(inactiveGraphic, false);
            ToggleGraphic(activeGraphic, true);
        }


        /// <summary> 
        /// Changes the graphic to activated.
        /// <para>This only changes the visual. Doesn't update the "active" bool</para>
        /// </summary>
        public void InactiveVisualUpdate()
        {
            ToggleGraphic(inactiveGraphic, true);
            ToggleGraphic(activeGraphic, false);
        }

        private void ToggleGraphic(GameObject graphic, bool enable)
        {
            if (graphic)
                graphic.SetActive(enable);
        }


        /// <summary> 
        /// Editor only. Adds the toggle event to attached button.
        /// <para>Used by menu item when creating a toggle gameobject</para>
        /// </summary>
#if UNITY_EDITOR
        public void AddEventToButton()
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(GetComponent<MText_UI_Button>().onClick, delegate { Toggle(); });
        }
#endif
    }
}