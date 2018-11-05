using IEE.Infrastructure.DbContext;
using System;

namespace IEE.Infrastructure
{
    public interface IDatabaseFactory 
    {
        SATEntities Get();
    }
}
