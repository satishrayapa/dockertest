using System;
using System.Collections.Generic;
using System.Text;

namespace TR.OGT.ChangeLedger.Common
{
	public readonly struct Result
	{
		public Exception Error { get; }

		public bool IsFailed { get => Error != null; }
		public bool IsOk => !IsFailed;

		public Result(Exception error)
		{
			Error = error;
		}

		public static implicit operator Result(Exception error) => new Result(error);
	}

	public struct Result<T>
	{
		public T Value { get; }
		public Exception Error { get; }

		public bool IsFailed { get => Error != null; }
		public bool IsOk => !IsFailed;

		public Result(T value)
		{
			Error = default;
			Value = value;
		}

		public Result(Exception error)
		{
			Error = error;
			Value = default;
		}

		public static implicit operator Result<T>(Exception error) => new Result<T>(error);
		public static implicit operator Result<T>(T ok) => new Result<T>(ok);
	}
}
