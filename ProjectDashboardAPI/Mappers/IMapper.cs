using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public interface IMapper<TContext, TSource , TDestination>
    {
        TDestination Map(TContext context, TSource entity);
    }
}
