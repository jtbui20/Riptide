using UnityEngine;

public class SelectableObjectBehaviour : MonoBehaviour
{
    public int timesNeededToBeSelected = 5;
    public int timesCurrentlySelected = 0;

    public bool isFullySelected => timesCurrentlySelected >= timesNeededToBeSelected;

    public void OnSelected()
    {
        timesCurrentlySelected++;
        if (isFullySelected)
        {
            Debug.Log("Object fully selected: " + gameObject.name);
        }
    }
}
