using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector3 firstPosition;
    private Vector3 finalPosition;
    private float swipeAngle;
    private Vector3 tempPosition;
    //Menampung data posisi tile
    public float xPosition;
    public float yPosition;
    public int column;
    public int row;
    public int awal = 0;
    public int awalku = 0;
    private Grid grid;
    private GameObject otherTile;

    public int previousColumn { get; private set; }
    public int previousRow { get; private set; }

    public bool isMatched = false;

    // Start is called before the first frame update
    void Start()
    {
        //Menentukan posisi dari tile
        grid = FindObjectOfType<Grid>();
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        column = Mathf.RoundToInt((xPosition - grid.startPos.x) / grid.offset.x);
        row = Mathf.RoundToInt((yPosition - grid.startPos.y) / grid.offset.x);
    }
    // Update is called once per frame
    void Update()
    {
        xPosition = (column * grid.offset.x) + grid.startPos.x;
        yPosition = (row * grid.offset.y) + grid.startPos.y;
        SwipeTile();
    }
    void OnMouseDown()
    {
        //Mendapatkan titik awal sentuhan jari  
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void OnMouseUp()
    {
        //Mendapatkan titik akhir sentuhan jari
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }
    void CalculateAngle()
    {
        //Menghitung sudut antara posisi awal dan posisi akhir
        swipeAngle = Mathf.Atan2(finalPosition.y - firstPosition.y, finalPosition.x - firstPosition.x) * 180 / Mathf.PI;
        MoveTile();
    }
    public void SwipeTile()
    {
        if (GameManager.instance.moveActive)
        {
            if (Mathf.Abs(xPosition - transform.position.x) > .1)
            {
                //Move towards the target
                tempPosition = new Vector2(xPosition, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            }
            else
            {
                //Directly set the position
                tempPosition = new Vector2(xPosition, transform.position.y);
                transform.position = tempPosition;
                grid.tiles[column, row] = this.gameObject;
            }
            if (Mathf.Abs(yPosition - transform.position.y) > .1)
            {
                //Move towards the target
                tempPosition = new Vector2(transform.position.x, yPosition);
                transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            }
            else
            {
                //Directly set the position
                tempPosition = new Vector2(transform.position.x, yPosition);
                transform.position = tempPosition;
                grid.tiles[column, row] = this.gameObject;
            }
        }
    }
    void MoveTile()
    {
        if (GameManager.instance.moveActive)
        {
            GameManager.instance.checkTile = false;
            previousColumn = column;
            previousRow = row;

            if (swipeAngle > -45 && swipeAngle <= 45)
            {
                //Right swipe
                SwipeRightMove();
            }
            else if (swipeAngle > 45 && swipeAngle <= 135)
            {
                //Up swipe
                SwipeUpMove();
            }
            else if (swipeAngle > 135 || swipeAngle <= -135)
            {
                //Left swipe
                SwipeLeftMove();
            }
            else if (swipeAngle < -45 && swipeAngle >= -135)
            {
                //Down swipe
                SwipeDownMove();
            }
            StartCoroutine(checkMove());
        }
    }
    //Method untuk menentukan arah dari swipe
    void SwipeRightMove()
    {
        if (column + 1 < grid.gridSizeX)
        {
            //Menukar posisi tile dengan sebelah kanan nya
            otherTile = grid.tiles[column + 1, row];
            otherTile.GetComponent<Tile>().column -= 1;
            column += 1;
            otherTile.name = "(" + otherTile.GetComponent<Tile>().column + "," + otherTile.GetComponent<Tile>().row + ")";
            name = "(" + column + "," + row + ")";
            GameManager.instance.checkTile = true;
        }
    }
    void SwipeUpMove()
    {
        if (row + 1 < grid.gridSizeY)
        {
            //Menukar posisi tile dengan sebelah atasnya
            otherTile = grid.tiles[column, row + 1];
            otherTile.GetComponent<Tile>().row -= 1;
            row += 1;
            otherTile.name = "(" + otherTile.GetComponent<Tile>().column + "," + otherTile.GetComponent<Tile>().row + ")";
            name = "(" + column + "," + row + ")";
            GameManager.instance.checkTile = true;
        }
    }
    void SwipeLeftMove()
    {
        if (column - 1 >= 0)
        {
            //Menukar posisi tile dengan sebelah kiri nya
            otherTile = grid.tiles[column - 1, row];
            otherTile.GetComponent<Tile>().column += 1;
            column -= 1;
            otherTile.name = "(" + otherTile.GetComponent<Tile>().column + "," + otherTile.GetComponent<Tile>().row + ")";
            name = "(" + column + "," + row + ")";
            GameManager.instance.checkTile = true;
        }
    }
    void SwipeDownMove()
    {
        if (row - 1 >= 0)
        {
            //Menukar posisi tile dengan sebelah bawahnya
            otherTile = grid.tiles[column, row - 1];
            otherTile.GetComponent<Tile>().row += 1;
            row -= 1;
            otherTile.name = "(" + otherTile.GetComponent<Tile>().column + "," + otherTile.GetComponent<Tile>().row + ")";
            name = "(" + column + "," + row + ")";
            GameManager.instance.checkTile = true;
        }
    }
    public void CheckMatches()
    {
        //Check horizontal matching
        if (column > 0 && column < grid.gridSizeX - 1)
        {
            //Check samping kiri dan kanan nya
            GameObject leftTile = grid.tiles[column - 1, row];
            GameObject rightTile = grid.tiles[column + 1, row];
            if (leftTile != null && rightTile != null)
            {
                if (rightTile.tag == "Bomb" && leftTile.tag == "Bomb" && gameObject.tag != "Bomb" && !GameManager.instance.decreaseAktif)
                {

                    if (awalku == 0)
                    {
                        string namatag = gameObject.tag;
                        for (int x = 0; x < grid.gridSizeX; x++)
                        {
                            for (int y = 0; y < grid.gridSizeY; y++)
                            {

                                checkGrid(x, y, namatag);
                            }
                        }
                        rightTile.GetComponent<Tile>().isMatched = true;
                        SpriteRenderer sprite = rightTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;
                        leftTile.GetComponent<Tile>().isMatched = true;
                         sprite = leftTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;

                        if (isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb -= 2;
                            Debug.Log("BOMB2");
                        }
                        awalku++;
                        GameManager.instance.doneHapus = true;
                    }

                }
                if (gameObject.tag == "Bomb" && rightTile.tag == "Bomb" && leftTile.tag != "Bomb" && !GameManager.instance.decreaseAktif)
                {

                    if (awalku == 0)
                    {
                        string namatag = leftTile.tag;
                        for (int x = 0; x < grid.gridSizeX; x++)
                        {
                            for (int y = 0; y < grid.gridSizeY; y++)
                            {

                                checkGrid(x, y, namatag);
                            }
                        }
                        if (column+2 >= grid.gridSizeX)
                        {
                            namatag = grid.tiles[column + 2, row].tag;
                            for (int x = 0; x < grid.gridSizeX; x++)
                            {
                                for (int y = 0; y < grid.gridSizeY; y++)
                                {

                                    checkGrid(x, y, namatag);
                                }
                            }
                        }
                        isMatched = true;
                        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;
                        rightTile.GetComponent<Tile>().isMatched = true;
                        sprite = rightTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;

                        if (isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb -= 2;
                            Debug.Log("BOMB2");
                        }
                        awalku++;
                        GameManager.instance.doneHapus = true;
                    }

                }
                if (gameObject.tag == "Bomb" && rightTile.CompareTag(leftTile.tag) && !GameManager.instance.decreaseAktif)
                {

                    if (awalku == 0)
                    {
                        string namatag = rightTile.tag;
                        for (int x = 0; x < grid.gridSizeX; x++)
                        {
                            for (int y = 0; y < grid.gridSizeY; y++)
                            {

                                checkGrid(x, y, namatag);
                            }
                        }
                        isMatched = true;
                        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;
                        if (isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb--;
                            Debug.Log("BOMB1");
                        }
                        awalku++;
                        GameManager.instance.doneHapus = true;
                    }

                }
                if (leftTile.tag == "Bomb" && rightTile.CompareTag(gameObject.tag) && !GameManager.instance.decreaseAktif)
                {

                    if (awalku == 0)
                    {
                        string namatag = gameObject.tag;
                        Tile tile = leftTile.GetComponent<Tile>();

                        for (int x = 0; x < grid.gridSizeX; x++)
                        {
                            for (int y = 0; y < grid.gridSizeY; y++)
                            {
                                checkGrid(x, y, namatag);

                            }
                        }
                        tile.isMatched = true;
                        SpriteRenderer sprite = leftTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;

                        if (tile.isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb--;
                            Debug.Log("BOMB1");
                        }
                        awalku++;
                    }

                }
                if (rightTile.tag == "Bomb" && leftTile.CompareTag(gameObject.tag) && !GameManager.instance.decreaseAktif)
                {
                    if (awalku == 0)
                    {
                        string namatag = gameObject.tag;
                        Tile tile = rightTile.GetComponent<Tile>();

                        for (int x = 0; x < grid.gridSizeX; x++)
                        {
                            for (int y = 0; y < grid.gridSizeY; y++)
                            {
                                checkGrid(x, y, namatag);

                            }
                        }

                        tile.isMatched = true;
                        SpriteRenderer sprite = rightTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;

                        if (tile.isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb--;
                            Debug.Log("BOMB1");
                        }
                        awalku++;

                    }
                }
                if (gameObject.tag == "Bomb" && rightTile.tag == "Bomb" && leftTile.tag == "Bomb" && !GameManager.instance.decreaseAktif)
                {
                    isMatched = true;
                    SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;
                    rightTile.GetComponent<Tile>().isMatched = true;
                    sprite = rightTile.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;
                    leftTile.GetComponent<Tile>().isMatched = true;
                    sprite = leftTile.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;

                    if (isMatched && awal == 0)
                    {
                        GameManager.instance.comboMultiply += 1;
                        GameManager.instance.ShowComboBanner();
                        awal++;
                        grid.indexBomb -= 3;
                        Debug.Log("BOMB3");
                    }
                }
                if (gameObject.tag != "Bomb" && (leftTile.CompareTag(gameObject.tag)) && (rightTile.CompareTag(gameObject.tag)))
                {
                    isMatched = true;
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;

                    rightTile.GetComponent<Tile>().isMatched = true;
                    sprite = rightTile.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;

                    leftTile.GetComponent<Tile>().isMatched = true;
                    sprite = leftTile.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;
                    if (isMatched && awal == 0)
                    {
                        GameManager.instance.comboMultiply += 1;
                        GameManager.instance.ShowComboBanner();
                        awal++;
                    }
                }
            }
        }
        //Check vertical matching
        if (row > 0 && row < grid.gridSizeY - 1)
        {
            //Check samping atas dan bawahnya
            GameObject upTile = grid.tiles[column, row + 1];
            GameObject downTile = grid.tiles[column, row - 1];
            if (upTile != null && downTile != null)
            {
                /*if(this.gameObject.tag == "Bomb" && awalhancur == 0 && (upTile.tag == "Bomb" || upTile.CompareTag(downTile.tag)) && (downTile.tag == "Bomb" || downTile.CompareTag(upTile.tag)))
                {
                    if (upTile.tag != "Bomb")
                    {
                        GameManager.instance.namatag = upTile.tag;
                    }
                    else if (downTile.tag != "Bomb")
                    {
                        GameManager.instance.namatag = downTile.tag;
                        
                    }
                    for (int x = 0; x < grid.gridSizeX; x++)
                    {
                        for (int y = 0; y < grid.gridSizeY; y++)
                        {
                            if (grid.tiles[x, y].tag == GameManager.instance.namatag)
                            {
                                grid.tiles[x, y].GetComponent<Tile>().isMatched = true;
                            }
                        }
                    }
                    GameManager.instance.namatag = "";
                    isMatched = true;
                    downTile.GetComponent<Tile>().isMatched = true;
                    upTile.GetComponent<Tile>().isMatched = true;
                    if (isMatched && awal == 0)
                    {
                        GameManager.instance.comboMultiply += 1;
                        GameManager.instance.ShowComboBanner();
                        awal++;
                    }
                    awalhancur++;
                }*/
                //if ((upTile.tag == "Bomb" || upTile.CompareTag(gameObject.tag)) && (downTile.tag == "Bomb" || downTile.CompareTag(gameObject.tag)))
                //{
                    
                    if (upTile.tag == "Bomb" && downTile.tag == "Bomb" && gameObject.tag != "Bomb" && !GameManager.instance.decreaseAktif)
                    {

                        if (awalku == 0)
                        {
                            string namatag = gameObject.tag;
                            for (int x = 0; x < grid.gridSizeX; x++)
                            {
                                for (int y = 0; y < grid.gridSizeY; y++)
                                {

                                    checkGrid(x, y, namatag);
                                }
                            }
                            upTile.GetComponent<Tile>().isMatched = true;
                            SpriteRenderer sprite = upTile.GetComponent<SpriteRenderer>();
                            sprite.color = Color.grey;
                            downTile.GetComponent<Tile>().isMatched = true;
                            sprite = downTile.GetComponent<SpriteRenderer>();
                            sprite.color = Color.grey;
                            if (isMatched && awal == 0)
                            {
                                GameManager.instance.comboMultiply += 1;
                                GameManager.instance.ShowComboBanner();
                                awal++;
                                grid.indexBomb -= 2;
                                Debug.Log("BOMB2");
                            }
                            awalku++;
                            GameManager.instance.doneHapus = true;
                        }

                    }
                    if (gameObject.tag == "Bomb" && downTile.tag == "Bomb" && upTile.tag != "Bomb" && !GameManager.instance.decreaseAktif)
                    {

                        if (awalku == 0)
                        {
                            string namatag = upTile.tag;
                            for (int x = 0; x < grid.gridSizeX; x++)
                            {
                                for (int y = 0; y < grid.gridSizeY; y++)
                                {

                                    checkGrid(x, y, namatag);
                                }
                            }
                            if (row - 2 < 0)
                            {
                                namatag = grid.tiles[column, row - 2].tag;
                                for (int x = 0; x < grid.gridSizeX; x++)
                                {
                                    for (int y = 0; y < grid.gridSizeY; y++)
                                    {

                                        checkGrid(x, y, namatag);
                                    }
                                }
                            }
                            isMatched = true;
                            SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
                            sprite.color = Color.grey;
                            downTile.GetComponent<Tile>().isMatched = true;
                            sprite = downTile.GetComponent<SpriteRenderer>();
                            sprite.color = Color.grey;
                            if (isMatched && awal == 0)
                            {
                                GameManager.instance.comboMultiply += 1;
                                GameManager.instance.ShowComboBanner();
                                awal++;
                                grid.indexBomb -= 2;
                                Debug.Log("BOMB2");
                            }
                            awalku++;
                            GameManager.instance.doneHapus = true;
                        }

                    }
                    if (gameObject.tag == "Bomb" && downTile.CompareTag(upTile.tag) && !GameManager.instance.decreaseAktif)
                    {

                        if (awalku == 0)
                        {
                            string namatag = downTile.tag;
                            for (int x = 0; x < grid.gridSizeX; x++)
                            {
                                for (int y = 0; y < grid.gridSizeY; y++)
                                {

                                    checkGrid(x, y, namatag);
                                }
                            }
                            isMatched = true;
                            SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
                            sprite.color = Color.grey;
                            if (isMatched && awal == 0)
                            {
                                GameManager.instance.comboMultiply += 1;
                                GameManager.instance.ShowComboBanner();
                                awal++;
                                grid.indexBomb--;
                                Debug.Log("BOMB1");
                            }
                            awalku++;
                            GameManager.instance.doneHapus = true;
                        }

                    }
                    if (upTile.tag == "Bomb" && downTile.CompareTag(gameObject.tag) && !GameManager.instance.decreaseAktif)
                    {
                           
                        if (awalku == 0)
                        {
                            string namatag = gameObject.tag;
                            Tile tile = upTile.GetComponent<Tile>();

                            for (int x = 0; x < grid.gridSizeX; x++)
                            {
                                for (int y = 0; y < grid.gridSizeY; y++)
                                {
                                    checkGrid(x,y,namatag);
                                    
                                }
                            }
                            tile.isMatched = true;
                            SpriteRenderer sprite = upTile.GetComponent<SpriteRenderer>();
                            sprite.color = Color.grey;

                            if (tile.isMatched && awal == 0)
                            {
                                GameManager.instance.comboMultiply += 1;
                                GameManager.instance.ShowComboBanner();
                                awal++;
                                grid.indexBomb--;
                                Debug.Log("BOMB1");
                            }
                            awalku++;
                        }

                    }
                    if (downTile.tag == "Bomb" && upTile.CompareTag(gameObject.tag) && !GameManager.instance.decreaseAktif)
                    {
                    if (awalku == 0)
                    {
                        string namatag = gameObject.tag;
                        Tile tile = downTile.GetComponent<Tile>();

                        for (int x = 0; x < grid.gridSizeX; x++)
                        {
                            for (int y = 0; y < grid.gridSizeY; y++)
                            {
                                checkGrid(x, y, namatag);

                            }
                        }

                        tile.isMatched = true;
                        SpriteRenderer sprite = downTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;

                        if (tile.isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb--;
                            Debug.Log("BOMB1");
                        }
                        awalku++;
                    }

                    }
                    if (upTile.tag == "Bomb" && downTile.tag == "Bomb" && upTile.tag == "Bomb" && !GameManager.instance.decreaseAktif)
                    {
                        isMatched = true;
                        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;
                        downTile.GetComponent<Tile>().isMatched = true;
                         sprite = downTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;
                        upTile.GetComponent<Tile>().isMatched = true;
                         sprite = upTile.GetComponent<SpriteRenderer>();
                        sprite.color = Color.grey;

                        if (isMatched && awal == 0)
                        {
                            GameManager.instance.comboMultiply += 1;
                            GameManager.instance.ShowComboBanner();
                            awal++;
                            grid.indexBomb -= 3;
                            Debug.Log("BOMB3");
                        }
                    }
                if(gameObject.tag != "Bomb" && downTile.CompareTag(gameObject.tag) && upTile.CompareTag(gameObject.tag) )
                {

                    isMatched = true;
                    SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;

                    downTile.GetComponent<Tile>().isMatched = true;
                    sprite = downTile.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;

                    upTile.GetComponent<Tile>().isMatched = true;
                    sprite = upTile.GetComponent<SpriteRenderer>();
                    sprite.color = Color.grey;


                    if (isMatched && awal == 0)
                    {
                        GameManager.instance.comboMultiply += 1;
                        GameManager.instance.ShowComboBanner();
                        awal++;
                    }

                }
                //}
            }
        }
    }

    void checkGrid(int x,int y,string namatag)
    {
        //yield return new WaitUntil(() => grid.tiles[x, y] != null);
        //yield return new WaitUntil(() => grid.tiles[x, y].GetComponent<Tile>() != null);
        if (grid.tiles[x, y] != null)
        if (grid.tiles[x, y].GetComponent<Tile>() != null)
        if (grid.tiles[x, y].GetComponent<Tile>().tag == namatag)
        {
            grid.tiles[x, y].GetComponent<Tile>().isMatched = true;
            SpriteRenderer sprite = grid.tiles[x, y].GetComponent<SpriteRenderer>();
            sprite.color = Color.grey;

        }
    }

    IEnumerator checkMove()
    {
        yield return new WaitUntil(() => GameManager.instance.checkTile == true);
        int lulus = 0;
        for (int x = 0; x < grid.gridSizeX; x++)
        {
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                grid.tiles[x, y].GetComponent<Tile>().CheckMatches();
            }
        }
        GameManager.instance.doneHapus = true;
        GameManager.instance.checkTile = false;
        yield return new WaitUntil(() => GameManager.instance.doneHapus == true);
        yield return new WaitForSeconds(.5f);
        //Cek jika tile nya tidak sama kembalikan, jika ada yang sama panggil DestroyMatches
        if (otherTile != null)
        {
            if (!isMatched && !otherTile.GetComponent<Tile>().isMatched)
            {
                GameManager.instance.namatag = "";
                GameManager.instance.checkTile = false;
                otherTile.GetComponent<Tile>().row = row;
                otherTile.GetComponent<Tile>().column = column;
                row = previousRow;
                column = previousColumn;
                GameManager.instance.checkTile = false;
                GameManager.instance.comboMultiply = 1;
                GameManager.instance.ShowComboBanner();
            }
            else
            {
                StartCoroutine(HapusMatches());
            }
        }
        otherTile = null;
    }
    IEnumerator HapusMatches()
    {
        yield return new WaitUntil(() => GameManager.instance.doneHapus == true);
        grid.DestroyMatches();
        GameManager.instance.doneHapus = false;
    }
}
