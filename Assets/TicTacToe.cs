using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TicTacToe : MonoBehaviour
{
    public bool playerStart = true;
    public int counts = 0;

    // 0 = not taken
    // 1 = taken by player
    // 2 = taken by AI
    public int[] board = new int[9];

    public int spacesTaken = 0;
    public bool aiTurn = false;
    public Button[] buttons = new Button[9];
    public Text[] overlaytext = new Text[9];

    //              8 possible wins
    //     1 1 1         0 0 0         0 0 0
    //     0 0 0         1 1 1         0 0 0
    //     0 0 0         0 0 0         1 1 1
    // ------------- ------------- -------------

    //     1 0 0         0 1 0         0 0 1
    //     1 0 0         0 1 0         0 0 1
    //     1 0 0         0 1 0         0 0 1
    // ------------- ------------- -------------

    //     1 0 0         0 0 1
    //     0 1 0         0 1 0
    //     0 0 1         1 0 0
    // ------------- -------------

    //    Indexes
    //     0 1 2    
    //     3 4 5    
    //     6 7 8    
    // -------------

    public int EvaluateWin(int[] boardToEvaluate)
    {
        // Horizontal
        if (boardToEvaluate[0] != 0 &&
            boardToEvaluate[0] == boardToEvaluate[1] && boardToEvaluate[0] == boardToEvaluate[2])
        {
            return boardToEvaluate[0];
        }

        if (boardToEvaluate[3] != 0 &&
                boardToEvaluate[3] == boardToEvaluate[4] && boardToEvaluate[3] == boardToEvaluate[5])
        {
            return boardToEvaluate[3];
        }

        if (boardToEvaluate[6] != 0 &&
                boardToEvaluate[6] == boardToEvaluate[7] && boardToEvaluate[6] == boardToEvaluate[8])
        {
            return boardToEvaluate[6];
        }

        // Vertical
        if (boardToEvaluate[0] != 0 &&
                boardToEvaluate[0] == boardToEvaluate[3] && boardToEvaluate[0] == boardToEvaluate[6])
        {
            return boardToEvaluate[0];
        }

        if (boardToEvaluate[1] != 0 &&
                boardToEvaluate[1] == boardToEvaluate[4] && boardToEvaluate[1] == boardToEvaluate[7])
        {
            return boardToEvaluate[1];
        }

        if (boardToEvaluate[2] != 0 &&
                boardToEvaluate[2] == boardToEvaluate[5] && boardToEvaluate[2] == boardToEvaluate[8])
        {
            return boardToEvaluate[2];
        }

        // Diagonal
        if (boardToEvaluate[0] != 0 &&
                boardToEvaluate[0] == boardToEvaluate[4] && boardToEvaluate[0] == boardToEvaluate[8])
        {
            return boardToEvaluate[0];
        }

        if (boardToEvaluate[2] != 0 &&
                boardToEvaluate[2] == boardToEvaluate[4] && boardToEvaluate[2] == boardToEvaluate[6])
        {
            return boardToEvaluate[2];
        }

        return 0;
    }

    [ContextMenu("TestAI")]
    public void AITurn()
    {
        int indexToPlay = Minimax();

        buttons[indexToPlay].GetComponentInChildren<Text>().text = "O";
        board[indexToPlay] = 2;

        int win = EvaluateWin(board);

        Win(win);

        spacesTaken++;
        aiTurn = !aiTurn;

    }

    // returns best index to place in
    [ContextMenu("Test Minimax")]
    public int Minimax()
    {
        return CalcGameFromSpace(0, board, 2, 9 - spacesTaken);
    }

    // returns score from space
    public int CalcGameFromSpace(int depth, int[] currentBoard, int playerIndex, int freeSpaces)
    {
        int gameState = EvaluateWin(currentBoard);
        int[] newBoard = new int[9];
        List<int> values = new List<int>();
        List<int> indexes = new List<int>();

        if (gameState == 0 && freeSpaces > 0)         // Not finished
        {
            for (int j = 0; j < 9; j++)
            {
                // Make a new board
                currentBoard.CopyTo(newBoard, 0);

                if (newBoard[j] != 1 && newBoard[j] != 2)
                {
                    newBoard[j] = playerIndex;

                    // Swap player and AI turn
                    int otherPlayerIndex = 0;
                    if (playerIndex == 1)       // Is Player
                        otherPlayerIndex = 2;
                    else if (playerIndex == 2)  // Is AI
                        otherPlayerIndex = 1;

                    indexes.Add(j);
                    values.Add(CalcGameFromSpace(depth + 1, newBoard, otherPlayerIndex, freeSpaces - 1));
                }
            }

            int maxIndex = values.IndexOf(values.ToArray().Max());
            int minIndex = values.IndexOf(values.ToArray().Min());
            if (playerIndex == 1)       // Is Player
            {
                return values[minIndex];
            }
            else                        // Is AI
            {
                if (depth == 0)
                {
                    for (int k = 0; k < values.Count; k++)
                    {
                        overlaytext[k].text = "" + values[k];
                    }
                    return indexes[maxIndex];
                }
                else
                {
                    return values[maxIndex];
                }
            }
        }
        else if (gameState == 1)    // Player Win
        {
            return depth - 10;
        }
        else if (gameState == 2)    // AI Win
        {
            return 10 - depth;
        }
        else                        // Tie
        {
            return 0;
        }
    }

    public void ButtonPressed(int space)
    {
        int win = EvaluateWin(board);

        if (!aiTurn && spacesTaken < 9 && board[space] == 0 && win == 0)
        {
            buttons[space].GetComponentInChildren<Text>().text = "X";

            board[space] = 1;

            spacesTaken++;
            aiTurn = !aiTurn;

            Win(win);

            if (win == 0 && spacesTaken < 9)
            {
                if (aiTurn)
                    AITurn();
            }
        }
    }

    public void Win(int winner)
    {
        if (winner == 1)
            print("Player Won!");
        else if (winner == 2)
            print("AI Won!");
    }

    public void Draw()
    {
        print("Draw! No winner!");
    }
}