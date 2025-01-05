using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

public class MinesweeperForm : Form
{
    private Button[,] buttons; // 儲存所有按鈕的二維陣列
    private char[,] grid; // 儲存地雷和數字的遊戲網格
    private const int SIZE = 10; // 網格的大小
    private const int MINE_COUNT = 10; // 地雷的數量
    private const int MIN_BUTTON_SIZE = 20; // 最小按鈕大小
    private int buttonSize = 30; // 當前按鈕大小
    private Label mineCounter; // 顯示剩餘地雷數量的標籤
    private readonly int[][] directions = new int[][] // 用於計算周圍地雷數的方向陣列
    {
        new int[] {-1, -1}, new int[] {-1, 0}, new int[] {-1, 1},
        new int[] {0, -1},                     new int[] {0, 1},
        new int[] {1, -1},  new int[] {1, 0},  new int[] {1, 1}
    };
    private bool isGameOver = false; // 遊戲是否結束的標誌
    private int flagCount = 0; // 追蹤插旗數量

    // 建立一個 enum 來表示格子的狀態
    enum CellState
    {
        Unrevealed, // 未揭露
        Revealed,   // 已揭露
        Flagged,    // 已插旗
        Mine        // 地雷
    }

    // 在 MinesweeperForm 類別中使用 enum
    private CellState[,] cellStates; // 儲存每個格子的狀態

    public MinesweeperForm()
    {
        InitializeComponents(); // 初始化 UI 元件
        InitializeGame(); // 初始化遊戲狀態
    }

