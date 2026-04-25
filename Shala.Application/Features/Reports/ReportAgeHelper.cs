namespace Shala.Application.Features.Reports;

public static class ReportAgeHelper
{
    public static int? GetAgeInYears(DateTime? dateOfBirth, DateTime? asOnDate = null)
    {
        if (!dateOfBirth.HasValue)
            return null;

        var dob = dateOfBirth.Value.Date;
        var today = (asOnDate ?? DateTime.Today).Date;

        if (dob > today)
            return null;

        var years = today.Year - dob.Year;

        if (dob > today.AddYears(-years))
            years--;

        return years;
    }

    public static string GetAgeText(DateTime? dateOfBirth, DateTime? asOnDate = null)
    {
        if (!dateOfBirth.HasValue)
            return "-";

        var dob = dateOfBirth.Value.Date;
        var today = (asOnDate ?? DateTime.Today).Date;

        if (dob > today)
            return "-";

        var years = today.Year - dob.Year;
        var months = today.Month - dob.Month;

        if (today.Day < dob.Day)
            months--;

        if (months < 0)
        {
            years--;
            months += 12;
        }

        if (years < 0)
            return "-";

        if (years == 0 && months == 0)
            return "0 Month";

        if (years == 0)
            return $"{months} Month{(months == 1 ? "" : "s")}";

        if (months == 0)
            return $"{years} Year{(years == 1 ? "" : "s")}";

        return $"{years} Year{(years == 1 ? "" : "s")} {months} Month{(months == 1 ? "" : "s")}";
    }
}