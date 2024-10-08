using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerOption
{
    NONE, //0
    X, // 1
    O // 2
}

public class TTT : MonoBehaviour
{
    public int Rows;
    public int Columns;
    [SerializeField] BoardView board;
    [SerializeField] TMP_Text text;
    [SerializeField] Button switchButton;

    PlayerOption currentPlayer = PlayerOption.X;
    Cell[,] cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[Columns, Rows];

        board.InitializeBoard(Columns, Rows);

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                cells[j, i] = new Cell();
                cells[j, i].current = PlayerOption.NONE;
            }
        }
    }

    public void MakeOptimalMove()
    {
        bool aCellChosen;

        #region Look for winning cells
        
        aCellChosen = PlayWinningCell();

        if (aCellChosen)
            return;

        aCellChosen = PlayBlockingCell();

        if (aCellChosen)
            return;
        #endregion

        #region First Moves

        //If X goes corner
        if (cells[0, 0].current == PlayerOption.X || cells[2,0].current == PlayerOption.X || cells[0,2].current == PlayerOption.X || cells[2,2].current == PlayerOption.X)
        {
            if (cells[1, 1].current == PlayerOption.NONE)
            {
                ChooseSpace(1, 1);
                return;
            }
        }

        //If X goes middle
        if (cells[1, 1].current == PlayerOption.X)
        {
            if (cells[0, 0].current == PlayerOption.NONE)
            {
                ChooseSpace(0, 0);
                return;
            }
        }

        //If X goes side/top
        if (cells[0,1].current == PlayerOption.X || cells[1,0].current == PlayerOption.X)
        {
            if (cells[2, 2].current == PlayerOption.NONE)
            {
                ChooseSpace(2, 2);
                return;
            }
        }
        else if (cells[1,2].current == PlayerOption.X || cells[2,1].current == PlayerOption.X)
        {
            if (cells[0, 0].current == PlayerOption.NONE)
            {
                ChooseSpace(0, 0);
                return;
            }
        }

        //If going first put X in the corner
        for (int i =0; i < Rows; i++)
        {
            bool broke = false;
            for (int j= 0; j < Columns;j++)
            {
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    continue;
                }
                else
                {
                    broke = true;
                    break;
                }
            }
            if (broke)
                break;

            if (cells[0,0].current == PlayerOption.NONE && i == 2)
            {
                ChooseSpace(0, 0);
                return;
            }
        }


        #endregion

        #region Second Move
        //Cases for X choosing a cross configuration
        if (cells[1,2].current == PlayerOption.X && cells[2,1].current == PlayerOption.X)
        {
            if (cells[1,1].current == PlayerOption.NONE)
            {
                ChooseSpace(1, 1);
                return;
            }
        }
        if (cells[1, 2].current == PlayerOption.X && cells[0, 1].current == PlayerOption.X)
        {
            if (cells[1, 1].current == PlayerOption.NONE)
            {
                ChooseSpace(1, 1);
                return;
            }
        }
        if (cells[1, 0].current == PlayerOption.X && cells[0, 1].current == PlayerOption.X)
        {
            if (cells[1, 1].current == PlayerOption.NONE)
            {
                ChooseSpace(1, 1);
                return;
            }
        }
        if (cells[1, 0].current == PlayerOption.X && cells[2, 1].current == PlayerOption.X)
        {
            if (cells[1, 1].current == PlayerOption.NONE)
            {
                ChooseSpace(1, 1);
                return;
            }
        }

        //Cases if X has gone in a corner for their second move
        if (cells[2,2].current == PlayerOption.X)
        {
            if (cells[1, 2].current == PlayerOption.NONE && currentPlayer == PlayerOption.O)
            {
                ChooseSpace(1, 2);
                return;
            }
        }   
        if (cells[0,2].current == PlayerOption.X)
        {
            if (cells[1,2].current == PlayerOption.NONE && currentPlayer == PlayerOption.O)
            {
                ChooseSpace(1, 2);
                return;
            }
        }

        //Option for O in middle and X second move
        if (cells[1,1].current == PlayerOption.O)
        {
            if (cells[2,2].current == PlayerOption.NONE && cells[0,0].current == PlayerOption.X)
            {
                ChooseSpace(2, 2);
                return;
            }
            else if (cells[2, 0].current == PlayerOption.NONE && cells[0, 2].current == PlayerOption.X)
            {
                ChooseSpace(2, 0);
                return;
            }
            else if (cells[0, 2].current == PlayerOption.NONE && cells[2, 0].current == PlayerOption.X)
            {
                ChooseSpace(0, 2);
                return;
            }
        }

        #endregion

        #region Pick first available

        PickFirstAvailable();

        #endregion

    }

    public void ChooseSpace(int column, int row)
    {
        // can't choose space if game is over
        if (GetWinner() != PlayerOption.NONE)
            return;

        // can't choose a space that's already taken
        if (cells[column, row].current != PlayerOption.NONE)
            return;

        // set the cell to the player's mark
        cells[column, row].current = currentPlayer;

        // update the visual to display X or O
        board.UpdateCellVisual(column, row, currentPlayer);

        // if there's no winner, keep playing, otherwise end the game
        if(GetWinner() == PlayerOption.NONE)
            EndTurn();
        else
        {
            Debug.Log("Player " + currentPlayer + " won!");
            Debug.Log("GAME OVER!");
        }
    }

    public void EndTurn()
    {
        // increment player, if it goes over player 2, loop back to player 1
        currentPlayer += 1;
        if ((int)currentPlayer > 2)
            currentPlayer = PlayerOption.X;
        UpdatePlayerTurnText();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("TTT");
    }

    public void UpdatePlayerTurnText()
    {
        text.text = "Player's Turn: " + currentPlayer;
    }

    public void PickFirstAvailable()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0;  j < Columns; j++)
            {
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    ChooseSpace(j, i);
                    return;
                }
            }
        }
    }

    public bool PlayWinningCell()
    {
        int sum = 0;
        int num1 = 0;
        int num2 = 0;
        
        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;
                else if (cells[j,i].current == PlayerOption.NONE)
                {
                    num1 = j;
                    num2 = i;
                }
                    

                sum += value;
            }

            if (sum == 2 && currentPlayer == PlayerOption.X)
            {
                ChooseSpace(num1, num2);
                return true;
            }
            else if (sum == -2 && currentPlayer == PlayerOption.O)
            {
                ChooseSpace(num1, num2);
                return true;
            }

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;
                else if (cells[j, i].current == PlayerOption.NONE)
                {
                    num1 = j;
                    num2 = i;
                }

                sum += value;
            }

            if (sum == 2 && currentPlayer == PlayerOption.X)
            {
                ChooseSpace(num1, num2);
                return true;
            }
            else if (sum == -2 && currentPlayer == PlayerOption.O)
            {
                ChooseSpace(num1, num2);
                return true;
            }

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;
            else if (cells[i, i].current == PlayerOption.NONE)
            {
                num1 = i;
                num2 = i;
            }

            sum += value;
        }

        if (sum == 2 && currentPlayer == PlayerOption.X)
        {
            ChooseSpace(num1, num2);
            return true;
        }
        
        else if (sum == -2 && currentPlayer == PlayerOption.O)
        {
            ChooseSpace(num1, num2);
            return true;
        }

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.NONE)
            {
                num1 = Columns - 1 - i;
                num2 = i;
            }

            sum += value;
        }

        if (sum == 2 && currentPlayer == PlayerOption.X)
        {
            ChooseSpace(num1, num2);
            return true;
        }
        else if (sum == -2 && currentPlayer == PlayerOption.O)
        {
            ChooseSpace(num1, num2);
            return true;
        }
        else
        {
            Debug.Log("We found no winning squares.");
            return false;
        }
    }

    public bool PlayBlockingCell()
    {
        int sum = 0;
        int num1 = 0;
        int num2 = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;
                else if (cells[j, i].current == PlayerOption.NONE)
                {
                    num1 = j;
                    num2 = i;
                }


                sum += value;
            }

            if (sum == 2 && currentPlayer == PlayerOption.O)
            {
                ChooseSpace(num1, num2);
                return true;
            }
            else if (sum == -2 && currentPlayer == PlayerOption.X)
            {
                ChooseSpace(num1, num2);
                return true;
            }

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;
                else if (cells[j, i].current == PlayerOption.NONE)
                {
                    num1 = j;
                    num2 = i;
                }

                sum += value;
            }

            if (sum == 2 && currentPlayer == PlayerOption.O)
            {
                ChooseSpace(num1, num2);
                return true;
            }
            else if (sum == -2 && currentPlayer == PlayerOption.X)
            {
                ChooseSpace(num1, num2);
                return true;
            }

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;
            else if (cells[i, i].current == PlayerOption.NONE)
            {
                num1 = i;
                num2 = i;
            }

            sum += value;
        }

        if (sum == 2 && currentPlayer == PlayerOption.O)
        {
            ChooseSpace(num1, num2);
            return true;
        }

        else if (sum == -2 && currentPlayer == PlayerOption.X)
        {
            ChooseSpace(num1, num2);
            return true;
        }

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.NONE)
            {
                num1 = Columns - 1 - i;
                num2 = i;
            }

            sum += value;
        }

        if (sum == 2 && currentPlayer == PlayerOption.O)
        {
            ChooseSpace(num1, num2);
            return true;
        }
        else if (sum == -2 && currentPlayer == PlayerOption.X)
        {
            ChooseSpace(num1, num2);
            return true;
        }
        else
        {
            Debug.Log("We found no blocking squares.");
            return false;
        }
    }

    public PlayerOption GetWinner()
    {
        // sum each row/column based on what's in each cell X = 1, O = -1, blank = 0
        // we have a winner if the sum = 3 (X) or -3 (O)
        int sum = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for(int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        return PlayerOption.NONE;
    }
}
