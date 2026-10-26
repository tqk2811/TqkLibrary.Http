using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.GoogleDocs;

namespace TestProject.GoogleDocs
{
    [TestClass]
    public class DocumentHelperTest
    {
        readonly DocumentHelper _documentHelper = new();
        public static IEnumerable<object[]> Ids
        {
            get
            {
                yield return new object[] { "1qe0kBEStRg1vo4-72DQ6Mhk6H4Od3H4Q6sJdi-KSaqE" };
            }
        }


        [TestMethod(), DynamicData(nameof(Ids))]
        public async Task ExportTest(string id)
        {
            using var stream = await _documentHelper.ExportAsync(DocumentHelper.ExportFormat.ziphtml, id);
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
        }
    }
}
