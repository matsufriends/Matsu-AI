using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line {
    public float height { get; private set; }
    public float angle { get; private set; }
    public int lineNum { get; private set; }
    private bool isUorL;
    public bool isHorV { get; private set; }
    private Group redGroup;
    private Group blueGroup;

    public Mesh lineMesh { get; private set; }

    public Line(float in_Height, float in_Angle, bool in_UorL, bool in_HorV, Group in_RedGroup, Group in_BlueGroup) {
        height = in_Height;
        angle = in_Angle;
        isUorL = in_UorL;
        isHorV = in_HorV;
        lineNum = Core.SetLineNum();
        redGroup = in_RedGroup;
        blueGroup = in_BlueGroup;
        redGroup.SetLine(this);
        blueGroup.SetLine(this);
        lineMesh = MeshMake.LineMesh(in_Angle, in_Height, in_HorV);
    }

    public bool IsCorrectlyJudged(Vector2 in_Pos,bool in_IsOnSafe) {
        if (angle == 0 && isHorV == false) {
            Core.Exit("Line has error");
            return false;
        }
        if (isHorV) {
            if (angle == 0) {//横直線
                if (Core.IsEqual(in_Pos.y, height)) {
                    return in_IsOnSafe;
                }
                if (isUorL) {//上
                    return in_Pos.y > height;
                } else {//下
                    return in_Pos.y < height;
                }
            } else {//縦直線
                if (Core.IsEqual(in_Pos.x, height)) {
                    return in_IsOnSafe;
                }
                if (isUorL) {//左
                    return in_Pos.x < height;
                } else {//右
                    return in_Pos.x > height;
                }
            }
        } else {//垂直でない直線
            if (Core.IsEqual(in_Pos.y, angle * in_Pos.x + height)) {
                return in_IsOnSafe;
            }
            if (isUorL) {//上
                return in_Pos.y > angle * in_Pos.x + height;
            } else {//下
                return in_Pos.y < angle * in_Pos.x + height;
            }
        }
    }

    public Group OtherGroup(Group in_BaseGroup) {
        if (in_BaseGroup == redGroup) {
            return blueGroup;
        } else if (in_BaseGroup == blueGroup) {
            return redGroup;
        } else {
            Core.Exit("Stranger Group");
            return null;
        }
    }

    public Vector2 EdgePos(bool in_IsUorL) {
        if (isHorV) {//縦か横
            if (angle==0) {//横の直線
                if (in_IsUorL) {//左側
                    return new Vector2(Main.display_Left,height);
                } else {//右側
                    return new Vector2(Main.display_Right, height);
                }
            } else {//縦の直線
                if (in_IsUorL) {//上側
                    return new Vector2(height,Main.display_Up);
                } else {//下側
                    return new Vector2(height,Main.display_Down);
                }
            }
        } else {
            if (in_IsUorL) {//中心より左側
                float tmp_y = angle * Main.display_Left + height;
                if (Main.display_Down<tmp_y&&tmp_y<Main.display_Up) {//左端
                    return new Vector2(Main.display_Left,tmp_y);
                } else if (tmp_y < Main.display_Down) {//下側
                    return new Vector2((Main.display_Down-height)/angle,Main.display_Down);
                } else {//上側
                    return new Vector2((Main.display_Up - height) / angle, Main.display_Up);
                }
            } else {//中心より右側
                float tmp_y = angle * Main.display_Right + height;
                if (Main.display_Down < tmp_y && tmp_y < Main.display_Up) {//→端
                    return new Vector2(Main.display_Right, tmp_y);
                } else if (tmp_y < Main.display_Down) {//下側
                    return new Vector2((Main.display_Down - height) / angle, Main.display_Down);
                } else {//上側
                    return new Vector2((Main.display_Up - height) / angle, Main.display_Up);
                }
            }
        }
    }

}
