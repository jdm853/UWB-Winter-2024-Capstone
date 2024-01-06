using UnityEngine;

/* This class is based on the implementation from https://awesometuts.com/blog/singletons-unity/ 
 * and https://www.youtube.com/watch?v=xppompv1DBg&ab_channel=Brackeys
*/
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // find the generic instance
                instance = FindObjectOfType<T>();

                // if it's null again create a new object
                // and attach the generic instance
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // create the instance
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(transform.root.gameObject);   // Don't destroy parent object. Allows our Singleton to persist across Unity Scenes.
            //Debug.Log("Singlton created for: " + instance.name);
        }
        else
        {
            Debug.Log("Error: Tried to create another instance of " + gameObject.name + "! Destroying the GameObject.");
            Destroy(gameObject);        // Destroy any other Gameobject that attempts to duplicate the Singleton
        }
    }
}
