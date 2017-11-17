using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public interface IMapper<TSource , TDestination>
    {
        TDestination Map(TSource entity);
    }
}
