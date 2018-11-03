using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test02
{
    class Generate
    {
        internal static int ts = 32; // basic tile size

        internal static int[,] map;

        internal static void GenerateMap()
        {
            Objects.Monster m = new Objects.Monster(256+64, 128);
            Objects.objects.Add(m);
            for (int y = 0; y < Convert.ToInt32(Global.canvas.Height)/ts; y++)
            {
                for (int x = 0; x < Convert.ToInt32(Global.canvas.Width)/ts; x++)
                {
                    if (x == 5)
                    {
                        if (y == 5)
                        {
                            Objects.EarthFloor e2 = new Objects.EarthFloor(x * ts, y * ts);
                            Objects.objects.Add(e2);
                            continue;
                        }
                        Objects.Wall w = new Objects.Wall(x * ts, y * ts);
                        Objects.objects.Add(w);
                        continue;
                    }
                    Objects.EarthFloor e = new Objects.EarthFloor(x*ts, y*ts);
                    Objects.objects.Add(e);
                }
            }
        }
    }
}
