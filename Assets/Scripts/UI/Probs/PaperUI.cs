using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace CustomUI
{
    public class PaperUI : MonoBehaviour
    {
        [SerializeField] TMP_Text txt_content;  // �ؽ�Ʈ�� ������ ó���� ������Ʈ
        [SerializeField] Image[] image_backs;   // ��׶��� �̹����� ������ �κ�

        public void SetPaper(string _content, PaperBackgroundInfo[] _info = null)
        {
            txt_content.text = _content;

            //�̹��� Ȱ��ȭ
            if(_info !=null)
            {
                int i = 0;
                for(i = 0; i < _info.Length; i++)
                {
                    image_backs[i].enabled = true;
                    image_backs[i].sprite = _info[i].Sprite;
                    image_backs[i].color = _info[i].Color;
                    image_backs[i].rectTransform.sizeDelta = new Vector2(_info[i].SizeX, _info[i].SizeY);
                    image_backs[i].rectTransform.position = _info[i].RectPosition + new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    image_backs[i].rectTransform.rotation = Quaternion.Euler(0, 0, _info[i].RotateZ);
                }

                for(int j = image_backs.Length - 1; j >= i; j--) 
                {
                    image_backs[i].enabled = false;
                }
            }
        }
    }
}
