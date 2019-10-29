using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshMake {

    public const float LineWeight = 1;
    public static int mesh_verticies = 8;
    public static float dotRadious = 30.0f;

    public static Mesh LineMesh(float in_Angle, float in_Height, bool is_HorV) {
        Mesh re_Mesh = new Mesh();
        if (is_HorV) {//縦か横の直線
            if (in_Angle == 0) {//横の直線
                Vector3[] tmp_vertices = new Vector3[] {
                    new Vector2(Main.left,in_Height+LineWeight),
                    new Vector2(Main.left,in_Height-LineWeight),
                    new Vector2(Main.right,in_Height-LineWeight),
                    new Vector2(Main.right,in_Height+LineWeight)
                };
                re_Mesh.vertices = tmp_vertices;
            } else {//縦の直線
                Vector3[] tmp_vertices = new Vector3[] {
                    new Vector2(in_Height-LineWeight,Main.up),
                    new Vector2(in_Height-LineWeight,Main.down),
                    new Vector2(in_Height+LineWeight,Main.down),
                    new Vector2(in_Height+LineWeight,Main.up)
                };
                re_Mesh.vertices = tmp_vertices;
            }
        } else {
            float tmp_DifHeight = LineWeight * (float)Mathf.Sqrt(1 + in_Angle * in_Angle);
            Vector3[] tmp_vertices = new Vector3[]{
                LineEdgePos(in_Angle,in_Height+tmp_DifHeight,false),
                LineEdgePos(in_Angle,in_Height-tmp_DifHeight,false),
                LineEdgePos(in_Angle,in_Height-tmp_DifHeight,true),
                LineEdgePos(in_Angle,in_Height+tmp_DifHeight,true)
            };
            re_Mesh.vertices = tmp_vertices;
        }
        int[] tmp_triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        re_Mesh.triangles = tmp_triangles;
        return re_Mesh;
    }

    private static Vector2 LineEdgePos(float in_Angle, float in_Height, bool isLeft) {
        float judgeX = Main.right;
        if (isLeft) {
            judgeX = Main.left;
        }

        float tmp_y = in_Angle * judgeX + in_Height;
        if (Main.up < tmp_y) {//画面上部
            return new Vector2((Main.up - in_Height) / in_Angle, Main.up);
        } else if (tmp_y < Main.down) {//画面下部
            return new Vector2((Main.down - in_Height) / in_Angle, Main.down);
        } else {//画面左部/右部
            return new Vector2(judgeX, tmp_y);
        }
    }

    private static Mesh LimitedLineMesh(Vector2 A, Vector2 B) {
        Mesh re_Mesh = new Mesh();
        Vector3[] tmp_vertices = new Vector3[4];
        if (A.x == B.x) {
            tmp_vertices[0] = new Vector2(A.x - LineWeight / 2, A.y);
            tmp_vertices[1] = new Vector2(A.x + LineWeight / 2, A.y);
            tmp_vertices[2] = new Vector2(B.x + LineWeight / 2, B.y);
            tmp_vertices[3] = new Vector2(B.x - LineWeight / 2, B.y);
        } else if (A.y == B.y) {
            tmp_vertices[0] = new Vector2(A.x, A.y + LineWeight / 2);
            tmp_vertices[1] = new Vector2(A.x, A.y - LineWeight / 2);
            tmp_vertices[2] = new Vector2(B.x, B.y - LineWeight / 2);
            tmp_vertices[3] = new Vector2(B.x, B.y + LineWeight / 2);
        } else {
            float tan = (A.y - B.y) / (A.x - B.x);
            float sin = tan / (Mathf.Sqrt(1 + tan * tan));
            float cos = 1 / (Mathf.Sqrt(1 + tan * tan));
            tmp_vertices[0] = new Vector2(A.x - LineWeight * sin / 2, A.y + LineWeight * cos / 2);
            tmp_vertices[1] = new Vector2(A.x + LineWeight * sin / 2, A.y - LineWeight * cos / 2);
            tmp_vertices[2] = new Vector2(B.x + LineWeight * sin / 2, B.y - LineWeight * cos / 2);
            tmp_vertices[3] = new Vector2(B.x - LineWeight * sin / 2, B.y + LineWeight * cos / 2);
        }
        re_Mesh.vertices = tmp_vertices;
        re_Mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        return re_Mesh;
    }

    public static Mesh OuterMesh(List<Dot> in_DotList, Vector2 in_COG) {
        if (in_DotList.Count == 2) {
            return LimitedLineMesh(in_DotList[0].pos, in_DotList[1].pos);
        }
        Mesh re_Mesh = new Mesh();
        for (int i = 0; i < in_DotList.Count; i++) {
            int j = i + 1;
            if (j == in_DotList.Count) {
                j = 0;
            }
            Mesh tmp_Mesh = MakeTriangleMesh(in_COG, in_DotList[i].pos, in_DotList[j].pos);
            re_Mesh = MergeMesh(re_Mesh, tmp_Mesh);
        }
        return re_Mesh;
    }

    public static Mesh RangeMesh(List<Line> in_LineList,bool in_IsRed) {
        List<Vector2> tmp_PosList = new List<Vector2>();
        List<int[]> tmp_LineNumList = new List<int[]>();
        List<Vector2> tmp_SafePosList = new List<Vector2>();
        tmp_PosList.Add(new Vector2(Main.left,Main.up));
        tmp_PosList.Add(new Vector2(Main.left,Main.down));
        tmp_PosList.Add(new Vector2(Main.right,Main.up));
        tmp_PosList.Add(new Vector2(Main.right,Main.down));
        tmp_LineNumList.Add(new int[2] { 0, 0 });
        tmp_LineNumList.Add(new int[2] { 0, 0 });
        tmp_LineNumList.Add(new int[2] { 0, 0 });
        tmp_LineNumList.Add(new int[2] { 0, 0 });
        for (int i = 0; i < in_LineList.Count; i++) {
            tmp_PosList.Add(in_LineList[i].EdgePos(true));
            tmp_PosList.Add(in_LineList[i].EdgePos(false));
            tmp_LineNumList.Add(new int[2] { in_LineList[i].lineNum, in_LineList[i].lineNum });
            tmp_LineNumList.Add(new int[2] { in_LineList[i].lineNum, in_LineList[i].lineNum });
            for (int j = i + 1; j < in_LineList.Count; j++) {
                int tmp_Count = tmp_PosList.Count;
                LineIntersectionPos(in_LineList[i],in_LineList[j],ref tmp_PosList);
                if (tmp_Count!=tmp_PosList.Count) {
                    tmp_LineNumList.Add(new int[2] { in_LineList[i].lineNum, in_LineList[j].lineNum });
                }
            }
        }

        for (int i=0;i<tmp_PosList.Count;i++) {
            bool tmp_safe = true;
            for (int j = 0; j < in_LineList.Count&&tmp_safe; j++) {
                if (in_LineList[j].lineNum!=tmp_LineNumList[i][0]&& in_LineList[j].lineNum != tmp_LineNumList[i][1]) {
                    tmp_safe = in_LineList[j].IsCorrectlyJudged(tmp_PosList[i], in_IsRed) == in_IsRed;
                }
            }
            if (tmp_safe) {
                tmp_SafePosList.Add(tmp_PosList[i]);
            }
        }

        Group tmp_Group = new Group(in_IsRed);
        for (int i=0;i<tmp_SafePosList.Count;i++) {
            tmp_Group.AddDot(new Dot(tmp_SafePosList[i],in_IsRed));
        }
        return tmp_Group.GetOuterMesh();
    }

    private static void LineIntersectionPos(Line in_ALine, Line in_BLine, ref List<Vector2> ref_PosList) {
        if (in_ALine.isHorV && in_BLine.isHorV) {//両方縦か横の直線
            if ((in_ALine.angle != 0 && in_BLine.angle == 0)) {//Aが縦でBが横
                ref_PosList.Add(new Vector2(in_ALine.height, in_BLine.height));
            } else if ((in_ALine.angle == 0 && in_BLine.angle != 0)) {//Aが横でBが縦
                ref_PosList.Add(new Vector2(in_BLine.height, in_ALine.height));
            }
        } else if (in_ALine.isHorV) {//Aのみ縦か横の直線
            VerticalLineIntersectionPos(in_ALine, in_BLine, ref ref_PosList);
        } else if (in_BLine.isHorV) {//Bのみ縦か横の直線
            VerticalLineIntersectionPos(in_BLine, in_ALine, ref ref_PosList);
        } else {
            float tmp_X = (in_ALine.height - in_BLine.height) / (in_BLine.angle - in_ALine.angle);
            float tmp_Y = (in_ALine.height * in_BLine.angle - in_BLine.height * in_ALine.angle) / (in_BLine.angle - in_ALine.angle);
            if (Main.left < tmp_X && tmp_X < Main.right && Main.down < tmp_Y && tmp_Y < Main.up) {
                ref_PosList.Add(new Vector2(tmp_X, tmp_Y));
            }
        }
    }

    private static void VerticalLineIntersectionPos(Line in_VerticalLine, Line in_ALine, ref List<Vector2> ref_PosList) {
        if (in_VerticalLine.angle == 0) {//Aが横の直線
            float tmp_X = (in_VerticalLine.height - in_ALine.height) / in_ALine.angle;
            if (Main.left < tmp_X && tmp_X < Main.right) {
                ref_PosList.Add(new Vector2(tmp_X, in_VerticalLine.height));
            }
        } else {//Aが縦の直線
            float tmp_Y = in_VerticalLine.height * in_ALine.angle + in_ALine.height;
            if (Main.down < tmp_Y && tmp_Y < Main.up) {
                ref_PosList.Add(new Vector2(in_VerticalLine.height, tmp_Y));
            }
        }
    }

    private static Mesh MakeTriangleMesh(Vector2 in_PosA, Vector2 in_PosB, Vector2 in_PosC) {
        Mesh re_Mesh = new Mesh();
        Vector3[] tmp_vertices = new Vector3[] { in_PosA, in_PosB, in_PosC };
        int[] tmp_triangles = new int[] { 0, 1, 2 };
        re_Mesh.vertices = tmp_vertices;
        re_Mesh.triangles = tmp_triangles;
        return re_Mesh;
    }

    private static Mesh MergeMesh(Mesh in_Mesh1, Mesh in_Mesh2) {
        Mesh re_Mesh = new Mesh();
        CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = in_Mesh1;
        combine[0].transform = Main.mainTransform.localToWorldMatrix;
        combine[1].mesh = in_Mesh2;
        combine[1].transform = Main.mainTransform.localToWorldMatrix;
        re_Mesh.CombineMeshes(combine);
        return re_Mesh;
    }

    public static void AddDotMesh(Vector2 in_Pos, ref Mesh ref_DotMesh) {
        var tmp_oldMesh = new Mesh();
        tmp_oldMesh.vertices = ref_DotMesh.vertices;
        tmp_oldMesh.triangles = ref_DotMesh.triangles;
        var tmp_newMesh = MakePolygonMesh(in_Pos);
        ref_DotMesh = MeshMake.MergeMesh(tmp_newMesh, ref_DotMesh);
    }

    public static Mesh MakePolygonMesh(Vector2 in_Pos) {//引数の座標中心のMeshを返す
        Vector3[] tmp_vertices = new Vector3[mesh_verticies + 1];
        int[] tmp_triangles = new int[mesh_verticies * 3];
        for (int i = 0; i < mesh_verticies; i++) {
            float _tmp_angle = (360 / mesh_verticies) * (-i) * Mathf.Deg2Rad;
            tmp_vertices[i + 1] = new Vector3(in_Pos.x + Mathf.Cos(_tmp_angle) * dotRadious, in_Pos.y + Mathf.Sin(_tmp_angle) * dotRadious, 0);
            tmp_triangles[3 * i] = 0;
            tmp_triangles[3 * i + 1] = i + 1;
            tmp_triangles[3 * i + 2] = i + 2;
        }
        tmp_vertices[0] = in_Pos;
        tmp_triangles[mesh_verticies * 3 - 1] = 1;
        Mesh re_Mesh = new Mesh();
        re_Mesh.vertices = tmp_vertices;
        re_Mesh.triangles = tmp_triangles;
        return re_Mesh;
    }

}
