namespace Shala.Web.Helpers;

public static class StudentMapper
{
    public static int MapGenderToInt(string? gender)
    {
        return gender?.ToLower() switch
        {
            "male" => 1,
            "female" => 2,
            "other" => 3,
            _ => 1
        };
    }

    public static int MapStatusToInt(string? status)
    {
        return status?.ToLower() switch
        {
            "active" => 1,
            "inactive" => 2,
            "left" => 3,
            "alumni" => 4,
            "suspended" => 5,
            _ => 1
        };
    }
}