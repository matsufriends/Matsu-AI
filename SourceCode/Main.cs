using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    [SerializeField]
    private Material redMaterial = null;
    [SerializeField]
    private Material blueMaterial = null;
    [SerializeField]
    private Material mouseMaterial = null;
    [SerializeField]
    private Material lineMaterial = null;

    [SerializeField]
    private Toggle ToggleRedRange = null;
    [SerializeField]
    private Toggle ToggleBlueRange = null;
    [SerializeField]
    private Toggle ToggleGroup = null;
    [SerializeField]
    private Toggle ToggleLine = null;
    [SerializeField]
    private Toggle ToggleStatus = null;
    [SerializeField]
    private InputField InputX = null;
    [SerializeField]
    private InputField InputY = null;
    [SerializeField]
    private InputField InputMergeK = null;
    [SerializeField]
    private InputField InputRadious = null;
    [SerializeField]
    private InputField InputVertices = null;
    [SerializeField]
    private Text redValue = null;
    [SerializeField]
    private Text blueValue = null;
    [SerializeField]
    private Text totalValue = null;


    public static Console console;
    private Mesh red_dotMesh;
    private Mesh blue_dotMesh;
    private Mesh mouse_Mesh;

    private static List<Dot> redMom;
    private static List<Dot> blueMom;
    private static List<Group> redGroupList;
    private static List<Group> blueGroupList;

    public static Transform mainTransform;
    public static float up;
    public static float left;
    public static float right;
    public static float down;
    private bool isProcessing = false;
    private bool isReset = false;
    private float timeToTook = 0;
    private string putLog = "";

    void Start() {
        if (redMaterial == null || blueMaterial == null || mouseMaterial == null || lineMaterial == null) {
            Core.Exit("Material isn't assigned");
        }
        if (ToggleRedRange == null || ToggleBlueRange == null || ToggleGroup == null || ToggleLine == null || ToggleStatus == null) {
            Core.Exit("Toggle isn't assigned");
        }
        if (InputX == null || InputY == null || InputMergeK == null || InputRadious == null || InputVertices == null) {
            Core.Exit("InputField isn't assigned");
        }

        console = gameObject.GetComponent<Console>();
        Core.groupMergeK = 1.5f;
        console.AllReset();
        mainTransform = transform;
        Cursor.visible = false;

        up = Screen.currentResolution.height / 2 - 10 - MeshMake.dotRadious;
        down = -Screen.currentResolution.height / 2 + 10 + MeshMake.dotRadious;
        left = -Screen.currentResolution.width / 2 + 10 + MeshMake.dotRadious;
        right = Screen.currentResolution.width / 2 - 500 - MeshMake.dotRadious;

        red_dotMesh = new Mesh();
        blue_dotMesh = new Mesh();
        mouse_Mesh = MeshMake.MakePolygonMesh(Vector3.zero);
        redMom = new List<Dot>();
        blueMom = new List<Dot>();
        redGroupList = new List<Group>();
        blueGroupList = new List<Group>();
        DefinedPut();
    }

    void DefinedPut() {
        /*
        for (int i=0;i<250;i++) {
            RandomPut(0);
        }
        UnityEngine.Debug.Log(putLog);
        */

        //100
        // DP(369, 393, true); DP(176, -429, false); DP(-110, 26, false); DP(289, 409, true); DP(286, 296, false); DP(-312, 330, false); DP(90, -245, true); DP(305, 209, false); DP(-487, -188, true); DP(-376, -8, true); DP(47, -440, false); DP(-829, -81, true); DP(-619, -101, false); DP(-394, -346, false); DP(-733, -96, true); DP(-91, 442, false); DP(-594, 491, true); DP(-360, -200, true); DP(419, 237, false); DP(-117, -484, false); DP(-767, 356, true); DP(-528, 321, true); DP(-784, -230, true); DP(-596, -215, false); DP(-622, 23, true); DP(-662, 267, true); DP(-870, -371, false); DP(-106, 127, true); DP(-487, 64, false); DP(31, 482, true); DP(-824, -475, true); DP(-259, 227, false); DP(-850, -314, true); DP(323, -58, false); DP(-37, -494, false); DP(-9, -208, false); DP(80, 157, true); DP(-633, 169, false); DP(-139, 274, true); DP(-248, 475, true); DP(378, -311, true); DP(388, -61, false); DP(327, -231, true); DP(-214, -282, true); DP(-876, 385, true); DP(163, -12, false); DP(213, 82, false); DP(-436, 275, false); DP(-911, -152, false); DP(-846, 149, true); DP(-629, 440, false); DP(339, 110, false); DP(-406, 428, false); DP(-694, -261, true); DP(-751, -284, false); DP(-139, -146, true); DP(-359, -454, true); DP(-689, 45, true); DP(-675, 340, false); DP(31, 228, false); DP(-155, -345, true); DP(-23, 33, false); DP(-431, -264, false); DP(171, 269, true); DP(85, -97, true); DP(-27, -367, true); DP(-159, 194, false); DP(96, 481, true); DP(55, 390, true); DP(206, 459, true); DP(-709, 127, false); DP(-498, 172, false); DP(-324, -377, true); DP(403, 463, true); DP(-324, 203, false); DP(-165, -12, false); DP(-273, -8, false); DP(-862, 485, false); DP(261, -467, false); DP(278, -156, false); DP(-337, -73, true); DP(-441, 122, false); DP(-82, -73, false); DP(304, 44, false); DP(-346, -295, true); DP(24, -299, false); DP(-521, -384, false); DP(-28, 288, true); DP(350, -142, false); DP(-880, 284, true); DP(7, 116, false); DP(175, -263, true); DP(138, 121, true); DP(-916, 179, false); DP(-758, 180, true); DP(72, -7, true); DP(-771, 484, false); DP(-587, 85, false); DP(-162, -218, true); DP(-743, 245, false);

        //200
        DP(-728, 216, true); DP(249, 37, false); DP(-644, -367, true); DP(1, 315, true); DP(-518, -196, false); DP(-193, 153, false); DP(-738, 306, true); DP(-566, -388, false); DP(382, -14, false); DP(-635, 221, true); DP(-360, 304, false); DP(-396, 436, false); DP(287, -103, false); DP(-285, 138, false); DP(-912, -423, false); DP(122, -98, false); DP(-730, -393, true); DP(-913, 128, true); DP(242, -281, false); DP(-323, 88, false); DP(194, 483, true); DP(311, -460, false); DP(-452, 160, false); DP(-14, -296, false); DP(-720, -60, false); DP(-74, 300, true); DP(-38, -77, true); DP(-235, -393, true); DP(362, -398, false); DP(-44, -380, true); DP(29, 234, true); DP(-660, 285, false); DP(-555, -76, true); DP(205, -83, false); DP(-206, 448, false); DP(-813, -465, true); DP(-167, -257, true); DP(-399, 130, false); DP(101, -214, true); DP(-745, -249, false); DP(-326, -203, true); DP(-27, 494, true); DP(43, -44, true); DP(-334, -16, false); DP(-916, -19, false); DP(242, -35, false); DP(117, -345, false); DP(377, -128, true); DP(-875, 235, false); DP(-359, -129, false); DP(134, -437, true); DP(-668, -106, true); DP(348, 97, false); DP(425, 156, true); DP(-773, 423, false); DP(-646, 75, false); DP(33, -424, false); DP(-843, 97, false); DP(-483, -275, true); DP(-90, 196, false); DP(-423, 354, true); DP(-505, 462, false); DP(117, 258, true); DP(-480, -58, false); DP(17, -105, false); DP(-126, 65, false); DP(360, 258, false); DP(-531, 404, false); DP(267, 359, true); DP(-181, 224, false); DP(-173, -413, false); DP(-246, 380, false); DP(-66, -454, true); DP(-698, 424, true); DP(-148, -333, true); DP(-588, -237, true); DP(9, 116, false); DP(-381, -329, false); DP(-587, -128, false); DP(-528, 222, false); DP(-917, -86, true); DP(-737, 62, true); DP(131, -16, false); DP(-175, 23, true); DP(-828, -39, true); DP(-915, -295, true); DP(-554, 53, true); DP(-805, -378, false); DP(-125, 364, true); DP(-145, -69, true); DP(-284, -56, false); DP(-327, -397, false); DP(-150, -147, false); DP(60, 470, false); DP(228, -484, false); DP(-825, 342, true); DP(-603, 458, false); DP(-160, 286, true); DP(-293, -467, true); DP(207, -405, true); DP(-569, 276, false); DP(387, -310, true); DP(137, 438, false); DP(-182, 345, true); DP(-634, 365, false); DP(-642, -33, false); DP(-648, -219, true); DP(-325, 391, true); DP(-832, -197, true); DP(60, 68, true); DP(270, 256, true); DP(243, 124, true); DP(378, -206, false); DP(206, 257, false); DP(-33, -169, true); DP(-257, -185, false); DP(-617, -457, false); DP(-891, 466, true); DP(172, -200, true); DP(-406, -403, false); DP(379, -493, true); DP(-583, 168, true); DP(355, 450, false); DP(19, 17, false); DP(296, -357, false); DP(-661, 475, false); DP(286, 443, true); DP(-396, 496, true); DP(-489, -466, false); DP(279, -217, false); DP(218, 316, true); DP(-776, -91, true); DP(130, -283, true); DP(-755, -6, false); DP(-19, 377, false); DP(296, 169, true); DP(-865, 408, false); DP(-292, 449, true); DP(-503, 300, true); DP(-394, -203, true); DP(413, -433, false); DP(-920, -153, false); DP(333, 393, true); DP(-426, 75, false); DP(-484, 107, true); DP(-749, -494, false); DP(-65, 83, true); DP(157, 330, true); DP(326, 318, true); DP(-805, 202, true); DP(-902, 47, false); DP(78, 403, false); DP(-409, -30, false); DP(11, -228, false); DP(-696, -162, true); DP(421, 468, true); DP(-283, -129, true); DP(-204, -473, false); DP(169, 188, false); DP(-557, -464, true); DP(-265, 20, true); DP(-431, 275, false); DP(54, -324, true); DP(-868, -369, true); DP(191, 61, true); DP(-717, 126, true); DP(-130, 144, false); DP(-274, 234, false); DP(313, -20, false); DP(-237, -281, true); DP(418, 361, false); DP(-351, 189, false); DP(-131, -489, true); DP(239, 192, false); DP(-93, -168, false); DP(38, -174, true); DP(-287, -352, false); DP(-823, 269, true); DP(-377, 244, true); DP(-814, -276, false); DP(-221, -58, false); DP(-912, -233, false); DP(-497, -406, true); DP(-742, 499, true); DP(-258, 298, false); DP(-138, 470, false); DP(196, -144, true); DP(-884, -480, true); DP(195, 411, false); DP(-303, -288, true); DP(-593, -328, true); DP(428, 274, false); DP(-31, 175, true); DP(-409, -486, false); DP(-409, -267, false); DP(-674, -427, true); DP(125, 107, true); DP(-216, -121, false); DP(-848, -106, false); DP(-858, 168, false);
    }

    void DP(int x,int y,bool isRed) {
        Put(x,y,isRed);
    }

    void Update() {
        Vector2 tmp_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool tmp_IsMouseSafe = CanPut(tmp_MousePos);
        if (tmp_IsMouseSafe) {
            mouseMaterial.color = Color.black;
        } else {
            mouseMaterial.color = Color.green;
        }

        bool tmp_Shift = Input.GetKey("left shift");

        if (Input.GetKeyDown("tab")) {
            if (tmp_Shift) {
                if (InputX.isFocused) {
                    InputVertices.Select();
                } else if (InputMergeK.isFocused) {
                    InputY.Select();
                } else if (InputRadious.isFocused) {
                    InputMergeK.Select();
                } else if (InputVertices.isFocused) {
                    InputRadious.Select();
                } else{
                    InputX.Select();
                }
            } else {
                if (InputX.isFocused) {
                    InputY.Select();
                } else if (InputY.isFocused) {
                    InputMergeK.Select();
                } else if (InputMergeK.isFocused) {
                    InputRadious.Select();
                } else if (InputRadious.isFocused) {
                    InputVertices.Select();
                } else {
                    InputX.Select();
                }
            }
        }

        if (Input.GetKeyDown("return")) {
            if (tmp_Shift) {
                PutButton(false);
            } else {
                PutButton(true);
            }
        }

        if (tmp_IsMouseSafe) {
            if (Input.GetMouseButtonDown(0)) {//左クリック
                Put(tmp_MousePos.x, tmp_MousePos.y, true);
            }
            if (Input.GetMouseButtonDown(1)) {//右クリック
                Put(tmp_MousePos.x, tmp_MousePos.y, false);
            }
            if (Input.GetMouseButtonDown(2)) {//中クリック
                bool tmp_Red = Judging(tmp_MousePos, redGroupList, true);
                bool tmp_Blue = Judging(tmp_MousePos, blueGroupList, false);
                if (!tmp_Red && !tmp_Blue) {
                    console.AddText(tmp_MousePos + "はどちらでも可能です");
                } else if (tmp_Red) {
                    console.AddText(tmp_MousePos + "は赤です");
                } else {
                    console.AddText(tmp_MousePos + "は青です");
                }
            }
        }
        Graphics.DrawMesh(red_dotMesh, Vector3.zero, Quaternion.identity, redMaterial, 0);
        Graphics.DrawMesh(blue_dotMesh, Vector3.zero, Quaternion.identity, blueMaterial, 0);
        Graphics.DrawMesh(mouse_Mesh, tmp_MousePos, Quaternion.identity, mouseMaterial, 0);
        if (ToggleGroup.isOn) {
            for (int i = 0; i < redGroupList.Count; i++) {
                Graphics.DrawMesh(redGroupList[i].GetOuterMesh(), Vector3.zero, Quaternion.identity, redMaterial, 0);
            }
            for (int i = 0; i < blueGroupList.Count; i++) {
                Graphics.DrawMesh(blueGroupList[i].GetOuterMesh(), Vector3.zero, Quaternion.identity, blueMaterial, 0);
            }
        }
        if (ToggleLine.isOn) {
            for (int i = 0; i < redGroupList.Count; i++) {
                for (int j = 0; j < redGroupList[i].lineList.Count; j++) {
                    Graphics.DrawMesh(redGroupList[i].lineList[j].lineMesh, Vector3.zero, Quaternion.identity, lineMaterial, 0);
                }
            }
        }
        if (ToggleRedRange.isOn) {
            for (int i = 0; i < redGroupList.Count; i++) {
                Graphics.DrawMesh(redGroupList[i].GetRangeMesh(), Vector3.zero, Quaternion.identity, redMaterial, 0);
            }
        }
        if (ToggleBlueRange.isOn) {
            for (int i = 0; i < blueGroupList.Count; i++) {
                Graphics.DrawMesh(blueGroupList[i].GetRangeMesh(), Vector3.zero, Quaternion.identity, blueMaterial, 0);
            }
        }
        if (ToggleStatus.isOn) {
            console.DrawDotStatus(redMom, blueMom);
        } else {
            console.RemoveDotStatus();
        }
    }

    private bool CanPut(Vector2 in_PutPos) {
        if (in_PutPos.x < left || right < in_PutPos.x) {
            return false;
        }
        if (in_PutPos.y < down || up < in_PutPos.y) {
            return false;
        }
        for (int i = 0; i < redMom.Count; i++) {
            if ((redMom[i].pos - in_PutPos).magnitude < MeshMake.dotRadious * 2) {
                return false;
            }
        }
        for (int i = 0; i < blueMom.Count; i++) {
            if ((blueMom[i].pos - in_PutPos).magnitude < MeshMake.dotRadious * 2) {
                return false;
            }
        }
        return true;
    }

    void Put(float in_X, float in_Y, bool in_IsRed) {
        if (in_IsRed) {
            putLog += "DP(" + in_X + "," + in_Y + ",true);";
        } else {
            putLog += "DP(" + in_X + "," + in_Y + ",false);";
        }
        
        if (!isProcessing) {
            Vector2 tmp_Pos = new Vector2(in_X, in_Y);
            if (in_IsRed) {
                Dot tmp_Dot = new Dot(tmp_Pos, true);
                redMom.Add(tmp_Dot);
                MeshMake.AddDotMesh(tmp_Pos, ref red_dotMesh);
            } else {
                Dot tmp_Dot = new Dot(tmp_Pos, false);
                blueMom.Add(tmp_Dot);
                MeshMake.AddDotMesh(tmp_Pos, ref blue_dotMesh);
            }
            StartProcessing();
        }
    }

    private void StartProcessing() {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        isProcessing = true;
        if (isReset) {
            console.ResetText();
            console.AddText("再構成します");
            redGroupList = new List<Group>();
            blueGroupList = new List<Group>();
            for (int i = 0; i < redMom.Count; i++) {
                redMom[i].LeaveGroup();
            }
            for (int i = 0; i < blueMom.Count; i++) {
                blueMom[i].LeaveGroup();
            }
            isReset = false;
        }
        ClassifyDot();
        IncludeGroup(0);
        CalculateBorderLine();
        StraddleGroup(0);
        BreakSoloGroup(0);

        isProcessing = false;
        sw.Stop();
        timeToTook += (float)sw.ElapsedMilliseconds;
        console.AddText("処理時間:" + sw.ElapsedMilliseconds + "ms" + " 合計処理時間:" + timeToTook + "ms");
        redValue.text = redMom.Count + "コ";
        blueValue.text = blueMom.Count + "コ";
        totalValue.text = "計" + (redMom.Count + blueMom.Count) + "コ";
    }

    private void ClassifyDot() {
        if (redMom.Count > 0 && blueMom.Count > 0) {
            Core.ClassifyPerColor(redGroupList, redMom, blueMom);
            Core.ClassifyPerColor(blueGroupList, blueMom, redMom);
        }
    }

    private void IncludeGroup(int in_Deep) {
        if (in_Deep != 10) {
            bool tmp_AllOK = true;
            List<Group> tmp_redGroupList = new List<Group>();
            List<Group> tmp_blueGroupList = new List<Group>();
            Core.AdjustIncludePerGroup(redGroupList, tmp_redGroupList, blueMom, ref tmp_AllOK);
            Core.AdjustIncludePerGroup(blueGroupList, tmp_blueGroupList, redMom, ref tmp_AllOK);
            if (!tmp_AllOK) {
                isReset = true;
                redGroupList = tmp_redGroupList;
                blueGroupList = tmp_blueGroupList;
                ClassifyDot();
                IncludeGroup(in_Deep + 1);
            }
        } else {
            Core.Exit("IncludeGroup has Heavy Processing");
        }
    }

    private void CalculateBorderLine() {
        for (int i = 0; i < redGroupList.Count; i++) {
            for (int j = 0; j < blueGroupList.Count; j++) {
                if (!redGroupList[i].HaveLine(blueGroupList[j])) {
                    Core.ClassfyBorderLinePattern(redGroupList[i], blueGroupList[j]);
                }
            }
        }
    }

    private void StraddleGroup(int in_Deep) {
        if (in_Deep < 10) {
            bool tmp_AllOK = true;
            List<Group> tmp_redGroupList = new List<Group>();
            List<Group> tmp_blueGroupList = new List<Group>();
            Core.AdjustStraddlePerColor(redGroupList, tmp_redGroupList, redMom, blueMom, ref tmp_AllOK, true);
            Core.AdjustStraddlePerColor(blueGroupList, tmp_blueGroupList, blueMom, redMom, ref tmp_AllOK, false);

            if (!tmp_AllOK) {
                isReset = true;
                redGroupList = tmp_redGroupList;
                blueGroupList = tmp_blueGroupList;
                CalculateBorderLine();
                StraddleGroup(in_Deep + 1);
            } else if (in_Deep > 0) {
                StraddleGroup(0);
            }
        } else {
            Core.Exit("SraddleGroup has Heavy Processing");
        }
    }

    private void BreakSoloGroup(int in_Deep) {
        if (in_Deep < 10) {
            bool tmp_AllOK = true;
            List<Group> tmp_RedGroupList = new List<Group>();
            List<Group> tmp_BlueGroupList = new List<Group>();
            Core.AdjustSoloPerColor(redGroupList, tmp_RedGroupList, ref tmp_AllOK);
            Core.AdjustSoloPerColor(blueGroupList, tmp_BlueGroupList, ref tmp_AllOK);
            if (!tmp_AllOK) {
                isReset = true;
                redGroupList = tmp_RedGroupList;
                blueGroupList = tmp_BlueGroupList;
                CalculateBorderLine();
                StraddleGroup(0);
                BreakSoloGroup(in_Deep + 1);
            }
        } else {
            Core.Exit("BreakSoloGroup has Heavy Processing");
        }
    }

    private bool Judging(Vector2 in_JudgePos, List<Group> in_JudgeGroup, bool in_IsRed) {
        for (int i = 0; i < in_JudgeGroup.Count; i++) {
            bool tmp_GroupOK = true;
            for (int j = 0; j < in_JudgeGroup[i].lineList.Count && tmp_GroupOK; j++) {
                if (in_JudgeGroup[i].lineList[j].IsCorrectlyJudged(in_JudgePos, in_IsRed) != in_IsRed) {
                    tmp_GroupOK = false;
                }
            }
            if (tmp_GroupOK) {
                return true;
            }
        }
        return false;
    }

    public void AllReset() {
        timeToTook = 0;
        red_dotMesh = new Mesh();
        blue_dotMesh = new Mesh();
        redMom = new List<Dot>();
        blueMom = new List<Dot>();
        redGroupList = new List<Group>();
        blueGroupList = new List<Group>();
        if (InputMergeK.text != "") {
            float k = float.Parse(InputMergeK.text);
            if (k < 0) {
                console.AddText("MergeKは0以上で入力してください");
            } else {
                Core.groupMergeK = k;
            }
        }
        if (InputRadious.text != "") {
            float k = float.Parse(InputRadious.text);
            if (k < 10 || 100 < k) {
                console.AddText("Radiousは10~100の範囲で入力してください");
            } else {
                MeshMake.dotRadious = k;
                mouse_Mesh = MeshMake.MakePolygonMesh(Vector3.zero);
            }
        }
        if (InputVertices.text != "") {
            int k = int.Parse(InputVertices.text);
            if (k < 3 || 12 < k) {
                console.AddText("Verticesは3～12の範囲で入力してください");
            } else {
                MeshMake.mesh_verticies = k;
                mouse_Mesh = MeshMake.MakePolygonMesh(Vector3.zero);
            }
        }
        console.AllReset();
        redValue.text = redMom.Count + "コ";
        blueValue.text = blueMom.Count + "コ";
        totalValue.text = "計" + (redMom.Count + blueMom.Count) + "コ";
        isReset = false;
    }

    public void RandomPut(int in_Deep) {
        if (in_Deep < 10) {
            int X = Random.Range((int)left, (int)right);
            int Y = Random.Range((int)down, (int)up);
            bool RED = Random.Range(0, 2) == 0;
            if (CanPut(new Vector2(X, Y))) {
                Put(X, Y, RED);
            } else {
                RandomPut(in_Deep + 1);
            }
        } else {
            console.AddText("手動で置いてください");
        }
    }

    public void PutButton(bool in_IsRed) {
        if (InputX.text != "" && InputY.text != "") {
            float x = float.Parse(InputX.text);
            float y = float.Parse(InputY.text);
            if (CanPut(new Vector2(x, y))) {
                Put(x, y, in_IsRed);
            } else {
                console.AddText("(" + x + "," + y + ")には置けません");
            }
        } else {
            console.AddText("座標を入力してください");
        }
    }

}
