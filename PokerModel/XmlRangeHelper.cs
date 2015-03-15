using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PokerModel
{
    /// <summary>
    /// Helper for Load and Save Range From XML File
    /// </summary>
    public static class XmlRangeHelper
    {
        /// <summary>
        /// Load Range from XML File
        /// </summary>
        /// <param name="pathToXmlFile">Path to File</param>
        /// <returns></returns>
        public static Range Load(string pathToXmlFile) {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof (Range));
                using (FileStream fs = new FileStream(pathToXmlFile, FileMode.Open)) {
                    return (Range) serializer.Deserialize(fs);
                }
            } catch (Exception ex) {
                Debug.Fail(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Save Range to XML File
        /// </summary>
        /// <param name="range">Hand Range</param>
        /// <param name="pathToXmlFile">path to XML File</param>
        public static void Save(Range range, string pathToXmlFile) {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(Range));
                using (FileStream fs = new FileStream(pathToXmlFile, FileMode.Create)) {
                    serializer.Serialize(fs, range);
                }
            } catch (Exception ex) {
                Debug.Fail(ex.Message);
            }
        }
    }
}
