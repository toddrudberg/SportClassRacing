using Microsoft.Web.WebView2.Core;
using System.Runtime.InteropServices;
using System.Xml.Serialization;


namespace SportClassAnalyzer
{
    public partial class Form1 : Form
    {

        public class cFormState
        {
            public static string sPylonFile = @"C:\LocalDev\SportClassRacing\SportClassOuterCourse.gpx";
           //public static string sRaceDataFile = @"C:\LocalDev\SportClassRacing\TestData.gpx";
            //public static string sRaceDataFile = @"C:\LocalDev\SportClassRacing\Slater Data\20241018_104841.gpx";
            //public static string sRaceDataFile = @"C:\LocalDev\SportClassRacing\Slater Data\20241018_142045.gpx";
            public static string sRaceDataFile = @"C:\LocalDev\SportClassRacing\Slater Data\20241018_142045.gpx";
        }

        //public List<pylonWpt> myPylons = new List<pylonWpt>();
        

        public cPylons myPylons = new cPylons();
        public cRaceData myRaceData = new cRaceData();

        public List<cLapCrossings> myLapCrossings = new List<cLapCrossings>();

        private bool isRaceBuilt = false;
        private TaskCompletionSource<bool> raceBuiltCompletionSource = new TaskCompletionSource<bool>();


        #region Console Output
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD size);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool absolute, ref SMALL_RECT consoleWindow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int STD_OUTPUT_HANDLE = -11;
        private static readonly IntPtr HWND_TOP = IntPtr.Zero;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOSIZE = 0x0001;

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        #endregion

        #region Constructor and Layout
        public Form1()
        {
            InitializeComponent();
            AllocConsole();
            // Set the console to be tall and narrow
            SetConsoleSize(40, 50); // Adjust width and height here
                                    // Set form size to a percentage of screen resolution
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            this.Size = new Size((int)(screenWidth * 0.8), (int)(screenHeight * 0.9)); // 75% of screen size
            // Set the form's position at the top right of the screen
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(screenWidth - this.Width, 0); // Right edge, top of screen
            buildRace();
            // Start race-building process asynchronously
            //_ = buildRaceAsync();
        }
        #endregion

        private async void frmMain_Load(object sender, EventArgs e)
        {
            // Wait until buildRace is complete before displaying the map
            //await raceBuiltCompletionSource.Task;
            await DisplayMap();
        }

        private void buildRace()
        {
            Console.WriteLine("Loading Pylon Data");
            // Load the pylons
            pylons pylons = null;
            XmlSerializer serializer = new XmlSerializer(typeof(pylons));
            using (FileStream fs = new FileStream(cFormState.sPylonFile, FileMode.Open))
            {
                pylons = (pylons)serializer.Deserialize(fs);
            }
            //write the pylons to a listbox
            myPylons.pylonWpts = pylons.wpt.ToList();
            myPylons.assignCartisianCoordinates();
            myPylons.assignSegments();


            foreach (pylonWpt wpt in pylons.wpt)
            {
                listBox1.Items.Add(wpt);
            }


            Console.WriteLine("Loading Race Data");
            // Load the race data
            gpx raceData = null;
            XmlSerializer raceSerializer = new XmlSerializer(typeof(gpx));
            using (FileStream fs = new FileStream(cFormState.sRaceDataFile, FileMode.Open))
            {
                raceData = (gpx)raceSerializer.Deserialize(fs);
            }
            // Process the race data as needed
            // For example, add race data to a listbox
            foreach (var gpxTrkTrkpt in raceData.trk.trkseg)
            {
                listBox2.Items.Add(gpxTrkTrkpt);
            }


            myRaceData.myRaceData = raceData.trk.trkseg.ToList();
            myRaceData.assignCartisianCoordinates(myPylons.homePylon());
            myRaceData.calculateSpeedsAndTruncate(100);
            myRaceData.detectLaps(myPylons, out myLapCrossings);
        }

