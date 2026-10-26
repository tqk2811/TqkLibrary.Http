using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Http.CloudStorage.OneDrive;

namespace TestProject.CloudStorage
{
    [TestClass]
    public class OneDriveTest
    {
        readonly OneDriveApiNonLogin _oneDriveApiNonLogin = new OneDriveApiNonLogin();
        public static IEnumerable<object[]> Folders
        {
            get
            {
                yield return new object[] { "https://1drv.ms/f/s!AqsZzkUJtHuHjF1K1mTHekJ4hBY4?e=itahEX" };
            }
        }
        public static IEnumerable<object[]> Files
        {
            get
            {
                yield return new object[] { "https://1drv.ms/v/s!ArCgyj9Sfq0AmBSRwi_1FYEP7yzj?e=0FOiX2" };
                yield return new object[] { "https://1drv.ms/v/s!ArCgyj9Sfq0AmBUBzKF9HtKYVCNq?e=yejeXi" };
                yield return new object[] { "https://1drv.ms/u/s!ArCgyj9Sfq0AmBbCwrIqg79M7wFB?e=qJFPDD" };
                yield return new object[] { "https://1drv.ms/u/s!ArCgyj9Sfq0AmBe8tB-YrkTBB_yC?e=iufm3q" };
            }
        }


        [TestMethod(), DynamicData(nameof(Folders))]
        public async Task ListChildInFolder(string folderUrl)
        {
            var urlInfo = await _oneDriveApiNonLogin.DecodeShortLinkAsync(new Uri(folderUrl));
            var items = await _oneDriveApiNonLogin.ListChildItems(urlInfo);
        }


        [TestMethod, DynamicData(nameof(Files))]
        public async Task GetMetaData(string fileUrl)
        {
            var urlInfo = await _oneDriveApiNonLogin.DecodeShortLinkAsync(new Uri(fileUrl));
            var metadata = await _oneDriveApiNonLogin.GetMetadataAsync(urlInfo);
        }

        [TestMethod, DynamicData(nameof(Files))]
        public async Task Download(string fileUrl)
        {
            var urlInfo = await _oneDriveApiNonLogin.DecodeShortLinkAsync(new Uri(fileUrl));
            using Stream stream = await _oneDriveApiNonLogin.DownloadFileAsync(urlInfo);
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
        }
    }
}
