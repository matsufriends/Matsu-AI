using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

    [SerializeField]
    private Text ConsoleText = null;
    [SerializeField]
    private ScrollRect ConsoleScroll = null;
    [SerializeField]
    private Canvas canvas = null;
    [SerializeField]
    private Text textPrehub = null;
    private static List<Text> dotTextList;

    private static int lines = 1;

    void Start() {
        if (ConsoleText == null || ConsoleScroll == null || canvas == null || textPrehub == null) {
            Core.Exit("Console isn't assigned");
        }
        dotTextList = new List<Text>();
    }

    public void AddText(string in_Message) {
        ConsoleText.text += lines++ + ":" + in_Message + "\n";
        ConsoleText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        ConsoleScroll.verticalNormalizedPosition = 0;
    }

    public void ResetText() {
        ConsoleText.text = "";
        ConsoleText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        ConsoleScroll.verticalNormalizedPosition = 0;
    }

    public void DrawDotStatus(List<Dot> in_RedMom, List<Dot> in_BlueMom) {
        if (in_RedMom.Count + in_BlueMom.Count != dotTextList.Count) {
            for (int i = 0; i < dotTextList.Count; i++) {
                Destroy(dotTextList[i].gameObject);
            }
            dotTextList = new List<Text>();
            for (int i = 0; i < in_RedMom.Count; i++) {
                AddDotStatus(in_RedMom[i]);
            }
            for (int i = 0; i < in_BlueMom.Count; i++) {
                AddDotStatus(in_BlueMom[i]);
            }
        }
    }

    private void AddDotStatus(Dot in_Dot) {
        Text tmp_text = Instantiate(textPrehub, canvas.transform);
        dotTextList.Add(tmp_text);
        tmp_text.transform.position = in_Dot.pos;
        if (in_Dot.group == null) {
            tmp_text.text = in_Dot.pos + ",N";
        } else {
            tmp_text.text = in_Dot.pos + "\n" + in_Dot.group.groupNum + "," + in_Dot.borderNom;
        }
    }

    public void RemoveDotStatus() {
        for (int i = 0; i < dotTextList.Count; i++) {
            Destroy(dotTextList[i].gameObject);
        }
        dotTextList = new List<Text>();
    }

    public void AllReset() {
        lines = 1;
        ResetText();
        RemoveDotStatus();
        AddText("初期化しました");
        AddText("MergeK・Radious・Verticesはリセット時適応");
        AddText("MergeK="+Core.groupMergeK+" Radious="+MeshMake.dotRadious+" Vertices="+MeshMake.mesh_verticies);
    }

}
