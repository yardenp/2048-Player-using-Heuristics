//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Object;
//using System.Drawing;

//namespace ConsoleApplication1
//{
//    class Program
//    {
//        public class Game2048 : JPanel
//        {
//            private static readonly Color BG_COLOR = new Color(0xbbada0);
//            private const string FONT_NAME = "Arial";
//            private const int TILE_SIZE = 64;
//            private const int TILES_MARGIN = 16;

//            private Tile[] myTiles;
//            internal bool myWin = false;
//            internal bool myLose = false;
//            internal int myScore = 0;

//            public Game2048()
//            {
//                Focusable = true;
//                addKeyListener(new KeyAdapterAnonymousInnerClassHelper(this));
//                resetGame();
//            }

//            private class KeyAdapterAnonymousInnerClassHelper : KeyAdapter
//            {
//                private readonly Game2048 outerInstance;

//                public KeyAdapterAnonymousInnerClassHelper(Game2048 outerInstance)
//                {
//                    this.outerInstance = outerInstance;
//                }

//                public override void keyPressed(KeyEvent e)
//                {
//                    if (e.KeyCode == KeyEvent.VK_ESCAPE)
//                    {
//                        outerInstance.resetGame();
//                    }
//                    if (!outerInstance.canMove())
//                    {
//                        outerInstance.myLose = true;
//                    }

//                    if (!outerInstance.myWin && !outerInstance.myLose)
//                    {
//                        switch (e.KeyCode)
//                        {
//                            case KeyEvent.VK_LEFT:
//                                outerInstance.left();
//                                break;
//                            case KeyEvent.VK_RIGHT:
//                                outerInstance.right();
//                                break;
//                            case KeyEvent.VK_DOWN:
//                                outerInstance.down();
//                                break;
//                            case KeyEvent.VK_UP:
//                                outerInstance.up();
//                                break;
//                        }
//                    }

//                    if (!outerInstance.myWin && !outerInstance.canMove())
//                    {
//                        outerInstance.myLose = true;
//                    }

//                    repaint();
//                }
//            }

//            public virtual void resetGame()
//            {
//                myScore = 0;
//                myWin = false;
//                myLose = false;
//                myTiles = new Tile[4 * 4];
//                for (int i = 0; i < myTiles.Length; i++)
//                {
//                    myTiles[i] = new Tile();
//                }
//                addTile();
//                addTile();
//            }

//            public virtual bool changed(Tile[] oldLine, Tile[] newLine)
//            {
//                for (int i = 0; i < 4; i++)
//                {
//                    if (oldLine[i].value != newLine[i].value)
//                    {
//                        return true;
//                    }
//                }
//                return false;
//            }
//            public virtual void left()
//            {
//                bool needAddTile = false;
//                for (int i = 0; i < 4; i++)
//                {
//                    Tile[] line = getLine(i);
//                    Tile[] merged = mergeLine(moveLine(line));

//                    setLine(i, merged);
//                    if (!needAddTile && !compare(line, merged) && changed(line, merged))
//                    {
//                        needAddTile = true;
//                    }
//                }

//                if (needAddTile)
//                {
//                    addTile();
//                }
//            }

//            public virtual void right()
//            {
//                myTiles = rotate(180);
//                left();
//                myTiles = rotate(180);
//            }

//            public virtual void up()
//            {
//                myTiles = rotate(270);
//                left();
//                myTiles = rotate(90);
//            }

//            public virtual void down()
//            {
//                myTiles = rotate(90);
//                left();
//                myTiles = rotate(270);
//            }

//            private Tile tileAt(int x, int y)
//            {
//                return myTiles[x + y * 4];
//            }

//            private void addTile()
//            {
//                IList<Tile> list = availableSpace();
//                if (availableSpace().Count > 0)
//                {
//                    int index = (int)(new Random(1).NextDouble() * list.Count) % list.Count;
//                    Tile emptyTime = list[index];
//                    emptyTime.value = new Random(2).NextDouble() < 0.9 ? 2 : 4;
//                }
//            }

//            private IList<Tile> availableSpace()
//            {
//                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                //ORIGINAL LINE: final java.util.List<Tile> list = new java.util.ArrayList<Tile>(16);
//                IList<Tile> list = new List<Tile>(16);
//                foreach (Tile t in myTiles)
//                {
//                    if (t.Empty)
//                    {
//                        list.Add(t);
//                    }
//                }
//                return list;
//            }

//            private bool Full
//            {
//                get
//                {
//                    return availableSpace().Count == 0;
//                }
//            }

