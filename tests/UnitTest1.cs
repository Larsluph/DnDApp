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
    }
}
