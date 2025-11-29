using UnityEngine;
using UnityEngine.UI;

public class BookClickable : MonoBehaviour, IClickable
{
    [SerializeField] GameObject minigameBookPrefab;
    [SerializeField] private Transform conteinerCanva;

    public void OnClick() {
        Debug.Log("Book Clicked");
        Instantiate(minigameBookPrefab,conteinerCanva,false);
    }
}
