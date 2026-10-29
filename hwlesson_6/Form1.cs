namespace hwlesson_6
{
    public partial class Form1 : Form
    {
        private Thread[] philosopherThreads = new Thread[5];
        private Mutex[] forks = new Mutex[5];
        private bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isRunning) return;
            isRunning = true;
            listBoxStatus.Items.Clear();

            for (int i = 0; i < 5; i++)
            {
                forks[i] = new Mutex();
            }

            for (int i = 0; i < 5; i++)
            {
                int philosopherId = i;
                philosopherThreads[i] = new Thread(() => PhilosopherLife(philosopherId));
                philosopherThreads[i].IsBackground = true;
                philosopherThreads[i].Start();
            }

            listBoxStatus.Items.Add("The simulation has begun. The philosophers sat down at the table.");
        }

        private void PhilosopherLife(int id)
        {
            int leftFork = id;
            int rightFork = (id + 1) % 5;

            while (isRunning)
            {
                // 1
                UpdateStatus(id, "Meditating..");
                Thread.Sleep(new Random().Next(1000, 2500));

                UpdateStatus(id, "Getting hungry. Taking the left fork..");

                // 2
                forks[leftFork].WaitOne();
                UpdateStatus(id, "Took the left fork. Picks up the right fork..");

                // 3
                forks[rightFork].WaitOne();

                // 4
                UpdateStatus(id, "Took both forks and is eating pasta!");
                Thread.Sleep(new Random().Next(1000, 2000));

                // 5
                forks[rightFork].ReleaseMutex();
                forks[leftFork].ReleaseMutex();

                UpdateStatus(id, "Ate, put down the forks. Returns to meditation.");
            }
        }

        private void UpdateStatus(int philosopherId, string status)
        {
            if (listBoxStatus.InvokeRequired)
            {
                listBoxStatus.Invoke(new Action(() => UpdateStatus(philosopherId, status)));
            }
            else
            {
                string message = $"Philosopher P{philosopherId + 1}: {status}";

                if (listBoxStatus.Items.Count > 120)
                    listBoxStatus.Items.RemoveAt(0);

                listBoxStatus.Items.Add(message);
                listBoxStatus.TopIndex = listBoxStatus.Items.Count - 1;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isRunning = false;
            listBoxStatus.Items.Add("The simulation has been stopped.");
        }
    }
}