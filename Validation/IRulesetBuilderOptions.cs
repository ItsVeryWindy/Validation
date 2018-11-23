using System;

namespace Validation
{
    public interface IRulesetBuilderOptions<TParent, TChild> : IRulesetBuilder<TParent, TChild>
    {
        IRulesetBuilderOptions<TParent, TChild> Configure(Func<IServiceProvider, IChildValidator<TChild>, IChildValidator<TChild>> action);
    }
}
