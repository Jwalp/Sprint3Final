using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCInteractionManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject interactionHintUI;
    private NPC currentNPC; //The closest NPPC
    private AudioSource currentAudioSource;

    void Update()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        NPC nearestNPC = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject npcObject in npcs)
        {
            NPC npc = npcObject.GetComponent<NPC>();
            if (npc != null)
            {
                float distance = Vector3.Distance(npc.player.position, npc.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestNPC = npc;
                }
            }
        }

        if (nearestNPC != null && nearestDistance <= nearestNPC.interactionRange)
        {
            ShowInteractionHint(nearestNPC);
            currentNPC = nearestNPC;
            currentAudioSource = currentNPC.GetComponent<AudioSource>();

            if (Input.GetKeyDown(KeyCode.E) && currentNPC != null)
            {
                ShowDialogue();
            }
        }
        else
        {
            interactionHintUI.SetActive(false);
            HideDialogue();
        }
    }

    void ShowInteractionHint(NPC npc)
    {
        interactionHintUI.SetActive(true);
        npc.FacePlayer();
    }

    void ShowDialogue()
    {
        if (currentNPC != null && dialogueText != null)
        {
            string formattedDialogue = currentNPC.dialogue.Replace("\\n", "\n");
            formattedDialogue = formattedDialogue.Replace("\\t", "\t");
            dialogueText.text = formattedDialogue;
            dialogueText.gameObject.SetActive(true);

            if (currentAudioSource != null && !currentAudioSource.isPlaying)
            {
                currentAudioSource.Play();
            }
        }
    }

    void HideDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(false);
        }
    }
}
