using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Extras
{

    public class StoredValue<T>
    {
        private string FileName = null;
        private string DesiredFilename = null;
        private bool IsDirty = true;
        public enum Location
        {
            Local,
            Cloud
        }
        public Location location = Location.Local;
        private T value;

        public StoredValue(string name, T initialValue, Location location)
        {
            DesiredFilename = name;
            value = initialValue;
            IsDirty = true;
            this.location = location;
        }

        public T Value
        {
            get
            {
                if (location == Location.Local)
                {
                    if (IsDirty && File.Exists(getFilename()))
                    {
                        Stream openFileStream = File.OpenRead(getFilename());
                        BinaryFormatter deserializer = new BinaryFormatter();
                        value = (T)deserializer.Deserialize(openFileStream);
                        openFileStream.Close();
                        IsDirty = false;
                    }
                }
                else
                {
                    //location is cloud

                }

                return value;
            }
            set
            {
                if (location == Location.Local)
                {
                    //store value only if value is new
                    if (!this.value.Equals(value))
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues");
                        Stream SaveFileStream = File.Create(getFilename());
                        BinaryFormatter serializer = new BinaryFormatter();
                        serializer.Serialize(SaveFileStream, value);
                        SaveFileStream.Close();
                        this.value = value;
                    }
                }
                else
                {
                    //location is cloud

                }
            }
        }
        public void ForceSave()
        {
            if (location == Location.Local)
            {
                if (File.Exists(getFilename()) == false)
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues");
                    var file = File.Create(getFilename());
                    file.Close();
                }

                Stream SaveFileStream = File.Create(getFilename());
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(SaveFileStream, value);
                SaveFileStream.Close();
            }
            else
            {
                //location is cloud
            }
            //if file does not exist, create it

        }
        private string getFilename()
        {
            if (FileName == null)
            {
                FileName = AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues\\{DesiredFilename}.StoredValue";
            }
            return FileName;
        }
        /*private static string GetCaller(int level = 2)
        {
            var m = new StackTrace().GetFrame(level).GetMethod();            
            var className = m.DeclaringType.FullName;
            return className.Replace(".", "");
        }*/
    }
}