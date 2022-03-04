
namespace AppData.Constants
{
    public static class Configuration
    {
        //This sets constants to be config items from the web.config file

        #region Database

        public const string SERVER = "Server";
        public const string DATABASE = "Database";
        public const string USER = "User";
        public const string PASSWORD = "Pass";
        public const string CONNECTION_TIMEOUT = "ConnectionTimeoutInSeconds";
        public const string INTEGRATED_SECURITY = "IntegratedSecurity";
        public const string COMMAND_TIMEOUT_IN_SECONDS = "CommandTimeoutInSeconds";

        #endregion

        #region Paging

        public const string PAGING_ROWS = "PagingRows";

        #endregion

        #region Documents

        public const string DOCUMENT_PATH = "DocPath";
        public const string ARCHIVE_DOCUMENT_PATH = "ArchiveDocPath";
        public const string EMAIL_STORE_PATH = "EmailStorePath";
        public const string STORAGE_CONNECTION = "StorageConnection";
        public const string STORAGE_FOLDER = "StorageFolder";

        #endregion

        #region Mail

        public const string SMTP_FROM = "SMTPFromAddress";
        public const string SMTP_SERVER = "SMTPServerName";
        public const string SMTP_PORT = "SMTPPort";
        public const string SMTP_USERNAME = "SMTPUserName";
        public const string SMTP_PASSWORD = "SMTPPassword";

        #endregion

        #region ErrorHandling

        public const string EVENT_SOURCE_NAME = "EventSourceName";

        #endregion

    }
}
