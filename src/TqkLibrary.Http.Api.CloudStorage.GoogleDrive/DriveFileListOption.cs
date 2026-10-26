using System;
using Google.Apis.Drive.v2.Data;

namespace TqkLibrary.Http.Api.CloudStorage.GoogleDrive
{
    public class DriveFileListOption
    {
        public DriveFileListOption()
        {

        }
        public DriveFileListOption(FileList fileList)
        {
            this.NextLink = fileList.NextLink;
            this.PageToken = fileList.NextPageToken;
        }
        public string? Query { get; set; }
        public string? PageToken { get; set; }
        public bool SupportsTeamDrives { get; set; } = true;
        public bool IncludeTeamDriveItems { get; set; } = true;
        public int MaxResults { get; set; } = 1000;
        public string Fields { get; set; } = "*";
        public string OrderBy { get; set; } = "folder,title_natural asc";
        public string? NextLink { get; set; }
        public string? FolderId { get; set; }
        public string? Resourcekey { get; set; }
        public static DriveFileListOption CreateQueryFolder(string folderId, string? resourcekey = null)
        {
            if (string.IsNullOrWhiteSpace(folderId)) throw new ArgumentNullException(nameof(folderId));
            DriveFileListOption driveFileListOption = new DriveFileListOption();
            driveFileListOption.Query = $"trashed = false and '{folderId}' in parents";
            driveFileListOption.Resourcekey = resourcekey;
            driveFileListOption.FolderId = folderId;
            return driveFileListOption;
        }
    }
}