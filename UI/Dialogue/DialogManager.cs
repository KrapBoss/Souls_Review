using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public DialogElement dialoguePrefab;
    public Transform dialogueContainer;
    public int maxDialogueCount = 3;

    Queue<DialogElement> s_dialogues = new Queue<DialogElement>(); // 저장된 거
    Queue<DialogElement> u_dialogues = new Queue<DialogElement>(); // 사용중인 거

    private void Awake()
    {
        for(int i = 0; i < maxDialogueCount; i++)
        {
            DialogElement element = Instantiate(dialoguePrefab, dialogueContainer);
            element.SetParent(this);
            s_dialogues.Enqueue(element);
        }
    }

    /// <summary> 대사를 나타냅니다. </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void AddDialogue(string text, float time, float delay = 0)
    {
        DialogElement item = null;
        if (s_dialogues.Count > 0) item = s_dialogues.Dequeue();
        else item = u_dialogues.Dequeue();

        item.Set(text, time, delay);
        u_dialogues.Enqueue(item);
    }

    /// <summary> 모든 오브젝트를 비활성화 합니다. </summary>
    public void AllDisappear()
    {
        while (u_dialogues.Count > 0)
        {
            DialogElement item = u_dialogues.Dequeue();
            item.Disappear();
            s_dialogues.Enqueue(item);
        }
    }
}
