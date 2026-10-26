// See https://aka.ms/new-console-template for more information

using ConsoleTest.CloudStorage;
using ConsoleTest.Mail;
using TqkLibrary.Http;

//await GoogleDriveTest.Test();
//await TempMailOrgTest.Test();


UrlBuilder urlBuilder = new UrlBuilder("https://abc.com");
urlBuilder.WithParam("abc", "abc");
urlBuilder.WithParam("abc", "def");
urlBuilder.WithParam("abc", "ghi");
Console.WriteLine(urlBuilder.ToString());
Console.ReadLine();