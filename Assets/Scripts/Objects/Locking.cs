using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ڹ��� ������ �ϸ� �ش� 
public class Locking : ExpandObject
{
    public Animation[] step;// �ִϸ��̼� �ܰ�
    public float[] playTime;

    public Door door; // ���� ���� ȸ����

    AudioSource source;

    protected override void Init()
    {
        base.Init();

        door.isLocked = true;
        source = GetComponent<AudioSource>();
    }

    public override bool Func(string name = null)
    {
        if (!LibraryEvent.Activation)
        {
            Debug.Log("������ �̺�Ʈ�� Ȱ��ȭ���� �ʾҽ��ϴ�.");

            //���� ǥ��
            string line = LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation");
            UI.topUI.ShowNotice(line, false);

            return false;
        }

        //��ȣ�ۿ� ������Ʈ�� �̸��� ���ٸ�? ��ȣ�ۿ��� �մϴ�.
        if (TargetObjectName.Equals(name))
        {
            gameObject.layer = DataSet.Instance.Layers.Default;
            PlayerEvent.instance.DestroyGrabedItem();
            StartCoroutine(OpenCroutine());
            return true;
        }

        return false;
    }

    IEnumerator OpenCroutine()
    {
        source.Play();
        for (int i = 0; i <  step.Length; i++)
        {
            step[i].Play();
            yield return new WaitForSeconds(playTime[i]);
        }

        yield return new WaitForSeconds(1.0f);

        AudioManager.instance.PlayEffectiveSound("DeepAmbient",1.0f);

        door.isLocked = false;

        Destroy(step[0].gameObject);
        Destroy(step[1].gameObject);

        Destroy(this);
    }
}
