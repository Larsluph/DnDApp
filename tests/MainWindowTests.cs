namespace DnDApp
{
    [TestClass]
    public class MainWindowTests
    {   
        [TestMethod]
        public void GetParent_Root()
        {
            // GIVEN
            string path = "C:\\";

            // THEN
            string result = MainWindow.GetParent(path);
            Assert.AreEqual(path, result);
        }
        [TestMethod]
        public void GetParent_Classic()
        {
            // GIVEN
            string path = "C:\\Windows\\System32";

            // THEN
            string result = MainWindow.GetParent(path);
            Assert.AreEqual("C:\\Windows", result);
        }
        [TestMethod]
        public void GetParent_TrailingBackslash()
        {
            // GIVEN
            string path = "C:\\Windows\\System32\\";

            // THEN
            string result = MainWindow.GetParent(path);
            Assert.AreEqual("C:\\Windows\\System32", result);
        }
        [TestMethod]
        public void GetParent_File()
        {
            // GIVEN
            string path = @"C:\Windows\System32\cmd.exe";

            // THEN
            string result = MainWindow.GetParent(path);
            Assert.AreEqual(@"C:\Windows\System32", result);
        }

        [TestMethod]
        [DataRow("C:\\Windows\\System32")]
        [DataRow("C:\\Windows\\System32\\")]
        public void GetDirName_Classic(string path)
        {
            // THEN
            string result = MainWindow.GetDirName(path);
            Assert.AreEqual("System32", result);
        }
        [TestMethod]
        public void GetDirName_Root()
        {
            // GIVEN
            string path = "C:\\";

            // THEN
            string result = MainWindow.GetDirName(path);
            Assert.AreEqual(path, result);
        }

        [TestMethod]
        public void GetDestination_File_WithoutSource()
        {
            // GIVEN
            string path = @"C:\test.txt";
            string? source = null;
            string target = @"C:\Documents";

            // THEN
            string result = MainWindow.GetDestination(path, target, source);
            Assert.AreEqual(@"C:\Documents\test.txt", result);
        }
        [TestMethod]
        public void GetDestination_Folder_WithoutSource()
        {
            // GIVEN
            string path = @"C:\New Folder";
            string? source = null;
            string target = @"C:\Documents";

            // THEN
            string result = MainWindow.GetDestination(path, target, source);
            Assert.AreEqual(@"C:\Documents\New Folder", result);
        }
        [TestMethod]
        public void GetDestination_File_WithSource()
        {
            // GIVEN
            string path = @"D:\Larsluph\Downloads\New Folder\ToReencode\test.txt";
            string? source = @"D:\Larsluph\Downloads";
            string target = @"D:\Larsluph\Documents";

            // THEN
            string result = MainWindow.GetDestination(path, target, source);
            Assert.AreEqual(@"D:\Larsluph\Documents\New Folder\ToReencode\test.txt", result);
        }
        [TestMethod]
        public void GetDestination_Folder_WithSource()
        {
            // GIVEN
            string path = @"D:\Larsluph\Downloads\New Folder";
            string? source = @"D:\Larsluph\Downloads";
            string target = @"D:\Larsluph\Documents";

            // THEN
            string result = MainWindow.GetDestination(path, target, source);
            Assert.AreEqual(@"D:\Larsluph\Documents\New Folder", result);
        }
    }
}
