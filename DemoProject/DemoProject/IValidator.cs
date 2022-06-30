using System;

namespace DemoProject
{
    public interface IValidator<T>
    {
        bool IsValid(T value);
    }
}
