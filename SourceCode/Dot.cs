using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot {

    public Vector2 pos { get; private set; }
    public Group group { get; private set; }
    public int borderNom { get; private set; }
    public bool isRed { get; private set; }
    public bool isClassified { get; private set; }


    public Dot(Vector2 in_Pos, bool in_IsRed) {
        pos = in_Pos;
        borderNom = -1;
        isRed = in_IsRed;
        isClassified = false;
    }

    public void LeaveGroup() {
        isClassified = false;
        borderNom = -1;
    }

    public void ReSetGroup(Group in_Group) {
        group = in_Group;
        in_Group.AddDot(this);
        isClassified = true;
    }

    public void SetBorderNum(int in_Num) {
        if (in_Num < 0) {
            Core.Exit("BorderNum has Error");
        }
        this.borderNom = in_Num;
    }

    public void ResetBorderNum() {
        this.borderNom = -1;
    }

    public void SetGroup(Group in_Group) {
        if (isClassified) {
            Core.Exit("Group is already set.");
        }
        group = in_Group;
        in_Group.AddDot(this);
        isClassified = true;
    }

}