    private void InitializeComponents()
    {
        this.Text = "踩地雷"; // 設定視窗標題
        this.MinimumSize = new Size(SIZE * MIN_BUTTON_SIZE + 50, SIZE * MIN_BUTTON_SIZE + 100); // 設定最小視窗大小
        this.Size = new Size(SIZE * buttonSize + 50, SIZE * buttonSize + 100); // 設定初始視窗大小
        this.Resize += Form_Resize; // 添加視窗大小改變事件

        mineCounter = new Label
        {
            Text = $"Mines: {MINE_COUNT}", // 顯示地雷數量
            Location = new Point(10, 10), // 設置在視窗頂部
            Size = new Size(100, 20),
            Anchor = AnchorStyles.Top | AnchorStyles.Left // 固定在左上角
        };
        this.Controls.Add(mineCounter);
        mineCounter.BringToFront(); // 確保計數器在最上層

        buttons = new Button[SIZE, SIZE];
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                buttons[r, c] = new Button
                {
                    Location = new Point(c * buttonSize + 10, r * buttonSize + 40), // 調整按鈕位置
                    Size = new Size(buttonSize, buttonSize),
                    Tag = new Point(r, c), // 使用 Tag 儲存按鈕的位置
                    ContextMenuStrip = new ContextMenuStrip(),
                    BackColor = Color.LightGray, // 未翻開的格子顏色
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat, // 設置扁平樣式
                    FlatAppearance = { BorderSize = 1, BorderColor = Color.Gray } // 設置邊框
                };
                buttons[r, c].MouseUp += Button_Click; // 添加按鈕點擊事件
                this.Controls.Add(buttons[r, c]);
            }
        }
    }

    private void Form_Resize(object sender, EventArgs e)
    {
        // 計算新的按鈕大小
        int newButtonSize = Math.Min(
            (this.ClientSize.Width - 20) / SIZE,
            (this.ClientSize.Height - 70) / SIZE
        );

        // 確保按鈕大小不小於最小值
        newButtonSize = Math.Max(newButtonSize, MIN_BUTTON_SIZE);

        // 如果按鈕大小改變，更新按鈕位置和大小
        if (newButtonSize != buttonSize)
        {
            buttonSize = newButtonSize;
            
            // 更新按鈕
            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {
                    buttons[r, c].Location = new Point(c * buttonSize + 10, r * buttonSize + 40);
                    buttons[r, c].Size = new Size(buttonSize, buttonSize);
                }
            }

            // 更新地雷計數器位置
            mineCounter.Location = new Point(10, 10);
            // 不需要更新 mineCounter 的字體大小，保持固定大小
            mineCounter.BringToFront();
        }
    }

    private void InitializeGame()
    {
        isGameOver = false; // 重置遊戲結束標誌
        flagCount = 0; // 重置插旗數量
        mineCounter.Text = $"Mines: {MINE_COUNT}"; // 更新地雷計數器

        grid = new char[SIZE, SIZE]; // 初始化遊戲網格
        cellStates = new CellState[SIZE, SIZE]; // 初始化格子狀態
        Array.Fill(grid.Cast<char>().ToArray(), '0'); // 將所有格子初始化為 '0'

        // 使用 Fisher-Yates 洗牌算法放置地雷
        var positions = Enumerable.Range(0, SIZE * SIZE).ToList();
        var rand = new Random();
        for (int i = 0; i < MINE_COUNT; i++)
        {
            int j = rand.Next(i, positions.Count);
            (positions[i], positions[j]) = (positions[j], positions[i]);
            grid[positions[i] / SIZE, positions[i] % SIZE] = 'M'; // 放置地雷
        }

        // 計算周圍地雷數
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                if (grid[r, c] == 'M') continue; // 跳過地雷格子
                grid[r, c] = CountMines(r, c); // 計算並設置地雷數
            }
        }

        // 初始化 cellStates
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                cellStates[r, c] = CellState.Unrevealed; // 將所有格子狀態設為未揭露
            }
        }
    }

    private char CountMines(int row, int col)
    {
        int mineCount = 0; // 計算周圍地雷數
        foreach (int[] dir in directions)
        {
            int newRow = row + dir[0];
            int newCol = col + dir[1];
            if (newRow >= 0 && newRow < SIZE && newCol >= 0 && newCol < SIZE && grid[newRow, newCol] == 'M')
            {
                mineCount++; // 增加地雷計數
            }
        }
        return mineCount.ToString()[0]; // 返回地雷數字符
    }

    private void Button_Click(object sender, MouseEventArgs e)
    {
        if (isGameOver) return; // 如果遊戲結束，則不處理點擊

        Button button = (Button)sender;
        Point pos = (Point)button.Tag;
        int row = pos.X;
        int col = pos.Y;

        if (e.Button == MouseButtons.Left)
        {
            if (cellStates[row, col] == CellState.Flagged) return; // 如果已插旗，則不處理

            if (grid[row, col] == 'M')
            {
                isGameOver = true; // 設置遊戲結束
                RevealAllMines(); // 揭露所有地雷
                MessageBox.Show("踩到地雷了！遊戲結束！"); // 顯示遊戲結束訊息
                InitializeGame(); // 重新初始化遊戲
                ResetButtons(); // 重置按鈕
                isGameOver = false; // 重置遊戲結束標誌
            }
            else
            {
                RevealEmptySquares(row, col); // 揭露空白格子
                if (CheckWin())
                {
                    isGameOver = true; // 設置遊戲結束
                    MessageBox.Show("恭喜獲勝！"); // 顯示勝利訊息
                    InitializeGame(); // 重新初始化遊戲
                    ResetButtons(); // 重置按鈕
                    isGameOver = false; // 重置遊戲結束標誌
                }
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            if (cellStates[row, col] == CellState.Unrevealed)
            {
                if (flagCount < MINE_COUNT)
                {
                    button.Text = "🚩"; // 插旗
                    cellStates[row, col] = CellState.Flagged; // 更新狀態
                    flagCount++; // 增加插旗數
                    mineCounter.Text = $"Mines: {MINE_COUNT - flagCount}"; // 更新地雷計數器
                }
            }
            else if (cellStates[row, col] == CellState.Flagged)
            {
                button.Text = ""; // 移除旗子
                cellStates[row, col] = CellState.Unrevealed; // 更新狀態
                flagCount--; // 減少插旗數
                mineCounter.Text = $"Mines: {MINE_COUNT - flagCount}"; // 更新地雷計數器
            }
            return;
        }
    }

    private void RevealAllMines()
    {
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                if (grid[r, c] == 'M')
                {
                    buttons[r, c].Text = "💣"; // 顯示地雷
                    buttons[r, c].BackColor = Color.Red; // 地雷格子顯示紅色
                }
            }
        }
    }

    private void ResetButtons()
    {
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                buttons[r, c].Text = ""; // 清空按鈕文字
                buttons[r, c].Enabled = true; // 啟用按鈕
                buttons[r, c].BackColor = Color.LightGray; // 重置背景顏色
                buttons[r, c].ForeColor = Color.Black; // 重置文字顏色
            }
        }
    }

    private bool CheckWin()
    {
        for (int r = 0; r < SIZE; r++)
        {
            for (int c = 0; c < SIZE; c++)
            {
                if (grid[r, c] != 'M' && buttons[r, c].Enabled)
                    return false; // 如果有未揭露的非地雷格子，則尚未獲勝
            }
        }
        return true; // 所有非地雷格子都已揭露，獲勝
    }

    private void RevealEmptySquares(int row, int col)
    {
        if (row < 0 || row >= SIZE || col < 0 || col >= SIZE || 
            buttons[row, col].Text != "" || grid[row, col] == 'M')
            return; // 如果超出範圍或已揭露或是地雷，則不處理

        buttons[row, col].Text = grid[row, col].ToString(); // 顯示格子數字
        buttons[row, col].Enabled = false; // 禁用按鈕
        buttons[row, col].BackColor = Color.White; // 已翻開的格子顏色

        // 根據數字設置不同顏色
        if (grid[row, col] != '0')
        {
            switch (grid[row, col])
            {
                case '1': buttons[row, col].ForeColor = Color.Blue; break;
                case '2': buttons[row, col].ForeColor = Color.Green; break;
                case '3': buttons[row, col].ForeColor = Color.Red; break;
                case '4': buttons[row, col].ForeColor = Color.DarkBlue; break;
                case '5': buttons[row, col].ForeColor = Color.DarkRed; break;
                default: buttons[row, col].ForeColor = Color.DarkGray; break;
            }
        }

        // 如果是0，則繼續擴展
        if (grid[row, col] == '0')
        {
            // 檢查周圍8個方向
            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int newRow = row + dr;
                    int newCol = col + dc;
                    
                    // 檢查是否在範圍內且未被揭開
                    if (newRow >= 0 && newRow < SIZE && newCol >= 0 && newCol < SIZE && 
                        buttons[newRow, newCol].Text == "")
                    {
                        RevealEmptySquares(newRow, newCol); // 遞迴揭露相鄰空白格子
                    }
                }
            }
        }
    }
}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles(); // 啟用視覺樣式
        Application.SetCompatibleTextRenderingDefault(false); // 設定文字渲染
        Application.Run(new MinesweeperForm()); // 啟動應用程式
    }
}