//            internal virtual bool canMove()
//            {
//                if (!Full)
//                {
//                    return true;
//                }
//                for (int x = 0; x < 4; x++)
//                {
//                    for (int y = 0; y < 4; y++)
//                    {
//                        Tile t = tileAt(x, y);
//                        if ((x < 3 && t.value == tileAt(x + 1, y).value) || ((y < 3) && t.value == tileAt(x, y + 1).value))
//                        {
//                            return true;
//                        }
//                    }
//                }
//                return false;
//            }

//            private bool compare(Tile[] line1, Tile[] line2)
//            {
//                if (line1 == line2)
//                {
//                    return true;
//                }
//                else if (line1.Length != line2.Length)
//                {
//                    return false;
//                }

//                for (int i = 0; i < line1.Length; i++)
//                {
//                    if (!line1[i].Equals(line2[i]))
//                    {
//                        return false;
//                    }
//                }
//                return true;
//            }

//            private Tile[] rotate(int angle)
//            {
//                Tile[] newTiles = new Tile[4 * 4];
//                int offsetX = 3, offsetY = 3;
//                if (angle == 90)
//                {
//                    offsetY = 0;
//                }
//                else if (angle == 270)
//                {
//                    offsetX = 0;
//                }

//                double rad = Math.toRadians(angle);
//                int cos = (int)Math.Cos(rad);
//                int sin = (int)Math.Sin(rad);
//                for (int x = 0; x < 4; x++)
//                {
//                    for (int y = 0; y < 4; y++)
//                    {
//                        int newX = (x * cos) - (y * sin) + offsetX;
//                        int newY = (x * sin) + (y * cos) + offsetY;
//                        newTiles[(newX) + (newY) * 4] = tileAt(x, y);
//                    }
//                }
//                return newTiles;
//            }

//            private Tile[] moveLine(Tile[] oldLine)
//            {
//                LinkedList<Tile> l = new LinkedList<Tile>();
//                for (int i = 0; i < 4; i++)
//                {
//                    if (!oldLine[i].Empty)
//                    {
//                        l.AddLast(oldLine[i]);
//                    }
//                }
//                if (l.Count == 0)
//                {
//                    return oldLine;
//                }
//                else
//                {
//                    Tile[] newLine = new Tile[4];
//                    ensureSize(l, 4);
//                    for (int i = 0; i < 4; i++)
//                    {
//                        newLine[i] = l.RemoveFirst();
//                    }
//                    return newLine;
//                }
//            }

//            private Tile[] mergeLine(Tile[] oldLine)
//            {
//                LinkedList<Tile> list = new LinkedList<Tile>();
//                for (int i = 0; i < 4 && !oldLine[i].Empty; i++)
//                {
//                    int num = oldLine[i].value;
//                    if (i < 3 && oldLine[i].value == oldLine[i + 1].value)
//                    {
//                        num *= 2;
//                        myScore += num;
//                        int ourTarget = 2048;
//                        if (num == ourTarget)
//                        {
//                            myWin = true;
//                        }
//                        i++;
//                    }
//                    list.AddLast(new Tile(num));
//                }
//                if (list.Count == 0)
//                {
//                    return oldLine;
//                }
//                else
//                {
//                    ensureSize(list, 4);
//                    return list.toArray(new Tile[4]);
//                }
//            }

//            private static void ensureSize(IList<Tile> l, int s)
//            {
//                while (l.Count != s)
//                {
//                    l.Add(new Tile());
//                }
//            }

//            private Tile[] getLine(int index)
//            {
//                Tile[] result = new Tile[4];
//                for (int i = 0; i < 4; i++)
//                {
//                    result[i] = tileAt(i, index);
//                }
//                return result;
//            }

//            private void setLine(int index, Tile[] re)
//            {
//                Array.Copy(re, 0, myTiles, index * 4, 4);
//            }

//            public override void paint(Graphics g)
//            {
//                base.paint(g);
//                g.Color = BG_COLOR;
//                g.fillRect(0, 0, this.Size.width, this.Size.height);
//                for (int y = 0; y < 4; y++)
//                {
//                    for (int x = 0; x < 4; x++)
//                    {
//                        drawTile(g, myTiles[x + y * 4], x, y);
//                    }
//                }
//            }

