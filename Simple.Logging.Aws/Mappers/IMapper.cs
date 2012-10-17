namespace Simple.Logging.Aws.Mappers
{
    public interface IMapper<in TIn, out TOut>
    {
        TOut Map(TIn toMap);
    }
}