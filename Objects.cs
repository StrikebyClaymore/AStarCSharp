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
            public static bool operator !=(Vector2D obj1, Vector2D obj2)
            {
                return !((obj1.x == obj2.x) && (obj1.x == obj2.y));
            }
            public static bool operator ==(Vector2D obj1, Vector2D obj2)
            {
                return ((obj1.x == obj2.x) && (obj1.y == obj2.y));
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
            public CollisionShape shape;

            public void FirstInit(double _x, double _y)
            {
                this.position.x = Convert.ToDouble(_x);
                this.position.y = Convert.ToDouble(_y);
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
            public int speed = 32;

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
                zIdx = 10;
                FirstInit(Convert.ToDouble(_x), Convert.ToDouble(_y));
                this.shape = new RectangleShape(32, 32, true)
                {
                    position = position
                };
                InitObject(this.image, this.position, this.zIdx);
                this.shape.Monitoring(this);
            }

            public void MoveAndCollide(Vector2D vel)
            {
                if (shape.colliding.Count == 0)
                {
                    position.x += vel.x;
                    position.y += vel.y;
                    shape.position = position;
                    Global.UpdateCanvas(image, position, zIdx);
                }
                if (shape.colliding.Count > 0)
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
                            Funcs.Print(Convert.ToString(shape.colliding.Count + shape.colliding[i].name + shape.colliding[i].shape.enabled));
                            break;
                        }
                    }
                    if (count == shape.colliding.Count)
                    {
                        position.x += vel.x;
                        position.y += vel.y;
                        shape.position = position;
                        Global.UpdateCanvas(image, position, zIdx);
                    }
                }
            }
        }

        public class Monster: KinematicObjects
        {
            public int speed = 16;

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
                FirstInit(Convert.ToDouble(_x), Convert.ToDouble(_y));
                this.shape = new RectangleShape(32, 32, true)
                {
                    position = position
                };
                InitObject(this.image, this.position, this.zIdx);
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
