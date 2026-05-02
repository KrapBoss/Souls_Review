using CustomUI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class ThrowingObject : InteractionObjects
{
    private AudioSource audioSource;
    public bool timeOut = false;

    public float distance = 5.0f;

    [SerializeField]
    public ItemShowText itemShowText;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
#endif

    public override bool GrabOn(Transform parent)
    {
        itemShowText.ShowText();
        return base.GrabOn(parent);
    }

    protected override void Init()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.volume = DataSet.Instance.SettingValue.Volume;

        timeOut = true;
        Invoke("TimeOut", 1.0f);

        base.Init();
    }
    public override bool Func()
    {
        return false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.layer.Equals(LayerMask.NameToLayer("Player")) && !timeOut)
        {
            //Debug.Log(collision.gameObject.layer + "사운드 출력");
            EventManager.instance.EventSound(this.transform, distance);
            audioSource.Play();
            timeOut = true;
            Invoke("TimeOut", 0.3f);
        }
    }

    void TimeOut()
    {
        timeOut = false;
    }
}