        private async Task DisplayMap()
        {
            Console.WriteLine("Building the map");

            webView2Control.Dock = DockStyle.Fill;
            await webView2Control.EnsureCoreWebView2Async();

            string htmlPath = System.IO.Path.Combine(Application.StartupPath, "map.html");
            webView2Control.Source = new Uri(htmlPath);

            // Attach NavigationCompleted event handler only once
            if (webView2Control.CoreWebView2 != null)
            {
                webView2Control.CoreWebView2.NavigationCompleted -= OnNavigationCompleted;
                webView2Control.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
            }
        }

        // Event handler for WebView2 navigation completion
        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                Console.WriteLine("Drawing Pylons");
                PlotPylons(myPylons.pylonWpts);
                Console.WriteLine("Drawing Race Data");
                PlotRaceData(myRaceData.myRaceData, myLapCrossings);
                Console.WriteLine("Map Updated");
            }
        }

        private void SetConsoleSize(int width, int height)
        {
            IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);

            // Set the buffer size
            COORD bufferSize = new COORD((short)width, (short)height);
            SetConsoleScreenBufferSize(consoleHandle, bufferSize);

            // Set the window size
            SMALL_RECT windowSize = new SMALL_RECT();
            windowSize.Left = 0;
            windowSize.Top = 0;
            windowSize.Right = (short)(width - 1);
            windowSize.Bottom = (short)(height - 1);
            SetConsoleWindowInfo(consoleHandle, true, ref windowSize);
            // Find the console window handle
            IntPtr consoleWindowHandle = FindWindow(null, Console.Title);
            // Position the console in the upper-left corner
            SetWindowPos(consoleWindowHandle, HWND_TOP, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
        }

        public async void PlotPylons(List<pylonWpt> pylons)
        {
            foreach (var pylon in pylons)
            {
                // Convert latitude and longitude from decimal to double
                double lat = (double)pylon.lat;
                double lon = (double)pylon.lon;
                string name = pylon.name;

                // Inject JavaScript to add marker to the map
                string script = $"addPylonMarker({lat}, {lon}, '{name}');";
                await webView2Control.ExecuteScriptAsync(script);
            }
        }

        // Define different colors for each lap
        //string[] lapColors = { "red", "blue", "green", "purple", "orange" };

        //int lapIndex = 0; // To track lap number

        //public async void PlotRaceData(List<racePoint> myRaceData)
        //{
        //    foreach (var dataPoint in myRaceData)
        //    {
        //        // Convert latitude and longitude from decimal to double
        //        double lat = (double)dataPoint.lat;
        //        double lon = (double)dataPoint.lon;

        //        // Inject JavaScript to add marker to the map without time
        //        string script = $"addRaceDataPoint({lat}, {lon});";
        //        await webView2Control.ExecuteScriptAsync(script);
        //    }
        //}
        // Define different colors for each lap
        string[] lapColors = { "red", "blue", "green", "purple", "orange" };

        int lapIndex = 0; // To track lap number

        public async void PlotRaceData(List<racePoint> myRaceData, List<cLapCrossings> lapCrossings)
        {

            lapIndex = 1;
            
            // Use a different color for each lap
            string lapColor = lapColors[lapIndex % lapColors.Length]; // Cycle through colors if needed

            // Call JavaScript to start a new lap (reset points for new polyline)
            await webView2Control.ExecuteScriptAsync("startNewLap();");


            foreach (var dataPoint in myRaceData)
            {
                // Convert latitude and longitude from decimal to double
                double lat = (double)dataPoint.lat;
                double lon = (double)dataPoint.lon;

                // Inject JavaScript to add the marker to the map with lap-specific color
                string script = $"addRaceDataPoint({lat}, {lon}, '{lapColor}');";
                await webView2Control.ExecuteScriptAsync(script);
            }
        }



    }
}
