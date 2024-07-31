using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public abstract class BaseService : IBaseService
{
    #region Properties

    public IMapper Mapper { get; init; }
    public CoreContext Context { get; init; }

    #endregion

    #region Constructor

    public BaseService(IMapper mapper, CoreContext context)
    {
        Mapper = mapper;
        Context = context;
    }

    #endregion

    #region Method

    public virtual BaseResult<Inner> GetBaseResult<Inner>(CodeMessage codeMessage, Inner? data = default, string message = "") =>
        new()
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = message
        };

    #endregion
}