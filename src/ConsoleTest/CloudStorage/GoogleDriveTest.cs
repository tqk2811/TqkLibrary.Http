using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.CloudStorage.GoogleDrive;

namespace ConsoleTest.CloudStorage
{
    internal class GoogleDriveTest
    {
        const string folderId = "0Bx154iMNwuyWR1FhbzFUX3VuT0E";
        const string folderResourceKey = "0-8tPJbWpik3QCtggrAEc-cA";
        const string fileId = "1TNjvaoiSxoR94Vfo_F4YDPqcEC-iOTPg";
        //const string fileId = "1Yz7FGBDdH3sStSd1YGhnw6Xc3BPYmYj5";
        const string fileIdLarge = "1RWBqh_A_r5WRF9srwsBJVpcnLKHI4TAK";
        public static async Task Test()
        {
            GoogleDriveApiNonLogin driveApiNonLogin = new GoogleDriveApiNonLogin();
            int count = 0;
            while (true)
            {
                //var metadatas = await driveApiNonLogin.ListPublicFolderAsync(DriveFileListOption.CreateQueryFolder(folderId, folderResourceKey));
                //var metadata = await driveApiNonLogin.GetMetadataAsync(fileId);
                using Stream stream = await driveApiNonLogin.DownloadFileAsync(fileIdLarge);
                byte[] buffer = new byte[1024 * 1024 * 8];
                long total = 0;
                while (true)
                {
                    int byte_read = stream.Read(buffer, 0, buffer.Length);
                    total += byte_read;
                    if (byte_read == 0) break;
                    Console.WriteLine($"Byte Read: {byte_read}({total}) Bytes; Done {count}");
                }
                count++;
                Console.WriteLine($"Done {count}");
            }
        }
    }
}
