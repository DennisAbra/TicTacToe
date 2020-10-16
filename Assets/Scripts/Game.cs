using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] Transform buttonsParent;
    [SerializeField] Transform imagesParent;

    Button[] buttons;
    Image[] images;

    Tile[,] tiles;
    Menu menu;

    private void Awake()
    {
        buttons = buttonsParent.GetComponentsInChildren<Button>();
        images = imagesParent.GetComponentsInChildren<Image>();
        menu = FindObjectOfType<Menu>();


        foreach (Image i in images)
        {
            i.enabled = false;
        }
        Initialize(buttons, images);
    }

    public void Initialize(Button[] Buttons, Image[] Images)
    {
        if (Buttons.Length == Images.Length)
        {
            int counter = 0;
            tiles = new Tile[3, 3];
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Tile newTile = new Tile(Buttons[counter], Images[counter], OccupiedBy.None);
                    tiles[x, y] = newTile;
                    counter++;
                }
            }
        }
    }

    public void AIStartMove()
    {
        Vector2Int value = FindBestMove(tiles);
        PlacePawn(value, false);
    }

    Vector2Int ConvertCountToVector2Int(int count)
    {
        if (count == 0)
        {
            return new Vector2Int(0, 0);
        }
        else if (count == 1)
        {
            return new Vector2Int(1, 0);
        }
        else if (count == 2)
        {
            return new Vector2Int(2, 0);
        }
        else if (count == 3)
        {
            return new Vector2Int(0, 1);
        }
        else if (count == 4)
        {
            return new Vector2Int(1, 1);
        }
        else if (count == 5)
        {
            return new Vector2Int(2, 1);
        }
        else if (count == 6)
        {
            return new Vector2Int(0, 2);
        }
        else if (count == 7)
        {
            return new Vector2Int(1, 2);
        }
        else if (count == 8)
        {
            return new Vector2Int(2, 2);
        }
        else return Vector2Int.zero;
    }

    public void PlacePlayerPawn(int buttonIndex)
    {
        Vector2Int index = ConvertCountToVector2Int(buttonIndex);
        PlacePawn(index, true);

        if(HasMovesLeft(tiles))
        {
            Vector2Int value = FindBestMove(tiles);
            PlacePawn(value, false);
        }

        if (EvaluateBoard(tiles) != 0 || !HasMovesLeft(tiles))
            GameOver();

    }

    private void GameOver()
    {
        foreach (Tile t in tiles)
        {
            t.button.interactable = false;
        }
        menu.ShowLoseScreen(EvaluateBoard(tiles));
    }

    public void Restart()
    {
        foreach (Tile t in tiles)
        {
            ResetTile(t);
        }
        Initialize(buttons, images);
    }

    private void ResetTile(Tile t)
    {
        t.button.interactable = true;
        t.image.enabled = false;
        t.image.color = Color.white;
        t.occupiedBy = OccupiedBy.None;
    }


    public void PlacePawn(Vector2Int arrayPos, bool isPlayerPlacing)
    {
        if (tiles[arrayPos.x, arrayPos.y].occupiedBy == OccupiedBy.None)
        {
            tiles[arrayPos.x, arrayPos.y].image.color = isPlayerPlacing ? Color.green : Color.red;
            tiles[arrayPos.x, arrayPos.y].image.enabled = true;
            tiles[arrayPos.x, arrayPos.y].button.interactable = false;
            if (isPlayerPlacing)
                tiles[arrayPos.x, arrayPos.y].occupiedBy = OccupiedBy.Player;
            else
                tiles[arrayPos.x, arrayPos.y].occupiedBy = OccupiedBy.AI;
        }
    }

    public int EvaluateBoard(Tile[,] board)
    {
        for (int x = 0; x < 3; x++)
        {
            if (board[x, 0].occupiedBy == board[x, 1].occupiedBy && board[x, 1].occupiedBy == board[x, 2].occupiedBy)
            {
                if (board[x, 0].occupiedBy == OccupiedBy.Player)
                    return -10;
                else if (board[x, 0].occupiedBy == OccupiedBy.AI)
                    return 10;
            }
        }

        for (int x = 0; x < 3; x++)
        {
            if (board[0, x].occupiedBy == board[1, x].occupiedBy && board[1, x].occupiedBy == board[2, x].occupiedBy)
            {
                if (board[0, x].occupiedBy == OccupiedBy.Player)
                    return -10;
                else if (board[0, x].occupiedBy == OccupiedBy.AI)
                    return 10;
            }
        }

        if (board[0, 0].occupiedBy == board[1, 1].occupiedBy && board[1, 1].occupiedBy == board[2, 2].occupiedBy)
        {
            if (board[0, 0].occupiedBy == OccupiedBy.Player)
                return -10;
            else if (board[0, 0].occupiedBy == OccupiedBy.AI)
                return 10;

        }

        if (board[0, 2].occupiedBy == board[1, 1].occupiedBy && board[1, 1].occupiedBy == board[2, 0].occupiedBy)
        {
            if (board[0, 2].occupiedBy == OccupiedBy.Player)
                return -10;
            else if (board[0, 2].occupiedBy == OccupiedBy.AI)
                return 10;

        }

        return 0;
    }

    public bool HasMovesLeft(Tile[,] board)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (board[y, x].occupiedBy == OccupiedBy.None)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int MiniMax(Tile[,] board, int depth, bool isMaximizer, int alpha, int beta)
    {
        int score = EvaluateBoard(board);
        if (score == 10)
            return score + depth;
        if (score == -10)
            return score - depth;


        if (!HasMovesLeft(board))
            return 0;

        if (isMaximizer)
        {
            int best = int.MinValue;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (board[x, y].occupiedBy == OccupiedBy.None)
                    {
                        board[x, y].occupiedBy = OccupiedBy.AI;
                        int tryBest = Mathf.Max(best, MiniMax(board, depth + 1, !isMaximizer, alpha, beta));
                        board[x, y].occupiedBy = OccupiedBy.None;
                        best = Mathf.Max(best, tryBest);
                        alpha = Mathf.Max(alpha, best);
                        if (beta <= alpha) break;
                    }
                }
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (board[x, y].occupiedBy == OccupiedBy.None)
                    {
                        board[x, y].occupiedBy = OccupiedBy.Player;
                        int tryBest = Mathf.Min(best, MiniMax(board, depth + 1, !isMaximizer, alpha, beta));
                        board[x, y].occupiedBy = OccupiedBy.None;
                        best = Mathf.Min(best, tryBest);
                        beta = Mathf.Min(beta, best);
                        if (beta <= alpha) break;
                    }
                }
            }
            return best;
        }
    }

    public Vector2Int FindBestMove(Tile[,] board)
    {
        int bestValue = int.MinValue;
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (board[x, y].occupiedBy == OccupiedBy.None)
                {
                    board[x, y].occupiedBy = OccupiedBy.AI;
                    int moveValue = MiniMax(board, 0, false, alpha, beta);
                    board[x, y].occupiedBy = OccupiedBy.None;

                    if (moveValue > bestValue)
                    {
                        bestMove.x = x;
                        bestMove.y = y;
                        bestValue = moveValue;
                    }

                    alpha = Mathf.Max(alpha, bestValue);
                    if (beta <= alpha)
                        break;
                }
            }
        }
        return bestMove;
    }
}

public enum OccupiedBy
{
    Player = 1,
    AI = -1,
    None = 0
}

public struct Tile
{
    public Image image;
    public Button button;
    public OccupiedBy occupiedBy;
    public Tile(Button _button, Image _image, OccupiedBy _occupiedBy)
    {
        button = _button;
        image = _image;
        occupiedBy = _occupiedBy;
    }
}
