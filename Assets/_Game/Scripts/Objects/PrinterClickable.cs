using UnityEngine;

public class PrinterClickable : MonoBehaviour, IClickable
{
    public void OnClick() {
        Debug.Log("Printer clicked!");
    }
}
