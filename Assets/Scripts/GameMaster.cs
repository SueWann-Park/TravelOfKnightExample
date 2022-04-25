using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public static GameMaster script;
    private Transform[][] tiles;
    private bool[][] isTaken;
    private Color[][] originColor;
    private Transform board;
    private Vector2Int knightPos;
    private Transform countButton;

    private int state;
    private List<KeyValuePair<Transform,Vector2Int>> possibleTiles;

    void Start()
    {
        if (script == null)
        {
            script = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        state = 0;
        knightPos = new Vector2Int(0, 0);
        possibleTiles = new List<KeyValuePair<Transform, Vector2Int>>();
        countButton = GameObject.Find("Canvas/CountButton").transform;
        countButton.GetComponent<Button>().enabled = false;
        InitializeBoard();
        Application.targetFrameRate = 100;
    }

    private void InitializeBoard()
    {
        board = GameObject.Find("Canvas/Board").transform;
        GameObject tile_old = GameObject.Find("Canvas/Button_Origin");
        tiles = new Transform[8][];
        isTaken = new bool[8][];
        originColor = new Color[8][];
        for (int i = 0; i < 8; i++)
        {
            tiles[i] = new Transform[8];
            isTaken[i] = new bool[8];
            originColor[i] = new Color[8];
        }

        bool isWhite = true;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int local_i = i;
                int local_j = j;
                tiles[i][j] = Instantiate(tile_old).transform;
                tiles[i][j].SetParent(board);
                tiles[i][j].gameObject.name = "tile" + "_" + i.ToString() + "_" + j.ToString();
                tiles[i][j].localPosition = new Vector3(80 + i * 50, 70 + j * 50, 0);
                tiles[i][j].localScale = new Vector3(1, 1, 1);
                tiles[i][j].GetComponent<Button>().onClick.AddListener(() => OnClickTile(local_i, local_j));
                tiles[i][j].GetComponent<Button>().enabled = false;

                Color c = new Color(1, 1, 1) * (isWhite == true ? 1 : 0.5f);
                tiles[i][j].GetComponent<Image>().color = c;
                originColor[i][j] = c;
                isTaken[i][j] = false;
                isWhite = !isWhite;
            }
            isWhite = !isWhite;
        }

        tile_old.gameObject.SetActive(false);
        MoveKnight(0, 0);
        TakePossibleTiles(0, 0);
    }

    private Color movedColor = new Color(0.7f, 0.7f, 0.3f);
    private void MoveKnight(int i, int j)
    {
        if (i < 0 || i >= 8 || j < 0 || j >= 8)
            return;

        tiles[knightPos.x][knightPos.y].GetComponentInChildren<Text>().text = "";
        tiles[knightPos.x][knightPos.y].GetComponent<Button>().enabled = false;
        knightPos = new Vector2Int(i, j);
        tiles[i][j].GetComponentInChildren<Text>().text = "Knight";
        tiles[i][j].GetComponent<Button>().enabled = true;
        tiles[i][j].GetComponent<Image>().color = movedColor;

        isTaken[i][j] = true;
    }

    private int count = 1;
    public void OnClickTile(int x, int y)
    {
        if (knightPos.x == x && knightPos.y == y)
            return;

        MoveKnight(x, y);
        ResetPossibleTiles();
        TakePossibleTiles(x, y);
        SetCountText();
        CheckStuck();
    }
    private void CheckStuck()
    {
        if(possibleTiles.Count == 0 && count != 64)
        {
            countButton.GetComponentInChildren<Text>().text = "Fail...";
        }
    }
    private void SetCountText()
    {
        countButton.GetComponentInChildren<Text>().text = (++count).ToString();
        if (count == 64)
        {
            countButton.GetComponentInChildren<Text>().text = "Clear!";
        }
    }

    private void ResetPossibleTiles()
    {
        foreach (KeyValuePair<Transform, Vector2Int> pair in possibleTiles)
        {
            if (isTaken[pair.Value.x][pair.Value.y] == true)
                continue;

            pair.Key.GetComponent<Image>().color = originColor[pair.Value.x][pair.Value.y];
            pair.Key.GetComponent<Button>().enabled = false;
        }
        possibleTiles.Clear();
    }

    private Color possibleColor = new Color(0.3f, 0.3f, 0.7f);
    private readonly int[] pt = { 1, 2, 1, -2, -1, 2, -1, -2, 2, 1, 2, -1, -2, 1, -2, -1 };
    private void TakePossibleTiles(int x, int y)
    {
        for(int i = 0; i < 8; i++)
        {
            int px = x + pt[i * 2];
            int py = y + pt[i * 2 + 1];

            if (px < 0 || px >= 8 || py < 0 || py >= 8)
                continue;
            if (isTaken[px][py] == true)
                continue;

            tiles[px][py].GetComponent<Button>().enabled = true;
            tiles[px][py].GetComponent<Image>().color = possibleColor;
            possibleTiles.Add(new KeyValuePair<Transform, Vector2Int>(tiles[px][py], new Vector2Int(px, py)));
        }
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }
}
