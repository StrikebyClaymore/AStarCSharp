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
namespace test02
{
    class Funcs
    {
        static Random rnd = new Random();

        //

        internal static void Print(string s)
        {
            ShowText(s);
        }
        internal static void Print(int i)
        {
            ShowText(Convert.ToString(i));
        }
        internal static void Print(double d)
        {
            ShowText(Convert.ToString(d));
        }
        internal static void Print(bool b)
        {
            ShowText(Convert.ToString(b));
        }
        internal static void Print(List<double> ld)
        {
            string str = "";
            for (int j = 0; j < ld.Count; j++)
            {
                if (j == 0)
                {
                    str += "[";
                }
                str += Convert.ToString(ld[j]);
                if (j < ld.Count - 1)
                {
                    str += "; ";
                }
                if (j == ld.Count - 1)
                {
                    str += "]";
                }
            }
        }
        internal static void Print(Objects.Vector2D vec2d)
        {
            ShowText("{" + vec2d.x.ToString() + "; " + vec2d.y.ToString() + "}");
        }
        internal static void Print(List<int> list)
        {
            string str = "[ ";
            for (int i = 0; i < list.Count; i++)
            {
                str += Convert.ToString(list[i] + " ");
                if (i > 0 && (i+1)%Convert.ToInt32(Global.canvas.Width/32) == 0)
                {
                    if (i == list.Count - 1) { continue; }
                    str += "\n";
                    str += "  ";
                }
            }
            str += "] \n";
            ShowText(str);
        }

        internal static void ShowText(string str)
        {
            //MessageBox.Show(str);
            Global.console.Text += str;
        }

        internal static int GetRandom(int? a = null, int? b = null)
        {
            int value = 0;

            if (a != null && b != null)
            {
                value = rnd.Next(Convert.ToInt32(a), Convert.ToInt32(b));
            }
            else
            {
                value = rnd.Next();
            }

            return value;
        }

        internal static int ExcludingRandInt(int a, int b, List<int> c = null, int? d = null)
        {
            List<int> mas = new List<int> { };

            for (int i = a; i < b + 1; i++)
            {
                if (c != null && c.Contains(i))
                {
                    continue;
                }
                else if (d != null && d == i)
                {
                    continue;
                }
                else
                {
                    mas.Add(i);
                }
            }
            return mas[GetRandom(0, mas.Count)];
        }

        internal static bool CheckIncludes(Objects.Vector2D pos1, Objects.Vector2D size1, Objects.Vector2D pos2, Objects.Vector2D size2, Objects.Vector2D vec = null)
        {
            if (vec == null)
            {
                return ((pos1.x + size1.x < pos2.x) || (pos1.x > pos2.x + size2.x) || ((pos1.x + size1.x >= pos2.x && pos1.x <= pos2.x + size2.x) && (pos1.y + size1.y < pos2.y || pos1.y > pos2.y + size2.y)));
            }
            return ((pos1.x + vec.x < pos2.x) || (pos1.x + vec.x > pos2.x) || ((pos1.x + vec.x >= pos2.x && pos1.x + vec.x <= pos2.x) && (pos1.y + vec.y < pos2.y || pos1.y + vec.y > pos2.y)));
        }

        internal static Objects.Vector2D GlobalToMap(Objects.Vector2D pos)
        {
            return new Objects.Vector2D(Math.Floor(pos.x), Math.Floor(pos.y));
        }

        internal static Objects.Vector2D MapToCell(Objects.Vector2D pos)
        {
            return new Objects.Vector2D(pos.x/Generate.ts, pos.y/Generate.ts);
        }

        internal static Objects.Vector2D CellToMap(Objects.Vector2D point)
        {
            return new Objects.Vector2D(point.x*32, point.y*32);
        }

        internal static int GetCellIndex(Objects.Vector2D point)
        {
            return Convert.ToInt32(point.y * Generate.map.GetLength(0) + point.x);
        }

        internal static void LockMapCell(Objects.Vector2D pos, int type)
        {
            var point = MapToCell(GlobalToMap(pos));
            Generate.map[Convert.ToInt32(point.x), Convert.ToInt32(point.y)] = type;
        }

        internal static void OpenMapCell(Objects.Vector2D pos)
        {
            var point = MapToCell(GlobalToMap(pos));
            Generate.map[Convert.ToInt32(point.x), Convert.ToInt32(point.y)] = 0;
        }
    }
}