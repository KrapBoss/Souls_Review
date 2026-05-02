using UnityEngine;
using TMPro;
using CustomUI;

public class EnterPassword : MonoBehaviour
{
    public TMP_Text txt_right;
    public TMP_Text txt_left;

    int in_right, in_left;


    private void OnEnable()
    {
        in_right = 0;
        in_left = 0;

        txt_right.text = in_right.ToString();
        txt_left.text = in_left.ToString();
    }

    public void UpButton_Right()
    {
        in_right = (in_right + 1) % 10;
        txt_right.text = in_right.ToString();
        AudioManager.instance.PlayUISound("Select", 1.0f);
    }
    public void DownButton_Right()
    {
        in_right = (in_right - 1) < 0 ? 9 : (in_right - 1);
        txt_right.text = in_right.ToString();
        AudioManager.instance.PlayUISound("Select", 1.0f);
    }

    public void UpButton_Left()
    {
        in_left = (in_left + 1) % 10;
        txt_left.text = in_left.ToString();
        AudioManager.instance.PlayUISound("Select", 1.0f);
    }
    public void DownButton_Left()
    {
        in_left = (in_left - 1) < 0 ? 9 : (in_left - 1);
        txt_left.text = in_left.ToString();
        AudioManager.instance.PlayUISound("Select", 1.0f);
    }

    public void Button_DecidePassword()
    {
        UI.semiStaticUI.DecideEnterPassword(new int[] { in_left, in_right });
        AudioManager.instance.PlayUISound("Check", 1.0f);
    }
}
