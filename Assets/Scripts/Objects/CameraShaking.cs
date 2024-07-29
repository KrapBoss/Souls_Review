using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//ī�޶� ���� �ִ� �ڽ� ������Ʈ
public class CameraShaking : MonoBehaviour
{
    

    float _t, _force;//������ �ð��� ������.
    IEnumerator ShakeCroutine = null;
    public void Shake(float t, float force)
    {
        if(ShakeCroutine != null)//���ο� �Է��� ������ ��, ����ũ ������ �ʱ�ȭ��Ų��.
        {
            //�ð��� ���� ���� �Է� �ð��� �� ũ�ٸ� ����ð��� ����.
            if (_t < t)
            {
                _t = t;
                _force = force;
            }
            else if(_force > force)//�������Ⱑ �� ũ�⸸ �ϴٸ�, �������⸸�� �������ش�.
            {
                _force = force;
            }

            return;
        }

        ShakeCroutine = CameraShakeCroutine(t, force);
        StartCoroutine(ShakeCroutine);
    }

    IEnumerator CameraShakeCroutine(float t, float force)
    {
        _t = t;
        _force = force;
        while (_t > 0.0f)
        {
            float x = Random.Range(-_force, _force);
            float y = Random.Range(-_force, _force);

            transform.localPosition = new Vector3(x, y, transform.localPosition.z);

            _t -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        ShakeCroutine = null;
    }
}
