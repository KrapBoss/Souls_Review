using UnityEngine;
using UnityEngine.UI;

public class FrozenOffButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameConfig.IsPc()) Destroy(gameObject);
        GetComponent<Button>().onClick.AddListener(GameManager.Instance.FrozenOffGame); 
    }
}