//            private void drawTile(Graphics g2, Tile tile, int x, int y)
//            {
//                Graphics2D g = ((Graphics2D)g2);
//                g.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
//                g.setRenderingHint(RenderingHints.KEY_STROKE_CONTROL, RenderingHints.VALUE_STROKE_NORMALIZE);
//                int value = tile.value;
//                int xOffset = offsetCoors(x);
//                int yOffset = offsetCoors(y);
//                g.Color = tile.Background;
//                g.fillRoundRect(xOffset, yOffset, TILE_SIZE, TILE_SIZE, 14, 14);
//                g.Color = tile.Foreground;
//                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                //ORIGINAL LINE: final int size = value < 100 ? 36 : value < 1000 ? 32 : 24;
//                int size = value < 100 ? 36 : value < 1000 ? 32 : 24;
//                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                //ORIGINAL LINE: final Font font = new Font(FONT_NAME, Font.BOLD, size);
//                Font font = new Font(FONT_NAME, Font.BOLD, size);
//                g.Font = font;

//                string s = Convert.ToString(value);
//                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                //ORIGINAL LINE: final FontMetrics fm = getFontMetrics(font);
//                FontMetrics fm = getFontMetrics(font);

//                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                //ORIGINAL LINE: final int w = fm.stringWidth(s);
//                int w = fm.stringWidth(s);
//                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//                //ORIGINAL LINE: final int h = -(int) fm.getLineMetrics(s, g).getBaselineOffsets()[2];
//                int h = -(int)fm.getLineMetrics(s, g).BaselineOffsets[2];

//                if (value != 0)
//                {
//                    g.drawString(s, xOffset + (TILE_SIZE - w) / 2, yOffset + TILE_SIZE - (TILE_SIZE - h) / 2 - 2);
//                }

//                if (myWin || myLose)
//                {
//                    g.Color = new Color(255, 255, 255, 30);
//                    g.fillRect(0, 0, Width, Height);
//                    g.Color = new Color(78, 139, 202);
//                    g.Font = new Font(FONT_NAME, Font.BOLD, 48);
//                    if (myWin)
//                    {
//                        g.drawString("You won!", 68, 150);
//                    }
//                    if (myLose)
//                    {
//                        g.drawString("Game over!", 50, 130);
//                        g.drawString("You lose!", 64, 200);
//                    }
//                    if (myWin || myLose)
//                    {
//                        g.Font = new Font(FONT_NAME, Font.PLAIN, 16);
//                        g.Color = new Colors(128, 128, 128, 128);
//                        g.drawString("Press ESC to play again", 80, Height - 40);
//                    }
//                }
//                g.Font = new Font(FONT_NAME, Font.PLAIN, 18);
//                g.drawString("Score: " + myScore, 200, 365);

//            }



//            private static int offsetCoors(int arg)
//            {
//                return arg * (TILES_MARGIN + TILE_SIZE) + TILES_MARGIN;
//            }

//            internal class Tile
//            {
//                internal int value;

//                public Tile()
//                    : this(0)
//                {
//                }

//                public Tile(int num)
//                {
//                    value = num;
//                }

//                public virtual bool Empty
//                {
//                    get
//                    {
//                        return value == 0;
//                    }
//                }

//                public virtual Color Foreground
//                {
//                    get
//                    {
//                        return value < 16 ? new Color(0x776e65) : new Color(0xf9f6f2);
//                    }
//                }

//                public virtual Color Background
//                {
//                    get
//                    {
//                        switch (value)
//                        {
//                            case 2:
//                                return new Color(0xeee4da);
//                            case 4:
//                                return new Color(0xede0c8);
//                            case 8:
//                                return new Color(0xf2b179);
//                            case 16:
//                                return new Color(0xf59563);
//                            case 32:
//                                return new Color(0xf67c5f);
//                            case 64:
//                                return new Color(0xf65e3b);
//                            case 128:
//                                return new Color(0xedcf72);
//                            case 256:
//                                return new Color(0xedcc61);
//                            case 512:
//                                return new Color(0xedc850);
//                            case 1024:
//                                return new Color(0xedc53f);
//                            case 2048:
//                                return new Color(0xedc22e);
//                        }
//                        return new Color(0xcdc1b4);
//                    }
//                }
//            }

//            public static void Main(string[] args)
//            {
//                JFrame game = new JFrame();
//                game.Title = "2048 Game";
//                game.DefaultCloseOperation = WindowConstants.EXIT_ON_CLOSE;
//                game.setSize(340, 400);
//                game.Resizable = false;

//                game.add(new Game2048());

//                game.LocationRelativeTo = null;
//                game.Visible = true;
//            }
//        }
//    }
//}
