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
    public class Objects
    {
        internal static List<int> indexes = new List<int> { };
        internal static List<PhysicsObjects> objects = new List<PhysicsObjects> { };
        internal static List<CollisionShape> collisions = new List<CollisionShape> { };

        public class Vector2D
        {
            public double x;
            public double y;

            public Vector2D(double _x = 0, double _y = 0)
            {   
                this.x = _x;
                this.y = _y;
            }

            public override bool Equals(object obj)
            {
                if (obj is Vector2D)
                {
                    if (((Vector2D)obj).x == this.x && ((Vector2D)obj).y == this.y)
                        return true;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }
            public static bool operator ==(Vector2D obj1, Vector2D obj2)
            {
                if (obj1 is null && obj2 is null) return true;
                else if (obj2 is null) return false;
                else return ((obj1.x == obj2.x) && (obj1.y == obj2.y));
            }
            public static bool operator !=(Vector2D obj1, Vector2D obj2)
            {   
                return ((obj1.x == obj2.x) && (obj1.y == obj2.y));
            }
            public static Vector2D operator +(Vector2D obj1, Vector2D obj2)
            {
                return new Vector2D((obj1.x + obj2.x), (obj1.y + obj2.y));
            }
            public static Vector2D operator -(Vector2D obj1, Vector2D obj2)
            {
                return new Vector2D((obj1.x - obj2.x), (obj1.y - obj2.y));
            }
            public static Vector2D operator *(Vector2D obj1, int i)
            {
                return new Vector2D(obj1.x*i, obj1.y*i);
            }
            public static Vector2D operator /(Vector2D obj1, int i)
            {
                return new Vector2D(obj1.x / i, obj1.y / i);
            }
        }

        public class CollisionShape
        {
            public bool exists = false;
            public string name = "";
            public Vector2D position = new Vector2D();
            public Vector2D size = new Vector2D();
            public bool enabled = false;
            public bool collide = false;
            public List<PhysicsObjects> colliding = new List<PhysicsObjects> { };

            async public Task Monitoring(PhysicsObjects self)
            {
                while (exists)
                {
                    await Task.Delay(1);
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (self != objects[i] && self.shape.enabled && objects[i].shape.enabled && !Funcs.CheckIncludes(self.shape.position, self.shape.size, objects[i].shape.position, objects[i].shape.size))
                        {
                            if (!self.shape.colliding.Contains(objects[i]))
                            {
                                self.shape.colliding.Add(objects[i]);
                            }
                            self.shape.collide = true;
                            if (!objects[i].shape.colliding.Contains(self))
                            {
                                objects[i].shape.colliding.Add(self);
                            }
                            objects[i].shape.collide = true;
                            
                        }
                        else
                        {
                            if (self.shape.colliding.Contains(objects[i]))
                            {
                                self.shape.colliding.Remove(objects[i]);
                            }
                            self.shape.collide = false;
                            if (objects[i].shape.colliding.Contains(self))
                            {
                                objects[i].shape.colliding.Remove(self);
                            }
                            objects[i].shape.collide = false;
                        }
                    }
                }
            }
        }

        public class RectangleShape: CollisionShape
        {
            public RectangleShape()
            {
                this.name = "RectangleShape";
            }

            public RectangleShape(double _x, double _y, bool en)
            {
                this.name = "RectangleShape";
                this.size.x = _x;
                this.size.y = _y;
                this.exists = true;
                this.enabled = en;
            }
        }

        public class PhysicsObjects
        {
            public string name = "PhysicsObjects";
            public int id = GenerateId();
            public Vector2D position = new Vector2D();
            public Image image = new Image();
            public int zIdx = 0;
            public int type = 0;
            public CollisionShape shape;
            public List<string> groups = new List<string> { };

            public void FirstInit(double _x, double _y)
            {
                this.position.x = Convert.ToDouble(_x);
                this.position.y = Convert.ToDouble(_y);
                Funcs.LockMapCell(this.position, this.type);
            }

            public Vector2D GetPosition()
            {
                return position;
            }
        }
        public class KinematicObjects : PhysicsObjects
        {
            public KinematicObjects()
            {
                this.name = "KinematicObjects";
            }

            public void MoveAndCollide(Vector2D vel)
            {
                if (shape.colliding.Count == 0)
                {
                    Funcs.OpenMapCell(position);
                    position.x += vel.x;
                    position.y += vel.y;
                    shape.position = position;
                    Global.UpdateCanvas(image, position, zIdx);
                    Funcs.LockMapCell(position, type);
                }
                else if (shape.colliding.Count > 0)
                {
                    int count = 0;
                    for (int i = 0; i < shape.colliding.Count; i++)
                    {
                        if (Funcs.CheckIncludes(shape.position, shape.size, shape.colliding[i].shape.position, shape.colliding[i].shape.size, vel))
                        {
                            count++;
                        }
                        else
                        {
                            Funcs.Print(Convert.ToString(this.name + " collide with " + shape.colliding.Count + shape.colliding[i].name + " "));
                            break;
                        }
                    }
                    if (count == shape.colliding.Count)
                    {
                        Funcs.OpenMapCell(position);
                        position.x += vel.x;
                        position.y += vel.y;
                        shape.position = position;
                        Global.UpdateCanvas(image, position, zIdx);
                        Funcs.LockMapCell(position, type);
                    }
                }
            }
        }

        public class StaticObjects : PhysicsObjects
        {
            public StaticObjects()
            {
                this.name = "StaticObjects";
            }
        }

        // <Basic game classes>

        // <Turfs and Structs>
        public class EarthFloor: StaticObjects
        {
            public EarthFloor()
            {
                Init();
            }
            public EarthFloor(double posX, double posY)
            {
                Init(posX, posY);
            }
            public void Init(double? _x = null, double? _y = null)
            {
                this.image.Source = new BitmapImage(new Uri("C:/Users/True/Desktop/code/projects/c#/test02/test02/imgs/earth.png", UriKind.RelativeOrAbsolute));
                this.name = "EarthFloor";
                zIdx = 1;
                this.type = 0;
                FirstInit(Convert.ToDouble(_x), Convert.ToDouble(_y));
                this.shape = new RectangleShape(32, 32, false)
                {
                    position = position
                };
                InitObject(this.image, this.position, this.zIdx);
            }
        }

        public class Wall : StaticObjects
        {
            public Wall()
            {
                Init();
            }
            public Wall(double posX, double posY)
            {
                Init(posX, posY);
            }
            public void Init(double? _x = null, double? _y = null)
            {
                this.image.Source = new BitmapImage(new Uri("C:/Users/True/Desktop/code/projects/c#/test02/test02/imgs/stoneWall.png", UriKind.RelativeOrAbsolute));
                this.name = "Wall";
                zIdx = 2;
                this.type = 3;
                FirstInit(Convert.ToDouble(_x), Convert.ToDouble(_y));
                this.shape = new RectangleShape(32, 32, true)
                {
                    position = position
                };
                InitObject(this.image, this.position, this.zIdx);
            }
        }

        // <Mobs>
        public class Player : KinematicObjects
        {
            public int step = 32;

            public Player()
            {
                Init();
            }
            public Player(double posX, double posY)
            {
                Init(posX, posY);
            }
            public void Init(double? _x = null, double? _y = null)
            {
                this.image.Source = new BitmapImage(new Uri("C:/Users/True/Desktop/code/projects/c#/test02/test02/imgs/player.png", UriKind.RelativeOrAbsolute));
                this.name = "Player";
                this.groups.Add("MobTarget");
                zIdx = 10;
                this.type = 9;
                FirstInit(Convert.ToDouble(_x), Convert.ToDouble(_y));
                this.shape = new RectangleShape(32, 32, true)
                {
                    position = position
                };
                InitObject(this.image, this.position, this.zIdx);
                this.shape.Monitoring(this);
            }

            
        }

        public class Monster: KinematicObjects
        {
            public int step = 32;
            public int stepTime = 600;
            public PhysicsObjects target;
            public int searchRange = 10;
            public bool life = false;
            public enum Status { Stay, Move, Fight }
            public Status status = Status.Stay;

            public List<Vector2D> path = new List<Vector2D> { };

            public Monster()
            {
                Init();
            }
            public Monster(double posX, double posY)
            {
                Init(posX, posY);
            }
            public void Init(double? _x = null, double? _y = null)
            {
                this.image.Source = new BitmapImage(new Uri("C:/Users/True/Desktop/code/projects/c#/test02/test02/imgs/enemy.png", UriKind.RelativeOrAbsolute));
                this.name = "Monster";
                this.zIdx = 9;
                this.type = 5;
                FirstInit(Convert.ToDouble(_x), Convert.ToDouble(_y));
                this.shape = new RectangleShape(32, 32, true)
                {
                    position = position
                };
                InitObject(this.image, this.position, this.zIdx);
                this.life = true;
                this.shape.Monitoring(this);
                AI();
            }

            public async Task AI()
            {
                while (life)
                {
                    await Task.Delay(1);
                    if (status == Status.Stay) await FindTarget();
                    else if (status == Status.Move) await MoveToTarget();
                }
            }

            public async Task MoveToTarget()
            {
                while (path.Count > 2)
                {
                    await Task.Delay(stepTime);
                    MoveAndCollide((path[1] - path[0])*step);
                    //var point = path[1] - path[0];
                    //Generate.map[Convert.ToInt32(point.x), Convert.ToInt32(point.y)] = 5;
                    path.Remove(path[0]);
                    var newPath = PathFinder.GetPath(Generate.map, Funcs.GlobalToMap(Funcs.MapToCell(position)), Funcs.GlobalToMap(Funcs.MapToCell(target.position)));
                    if (newPath == null)
                    {
                        path.Clear();
                        status = Status.Stay;
                        break;
                    }
                    if (newPath.Count <= searchRange) path = newPath;
                }
                path.Clear();
                status = Status.Stay;
            }

            public async Task FindTarget()
            {
                await Task.Delay(1);

                foreach (PhysicsObjects trg in objects)
                {
                    if (trg.groups.Contains("MobTarget"))
                    {
                        var newPath = PathFinder.GetPath(Generate.map, Funcs.GlobalToMap(Funcs.MapToCell(position)), Funcs.GlobalToMap(Funcs.MapToCell(trg.position)));
                        if (newPath == null) continue;
                        if (newPath.Count >= searchRange) continue;
                        if (this.path.Count == 0)
                        {
                            this.path = newPath;
                            this.target = trg;
                        }
                        else if (newPath.Count < this.path.Count)
                        {
                            this.path = newPath;
                            this.target = trg;
                        }
                    }
                }
                if (this.path.Count > 1) status = Status.Move;
            }
        }

        internal static void InitObject(Image img, Objects.Vector2D pos, int zIdx)
        {
            Global.canvas.Children.Add(img);
            img.Stretch = Stretch.None;
            Canvas.SetLeft(img, pos.x);
            Canvas.SetTop(img, pos.y);
            Canvas.SetZIndex(img, zIdx);
        }

        static int GenerateId()
        {
            int index = Funcs.ExcludingRandInt(0, 10000, indexes);
            indexes.Add(index);
            return index;
        }


    }
}
