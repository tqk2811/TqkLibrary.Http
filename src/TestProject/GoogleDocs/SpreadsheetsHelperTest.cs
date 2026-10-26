using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.GoogleDocs;

namespace TestProject.GoogleDocs
{
    [TestClass]
    public class SpreadsheetsHelperTest
    {
        readonly SpreadsheetsHelper _spreadsheetsHelper = new();
        public static IEnumerable<object[]> Ids
        {
            get
            {
                yield return new object[] { "1olq8d_cPt6BlLSnql3zSVmNsMa8uJCD-9qpRzpSHR6U" };
            }
        }


        [TestMethod(), DynamicData(nameof(Ids))]
        public async Task ExportTest(string id)
        {
            using var stream = await _spreadsheetsHelper.ExportAsync(SpreadsheetsHelper.ExportFormat.xlsx, id);
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
        }
    }
}
