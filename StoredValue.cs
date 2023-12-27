using System;
using System.IO;

namespace Extras
{

    public class StoredValue<T>
    {
        private string FileName = "";
        private readonly string DesiredFilename;
        private bool IsDirty = true;
        private static readonly Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        public enum Location
        {
            Local,
            LocalPersistent,
            Cloud
        }
        public Location location = Location.LocalPersistent;
        private T value;
        private static bool IsConfigured = false;
        private static string LocalStoragePath = "";

        public static void SetConfig(string localStoragePath, string CloudAPIKey)
        {
            if (string.IsNullOrEmpty(localStoragePath))
            {
                LocalStoragePath = AppDomain.CurrentDomain.BaseDirectory + $"\\StoredValues";
            }
            if (!Directory.Exists(localStoragePath))
            {
                try
                {
                    Directory.CreateDirectory(localStoragePath);
                    LocalStoragePath = localStoragePath;
                }
                catch (Exception e)
                {
                    IsConfigured = false;
                    throw new Exception("Could not create directory for StoredValues. " + e.Message);
                }
            }
            else
            {
                LocalStoragePath = localStoragePath;
            }
            IsConfigured = true;
        }

        public StoredValue(string name, T initialValue, Location location= Location.Local)
        {
            if (IsConfigured == false)
            {
                throw new Exception("StoredValue is not configured. Call StoredValue.SetConfig() at least once before using.");
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
                if (location == Location.LocalPersistent)
                {
                    if (IsDirty)
                    {
                        Stream ReadFileStream = File.OpenRead(GetFilename());
                        if (ReadFileStream.Length == 0)
                        {
                            IsDirty = false;
                            ReadFileStream.Close();
                            ForceSave();
                            return value;
                        }
                        StreamReader reader = new StreamReader(ReadFileStream);
                        Newtonsoft.Json.JsonTextReader jsonReader = new Newtonsoft.Json.JsonTextReader(reader);
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
                if (location == Location.LocalPersistent)
                {
                    //store value only if value is new
                    
                    if (this.value != null && !this.value.Equals(value))
                    {
                       
                        Stream SaveFileStream = File.Create(GetFilename());
                        StreamWriter writer = new StreamWriter(SaveFileStream);
                        Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(writer);
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
            if (location == Location.LocalPersistent)
            {
                if (File.Exists(GetFilename()) == false)
                {
                    var file = File.Create(GetFilename());
                    file.Close();
                }

                Stream SaveFileStream = File.Create(GetFilename());
                StreamWriter writer = new StreamWriter(SaveFileStream);
                Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(writer);
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
               
                if (!File.Exists(LocalStoragePath + $"\\{DesiredFilename}.StoredValue"))
                {
                    FileName = LocalStoragePath + $"\\{DesiredFilename}.StoredValue";
                    var file = File.Create(FileName);
                    file.Close();
                }
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