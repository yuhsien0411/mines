# 踩地雷遊戲

這是一個使用 C# 和 Windows Forms 實作的簡單踩地雷遊戲。遊戲提供了一個 10x10 的網格，其中隨機分佈著 10 個地雷。玩家需要揭露所有非地雷的格子以獲勝。

## 遊戲流程

1. **初始化遊戲**：
   - 當遊戲開始時，會初始化一個 10x10 的網格。
   - 使用 Fisher-Yates 洗牌算法隨機分配 10 個地雷到網格中。
   - 計算並標記每個格子周圍的地雷數量。

2. **遊戲操作**：
   - 玩家可以左鍵點擊格子來揭露它。
   - 如果揭露的是地雷，遊戲結束。
   - 如果揭露的是空白格子，會自動揭露相鄰的空白格子。
   - 玩家可以右鍵點擊格子來插旗，標記可能有地雷的格子。

3. **勝利條件**：
   - 當所有非地雷的格子都被揭露時，玩家獲勝。

4. **遊戲結束**：
   - 如果玩家踩到地雷，所有地雷會被揭露，並顯示遊戲結束訊息。
   - 玩家可以重新開始遊戲。

## 程式運行流程

1. **啟動應用程式**：
   - 執行 `ConsoleApp1.sln` 解決方案，Visual Studio 會編譯並運行專案。
   - 應用程式啟動後，顯示一個 10x10 的遊戲網格。

2. **初始化**：
   - `MinesweeperForm` 類別的建構函式被調用，初始化 UI 和遊戲狀態。
   - 呼叫 `InitializeComponents()` 設定 UI 元件。
   - 呼叫 `InitializeGame()` 設定遊戲邏輯。

3. **遊戲進行**：
   - 玩家通過點擊網格上的按鈕進行遊戲。
   - `Button_Click()` 方法處理每次點擊事件，更新遊戲狀態。
   - 根據點擊的結果，可能會調用 `RevealEmptySquares()` 或 `RevealAllMines()`。

4. **檢查遊戲狀態**：
   - 每次玩家操作後，檢查是否達到勝利條件。
   - 如果玩家贏得遊戲，顯示勝利訊息。
   - 如果玩家踩到地雷，顯示遊戲結束訊息。

5. **重新開始遊戲**：
   - 玩家可以選擇重新開始遊戲，調用 `InitializeGame()` 和 `ResetButtons()` 重置遊戲狀態。

## 方法調用詳解

- **`InitializeComponents()`**：
  - 設定視窗標題、大小和最小大小。
  - 建立並配置按鈕和標籤，並將它們添加到視窗中。

- **`InitializeGame()`**：
  - 初始化遊戲網格和格子狀態。
  - 隨機放置地雷，並計算每個格子周圍的地雷數量。

- **`Button_Click(object sender, MouseEventArgs e)`**：
  - 處理按鈕的點擊事件。
  - 根據點擊的按鈕位置，更新遊戲狀態。
  - 如果點擊到地雷，調用 `RevealAllMines()`。
  - 如果點擊到空白格子，調用 `RevealEmptySquares()`。

- **`RevealAllMines()`**：
  - 揭露所有地雷，當玩家踩到地雷時使用。

- **`ResetButtons()`**：
  - 重置所有按鈕的狀態，當遊戲重新開始時使用。

- **`CheckWin()`**：
  - 檢查玩家是否已經贏得遊戲。

- **`RevealEmptySquares(int row, int col)`**：
  - 遞迴揭露相鄰的空白格子。

## 物件導向設計

### 類別結構

- **MinesweeperForm**：繼承自 `Form`，是遊戲的主要視窗類別。
  - **屬性**：
    - `buttons`：二維陣列，儲存所有按鈕。
    - `grid`：二維陣列，儲存地雷和數字。
    - `cellStates`：二維陣列，儲存每個格子的狀態（未揭露、已揭露、插旗、地雷）。
    - `mineCounter`：顯示剩餘地雷數量的標籤。
    - `isGameOver`：布林值，表示遊戲是否結束。
    - `flagCount`：整數，追蹤插旗數量。

## 如何運行

1. 確保已安裝 .NET Framework。
2. 使用 Visual Studio 開啟 `ConsoleApp1.sln` 專案。
3. 編譯並運行專案。

## 專案結構

- `ConsoleApp1.sln`：Visual Studio 解決方案檔案。
- `ConsoleApp1/Program.cs`：主要的程式碼檔案，包含遊戲邏輯和 UI 元件。
- `ConsoleApp1/ConsoleApp1.csproj`：專案檔案，定義專案的設定和依賴。
- `ConsoleApp1/ConsoleApp1.csproj.user`：使用者特定的專案設定檔案。

## 貢獻

歡迎對此專案提出建議或貢獻代碼。請提交 Pull Request 或開啟 Issue。