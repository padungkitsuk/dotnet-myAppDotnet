namespace MyBackend.Utils.Constants
{
    public static class StatusConstant
    {
        public const string SuccessMessage = "Transaction success.";
        public const string SuccessCode = "00";

        public const string ErrorMessage = "Transaction failed.";
        public const string ErrorCode = "99";

        public const string DuplicateMessage = "Data is Duplicate.";
        public const string DuplicateCode = "01";

        public const string NotFoundMessage = "Data not found.";
        public const string NotFoundCode = "02";

        public const string NotMatchMessage = "Data not match.";
        public const string NotMatchCode = "03";

        public const string NotExpiredMessage = "Data Expired.";
        public const string NotExpiredCode = "04";



        // ======================= other ========================= //
        public const string BadRequestMessage = "Bad Request.";
        public const string BadRequestCode = "400";
    }
}