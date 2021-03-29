using System;
using System.Threading.Tasks;

namespace TR.OGT.ChangeLedger.Common
{
	public static class ResultExtensions
	{
		public async static Task<Result> TryCatch(Func<Task> func)
		{
			try
			{
				await func().ConfigureAwait(false);
				return default;
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		public async static Task<Result<T>> TryCatch<T>(Func<Task<T>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		public async static Task<Result> TryCatch(Func<Task<Result>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		public async static Task<Result<T>> TryCatch<T>(Func<Task<Result<T>>> func)
		{
			try
			{
				return await func();
			}
			catch (Exception ex)
			{
				return ex;
			}
		}

		public static async Task<Result<R>> Bind<T, R>(this Task<Result<T>> resultTask, Func<T, Task<Result<R>>> func)
		{
			var result = await resultTask.ConfigureAwait(false);

			if (result.IsFailed)
				return result.Error;

			return await func(result.Value).ConfigureAwait(false);
		}

		public async static Task<Result> Next(this Task<Result> task, Func<Task> next)
		{
			var result = await task;
			if (result.IsFailed)
				return result.Error;

			return await TryCatch(next);
		}

		public async static Task<Result> Next<T>(this Task<Result<T>> task, Func<Task> next)
		{
			var result = await task;
			if (result.IsFailed)
				return result.Error;

			return await TryCatch(next);
		}

		public async static Task<Result> Next<T>(this Task<Result<T>> task, Func<T, Task> next)
		{
			var result = await task.ConfigureAwait(false);
			if (result.IsFailed)
				return result.Error;

			return await TryCatch(async () => await next(result.Value)).ConfigureAwait(false);
		}

		public async static Task<Result<T>> Next<T>(this Task<Result> task, Func<Task<T>> next)
		{
			var result = await task;
			if (result.IsFailed)
				return result.Error;

			return await TryCatch(next);
		}

		public async static Task<Result<T>> Next<T>(this Task<Result> task, Func<Task<Result<T>>> next)
		{
			var result = await task;
			if (result.IsFailed)
				return result.Error;

			return await TryCatch(next);
		}

		public async static Task<Result<T>> Next<T>(this Task<Result<T>> task, Func<Task<T>> next)
		{
			var result = await task;
			if (result.IsFailed)
				return result.Error;

			return await TryCatch(next);
		}

		public async static Task<Result> OnError(this Task<Result> task, Func<Result, Task> onError)
		{
			var result = await task;
			if (result.IsFailed)
				await onError(result);

			return result;
		}

		public async static Task<Result<T>> OnError<T>(this Task<Result<T>> task, Func<Task> onError)
		{
			var result = await task;
			if (result.IsFailed)
				await onError();

			return result;
		}

		public async static Task<Result> OnError(this Task<Result> task, Action<Result> onError)
		{
			var result = await task;
			if (result.IsFailed)
				onError(result);

			return result;
		}

		public async static Task<Result<T>> OnError<T>(this Task<Result<T>> task, Action<Result<T>> onError)
		{
			var result = await task.ConfigureAwait(false);
			if (result.IsFailed)
				onError(result);

			return result;
		}

		public async static Task<Result<T>> OnError<T>(this Task<Result<T>> task, Func<Result<T>, Task> onError)
		{
			var result = await task;
			if (result.IsFailed)
				await onError(result);

			return result;
		}

		public async static Task<Result<T>> Tap<T>(this Task<Result<T>> task, Func<Task> tap)
		{
			var result = await task;
			if (result.IsFailed)
				return result.Error;

			var tapResult = await TryCatch(tap);
			if (tapResult.IsFailed)
				return tapResult.Error;

			return result;
		}

		public async static Task<Result<T>> Tap<T>(this Task<Result<T>> task, Func<T, Task> tap)
		{
			var result = await task.ConfigureAwait(false);
			if (result.IsFailed)
				return result.Error;

			var tapResult = await TryCatch(() => tap(result.Value)).ConfigureAwait(false);
			if (tapResult.IsFailed)
				return tapResult.Error;

			return result;
		}

		public async static Task<Result<R>> Apply<T, R>(this Task<Result<T>> task, Func<T, Task<R>> apply)
		{
			var taskResult = await task;
			if (taskResult.IsFailed)
				return taskResult.Error;

			return await TryCatch(() => apply(taskResult.Value));
		}

		public async static Task<Result<R>> Map<T, R>(this Task<Result<T>> task, Func<T, R> map)
		{
			var taskResult = await task.ConfigureAwait(false);
			if (taskResult.IsFailed)
				return taskResult.Error;

			return map(taskResult.Value);
		}

		public async static Task<Result<T>> Mutate<T>(this Task<Result<T>> task, Action<T> mutate)
		{
			var taskResult = await task;
			if (taskResult.IsFailed)
				return taskResult.Error;

			mutate(taskResult.Value);

			return taskResult;
		}
	}
}
