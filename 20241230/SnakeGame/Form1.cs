
using System.Diagnostics;
using System.Windows.Forms;

namespace SnakeGame
{

    public partial class Form1 : Form
    {
        const int LVUP_POINT = 3;
        const int TICK_REDUCE = 40;
        
        
        const int FEED_TICK = 10;
        const int TIMER_INTERVAL = 400;
        const int COUNTDOWN_SIZE = 200;

        
        public const int PIXEL_CNT = LINE_PIXEL_CNT * LINE_PIXEL_CNT;
        public const int LINE_PIXEL_CNT = 20;
        public const int PIXEL_SIZE = 30;
        public const int CENTER_POINT = PIXEL_SIZE * (LINE_PIXEL_CNT / 2);

        Point[] NotUsePoint = new Point[4]{
            new Point(CENTER_POINT - PIXEL_SIZE, CENTER_POINT),
            new Point(CENTER_POINT + PIXEL_SIZE, CENTER_POINT),
            new Point(CENTER_POINT, CENTER_POINT - PIXEL_SIZE),
            new Point(CENTER_POINT, CENTER_POINT + PIXEL_SIZE)
        };

        bool keydownFlag = false;

        Random rand = new Random();

        bool chckStartFlag = false;

        Snake snake;
        Obstacle obstacle;
        Feed feed;
        
        Label lbl_countdown = new Label();
        int Jumsu = 0;
        int Length = 0;
        int LV = 1;
        int feed_tick = FEED_TICK;

        bool test = true;
        Dictionary<Point, int> useLocationDict = new Dictionary<Point, int>();

        Point CenterPoint = new Point(CENTER_POINT, CENTER_POINT);
        Size pixel = new Size(PIXEL_SIZE, PIXEL_SIZE);
        Rectangle gamepan = new();

        int startCountdown = 3;
        public Form1()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            startCountdown = 3;
            Jumsu = 0;
            LV = 1;
            Length = 0;
            timer_Game.Interval = TIMER_INTERVAL;
            setCountdown();
        }

        private void Restart()
        {
            useLocationDict.Clear();
            snake.BodyClear();
            feed.FeedListClear();
            removeObstacle();

            Initialize();
            snake.setHeadLocation(CenterPoint);
            snake.setHeadPosition();
            setObstacle();
            StatusTextUpdate();
            CountDown();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            // ���� 16 ���� 39  (this.Size - this.ClientSize)
            Size size = this.Size - this.ClientSize + pixel * LINE_PIXEL_CNT;
            
            this.Width = size.Width;
            this.Height = size.Height + strip_Status.Height + getMenuHeight();
            
            gamepan.Offset(0, getMenuHeight());
            gamepan.Size = pixel * LINE_PIXEL_CNT;
        }

