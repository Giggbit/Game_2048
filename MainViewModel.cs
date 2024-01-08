using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace _2048
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        Block[,] blks = new Block[4, 4];
        Block[,] OldBlks = new Block[4, 4];
        int score, PrevScore;
        Button[,] Btns = new Button[4, 4];
        DoubleAnimation DAnimation;
        Storyboard SBoard;
        Grid GameGrid;
        object sender;
        KeyEventArgs e;

        public MainViewModel(string name) { 
            if(GameGrid != null) {
                GameGrid.Name = name;
            }

            DAnimation = new DoubleAnimation();
            DAnimation.From = 80;
            DAnimation.To = 40;
            DAnimation.Duration = TimeSpan.FromMilliseconds(250);

            SBoard = new Storyboard();
            SBoard.Children.Add(DAnimation);
            
            NewGame();
        }


        private ICommand _score;
        public ICommand Score {
            get { return  _score; }
            set {
                if(_score != value) { 
                    _score = value;
                    OnPropertyChanged(nameof(Score));
                }
            }
        }


        private ICommand _startGame;
        public ICommand StartGame { 
            get { return _startGame ?? (_startGame = new RelayCommand(() => NewGame())); }
        }

        private void NewGame() {
            score = 0;

            Block.InitNewGameBlocks(ref blks);
            Block.InitBlocks(ref OldBlks);
            Block.CoppyBlock(ref OldBlks, ref blks);

            //Score.Execute(score);
            DrawNewBlock();
        }

        //private void BackGame() {
        //    score = PrevScore;

        //    Block.CoppyBlock(ref blks, ref OldBlks);

        //    Score.Execute(score);
        //    DrawNewBlock();
        //}

        private void LoadGame(Block[,] NewBlocks, int NewScore) {
            score = NewScore;
            Block.CoppyBlock(ref blks, ref NewBlocks);

            //Score.Execute(NewScore);
            DrawNewBlock();
        }

        private bool GameOver(Block[,] blks) {
            //Check is there a block zero (null block)
            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < 4; col++) {
                    if (blks[row, col].num == 0)
                        return false;
                }
            }
            //Check every couples
            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < 3; col++) {
                    if (blks[row, col].num == blks[row, col + 1].num)
                        return false;
                }
            }
            for (int col = 0; col < 4; col++) {
                for (int row = 0; row < 3; row++) {
                    if (blks[row + 1, col].num == blks[row, col].num)
                        return false;
                }
            }
            //You dead my friend
            return true;
        }


        private Grid _grid;
        public Grid Grid { 
            get { return _grid; }
            set { 
                if (_grid != value) {
                    _grid = value;
                    OnPropertyChanged(nameof(Grid));
                }
            }
        }

        private void GridClear() {
            for (int i = 0; i < GameGrid.Children.Count; i++) {
                if ((Grid.GetColumn(GameGrid.Children[i]) != 4)) {
                    GameGrid.Children.Remove(GameGrid.Children[i]);
                }
            }
        }

        private void DrawNewBlock() {
            GridClear();
            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < 4; col++) {

                    Btns[row, col] = new Button();
                    Btns[row, col].BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
                    Btns[row, col].BorderThickness = new Thickness(3);
                    Btns[row, col].FontStretch = new FontStretch();

                    if (blks[row, col].num != 0) {
                        Btns[row, col].Background = Block.GetTitleBlocksColor(blks[row, col]);
                        Btns[row, col].Content = blks[row, col].num.ToString();
                        Btns[row, col].FontSize = 40;

                        if (blks[row, col].Combined == true || blks[row, col].NewBlock == true) {
                            Storyboard.SetTarget(DAnimation, Btns[row, col]);
                            Storyboard.SetTargetProperty(DAnimation, new PropertyPath(Button.FontSizeProperty));
                            SBoard.Begin(Btns[row, col]);
                        }
                    }

                    else {
                        Btns[row, col].Background = Block.GetBlocksNoneTitleColor();
                    }

                    Grid.SetColumn(Btns[row, col], col);
                    Grid.SetRow(Btns[row, col], row);
                    GameGrid.Children.Add(Btns[row, col]);
                }
            }
        }

        private void MoveUp() {
            PrevScore = score;
            if (Block.TryUp(ref blks, ref OldBlks, ref score) == true) {

                Block.GenerateABlock(ref blks);

                DrawNewBlock();
                //Score.Execute(score);
                //Score.Text = score.ToString();
            }
        }

        private void MoveDown() {
            PrevScore = score;
            if (Block.TryDown(ref blks, ref OldBlks, ref score) == true) {

                Block.GenerateABlock(ref blks);

                DrawNewBlock();
                //Score.Execute(score);
                //Score.Text = score.ToString();
            }
        }

        private void MoveLeft() {
            PrevScore = score;
            if (Block.TryLeft(ref blks, ref OldBlks, ref score) == true) {

                Block.GenerateABlock(ref blks);

                DrawNewBlock();
                //Score.Execute(score);
                //Score.Text = score.ToString();
            }
        }

        private void MoveRight() {
            PrevScore = score;
            if (Block.TryRight(ref blks, ref OldBlks, ref score) == true) {

                Block.GenerateABlock(ref blks);

                DrawNewBlock();
                //Score.Execute(score);
                //Score.Text = score.ToString();
            }
        }
        
        public void KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Up:
                    MoveUp();
                    break;
                case Key.Down:
                    MoveDown();
                    break;
                case Key.Right:
                    MoveRight();
                    break;
                case Key.Left:
                    MoveLeft();
                    break;
                default:
                    break;
            }

            if (GameOver(blks)) {
                MessageBox.Show("Game over! Score: " + score.ToString(), "Notification");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
