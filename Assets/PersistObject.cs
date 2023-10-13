using UnityEngine;

public class PersistObject : MonoBehaviour
{
    private static PersistObject instance;

    private void Awake()
    {
        if (instance == null)
        {
            // This is the first instance, so make it persist through scene changes.
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            PersistChildren();
        }
        else if (instance != this)
        {
            // This is a duplicate, so destroy it.
            Destroy(this.gameObject);
        }
    }

    private void PersistChildren()
    {
        foreach (Transform child in transform)
        {
            PersistObject childPersistScript = child.GetComponent<PersistObject>();
            if (childPersistScript != null)
            {
                // Call Awake recursively on children with the PersistObject script.
                childPersistScript.Awake();
            }
        }
    }
}
