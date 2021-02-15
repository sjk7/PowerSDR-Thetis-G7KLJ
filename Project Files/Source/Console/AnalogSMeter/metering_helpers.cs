using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace Thetis {
namespace metering {

    public static class serial {
        public static T XmlDeserialize<T>(this string toDeserialize) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(toDeserialize)) {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string XmlSerialize<T>(this T toSerialize) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static byte[] ImageToString(Bitmap bmp) {
            if (bmp == null)
                throw new ArgumentNullException("ImageToString (bmp)");
            MemoryStream ms = new MemoryStream();
            Image img = (Image)bmp;
            img.Save(ms, ImageFormat.Jpeg);
            byte[] array = ms.ToArray();
            return array;
            // return Convert.ToBase64String(array);
        }

        public static Image StringToImage(string imageString) {
            if (imageString == null)
                throw new ArgumentNullException("imageString");
            byte[] array = Convert.FromBase64String(imageString);
            Image image = Image.FromStream(new MemoryStream(array));
            return image;
        }
    }
    [Serializable]
    public enum MeterTypes { SMeter, AudioMeter, SWRMeter, CompressionMeter }
    [Serializable]
    public enum MeterScaleKinds { log_scale, lin_scale }
    [Serializable]
    public enum BuiltInMeters { None, Original, Blue, Tango, PPM, Custom }

    [Serializable]
    public class ImageStruct {
        public MeterScaleKinds scaleKind;
        public MeterTypes meterType;
        public string fileName;
        public BuiltInMeters resourceid;
    }

    public class ImageStructEx {
        public ImageStructEx(ImageStruct d) { data = d; }
        public ImageStructEx() { this.data = new ImageStruct(); }
        public ImageStruct data = new ImageStruct();
        public Bitmap bitmap;
        public metering.BuiltInMeters resourceId {
            get { return data.resourceid; }
            set {
                data.resourceid = value;
                if (value == metering.BuiltInMeters.Original)
                    bitmap = Properties.Resources.NewVFOAnalogSignalGauge;
                else if (value == metering.BuiltInMeters.Blue)
                    bitmap = Properties.Resources.OLDAnalogSignalGauge;
                else if (value == metering.BuiltInMeters.Tango)
                    bitmap = Properties.Resources.SMeterTango;
                else if (value == metering.BuiltInMeters.None)
                    bitmap = Properties.Resources.NewVFOAnalogSignalGauge;
            }
        }
    }

    public class helpers {
        private Thetis.Console m_console;
        public helpers(Console c) { m_console = c; }
        // makes a "GDI friendly" bitmap for efficient display
        public static Bitmap makeBitmap(Image value) {
            Bitmap bmp = new Bitmap(value.Width, value.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            using (Graphics gr = Graphics.FromImage(bmp)) {
                gr.DrawImage(
                    value, new Rectangle(0, 0, value.Width, value.Height));
            }
            return bmp;
        }

        public string DataFolder {
            get {
                return m_console.CurrentSkinsFolder() + "\\" + "analogmeters";
            }
        }

        public string ImageFilePath(
            MeterTypes mt, BuiltInMeters bim = BuiltInMeters.Original) {
            int imt = (int)mt;
            int ibim = (int)bim;
            string mypath
                = DataFolder + "\\" + imt.ToString() + ibim.ToString() + ".jpg";
            return mypath;
        }

        public string ImageDataPath(
            MeterTypes mt, BuiltInMeters bim = BuiltInMeters.Original) {
            int imt = (int)mt;
            int ibim = (int)bim;
            string mypath
                = DataFolder + "\\" + imt.ToString() + ibim.ToString() + ".bin";
            return mypath;
        }
        public string ImageDataPath(string id) {
            return DataFolder + "\\" + id + ".bin";
        }

        private static ImageStructEx defaultImageStruct() {
            var ret = new ImageStructEx();
            ret.data.meterType = MeterTypes.SMeter;
            ret.data.scaleKind = MeterScaleKinds.log_scale;
            ret.data.resourceid = BuiltInMeters.Original;
            ret.bitmap = makeBitmap(
                Thetis.Properties.Resources.NewVFOAnalogSignalGauge);
            return ret;
        }
        public ImageStructEx ImageStructExDeSerialize(
            MeterTypes mt, BuiltInMeters bim = BuiltInMeters.Original) {
            ImageStructEx ret = new ImageStructEx();
            string folder = DataFolder;
            if (!System.IO.Directory.Exists(folder))
                return defaultImageStruct();

            string imgpath = ImageFilePath(mt, bim);
            if (!System.IO.File.Exists(imgpath)) return defaultImageStruct();

            byte[] imgarray = File.ReadAllBytes(imgpath);
            byte[] dataarray = File.ReadAllBytes(ImageDataPath(mt, bim));

            using (var ms = new MemoryStream(imgarray)) {
                ret.bitmap = new Bitmap(ms);
            }

            string structpath = ImageDataPath(mt, bim);
            if (!File.Exists(structpath)) return ret;
            using (var file = File.OpenRead(structpath)) {
                var reader = new BinaryFormatter();

                ret.data = (ImageStruct)reader.Deserialize(file);
            }

            return ret;
        }

        public void ImageStructExSerialize(ImageStructEx ise) {
            string folder = DataFolder;
            if (!System.IO.Directory.Exists(folder)) {
                System.IO.Directory.CreateDirectory(folder);
            }

            string path = ImageFilePath(ise.data.meterType, ise.resourceId);
            var array = serial.ImageToString(ise.bitmap);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            using (BinaryWriter binWriter
                = new BinaryWriter(File.Open(path, FileMode.Create))) {
                binWriter.Write(array);
            }

            string structpath
                = ImageDataPath(ise.data.meterType, ise.data.resourceid);
            using (var file = File.OpenWrite(structpath)) {
                var writer = new BinaryFormatter();
                writer.Serialize(file, ise.data);
            }
        }
    }

} // metering

} // Thetis
