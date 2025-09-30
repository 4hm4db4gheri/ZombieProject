using UnityEngine;
using System.Collections.Generic;
using System;
[RequireComponent(typeof(DialoguePlayer))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    [Tooltip("The dialogue types and their rates to be played \nNOTE: If dialogue types are duplicated, then the first value would be used")]
    [SerializeField] private DialogueList dialogueTypesList;
    private DialoguePlayer dialoguePlayer;

    private void Start()
    {
        Instance = this;
        dialoguePlayer = GetComponent<DialoguePlayer>();
    }
    public void PlayDialogue(DialogueType DialogueType)
    {
        dialogueTypesList.IncrementDialogueCounter(DialogueType);
        if (dialogueTypesList.GetDialogueCounter(DialogueType) >= dialogueTypesList.GetDialogueRate(DialogueType))
        {
            dialoguePlayer.PlayDialogue(DialogueType.ToString());
        }
    }
}

[Serializable]
public class DialogueList
{
    [SerializeField] private List<DialogueItem> dialogueItems;

    public bool Contains(DialogueType DialogueType)
    {
        foreach (var t in dialogueItems)
        {
            if (t.DialogueType == DialogueType)
            {
                return true;
            }
        }
        return false;
    }
    public int GetDialogueRate(DialogueType DialogueType)
    {
        foreach (var t in dialogueItems)
        {
            if (t.DialogueType == DialogueType)
            {
                return t.DialogueRate;
            }
        }
        return 0;
    }
    public int GetDialogueCounter(DialogueType DialogueType)
    {
        foreach (var t in dialogueItems)
        {
            if (t.DialogueType == DialogueType)
            {
                return t.counter;
            }
        }
        return 0;
    }
    public void IncrementDialogueCounter(DialogueType DialogueType)
    {
        foreach (var t in dialogueItems)
        {
            if (t.DialogueType == DialogueType)
            {
                t.counter++;
            }
        }
    }
}

[Serializable]
public class DialogueItem
{
    [Tooltip("The type of dialogue to be played")]
    [SerializeField] public DialogueType DialogueType;
    [Tooltip("The rate at which the dialogue will be played")]
    [SerializeField] public int DialogueRate;
    [HideInInspector] public int counter = 0;

}

public enum DialogueType
{
    Headshot,
    Kill,
    Death,
    Damage,
    Heal,
    TripleKill,
    TeammateDied,
}
