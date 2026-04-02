namespace BucketSurvey.Api.Abstractions;

public class Result
{
    public bool IsSuccess { get; } = default;
    public bool Isfaluire => !IsSuccess;
    public Error error { get; } = default!;


    public Result(bool issucess, Error error)
    { 
        if ((issucess && error != Error.None ) || ( !issucess && error == Error.None ))
            throw new InvalidOperationException();

        IsSuccess = issucess;
        this.error = error;
    }

    public static Result Success() => new Result(true, Error.None);
    public static Result Faluire(Error error) => new Result(false, error);
    public static Result<Tvalue> Success<Tvalue>(Tvalue value) => new Result<Tvalue>(value, true, Error.None);
    public static Result<Tvalue> Faluire<Tvalue>( Error error ) => new Result<Tvalue>(default!,false, error);

}

public class Result<TValue> : Result 
{
    private readonly TValue _Value;

    public Result(TValue value , bool issucess, Error error) : base(issucess , error)
    {
        _Value = value;
    }
    
    public TValue Value => IsSuccess ? _Value : throw new InvalidOperationException("Faluire results can't have value!");

}