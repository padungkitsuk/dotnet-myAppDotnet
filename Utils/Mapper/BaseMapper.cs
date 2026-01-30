using AutoMapper;

public class BaseMapper : IBaseMapper
{
    private readonly IMapper _mapper;
    public BaseMapper(IMapper mapper) => _mapper = mapper;

    public TDestination Map<TDestination>(object source) 
        => _mapper.Map<TDestination>(source);
}