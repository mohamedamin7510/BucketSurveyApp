namespace BucketSurvey.Api.Abstractions;

public record Error
{
    public readonly string _Code;
    public readonly string _Descreption;
    public readonly int? _StatusCode;

    public Error(string Code, string Descreption, int? StatusCode) 
    {
        this._Code = Code;
        this._Descreption = Descreption;
        this._StatusCode = StatusCode;
    }
    public static Error None = new(string.Empty, string.Empty, null);

}
