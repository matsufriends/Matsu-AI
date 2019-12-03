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

    [SerializeField]
    private RectTransform panel = null;


    public static Console console;
    private Mesh red_dotMesh;
    private Mesh blue_dotMesh;
    private Mesh mouse_Mesh;

    private static List<Dot> redMom;
    private static List<Dot> blueMom;
    private static List<Group> redGroupList;
    private static List<Group> blueGroupList;

    public static Transform mainTransform;
    public static float display_Up;
    public static float display_Left;
    public static float display_Right;
    public static float display_Down;
    private bool isProcessing = false;
    private bool isReset = false;
    private float totalProcessTime = 0;

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
        console.AllReset();
        mainTransform = transform;
        Cursor.visible = false;
        float tmp_Aspect_k = 990 / panel.rect.height;
        display_Up = 530 * tmp_Aspect_k;
        display_Down = -530 * tmp_Aspect_k;
        display_Left = -955 * tmp_Aspect_k;
        display_Right = 460 * tmp_Aspect_k;

        red_dotMesh = new Mesh();
        blue_dotMesh = new Mesh();
        mouse_Mesh = MeshMake.MakePolygonMesh(Vector3.zero);
        redMom = new List<Dot>();
        blueMom = new List<Dot>();
        redGroupList = new List<Group>();
        blueGroupList = new List<Group>();
    }

    void Update() {
        Vector2 tmp_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool tmp_IsCanPut = CanPut(tmp_MousePos);
        if (tmp_IsCanPut) {
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
                } else {
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

        if (tmp_IsCanPut) {
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

    private bool Nijikansu(float x, float y) {
        float a = (x + 250) * 0.005f;
        float b = y * 0.01f;
        return a * a - 3 > b;
    }

    private bool Sanjikansu(float x, float y) {
        float a = (x + 250) * 0.004f;
        float b = y * 0.01f;
        return a * a * a - a > b;
    }

    /*
    float a = (tmp_MousePos.x + 250)*0.004f;
    // float a = (tmp_MousePos.x + 250)*0.005f;
     float b = tmp_MousePos.y*0.01f;
    Put(tmp_MousePos.x,tmp_MousePos.y,a*a*a-a>b);
    //Put(tmp_MousePos.x,tmp_MousePos.y,a*a-3>b);
     */

    private bool CanPut(Vector2 in_PutPos) {
        if (in_PutPos.x < display_Left || display_Right < in_PutPos.x) {
            return false;
        }
        if (in_PutPos.y < display_Down || display_Up < in_PutPos.y) {
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
        totalProcessTime += (float)sw.ElapsedMilliseconds;
        console.AddText("処理時間:" + sw.ElapsedMilliseconds + "ms" + " 合計処理時間:" + totalProcessTime + "ms");
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
            Core.Exit("SraddleGroup has too Heavy Processing");
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
            Core.Exit("BreakSoloGroup has too Heavy Processing");
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
        totalProcessTime = 0;
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
            int X = Random.Range((int)display_Left, (int)display_Right);
            int Y = Random.Range((int)display_Down, (int)display_Up);
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
