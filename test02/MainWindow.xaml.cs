using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
//using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Collections.ObjectModel;

namespace test02
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class Global : Window
    {
        enum Keys { W, D, S, A }

        public bool status = false;

        internal static Canvas canvas;
        internal static TextBlock console; 
        internal static Objects.Player player;

        public Global()
        {
            InitializeComponent();
            //Init();

            canvas = MyCanvas;
            console = MyConsole;
            Generate.map = new int[5, 5];

            Objects.Vector2D start = new Objects.Vector2D(0, 0);
            Objects.Vector2D end = new Objects.Vector2D(4, 4);

            Funcs.Print(start != end);

            //var path = PathFinder.GetPath(Generate.map, start, end);
            //Funcs.Print(path[0]);
        }

        public void Init()
        {
            Generate.map = new int[Convert.ToInt32(Global.canvas.Height / Generate.ts), Convert.ToInt32(Global.canvas.Width / Generate.ts)];
            canvas = MyCanvas;
            player = new Objects.Player(256, 256);
            Generate.GenerateMap();
            status = true;

            for (int i = 0; i < Objects.objects.Count; i++)
            {
                MyConsole.Text += Convert.ToString(Objects.objects[i].name + ", ");
            }
        }

        internal static void UpdateCanvas(Image img, Objects.Vector2D pos, int zIdx)
        {
            /*
            for (int i = 0; i < Global.canvas.Children.Count; i++)
            {
            }*/
            Canvas.SetLeft(img, pos.x);
            Canvas.SetTop(img, pos.y);
            Canvas.SetZIndex(img, zIdx);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (status)
            {
                base.OnKeyDown(e);
                if (e.Key == Key.W)
                {
                    if (CanMove(player.position, 0)) { player.MoveAndCollide(new Objects.Vector2D(0, -player.speed)); }
                }
                if (e.Key == Key.D)
                {
                    if (CanMove(player.position, 1)) { player.MoveAndCollide(new Objects.Vector2D(player.speed, 0)); }
                }
                if (e.Key == Key.S)
                {
                    if (CanMove(player.position, 2)) { player.MoveAndCollide(new Objects.Vector2D(0, player.speed)); }
                }
                if (e.Key == Key.A)
                {
                    if (CanMove(player.position, 3)) { player.MoveAndCollide(new Objects.Vector2D(-player.speed, 0)); }
                }
            }
        }
        public bool CanMove(Objects.Vector2D pos, int dir)
        {
            if (dir == 0) { return pos.y > 0; }
            if (dir == 1) { return pos.x < canvas.Width - 32; }
            if (dir == 2) { return pos.y < canvas.Height - 32; }
            else { return pos.x > 0; }
        }
    }
}
