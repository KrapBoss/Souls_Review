using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//카메라 흔들어 주는 자식 오브젝트
public class CameraShaking : MonoBehaviour
{
    

    float _t, _force;//실행한 시간을 가진다.
    IEnumerator ShakeCroutine = null;
    public void Shake(float t, float force)
    {
        if(ShakeCroutine != null)//새로운 입력이 들어왔을 때, 쉐이크 진행을 초기화시킨다.
        {
            //시간이 새로 들어온 입력 시간이 더 크다면 실행시간을 증가.
            if (_t < t)
            {
                _t = t;
                _force = force;
            }
            else if(_force > force)//진동세기가 더 크기만 하다면, 진동세기만을 변경해준다.
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
