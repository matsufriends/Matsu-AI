using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Core {

    public static float groupMergeK = 0;
    private static int lineNum = 1;
    private static int redGroupNum = 1;
    private static int blueGroupNum = 1;

    public static int SetGroupNum(bool in_IsRed) {
        if (in_IsRed) {
            redGroupNum++;
            return redGroupNum - 1;
        } else {
            blueGroupNum++;
            return blueGroupNum - 1;
        }
    }

    public static int SetLineNum() {
        lineNum++;
        return lineNum - 1;
    }

    public static void ClassifyPerColor(List<Group> in_GroupList, List<Dot> in_Mom, List<Dot> in_OtherMom) {
        for (int i = 0; i < in_Mom.Count; i++) {
            Dot A = in_Mom[i];
            if (A.isClassified == false) {//点Aが未分類
                string tmp_Message = A.pos + ",";
                Dot B = MinDot(A.pos, in_Mom, true);
                if (B == null) {//Bが存在しない
                    MakeNewGroup(A, in_GroupList);
                    Main.console.AddText(tmp_Message + "単独");
                } else {//Bが存在
                    if (B.isClassified) {//Bが分類済み
                        if (IsJoiningAccepted(A, B.group, in_OtherMom)) {
                            A.SetGroup(B.group);
                            Main.console.AddText(tmp_Message + B.pos + "参加〇");
                            KeepFindingFriend(A.group, in_Mom, in_OtherMom);
                        } else {
                            MakeNewGroup(A, in_GroupList);
                            Main.console.AddText(tmp_Message + B.pos + "参加×→単独");
                        }
                    } else {//Bが未分類
                        if (OnOtherGroup(A.pos, B.pos, in_OtherMom)) {
                            MakeNewGroup(A, in_GroupList);
                            Main.console.AddText(tmp_Message + B.pos + "線分×→単独");
                        } else {
                            MakeNewGroup(A, in_GroupList);
                            B.SetGroup(A.group);
                            Main.console.AddText(tmp_Message + B.pos + "新規共同");
                            KeepFindingFriend(A.group, in_Mom, in_OtherMom);
                        }
                    }
                }
            }
        }
    }

    private static Dot MinDot(Vector2 in_FromPos, List<Dot> in_Group, bool in_IsAllowClassfiedDot) {
        Dot re_minDot = null;
        float _minDistance = 0;
        for (int i = 0; i < in_Group.Count; i++) {
            if (in_FromPos != in_Group[i].pos) {
                if (in_Group[i].isClassified == false || in_Group[i].isClassified == in_IsAllowClassfiedDot) {
                    float tmp_Distance = (in_FromPos - in_Group[i].pos).magnitude;
                    if (tmp_Distance < _minDistance || _minDistance == 0) {
                        _minDistance = tmp_Distance;
                        re_minDot = in_Group[i];
                    }
                }
            }
        }
        return re_minDot;
    }

    private static void MakeNewGroup(Dot in_Dot, List<Group> in_GroupList) {
        Group tmp_Group = new Group(in_Dot.isRed);
        in_GroupList.Add(tmp_Group);
        in_Dot.SetGroup(tmp_Group);
    }

    private static bool IsJoiningAccepted(Dot in_JudgeDot, Group in_JudgeGroup, List<Dot> in_OtherMom) {
        if (IsInPolygon(in_JudgeDot, in_JudgeGroup)) {
            return true;
        }
        if (IsFarFromAverageDistance(in_JudgeDot, in_JudgeGroup)) {//距離的問題
            return false;
        }
        if (IsIncludeDot(in_JudgeDot, in_JudgeGroup, in_OtherMom)) {//他色包括問題
            return false;
        }
        return true;
    }

    private static bool IsInPolygon(Dot in_JudgeDot, Group in_JudgeGroup) {
        if (in_JudgeGroup.outerDotList.Count <= 2) {
            return false;
        }
        for (int i = 0; i < in_JudgeGroup.outerDotList.Count; i++) {
            int j = i + 1;
            if (j == in_JudgeGroup.outerDotList.Count) {
                j = 0;
            }
            if (IsInTriangle(in_JudgeDot.pos, in_JudgeGroup.outerDotList[i].pos, in_JudgeGroup.outerDotList[j].pos, in_JudgeGroup.cOG)) {
                return true;
            }
        }
        return false;
    }

    private static bool IsInTriangle(Vector2 in_JudgePos, Vector2 in_APos, Vector2 in_BPos, Vector2 in_CPos) {
        if (IsTriangle(in_APos, in_BPos, in_CPos) == false) {
            return OnOther(in_JudgePos, in_APos, in_BPos);
        }
        AdjustParallel(ref in_APos, ref in_BPos, ref in_CPos);
        if (in_APos.x == in_BPos.x) {
            return IsInRightTriangle(in_JudgePos, in_APos, in_BPos, in_CPos);
        } else if (in_APos.x == in_CPos.x) {
            return IsInRightTriangle(in_JudgePos, in_APos, in_CPos, in_BPos);
        } else if (in_APos.y == in_BPos.y) {
            return IsInRightTriangle(in_JudgePos, in_CPos, in_BPos, in_APos);
        } else if (in_APos.y == in_CPos.y) {
            return IsInRightTriangle(in_JudgePos, in_BPos, in_CPos, in_APos);
        }
        float ax = in_APos.x;
        float ay = in_APos.y;
        float bx = in_BPos.x;
        float by = in_BPos.y;
        float cx = in_CPos.x;
        float cy = in_CPos.y;
        float dx = in_JudgePos.x;
        float dy = in_JudgePos.y;
        float s = ((dy - ay) * (cx - ax) - (cy - ay) * (dx - ax)) / ((by - ay) * (cx - ax) - (bx - ax) * (cy - ay));
        float k = ((dx - ax) - s * (bx - ax)) / (cx - ax);
        if (IsEqual(s, 0) || float.IsNaN(s)) {
            s = 0;
        }
        if (IsEqual(k, 0) || float.IsNaN(k)) {
            k = 0;
        }
        if (IsSmaller(s + k, 1) && s >= 0 && k >= 0) {
            return true;
        }
        return false;
    }

    private static bool IsTriangle(Vector2 in_APos, Vector2 in_BPos, Vector2 in_CPos) {
        Vector2 A_B = in_BPos - in_APos;
        Vector2 A_C = in_CPos - in_APos;
        if (A_B.x == 0 && A_B.y == 0) {
            return false;
        } else if (A_B.x == 0) {
            return A_C.x != 0;
        } else if (A_B.y == 0) {
            return A_C.y != 0;
        }

        return !IsEqual(A_C.y / A_B.y, A_C.x / A_B.x);
    }

    private static bool OnOther(Vector2 in_JudgePos, Vector2 in_APos, Vector2 in_BPos) {
        if (in_APos.x == in_BPos.x) {//縦の直線
            if (in_JudgePos.x == in_APos.x) {//直線上
                return (in_APos.y < in_JudgePos.y && in_JudgePos.y < in_BPos.y) || (in_BPos.y < in_JudgePos.y && in_JudgePos.y < in_APos.y);
            } else {
                return false;
            }
        } else if (in_APos.y == in_BPos.y) {//横の直線
            if (in_JudgePos.y == in_APos.y) {//直線上
                return (in_APos.x < in_JudgePos.x && in_JudgePos.x < in_BPos.x) || (in_BPos.x < in_JudgePos.x && in_JudgePos.x < in_APos.x);
            } else {
                return false;
            }
        } else {
            float tmp_Angle = (in_APos.y - in_BPos.y) / (in_APos.x - in_BPos.x);
            float tmp_Height = in_APos.y - tmp_Angle * in_APos.x;

            if (IsEqual(in_JudgePos.y, in_JudgePos.x * tmp_Angle + tmp_Height)) {
                return (in_APos.x < in_JudgePos.x && in_JudgePos.x < in_BPos.x) || (in_BPos.x < in_JudgePos.x && in_JudgePos.x < in_APos.x);
            } else {
                return false;
            }
        }
    }

    private static void AdjustParallel(ref Vector2 ref_APos, ref Vector2 ref_BPos, ref Vector2 ref_CPos) {
        if (ref_APos.x == ref_BPos.x) {//AとBが縦の直線
            ChangeVector2(ref ref_APos, ref ref_CPos);//BとC(A)が縦の直線に。
        } else if (ref_APos.x == ref_CPos.x) {//AとCが縦の直線
            ChangeVector2(ref ref_APos, ref ref_BPos);//B(A)とCが縦の直線に。
        } else if (ref_BPos.x == ref_CPos.x) {//BとCが縦の直線→OK
        } else if (ref_APos.y == ref_BPos.y) {//AとBが横の直線
            ChangeVector2(ref ref_APos, ref ref_CPos);//BとC(A)が横の直線
        } else if (ref_APos.y == ref_CPos.y) {//AとCが横の直線
            ChangeVector2(ref ref_APos, ref ref_BPos);//B(A)とCが横の直線
        } else if (ref_BPos.y == ref_CPos.y) {//BとCが横の直線→OK
        }
    }

    private static void ChangeVector2(ref Vector2 ref_APos, ref Vector2 ref_BPos) {
        ref_BPos += ref_APos;
        ref_APos = ref_BPos - ref_APos;
        ref_BPos -= ref_APos;
    }

    private static bool IsInRightTriangle(Vector2 in_JudgePos, Vector2 in_EdgeUpPos, Vector2 in_BaseEdgePos, Vector2 in_EdgeSidePos) {
        Vector2 Up = in_EdgeUpPos - in_BaseEdgePos;
        Vector2 Side = in_EdgeSidePos - in_BaseEdgePos;
        Vector2 _J = in_JudgePos - in_BaseEdgePos;
        float s = _J.y / Up.y;
        float t = _J.x / Side.x;
        return IsSmaller(s + t, 1) && s >= 0 && t >= 0;
    }

    private static bool IsFarFromAverageDistance(Dot in_JudgeDot, Group in_JudgeGroup) {//点を加えた重心からの平均距離比較
        Vector2 tmp_AddedCOGPos = (in_JudgeGroup.cOG * in_JudgeGroup.dotCount + in_JudgeDot.pos) / (in_JudgeGroup.dotCount + 1);
        if ((in_JudgeDot.pos - tmp_AddedCOGPos).magnitude >= in_JudgeGroup.GetAvDisFromPos(tmp_AddedCOGPos) * groupMergeK) {
            return true;
        } else {
            return false;
        }
    }

    private static bool IsIncludeDot(Dot in_JudgeDot, Group in_JudgeGroup, List<Dot> in_OtherMom) {
        if (in_JudgeGroup.dotCount == 1) {
            return OnOtherGroup(in_JudgeDot.pos, in_JudgeGroup.GetDot(0).pos, in_OtherMom);
        }
        Dot tmp_A;
        Dot tmp_B;
        SteepDot(in_JudgeDot, out tmp_A, out tmp_B, in_JudgeGroup.outerDotList);
        for (int i = 0; i < in_OtherMom.Count; i++) {//他色をまたがない
            if (IsInTriangle(in_OtherMom[i].pos, in_JudgeDot.pos, tmp_A.pos, tmp_B.pos)) {
                return true;
            }
        }
        return false;
    }

    private static bool OnOtherGroup(Vector2 in_APos, Vector2 in_BPos, List<Dot> in_OtherMom) {
        for (int i = 0; i < in_OtherMom.Count; i++) {
            if (OnOther(in_OtherMom[i].pos, in_APos, in_BPos)) {
                return true;
            }
        }
        return false;
    }

    private static void SteepDot(Dot in_BaseDot, out Dot out_ADot, out Dot out_BDot, List<Dot> in_BorderDotList) {
        if (in_BorderDotList.Count <= 1) {
            Exit("Can't calculate SteepDot ");
        }
        out_ADot = in_BorderDotList[0];
        out_BDot = in_BorderDotList[1];
        float tmp_MinCos = 1;
        for (int i = 0; i < in_BorderDotList.Count; i++) {
            for (int j = i + 1; j < in_BorderDotList.Count; j++) {
                float tmp_Cos = CosFromA2BC(in_BaseDot.pos, in_BorderDotList[i].pos, in_BorderDotList[j].pos);
                if (IsEqual(tmp_Cos, tmp_MinCos)) {//cosが同じとき
                    float difA = (out_ADot.pos - in_BaseDot.pos).magnitude + (out_BDot.pos - in_BaseDot.pos).magnitude;
                    float difB = (in_BorderDotList[i].pos - in_BaseDot.pos).magnitude + (in_BorderDotList[j].pos - in_BaseDot.pos).magnitude;
                    if (IsSmaller(difA, difB)) {//より長い辺を採用
                        tmp_MinCos = tmp_Cos;
                        out_ADot = in_BorderDotList[i];
                        out_BDot = in_BorderDotList[j];
                    }
                } else if (IsSmaller(tmp_Cos, tmp_MinCos)) {
                    tmp_MinCos = tmp_Cos;
                    out_ADot = in_BorderDotList[i];
                    out_BDot = in_BorderDotList[j];
                }
            }
        }
    }

    private static float CosFromA2BC(Vector2 in_A, Vector2 in_B, Vector2 in_C) {
        float x = (in_B - in_A).magnitude;
        float y = (in_C - in_A).magnitude;
        float z = (in_C - in_B).magnitude;
        return (x * x + y * y - z * z) / (2 * x * y);
    }

    private static void KeepFindingFriend(Group in_Group, List<Dot> in_Mom, List<Dot> in_OtherMom) {
        Dot tmp_A = MinDot(in_Group.cOG, in_Mom, false);
        if (tmp_A != null && IsJoiningAccepted(tmp_A, in_Group, in_OtherMom)) {
            tmp_A.SetGroup(in_Group);
            Main.console.AddText("→" + tmp_A.pos + "参加");
            KeepFindingFriend(in_Group, in_Mom, in_OtherMom);
        }
    }

    public static void AdjustIncludePerGroup(List<Group> in_BaseGroupList, List<Group> in_NewGroupList, List<Dot> in_OtherMom, ref bool in_AllOK) {
        for (int i = 0; i < in_BaseGroupList.Count; i++) {
            if (IsIncludeOther(in_BaseGroupList[i], in_OtherMom)) {
                in_AllOK = false;
                for (int j = 0; j < in_BaseGroupList[i].dotCount; j++) {
                    in_BaseGroupList[i].GetDot(j).LeaveGroup();
                }
            } else {
                in_NewGroupList.Add(in_BaseGroupList[i]);
            }
        }
    }

    private static bool IsIncludeOther(Group in_JudgeGroup, List<Dot> in_OtherMom) {
        if (in_JudgeGroup.dotCount == 1) {
            return false;
        }
        for (int i = 0; i < in_JudgeGroup.outerDotList.Count; i++) {
            int j = i + 1;
            if (j == in_JudgeGroup.outerDotList.Count) {
                j = 0;
            }
            for (int k = 0; k < in_OtherMom.Count; k++) {
                if (IsInTriangle(in_OtherMom[k].pos, in_JudgeGroup.outerDotList[i].pos, in_JudgeGroup.outerDotList[j].pos, in_JudgeGroup.cOG)) {
                    return true;
                }
            }
        }
        return false;
    }

    public static void ClassfyBorderLinePattern(Group in_RedGroup, Group in_BlueGroup) {
        if (in_RedGroup.dotCount == 1 && in_BlueGroup.dotCount == 1) {//両方一個
            MakeBorderDot2Dot(in_RedGroup.GetDot(0), in_BlueGroup.GetDot(0));
        } else if (in_RedGroup.dotCount == 1) {//赤だけ一個
            CalculateBorder(in_RedGroup, in_BlueGroup, false);
        } else if (in_BlueGroup.dotCount == 1) {//青だけ一個
            CalculateBorder(in_BlueGroup, in_RedGroup, false);
        } else {//両方二個以上
            CalculateBorder(in_RedGroup, in_BlueGroup, true);
        }
    }

    private static void CalculateBorder(Group in_SmallGroup, Group in_LargeGroup, bool is_BothCheck) {
        Dot tmp_MinA = in_SmallGroup.outerDotList[0];
        Dot tmp_MinB = in_LargeGroup.outerDotList[0];
        Dot tmp_Base = in_SmallGroup.outerDotList[0];
        Dot tmp_LineA = in_LargeGroup.outerDotList[0];
        Dot tmp_LineB = in_LargeGroup.outerDotList[1];

        float tmp_MinDistanceDot = minDistanceDot2Dot(in_SmallGroup.outerDotList, in_LargeGroup.outerDotList, ref tmp_MinA, ref tmp_MinB);
        float tmp_MinDistanceLine = minDistanceVertical(in_SmallGroup.outerDotList, in_LargeGroup.outerDotList, ref tmp_Base, ref tmp_LineA, ref tmp_LineB, is_BothCheck);
        if (tmp_MinDistanceLine == -1 || tmp_MinDistanceDot < tmp_MinDistanceLine) {//点と点で引く
            MakeBorderDot2Dot(tmp_MinA, tmp_MinB);
        } else {//点と辺で引く
            MakeLineDot2Line(tmp_Base, tmp_LineA, tmp_LineB, tmp_Base.isRed);
        }
    }

    private static float minDistanceDot2Dot(List<Dot> in_SmallDotList, List<Dot> in_LargeDotList, ref Dot out_MinRedDot, ref Dot out_MinBlueDot) {
        float re_MinDistance = (out_MinRedDot.pos - out_MinBlueDot.pos).magnitude;
        for (int i = 0; i < in_SmallDotList.Count; i++) {
            for (int j = 0; j < in_LargeDotList.Count; j++) {
                float tmp_Distance = (in_SmallDotList[i].pos - in_LargeDotList[j].pos).magnitude;
                if (tmp_Distance < re_MinDistance) {
                    out_MinRedDot = in_SmallDotList[i];
                    out_MinBlueDot = in_LargeDotList[j];
                    re_MinDistance = tmp_Distance;
                }
            }
        }
        return re_MinDistance;
    }

    private static float minDistanceVertical(List<Dot> in_SmallList, List<Dot> in_LargeList, ref Dot out_Dot, ref Dot out_LineA, ref Dot out_LineB, bool is_BothCheck) {
        float re_NowDistance = -1;
        minDistanceVerticalPerColor(in_SmallList, in_LargeList, ref re_NowDistance, ref out_Dot, ref out_LineA, ref out_LineB);
        if (is_BothCheck) {
            minDistanceVerticalPerColor(in_LargeList, in_SmallList, ref re_NowDistance, ref out_Dot, ref out_LineA, ref out_LineB);
        }
        return re_NowDistance;
    }

    private static void minDistanceVerticalPerColor(List<Dot> in_DotList, List<Dot> in_LineList, ref float ref_NowDistance, ref Dot ref_Dot, ref Dot ref_LineA, ref Dot ref_LineB) {
        Dot tmp_Dot = in_DotList[0];
        Dot tmp_A = in_LineList[0];
        Dot tmp_B = in_LineList[1];
        float re_MinDistance = -1;
        for (int i = 0; i < in_DotList.Count; i++) {
            for (int j = 0; j < in_LineList.Count; j++) {
                for (int k = j + 1; k < in_LineList.Count; k++) {
                    float tmp_Distance = VerticalDistance(in_DotList[i].pos, in_LineList[j].pos, in_LineList[k].pos);
                    if (tmp_Distance != -1) {
                        if (tmp_Distance < re_MinDistance || re_MinDistance == -1) {
                            tmp_Dot = in_DotList[i];
                            tmp_A = in_LineList[j];
                            tmp_B = in_LineList[k];
                            re_MinDistance = tmp_Distance;
                        }
                    }
                }
            }
        }

        if (re_MinDistance != -1) {
            if (re_MinDistance < ref_NowDistance || ref_NowDistance == -1) {
                ref_NowDistance = re_MinDistance;
                ref_Dot = tmp_Dot;
                ref_LineA = tmp_A;
                ref_LineB = tmp_B;
            }
        }
    }

    private static float VerticalDistance(Vector2 in_BasePos, Vector2 in_LineAPos, Vector2 in_LineBPos) {
        if (CosFromA2BC(in_LineAPos, in_LineBPos, in_BasePos) < 0 || CosFromA2BC(in_LineBPos, in_LineAPos, in_BasePos) < 0) {
            return -1;
        } else {
            float a = (in_LineAPos - in_LineBPos).magnitude;
            float b = (in_LineAPos - in_BasePos).magnitude;
            float c = (in_LineBPos - in_BasePos).magnitude;
            float s = (a + b + c) / 2;
            float S = (float)System.Math.Sqrt(s * (s - a) * (s - b) * (s - c));
            return 2 * S / a;
        }
    }

    public static void AdjustStraddlePerColor(List<Group> in_BaseGroupList, List<Group> in_NewGroupList, List<Dot> in_Mom, List<Dot> in_OtherMom, ref bool in_AllOK, bool in_IsRed) {
        for (int i = 0; i < in_BaseGroupList.Count; i++) {//判定するグループ
            bool tmp_GroupOK = true;
            for (int j = 0; j < in_BaseGroupList[i].lineList.Count && tmp_GroupOK; j++) {//判定する線
                List<Dot> tmp_SafeList = new List<Dot>();
                List<Dot> tmp_OutList = new List<Dot>();
                for (int k = 0; k < in_BaseGroupList[i].dotCount; k++) {//判定する点
                    if (in_BaseGroupList[i].lineList[j].IsCorrectlyJudged(in_BaseGroupList[i].GetDot(k).pos,false) == in_IsRed) {
                        tmp_SafeList.Add(in_BaseGroupList[i].GetDot(k));
                    } else {
                        tmp_OutList.Add(in_BaseGroupList[i].GetDot(k));
                    }
                }
                if (tmp_OutList.Count > 0 && tmp_SafeList.Count > 0) {//アウトとセーフに別れた
                    Group tmp_SafeGroup = new Group(in_IsRed);
                    Group tmp_OutGroup = new Group(in_IsRed);

                    for (int k = 0; k < tmp_SafeList.Count; k++) {
                        tmp_SafeList[k].LeaveGroup();
                        tmp_SafeList[k].ReSetGroup(tmp_SafeGroup);
                    }
                    for (int k = 0; k < tmp_OutList.Count; k++) {
                        tmp_OutList[k].LeaveGroup();
                        tmp_OutList[k].ReSetGroup(tmp_OutGroup);
                    }
                    in_NewGroupList.Add(tmp_SafeGroup);
                    in_NewGroupList.Add(tmp_OutGroup);
                    in_AllOK = false;
                    tmp_GroupOK = false;
                } else if (tmp_SafeList.Count == 0) {//全てアウト
                    Debug.Log("こんなとこ呼ばれないと思う");
                    for (int k = 0; k < tmp_SafeList.Count; k++) {
                        tmp_SafeList[k].LeaveGroup();
                    }
                    ClassifyPerColor(in_NewGroupList, in_Mom, in_OtherMom);
                    in_AllOK = false;
                    tmp_GroupOK = false;
                }
            }
            if (tmp_GroupOK) {
                in_NewGroupList.Add(in_BaseGroupList[i]);
            }
        }
    }

    public static void AdjustSoloPerColor(List<Group> in_BaseGroupList, List<Group> in_NewGroupList,ref bool in_AllOK) {
        for (int i=0;i<in_BaseGroupList.Count;i++) {
            if (in_BaseGroupList[i].dotCount!=2) {
                in_NewGroupList.Add(in_BaseGroupList[i]);
            } else {
                in_AllOK = false;
                Main.console.AddText(in_BaseGroupList[i].GetDot(0).pos+","+ in_BaseGroupList[i].GetDot(1).pos+"→単集合x2");
                in_BaseGroupList[i].GetDot(0).LeaveGroup();
                in_BaseGroupList[i].GetDot(1).LeaveGroup();
                in_BaseGroupList[i].GetDot(0).group.RemoveAllLine();
                Group tmp_A = new Group(in_BaseGroupList[i].GetDot(0).isRed);
                Group tmp_B = new Group(in_BaseGroupList[i].GetDot(1).isRed);
                in_BaseGroupList[i].GetDot(0).SetGroup(tmp_A);
                in_BaseGroupList[i].GetDot(1).SetGroup(tmp_B);
                in_NewGroupList.Add(tmp_A);
                in_NewGroupList.Add(tmp_B);
            }
        }
    }

    private static void MakeBorderDot2Dot(Dot in_A, Dot in_B) {
        if (in_A.isRed) {
            MakeLineDot2Dot(in_A, in_B);
        } else {
            MakeLineDot2Dot(in_B, in_A);
        }
    }

    private static void MakeLineDot2Dot(Dot in_Red, Dot in_Blue) {
        float tmp_Angle, tmp_Height;
        bool tmp_UorL, tmp_HorV;

        if (in_Red.pos.x == in_Blue.pos.x) {
            tmp_HorV = true;
            tmp_Angle = 0;
            tmp_Height = (in_Red.pos.y + in_Blue.pos.y) / 2;
            if (in_Red.pos.y > in_Blue.pos.y) {
                tmp_UorL = true;
            } else {
                tmp_UorL = false;
            }
        } else if (in_Red.pos.y == in_Blue.pos.y) {
            tmp_HorV = true;
            tmp_Angle = 1;
            tmp_Height = (in_Red.pos.x + in_Blue.pos.x) / 2;
            if (in_Red.pos.x < in_Blue.pos.x) {
                tmp_UorL = true;
            } else {
                tmp_UorL = false;
            }
        } else {
            tmp_HorV = false;
            tmp_Angle = (in_Red.pos.x - in_Blue.pos.x) / (in_Blue.pos.y - in_Red.pos.y);
            tmp_Height = (in_Red.pos.y + in_Blue.pos.y) / 2 - tmp_Angle * (in_Red.pos.x + in_Blue.pos.x) / 2;
            if (in_Red.pos.y > in_Blue.pos.y) {
                tmp_UorL = true;
            } else {
                tmp_UorL = false;
            }
        }
        Line tmp_Line = new Line(tmp_Height, tmp_Angle, tmp_UorL, tmp_HorV, in_Red.group, in_Blue.group);
    }

    private static void MakeLineDot2Line(Dot in_Base, Dot in_LineA, Dot in_LineB, bool in_IsRedDot) {
        float tmp_Angle, tmp_Height;
        bool tmp_UorL, tmp_HorV;
        if (in_LineA.pos.x == in_LineB.pos.x) {
            tmp_HorV = true;
            tmp_Angle = 1;
            tmp_Height = (in_LineA.pos.x + in_Base.pos.x) / 2;
            if (in_Base.pos.x < in_LineA.pos.x) {
                tmp_UorL = true;
            } else {
                tmp_UorL = false;
            }
        } else if (in_LineA.pos.y == in_LineB.pos.y) {
            tmp_HorV = true;
            tmp_Angle = 0;
            tmp_Height = (in_LineA.pos.y + in_Base.pos.y) / 2;
            if (in_Base.pos.y > in_LineA.pos.y) {
                tmp_UorL = true;
            } else {
                tmp_UorL = false;
            }
        } else {
            tmp_HorV = false;
            tmp_Angle = (in_LineA.pos.y - in_LineB.pos.y) / (in_LineA.pos.x - in_LineB.pos.x);
            tmp_Height = (in_LineA.pos.y + in_Base.pos.y - tmp_Angle * (in_LineA.pos.x + in_Base.pos.x)) / 2;
            if (in_Base.pos.y > tmp_Angle * in_Base.pos.x + tmp_Height) {
                tmp_UorL = true;
            } else {
                tmp_UorL = false;
            }
        }
        if (in_IsRedDot) {
            Line tmp_Line = new Line(tmp_Height, tmp_Angle, tmp_UorL, tmp_HorV, in_Base.group, in_LineA.group);
        } else {
            tmp_UorL = !tmp_UorL;
            Line tmp_Line = new Line(tmp_Height, tmp_Angle, tmp_UorL, tmp_HorV, in_LineA.group, in_Base.group);
        }
    }

    public static void UpdateOuter(Dot in_JudgeDot, Group in_Group) {
        if (IsInPolygon(in_JudgeDot, in_Group)) {
            //現状維持
        } else {
            Dot lowDot, highDot;
            SteepDot(in_JudgeDot, out lowDot, out highDot, in_Group.outerDotList);
            int lowNum = lowDot.borderNom;
            int highNum = highDot.borderNom;
            if (lowNum > highNum) {
                lowNum += highNum;
                highNum = lowNum - highNum;
                lowNum -= highNum;
            }

            if (highNum - lowNum == 1) {//差が１の時
                int addHighNum = highNum + 1;
                if (addHighNum == in_Group.outerDotList.Count) {
                    addHighNum = 0;
                }
                if (IsInTriangle(in_Group.outerDotList[addHighNum].pos, in_JudgeDot.pos, lowDot.pos, highDot.pos)) {//三角形
                    LinkDotAfter(in_JudgeDot, in_Group, lowNum, highNum);
                } else {
                    LinkDotBetween(in_JudgeDot, in_Group, lowNum, highNum);
                }
            } else if (IsInTriangle(in_Group.outerDotList[lowNum + 1].pos, in_JudgeDot.pos, lowDot.pos, highDot.pos)) {
                LinkDotBetween(in_JudgeDot, in_Group, lowNum, highNum);
            } else {
                LinkDotAfter(in_JudgeDot, in_Group, lowNum, highNum);
            }
        }
    }

    private static void LinkDotBetween(Dot in_LinkDot, Group in_Group, int in_SmallerNum, int in_LargerNum) {
        List<Dot> tmp_List = new List<Dot>();
        bool tmp_Look = false;
        for (int i = 0; i < in_Group.outerDotList.Count; i++) {
            if (tmp_Look) {
                if (i == in_LargerNum) {
                    tmp_Look = false;
                    tmp_List.Add(in_Group.outerDotList[i]);
                }
            } else {
                tmp_List.Add(in_Group.outerDotList[i]);
                if (i == in_SmallerNum) {
                    tmp_List.Add(in_LinkDot);
                    tmp_Look = true;
                }
            }
        }
        in_Group.ResetOuter();
        for (int i = 0; i < tmp_List.Count; i++) {
            in_Group.AddOuter(tmp_List[i]);
        }
    }

    private static void LinkDotAfter(Dot in_LinkDot, Group in_Group, int in_SmallerNum, int in_LargerNum) {
        List<Dot> tmp_List = new List<Dot>();
        for (int i = in_SmallerNum; i < in_LargerNum + 1; i++) {
            tmp_List.Add(in_Group.outerDotList[i]);
        }
        tmp_List.Add(in_LinkDot);
        in_Group.ResetOuter();
        for (int i = 0; i < tmp_List.Count; i++) {
            in_Group.AddOuter(tmp_List[i]);
        }
    }

    public static bool IsEqual(float in_A, float in_B) {
        return System.Math.Abs(in_A - in_B) < 0.0001f;
    }

    public static bool IsSmaller(float in_Small, float in_Big) {
        return in_Small - in_Big <= 0.0001f;
    }

    public static void Exit(string message) {
        Debug.Log(message);
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }

}
