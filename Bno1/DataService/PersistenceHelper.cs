using System;
using System.IO;
using Windows.Storage;



namespace transmate.DataService
{
    public class PersistenceHelper: IPersistenceWriter
    {
        private String _basePath = ApplicationData.Current.LocalFolder.Path;


        public bool HasFile(string fileName)
        {
            return File.Exists(Path.Combine(_basePath, fileName));
        }

        public void WriteFile(string fileName, string xmlContent)
        {
            File.WriteAllText(Path.Combine(_basePath, fileName), xmlContent);
        }

        public void CreateAndWriteFile(string fileName, string xmlContent)
        {
            File.WriteAllText(Path.Combine(_basePath, fileName), xmlContent);
        }

        public string ReadFile(string fileName)
        {
            return File.ReadAllText(Path.Combine(_basePath, fileName));
        }

        public string GetFullPath(string fileName)
        {
            return Path.Combine(_basePath, fileName);
        }

        public StorageFolder GetFolder()
        {
            return ApplicationData.Current.LocalFolder;
        }
    }
}