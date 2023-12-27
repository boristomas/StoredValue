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
        private string? FileName = null;
        private readonly string DesiredFilename;
        private bool IsDirty = true;
        private static readonly Newtonsoft.Json.JsonSerializer serializer = new();
        public enum Location
        {
            Local,
            Cloud
        }
        public Location location = Location.Local;
        private T value;

        public StoredValue(string name, T initialValue, Location location)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues");
            }

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
                    if (IsDirty)
                    {
                        Stream ReadFileStream = File.OpenRead(GetFilename());
                        StreamReader reader = new(ReadFileStream);

                        Newtonsoft.Json.JsonTextReader jsonReader = new(reader);
                        value = serializer.Deserialize<T>(jsonReader);
                        jsonReader.Close();
                        reader.Close();
                        ReadFileStream.Close();
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
                    
                    if (this.value != null && !this.value.Equals(value))
                    {
                       
                        Stream SaveFileStream = File.Create(GetFilename());
                        StreamWriter writer = new StreamWriter(SaveFileStream);
                        Newtonsoft.Json.JsonTextWriter jsonWriter = new(writer);
                        serializer.Serialize(jsonWriter, value);
                        jsonWriter.Close();
                        writer.Close();
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
                if (File.Exists(GetFilename()) == false)
                {
                   
                    var file = File.Create(GetFilename());
                    file.Close();
                }

                Stream SaveFileStream = File.Create(GetFilename());
                StreamWriter writer = new StreamWriter(SaveFileStream);
                Newtonsoft.Json.JsonTextWriter jsonWriter = new(writer);
                serializer.Serialize(jsonWriter, value);
                jsonWriter.Close();
                writer.Close();
                SaveFileStream.Close();
            }
            else
            {
                //location is cloud
            }
            //if file does not exist, create it

        }
        private string GetFilename()
        {
            if (string.IsNullOrEmpty(FileName))
            {
               
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues\\{DesiredFilename}.StoredValue") == false)
                {
                    var file = File.Create(AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues\\{DesiredFilename}.StoredValue");
                    file.Close();
                }

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