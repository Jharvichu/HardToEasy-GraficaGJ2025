using UnityEngine;

public class BookClickable : MonoBehaviour, IClickable
{
    public void OnClick() {
        Debug.Log("Book Clicked");
    }
}
