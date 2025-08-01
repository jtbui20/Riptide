using System.Collections.Generic;
using UnityEngine;

public class CursorLoopLinePooler : MonoBehaviour
{
    public GameObject lineRendererPrefab;
    List<LineRenderer> lines = new List<LineRenderer>();

    public LineRenderer CreateNewLineRendererInstance()
    {
        LineRenderer newLine = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        return newLine;
    }
}
