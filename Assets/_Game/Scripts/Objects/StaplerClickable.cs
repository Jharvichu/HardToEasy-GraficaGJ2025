using UnityEngine;

public class StaplerClickable : MonoBehaviour, IClickable
{
    [SerializeField] private GameObject minigameStaplerPrefab;
    [SerializeField] private Transform conteinerCanva;

    public void OnClick() {
        Debug.Log("Stapler Clicked");
        Instantiate(minigameStaplerPrefab, conteinerCanva, false);
    }
}
