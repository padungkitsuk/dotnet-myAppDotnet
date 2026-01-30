public interface IBaseMapper
{
    TDestination Map<TDestination>(object source);
}