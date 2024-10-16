using System.Xml.Serialization;


namespace SportClassAnalyzer
{
    public partial class Form1 : Form
    {

        public class cFormState
        {
            public static string sPylonFile = @"C:\LocalDev\SportClassRacing\SportClassOuterCourse.gpx";
            public static string sRaceDataFile = @"C:\LocalDev\SportClassRacing\TestData.gpx";
        }

        public List<pylonsWpt> myPylons = new List<pylonsWpt>();
        public List<gpxTrkTrkpt> myRaceData = new List<gpxTrkTrkpt>();

        public Form1()
        {
            InitializeComponent();
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {

            // Set form size to a percentage of screen resolution
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            this.Size = new Size((int)(screenWidth * 0.9), (int)(screenHeight * 0.9)); // 75% of screen size
            this.StartPosition = FormStartPosition.CenterScreen;
            webView2Control.Dock = DockStyle.Fill;

            // Load the pylons
            pylons pylons = null;
            XmlSerializer serializer = new XmlSerializer(typeof(pylons));
            using (FileStream fs = new FileStream(cFormState.sPylonFile, FileMode.Open))
            {
                pylons = (pylons)serializer.Deserialize(fs);
            }
            //write the pylons to a listbox
            myPylons = pylons.wpt.ToList();
            
            foreach (pylonsWpt wpt in pylons.wpt)
            {
                listBox1.Items.Add(wpt);
            }

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


            myRaceData = raceData.trk.trkseg.ToList();


            // Initialize WebView2 asynchronously
            await webView2Control.EnsureCoreWebView2Async();


            string htmlPath = System.IO.Path.Combine(Application.StartupPath, "map.html");

            webView2Control.Source = new Uri(htmlPath);

            // Call PlotPylons once the HTML file has loaded
            webView2Control.CoreWebView2.NavigationCompleted += (s, args) =>
            {
                if (args.IsSuccess)
                {
                    PlotPylons(myPylons);
                    PlotRaceData(myRaceData);
                }
            };

        }

        public async void PlotPylons(List<pylonsWpt> pylons)
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

        public async void PlotRaceData(List<gpxTrkTrkpt> myRaceData)
        {
            foreach (var dataPoint in myRaceData)
            {
                // Convert latitude and longitude from decimal to double
                double lat = (double)dataPoint.lat;
                double lon = (double)dataPoint.lon;

                // Inject JavaScript to add marker to the map without time
                string script = $"addRaceDataPoint({lat}, {lon});";
                await webView2Control.ExecuteScriptAsync(script);
            }
        }

    }


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class pylons
    {

        private pylonsWpt[] wptField;

        private decimal versionField;

        private string creatorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("wpt")]
        public pylonsWpt[] wpt
        {
            get
            {
                return this.wptField;
            }
            set
            {
                this.wptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string creator
        {
            get
            {
                return this.creatorField;
            }
            set
            {
                this.creatorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class pylonsWpt
    {

        private string nameField;

        private decimal latField;

        private decimal lonField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lat
        {
            get
            {
                return this.latField;
            }
            set
            {
                this.latField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal lon
        {
            get
            {
                return this.lonField;
            }
            set
            {
                this.lonField = value;
            }
        }

        public override string ToString()
        {
            string output = string.Format("{0} ({1}, {2})", name, lat, lon);
            return output;
        }
    }
}
