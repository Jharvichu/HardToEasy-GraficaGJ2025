using UnityEngine;

public class StaplerClickable : MonoBehaviour, IClickable
{
    public void OnClick() {
        Debug.Log("Stapler Clicked");
    }
}
