using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportClassAnalyzer
{



    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class gpx
    {

        private gpxTrk trkField;

        private decimal versionField;

        /// <remarks/>
        public gpxTrk trk
        {
            get
            {
                return this.trkField;
            }
            set
            {
                this.trkField = value;
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class gpxTrk
    {

        private string nameField;

        private gpxTrkTrkpt[] trksegField;

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
        [System.Xml.Serialization.XmlArrayItemAttribute("trkpt", IsNullable = false)]
        public gpxTrkTrkpt[] trkseg
        {
            get
            {
                return this.trksegField;
            }
            set
            {
                this.trksegField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class gpxTrkTrkpt
    {

        private decimal eleField;

        private System.DateTime timeField;

        private decimal latField;

        private decimal lonField;

        /// <remarks/>
        public decimal ele
        {
            get
            {
                return this.eleField;
            }
            set
            {
                this.eleField = value;
            }
        }

        /// <remarks/>
        public System.DateTime time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
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
            
                // Calculate total seconds to the nearest 0.1s
                double totalSeconds = time.TimeOfDay.TotalSeconds;
                string formattedSeconds = totalSeconds.ToString("F1"); // Format to 1 decimal place
                // I need the time converted to YY:MM:DD
                string formattedDate = time.ToString("yy:MM:dd");
                double elevationInFeet = (double)ele * 3.28084;
                string formattedElevation = elevationInFeet.ToString("F0");

        
                string output = string.Format("date: {4} time: {0} lat: {1}, lon: {2}, ele: {3}", formattedSeconds, lat, lon, formattedElevation, formattedDate);
                return output;
            
        }
    }


}
