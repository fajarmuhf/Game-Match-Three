using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //Properties untuk ukuran grid
    public int gridSizeX, gridSizeY;
    public Vector2 startPos, offset;
    //Prefab yang akan digunakan untuk background grid
    //Array untuk menampung prefab tile
    public GameObject tilePrefab;
    //Array 2 dimensi untuk membuat tile
    public GameObject[,] tiles;
    public GameObject[] candies;
    public int MaxBomb = 1;
    private int index;
    public int indexBomb = 0;
    void Start()
    {
        CreateGrid();
    }
    void CreateGrid()
    {
        tiles = new GameObject[gridSizeX, gridSizeY];
        //Menentukan offset, didapatkan dari size prefab
        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        //Menentukan posisi awal
        startPos = transform.position + (Vector3.left * (offset.x * gridSizeX / 2)) + (Vector3.down * (offset.y * gridSizeY / 3));
        //Looping untuk membuat tile
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 pos = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));
                GameObject backgroundTile = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + x + "," + y + ")";

                //Lakukan iterasi sampai tile tidak ada yang sama dengan sebelahnya
                int MAX_ITERATION = 0;
                do
                {
                    if (indexBomb >= this.MaxBomb)
                    {
                        index = Random.Range(0, candies.Length-1);
                    }
                    else
                    {
                        index = Random.Range(0, candies.Length);
                    }
                    MAX_ITERATION++;
                }while(MatchesAt(x, y, candies[index]));
                if(candies[index].tag == "Bomb")
                {
                    indexBomb++;
                }
                MAX_ITERATION = 0;

                //Create object
                GameObject candy = ObjectPooler.Instance.SpawnFromPool(index.ToString(), pos, Quaternion.identity);
                candy.name = "(" + x + "," + y + ")";
                tiles[x, y] = candy;
                tiles[x, y].tag = candy.tag;
                tiles[x, y].GetComponent<Tile>().column = x;
                tiles[x, y].GetComponent<Tile>().row = y;
            }
        }
        GameObject.Find("AchievementSystem").GetComponent<AchievementSystem>().addAchievement();
        GameObject.Find("AchievementSystem").GetComponent<AchievementSystem>().mulai = true;
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        //Cek jika ada tile yang sama dengan dibawah dan samping nya
        if (column > 1 && row > 1)
        {
            if (tiles[column - 1, row] != null && tiles[column - 2, row] != null)
            {
                if ((tiles[column - 1, row]?.tag == piece.tag) && (tiles[column - 2, row]?.tag == piece.tag))
                {
                    return true;
                }
                if ((tiles[column - 1, row]?.tag == "Bomb") && (tiles[column - 2, row]?.tag == piece.tag))
                {
                    return true;
                }
                if ((tiles[column - 1, row]?.tag == piece.tag) && (tiles[column - 2, row]?.tag == "Bomb"))
                {
                    return true;
                }
                if (piece.tag == "Bomb" && tiles[column - 1, row]?.tag == tiles[column - 2, row]?.tag)
                {
                    return true;
                }
            }
            if (tiles[column, row-1] != null && tiles[column, row-2] != null)
            {
                if ((tiles[column, row - 1]?.tag == piece.tag) && (tiles[column, row - 2]?.tag == piece.tag))
                {
                    return true;
                }
                else if ((tiles[column, row - 1]?.tag == "Bomb") && (tiles[column, row - 2]?.tag == piece.tag))
                {
                    return true;
                }
                else if ((tiles[column, row - 1]?.tag == piece.tag) && (tiles[column, row - 2]?.tag == "Bomb"))
                {
                    return true;
                }
                if (piece.tag == "Bomb" && tiles[column, row-1]?.tag == tiles[column, row-2]?.tag)
                {
                    return true;
                }
            }

            if (tiles[column - 2, row] != null) {
                if (piece.tag == "Bomb" && tiles[column - 2, row]?.tag == "Bomb")
                {
                    return true;
                }
            }
            if (tiles[column - 1, row] != null) {
                if (piece.tag == "Bomb" && tiles[column - 1, row]?.tag == "Bomb")
                {
                    return true;
                }
            }
            if (tiles[column, row - 2] != null) {
                if (piece.tag == "Bomb" && tiles[column, row - 2]?.tag == "Bomb")
                {
                    return true;
                }
            }
            if (tiles[column, row - 1] != null) {
                if (piece.tag == "Bomb" && tiles[column, row - 1]?.tag == "Bomb")
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            //Cek jika ada tile yang sama dengan atas dan sampingnya
            if (row > 1)
            {
                if (tiles[column, row - 1] != null && tiles[column, row - 2] != null)
                {
                    if ((tiles[column, row - 1]?.tag == piece.tag) && (tiles[column, row - 2]?.tag == piece.tag))
                    {
                        return true;
                    }
                    if ((tiles[column, row - 1]?.tag == "Bomb") && (tiles[column, row - 2]?.tag == piece.tag))
                    {
                        return true;
                    }
                    if ((tiles[column, row - 1]?.tag == piece.tag) && (tiles[column, row - 2]?.tag == "Bomb"))
                    {
                        return true;
                    }
                    else if (piece.tag == "Bomb" && tiles[column, row - 1]?.tag == tiles[column, row - 2]?.tag)
                    {
                        return true;
                    }
                }
                if (tiles[column, row - 2] != null) {
                    if (piece.tag == "Bomb" && tiles[column, row - 2]?.tag == "Bomb")
                    {
                        return true;
                    }
                }
                if (tiles[column, row - 1] != null) {
                    if (piece.tag == "Bomb" && tiles[column, row - 1]?.tag == "Bomb")
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (tiles[column - 1, row ] != null && tiles[column - 2, row ] != null)
                {
                    if ((tiles[column - 1, row]?.tag == piece.tag) && (tiles[column - 2, row]?.tag == piece.tag))
                    {
                        return true;
                    }
                    if ((tiles[column - 1, row]?.tag == "Bomb") && (tiles[column - 2, row]?.tag == piece.tag))
                    {
                        return true;
                    }
                    if ((tiles[column - 1, row]?.tag == piece.tag) && (tiles[column - 2, row]?.tag == "Bomb"))
                    {
                        return true;
                    }
                    else if (piece.tag == "Bomb" && tiles[column - 1, row]?.tag == tiles[column - 2, row]?.tag)
                    {
                        return true;
                    }
                }
                if (tiles[column - 2, row] != null)
                {
                    if (piece.tag == "Bomb" && tiles[column - 2, row]?.tag == "Bomb")
                    {
                        return true;
                    }
                }

                if (tiles[column - 1, row] != null)
                {
                    if (piece.tag == "Bomb" && tiles[column - 1, row]?.tag == "Bomb")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void DestroyMatchesAt(int column, int row)
    {

        //Destroy tile di indeks tertentu
        if (tiles[column, row].GetComponent<Tile>().isMatched)
        {
            GameManager.instance.GetScore(10);
            GameObject gm = tiles[column, row];
            gm.SetActive(false);
            gm.name = gm.tag;
            gm.GetComponent<Tile>().column = 99;
            gm.GetComponent<Tile>().row = 99;
            gm.GetComponent<Tile>().isMatched = false;
            //Destroy(gm.GetComponent<Tile>());
            tiles[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        GameManager.instance.decreaseAktif = true;
        //Lakukan looping untuk cek tile yang null lalu di Destroy
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }
    private void RefillBoard()
    {
        GameManager.instance.doneHapus = false;
        GameManager.instance.namatag = "";
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (tiles[x, y] == null)
                {
                    Vector2 tempPosition = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));
                    int candyToUse = Random.Range(0, candies.Length-1);
                    int MAX_ITERATION = 0;
                    do
                    {
                        if (indexBomb == MaxBomb)
                        {
                            candyToUse = Random.Range(0, candies.Length - 1);
                        }
                        else
                        {
                            candyToUse = Random.Range(0, candies.Length);
                        }
                        MAX_ITERATION++;
                    } while (MatchesAt(x, y, candies[candyToUse]) && MAX_ITERATION <= 100);
                    if (candies[candyToUse].tag == "Bomb")
                    {
                        indexBomb++;
                    }
                    /*int randomNumber = Random.Range(0, 100);
                    if (randomNumber < 1)
                    {
                        candyToUse = candies.Length-1;
                    }*/

                    GameObject tileToRefill = ObjectPooler.Instance.SpawnFromPool(candyToUse.ToString(), tempPosition, Quaternion.identity);
                    tileToRefill.name = "(" + x + "," + y + ")";
                    tiles[x, y] = tileToRefill;
                    tiles[x, y].tag = tileToRefill.tag;
                    tiles[x, y].GetComponent<Tile>().column = x;
                    tiles[x, y].GetComponent<Tile>().isMatched = false;
                    tiles[x, y].GetComponent<SpriteRenderer>().color = Color.white;
                    tiles[x, y].GetComponent<Tile>().row = y;
                }
            }
        }
        GameManager.instance.checkTile = true;
    }
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    if (tiles[i, j].GetComponent<Tile>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator DecreaseRow()
    {
        GameManager.instance.doneHapus = false;
        GameManager.instance.moveActive = false;
        int nullCount = 0;
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    tiles[i, j - nullCount] = tiles[i,j];
                    tiles[i, j - nullCount].GetComponent<Tile>().row -= nullCount;
                    tiles[i, j - nullCount].GetComponent<Tile>().xPosition = (tiles[i, j - nullCount].GetComponent<Tile>().column * offset.x) + startPos.x;
                    tiles[i, j - nullCount].GetComponent<Tile>().yPosition = (tiles[i, j - nullCount].GetComponent<Tile>().row * offset.y) + startPos.y;
                    GameManager.instance.moveActive = true;
                    tiles[i, j - nullCount].GetComponent<Tile>().SwipeTile();
                    //tiles[i,j].GetComponent<Tile>().row -= nullCount;
                    tiles[i, j - nullCount].name = "(" + tiles[i, j - nullCount].GetComponent<Tile>().column + "," + tiles[i, j - nullCount].GetComponent<Tile>().row + ")";
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitUntil(() => checkTileDown() == true);
        GameManager.instance.moveActive = false;
        yield return new WaitForSeconds(.5f);
        StartCoroutine(FillBoard());
    }
    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => checkAllTiles() == true);
        GameManager.instance.doneHapus = false;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                tiles[x, y].GetComponent<Tile>().CheckMatches();
            }
        }
        GameManager.instance.doneHapus = true;
        GameManager.instance.checkTile = false;
        yield return new WaitUntil(() => GameManager.instance.doneHapus == true);
        yield return new WaitForSeconds(.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        GameManager.instance.doneHapus = false;
        GameManager.instance.decreaseAktif = false;
        GameManager.instance.moveActive = true;
    }
    private bool checkAllTiles()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (tiles[x, y].GetComponent<Tile>() == null)
                {
                    return false;

                }
            }
        }
        return true;
    }
    private bool checkTileDown()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (tiles[x, y] != null)
                {
                    if (tiles[x, y].transform.position.y != tiles[x, y].GetComponent<Tile>().yPosition)
                    {
                        return false;
                    }
                    if (tiles[x, y].transform.position.x != tiles[x, y].GetComponent<Tile>().xPosition)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private void Update()
    {

    }
}