        public int getMenuHeight()
        {
            return �޴�MToolStripMenuItem.Height;
        }
        void GameStart()
        {
            Initialize();
            chckStartFlag = true;
            obstacle = new Obstacle(Controls, getMenuHeight());
            snake = new Snake(Controls, getMenuHeight());
            feed = new Feed(Controls, getMenuHeight());

            setObstacle();
            StatusTextUpdate();
            feed.FeedListClear();
            snake.setHeadLocation(CenterPoint);
            snake.setHeadPosition();

            CountDown();
        }
        void CountDown()
        {
            Controls.Add(lbl_countdown);
            timer_Countdown.Start();
        }
        void StatusTextUpdate()
        {
            toolStripStatusLabel_LV.Text = "LV : " + LV.ToString();
            toolStripStatusLabel_Jumsu.Text = "���� : " + Jumsu.ToString();
            toolStripStatusLabel_Bodycnt.Text = "������� : " + Length.ToString();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keydownFlag) return true;
            if (keyData == Keys.Left)
            {
                snake.Direction(SnakeVector.LEFT);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Right)
            {
                snake.Direction(SnakeVector.RIGHT);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Up)
            {
                snake.Direction(SnakeVector.UP);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Down)
            {
                snake.Direction(SnakeVector.DOWN);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Space)
            {
                test = !test;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        int tick = 0;
        private void timer_Game_Tick(object sender, EventArgs e)
        {
            keydownFlag = false;
            if (test) snake.Go();
            tick++;

            int resultGameover = isGameOver();
            if (resultGameover != 0)
            {
                timer_Game.Stop();
                string msg = "";
                if (resultGameover == 1) msg = "����� ������ ���������ϴ�.";
                else if (resultGameover == 2) msg = "�����θ� ������Ƚ��ϴ�.";
                else if (resultGameover == 3) msg = "��ֹ��� �Ӹ��� �ε��� �׾����ϴ�.";

                msg += "\n�ٽ� �����Ͻðڽ��ϱ�?";
                if (MessageBox.Show(msg, "�˸�", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Restart();
                }
                else
                {

                }
            }
            if (isSnakeFeeding())
            {
                snake.Growth();
                Length++;
                Jumsu++;

                if (isLevelUP())

                    LevelUP();

            }


            if (tick % feed_tick == 0 && snake.getSnakeLength() + useLocationDict.Count() < PIXEL_CNT)
            {
                Label lbl = feed.CreateFeed(RandomFeedLocation());
                useLocationDict.Add(new Point(lbl.Left - Feed.FEED_BLANK, lbl.Top - Feed.FEED_BLANK), 0);
                feed.FeedAdd(lbl);
            }
            StatusTextUpdate();
        }

        void removeLocation(Label label)
        {
            Point pt = label.Location;
            if (label.Name.Equals("feed"))
            {
                pt.X -= Feed.FEED_BLANK;
                pt.Y -= Feed.FEED_BLANK;
            }
            useLocationDict.Remove(pt);
        }
        void removeFeed(Label lbl)
        {
            removeLocation(lbl);
            feed.removeFeed(lbl);
        }

        bool isSnakeFeeding()
        {
            foreach (var feed in feed.getFeedList())
            {
                if (snake.SnakeHeadRect().Contains(feed.Location))
                {
                    removeFeed(feed);
                    return true;
                }
            }
            return false;
        }
        int isGameOver()
        {
            Label lbl = snake.GetSnakeHead();
            if (lbl.Top < gamepan.Top || lbl.Left < gamepan.Left ||
                lbl.Bottom > gamepan.Bottom || lbl.Right > gamepan.Right)
                return 1;
            else if (isSnakeBiteSelf())
                return 2;
            else if (isCrashObstacle())
                return 3;
            else
                return 0;
        }

        bool isCrashObstacle()
        {
            return obstacle.LocationContains(snake.getHeadLocation());
        }
        bool isSnakeBiteSelf()
        {
            foreach (var body in snake.GetSnakeBody())
            {
                if (snake.getHeadLocation() == body.Location)
                    return true;
            }
            return false;
        }

        
        int RandFeedPoint(int n)
        {
            return rand.Next(n) * PIXEL_SIZE + Feed.FEED_BLANK;
        }
        Point RandomFeedLocation()
        {
            Point pt = new();
            bool flag = false;

            while (!flag)
            {
                pt = new Point(
                    RandFeedPoint(LINE_PIXEL_CNT),
                    RandFeedPoint(LINE_PIXEL_CNT) + getMenuHeight());
                if (snake.SnakeHeadRect().Contains(pt)) continue;

                if (useLocationDict.ContainsKey(new Point(pt.X - Feed.FEED_BLANK, pt.Y - Feed.FEED_BLANK))) continue;

                flag = true;
                foreach (var item in snake.GetSnakeBody())
                {
                    Rectangle rect = new Rectangle(item.Location, item.Size);
                    if (rect.Contains(pt))
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return pt;
        }

        void LevelUP()
        {
            timer_Game.Stop();
            snake.BodyClear();

            feed.FeedListClear();
            removeObstacle();
            useLocationDict.Clear();

            LV++;
            Jumsu = 0;
            Length = snake.BodyCount();
            timer_Game.Interval = TIMER_INTERVAL - LV * TICK_REDUCE;
            feed_tick = FEED_TICK + LV;

            if (MessageBox.Show("������", "������", MessageBoxButtons.OK) == DialogResult.OK)
            {

                setObstacle();
                snake.setHeadLocation(CenterPoint);
                snake.setHeadPosition();
                CountDown();
            }
        }

        void ClearuseLocationDict()
        {
            useLocationDict.Clear();
        }
        void removeObstacle()
        {
            obstacle.Clear();
        }

        void setObstacle()
        {
            int cnt = LV + rand.Next(LV);

            while (cnt > 0)
            {
                Label lbl = obstacle.creteObstacle();
                Point pt = new Point();
                while (true)
                {
                    pt.X = rand.Next(LINE_PIXEL_CNT) * PIXEL_SIZE;
                    pt.Y = rand.Next(LINE_PIXEL_CNT) * PIXEL_SIZE;
                    if (!useLocationDict.ContainsKey(pt) && !NotUsePoint.Contains(pt)) break;
                }
                lbl.Location = pt;
                useLocationDict.Add(pt, 0);
                obstacle.Add(lbl);
                cnt--;
            }
        }

        bool isLevelUP()
        {
            return Jumsu >= LVUP_POINT + LV * 2;
        }

        void setCountdown()
        {
            Font font = new Font(lbl_countdown.Font.Name, 30);
            lbl_countdown.Font = font;
            lbl_countdown.Name = "obstacle";
            lbl_countdown.Size = new Size(COUNTDOWN_SIZE, COUNTDOWN_SIZE);
            lbl_countdown.Location = new Point(CENTER_POINT - 100, CENTER_POINT - COUNTDOWN_SIZE);
            lbl_countdown.TabIndex = 0;
            lbl_countdown.Text = startCountdown.ToString();
            lbl_countdown.TextAlign = ContentAlignment.MiddleCenter;
        }
        private void timer_Countdown_Tick(object sender, EventArgs e)
        {
            if (startCountdown > 0)
                lbl_countdown.Text = startCountdown.ToString();
            else if (startCountdown == 0)
                lbl_countdown.Text = "START";
            else
            {
                startCountdown = 3;
                Controls.Remove(lbl_countdown);
                lbl_countdown.Text = startCountdown.ToString();
                timer_Countdown.Stop();
                timer_Game.Start();
                return;
            }
            startCountdown--;
        }

        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("������ ���� �Ͻðڽ��ϱ�? (Yes or No)", "����", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                return;
            }
        }



        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameStart();
        }

        private void �ٽý���toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!chckStartFlag) return;

            Restart();
        }

        private void ����������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rule rule = new Rule();

            rule.ShowDialog();
        }

        private void ���۹�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manual manual = new Manual();

            manual.ShowDialog();
        }
    }
}
