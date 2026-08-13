using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }

        public IReadOnlyCollection<ResultError> Errors { get; }

        protected Result(
       bool isSuccess,
       IReadOnlyCollection<ResultError> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success()
            => new(true, []);

        public static Result Failure(
            params ResultError[] errors)
            => new(false, errors);
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(
            T value)
            : base(true, [])
        {
            Value = value;
        }

        private Result(
            IReadOnlyCollection<ResultError> errors)
            : base(false, errors)
        {
        }

        public static Result<T> Success(T value)
            => new(value);

        public static Result<T> Failure(
            params ResultError[] errors)
            => new(errors);
    }
}