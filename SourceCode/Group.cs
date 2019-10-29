using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group {

    private List<Dot> dotList;
    public List<Dot> outerDotList { get; private set; }
    public List<Line> lineList { get; private set; }
    public Vector2 cOG { get; private set; }
    public int groupNum { get; private set; }
    public int dotCount { get; private set; }
    private bool IsRed = true;
    private bool hadMadeOuterMesh = false;
    private bool hadMadeRangeMesh = false;
    private Mesh outerMesh;
    private Mesh rangeMesh;


    public Group(bool in_IsRed) {
        IsRed = in_IsRed;
        groupNum = Core.SetGroupNum(in_IsRed);
        dotList = new List<Dot>();
        outerDotList = new List<Dot>();
        lineList = new List<Line>();
    }

    public void AddDot(Dot in_Dot) {
        if (in_Dot.isClassified) {
            Core.Exit("Group is already set.");
        }
        dotList.Add(in_Dot);
        dotCount++;
        cOG = (cOG * (dotList.Count - 1) + in_Dot.pos) / dotList.Count;
        RemoveAllLine();
        if (outerDotList.Count < 3) {
            AddOuter(in_Dot);
        } else {
            Core.UpdateOuter(in_Dot, this);
        }
    }

    public void SetLine(Line in_Line) {
        lineList.Add(in_Line);
        hadMadeRangeMesh = false;
    }

    public bool HaveLine(Group in_OtherGroup) {
        for (int i = 0; i < lineList.Count; i++) {
            if (lineList[i].OtherGroup(this) == in_OtherGroup) {
                return true;
            }
        }
        return false;
    }

    public void RemoveAllLine() {
        for (int i = 0; i < lineList.Count; i++) {
            lineList[i].OtherGroup(this).RemoveLine(lineList[i].lineNum);
        }
        lineList = new List<Line>();
        hadMadeRangeMesh = false;
    }

    public void RemoveLine(int in_LineNum) {
        List<Line> tmp_List = new List<Line>();
        for (int i = 0; i < lineList.Count; i++) {
            if (lineList[i].lineNum != in_LineNum) {
                tmp_List.Add(lineList[i]);
            }
        }
        lineList = tmp_List;
        hadMadeRangeMesh = false;
    }

    public void ResetOuter() {
        for (int i = 0; i < outerDotList.Count; i++) {
            outerDotList[i].ResetBorderNum();
        }
        outerDotList = new List<Dot>();
        hadMadeOuterMesh = false;
    }

    public void AddOuter(Dot in_Dot) {
        in_Dot.SetBorderNum(outerDotList.Count);
        hadMadeOuterMesh = false;
        outerDotList.Add(in_Dot);
    }

    public Dot GetDot(int in_) {
        if (dotList.Count == 0 || in_ >= dotList.Count) {
            Core.Exit("Specified dot number is over");
            return null;
        } else {
            return dotList[in_];
        }
    }

    public float GetAvDisFromPos(Vector2 in_Pos) {
        float re_AvDisFromPos = 0;
        for (int i = 0; i < dotList.Count; i++) {
            re_AvDisFromPos += (in_Pos - dotList[i].pos).magnitude;
        }
        return re_AvDisFromPos / dotList.Count;
    }

    public Mesh GetOuterMesh() {
        if (hadMadeOuterMesh) {
            return outerMesh;
        } else {
            outerMesh = MeshMake.OuterMesh(outerDotList, cOG);
            hadMadeOuterMesh = true;
            return outerMesh;
        }
    }

    public Mesh GetRangeMesh() {
        if (hadMadeRangeMesh) {
            return rangeMesh;
        } else {
            rangeMesh = MeshMake.RangeMesh(lineList,IsRed);
            hadMadeRangeMesh = true;
            return rangeMesh;
        }
    }

}
