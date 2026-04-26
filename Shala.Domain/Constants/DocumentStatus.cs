namespace Shala.Domain.Constants
{
    public static class StudentDocumentStatuses
    {
        public const string Uploaded = "Uploaded";
        public const string Analyzed = "Analyzed";
        public const string Verified = "Verified";
        public const string NeedsReview = "NeedsReview";
        public const string Inactive = "Inactive";
    }

    public static class StudentDocumentAnalysisStatuses
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string NeedsReview = "NeedsReview";
    }

    public static class StudentDocumentFieldMatchStatuses
    {
        public const string Pending = "Pending";
        public const string Matched = "Matched";
        public const string PartialMatch = "PartialMatch";
        public const string Mismatch = "Mismatch";
        public const string MissingInForm = "MissingInForm";
        public const string MissingInDocument = "MissingInDocument";
    }
}