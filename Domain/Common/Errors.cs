using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public sealed record Error(string Code, string Message);

    public sealed class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        private Result(bool success, Error? error) { IsSuccess = success; Error = error; }
        public static Result Ok() => new(true, null);
        public static Result Fail(string code, string message) => new(false, new Error(code, message));
    }

    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error? Error { get; }

        private Result(bool success, T? value, Error? error)
        { IsSuccess = success; Value = value; Error = error; }

        public static Result<T> Ok(T value) => new(true, value, null);
        public static Result<T> Fail(string code, string message) => new(false, default, new Error(code, message));
    }
}
