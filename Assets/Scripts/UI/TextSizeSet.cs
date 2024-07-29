using TMPro;
using UnityEngine;

public class TextSizeSet : MonoBehaviour
{
    public float size_Default;
    public float size_Mobile;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TMP_Text>().fontSize = GameConfig.IsPc() ? size_Default : size_Mobile;
    }
}
