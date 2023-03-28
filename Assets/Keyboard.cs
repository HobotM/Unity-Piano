using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    public GameObject blackTile, whiteTile; 
    public GameObject content;
    public GameObject noteText;

    public int numberOfOctaves;
    
    void Start()
    {
        int startNote = 24;
        for (int i = 0; i < numberOfOctaves; i++)
        {
            createOctave(startNote+i*12, i);
        }
    }
    private void createOctave(int startNote, int octave){
        float width = content.GetComponent<RectTransform>().rect.width;
        float widthPerOctave = width / numberOfOctaves; 
        float widthPerNote = widthPerOctave / 7; 

        // 7 white tiles
        for (int i = 0; i < 7; i++)
        {
            int actualNoteIndex = getWhiteKeyIndex(i); 
            GameObject note = instantiateNote(whiteTile, actualNoteIndex, startNote);
            registerEvents(note);

            note.GetComponent<RectTransform>().sizeDelta = new Vector2(widthPerNote-1, note.GetComponent<RectTransform>().sizeDelta.y); 
            note.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(widthPerOctave*octave+widthPerNote*i + widthPerNote/2, -whiteTile.GetComponent<RectTransform>().rect.height/2,0);
    
        }

        // 5 black tiles 
        for (int i = 0; i < 5; i++)
        {
            int actualNoteIndex = getBlackKeyIndex(i); 
            GameObject note = instantiateNote(blackTile, actualNoteIndex, startNote); 
            registerEvents(note);

            note.GetComponent<RectTransform>().sizeDelta = new Vector2(widthPerNote/2, note.GetComponent<RectTransform>().sizeDelta.y);

            int blackIndex = i; 
            if(i > 1){
                blackIndex += 1; 
            }
            note.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(widthPerOctave*octave+widthPerNote*blackIndex+widthPerNote, -blackTile.GetComponent<RectTransform>().rect.height/2, 0);
        }
    }

    private void registerEvents(GameObject note){
        EventTrigger trigger = note.gameObject.AddComponent<EventTrigger>(); 
        var pointerDown = new EventTrigger.Entry(); 
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => keyOn(note.GetComponent<PianoTile>().midiNote));
        trigger.triggers.Add(pointerDown); 

        var pointerUp = new EventTrigger.Entry(); 
        pointerUp.eventID = EventTriggerType.PointerUp; 
        pointerUp.callback.AddListener((e) => keyOff(note.GetComponent<PianoTile>().midiNote)); 
        trigger.triggers.Add(pointerUp); 
    }

    public void keyOn(int midiNumber){
        Debug.Log("Clicked " + midiNumber); 
        GameObject.Find("SoundGen").GetComponent<SoundGen>().OnKey(midiNumber);
    }
  
    public void keyOff(int midiNumber){
        Debug.Log("Released " + midiNumber);
        GameObject.Find("SoundGen").GetComponent<SoundGen>().onKeyOff(midiNumber);
    }

private string noteName(int midiNumber) {
    switch (midiNumber % 12) {
        case 0:
            return "C";
        case 1:
            return "C#";
        case 2:
            return "D";
        case 3:
            return "D#";
        case 4:
            return "E";
        case 5:
            return "F";
        case 6:
            return "F#";
        case 7:
            return "G";
        case 8:
            return "G#";
        case 9:
            return "A";
        case 10:
            return "A#";
        case 11:
            return "B";
        default:
            return "";
    }
}


private GameObject instantiateNote(GameObject note, int actualNoteIndex, int startNote){
    GameObject newNote = Instantiate(note);
    newNote.transform.SetParent(content.transform, false);
    newNote.GetComponent<PianoTile>().midiNote = startNote + actualNoteIndex;
    GameObject noteTextObject = Instantiate(noteText);
    noteTextObject.transform.SetParent(newNote.transform, false);
    noteTextObject.GetComponent<Text>().text = noteName(startNote + actualNoteIndex);
    noteTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    noteTextObject.GetComponent<Text>().fontSize = 15;
    noteTextObject.GetComponent<Text>().color = Color.red;
    noteTextObject.transform.position = new Vector3(noteTextObject.transform.position.x + 25, noteTextObject.transform.position.y -60, noteTextObject.transform.position.z);
    
    return newNote;
}

    private int getWhiteKeyIndex(int i){
        // C
        int actualNoteIndex = 0;

        if(i == 1){
            // D
            actualNoteIndex = 2;
        } else if(i == 2){
            // E
            actualNoteIndex = 4; 
        } else if(i == 3){
            //F
            actualNoteIndex = 5; 
        }else if(i == 4){
            //G
            actualNoteIndex = 7; 
        }
        else if(i == 5){
            //A
            actualNoteIndex = 9; 
        }
        else if(i == 6){
            //B
            actualNoteIndex = 11; 
        }
        return actualNoteIndex;

    }

    private int getBlackKeyIndex(int i){
        // CS
        int actualNote = 1; 
        if(i == 1){
            // DS
            actualNote = 3; 
        } else if(i == 2){
            // ES
            actualNote = 6; 
        } else if(i == 3){
            //FS
            actualNote = 8; 
        }
        return actualNote;
    }

    


    // Update is called once per frame
    void Update()
    {
        
    }
}