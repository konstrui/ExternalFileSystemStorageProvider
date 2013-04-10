using System;
using System.IO;
using Telerik.Sitefinity.BlobStorage;
using Telerik.Sitefinity.Modules.Libraries.BlobStorage;

namespace SitefinityWebApp.Helpers {

    public class ExternalFileSystemStorageProvider : FileSystemStorageProvider, IExternalBlobStorageProvider {

        protected override void InitializeStorage(System.Collections.Specialized.NameValueCollection config) {

            base.InitializeStorage(config);

            var url = string.Concat(Telerik.Sitefinity.Services.SystemManager.RootUrl, config["rootUrl"]);
            this.rootUrl = url;

            if (this.rootUrl.IsNullOrEmpty()) {
                this.rootUrl = Telerik.Sitefinity.Services.SystemManager.RootUrl;
                if (this.StorageFolder.StartsWith(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, StringComparison.OrdinalIgnoreCase)) {
                    var pathFromRoot = this.StorageFolder.Substring(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath.Length);
                    pathFromRoot.Replace('\\', '/');
                    this.rootUrl += pathFromRoot;
                }
            }
        }

        public void Copy(IBlobContentLocation source, IBlobContentLocation destination) {
            File.Copy(this.GetFilePath(source), this.GetFilePath(destination));
        }

        public string GetItemUrl(IBlobContentLocation content) {
            return string.Concat(this.rootUrl, "/", content.FilePath);
        }

        public IBlobProperties GetProperties(IBlobContentLocation location) {
            return null;
        }

        public void Move(IBlobContentLocation source, IBlobContentLocation destination) {
            File.Move(this.GetFilePath(source), this.GetFilePath(destination));
        }

        public void SetProperties(IBlobContentLocation location, IBlobProperties properties) {
        }

        public override Stream GetUploadStream(IBlobContent content) {
            EnsureFilePath(content);
            return base.GetUploadStream(content);
        }

        protected override string GetFilePath(IBlobContentLocation location) {
            return Path.Combine(this.StorageFolder, location.FilePath);
        }

        private void EnsureFilePath(IBlobContentLocation location) {
            FileInfo file = new FileInfo(this.GetFilePath(location));
            if (!file.Directory.Exists)
                file.Directory.Create();
        }

        private string rootUrl;
    }
}
