using UnityEngine;

public class SelectableObjectBehaviour : MonoBehaviour
{
    public int timesNeededToBeSelected = 5;
    public int timesCurrentlySelected = 0;
    void Start()
    {

    }
    void Update()
    {

    }

    public void OnSelected()
    {
        timesCurrentlySelected++;
        if (timesCurrentlySelected >= timesNeededToBeSelected)
        {
            Debug.Log("Object fully selected: " + gameObject.name);
        }
    }
}
