using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.CloudStorage.GoogleDrive;

namespace TestProject.CloudStorage
{
    [TestClass]
    public class GoogleDriveTest
    {
        readonly GoogleDriveApiNonLogin _driveApiNonLogin = new GoogleDriveApiNonLogin();

        public static IEnumerable<object[]> Folders
        {
            get
            {
                yield return new object[] { "0Bx154iMNwuyWR1FhbzFUX3VuT0E", "0-8tPJbWpik3QCtggrAEc-cA" };
                yield return new object[] { "1fVmRpni2UR2WnNrVDxnE4yaj-IJwZh9V", "" };
            }
        }
        public static IEnumerable<object[]> Files
        {
            get
            {
                //yield return new object[] { "1TNjvaoiSxoR94Vfo_F4YDPqcEC-iOTPg" };//form post
                yield return new object[] { "1Yz7FGBDdH3sStSd1YGhnw6Xc3BPYmYj5" };
                //yield return new object[] { "1RWBqh_A_r5WRF9srwsBJVpcnLKHI4TAK" };
                //yield return new object[] { "1k0tcvNvTi3G2JBKAtnnY8a7Q9y8wuhyI" };//form get
            }
        }



        [TestMethod(), DynamicData(nameof(Folders))]
        public async Task ListChildsInFolder(string folderId, string folderResourceKey)
        {
            var list = await _driveApiNonLogin.ListPublicFolderAsync(DriveFileListOption.CreateQueryFolder(folderId, folderResourceKey));
        }


        [TestMethod, DynamicData(nameof(Files))]
        public async Task GetMetaData(string fileId)
        {
            var metadata = await _driveApiNonLogin.GetMetadataAsync(fileId);
        }

        [TestMethod, DynamicData(nameof(Files))]
        public async Task Download(string fileId)
        {
            using Stream stream = await _driveApiNonLogin.DownloadFileAsync(fileId);
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
        }
    }
}
