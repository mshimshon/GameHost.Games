namespace GameHost.Games.Lib.Installation.Extensions;

public static class ExecutionHelperExt
{
    public static async Task TrialProcess(Func<Task> action, CancellationToken ct = default)
    {
        int maxTry = 2;
        do
        {
            if (ct.IsCancellationRequested) return;
            Exception? failure = default;
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            bool hasFailed = failure != default;
            bool hasSucceeded = failure == default;
            if (ct.IsCancellationRequested) return;
            if (hasFailed && maxTry > 0)
            {
                maxTry--;
                continue;
            }
            else if (hasSucceeded)
            {
                break;
            }
            else
            {
                throw failure!;
            }

        } while (maxTry >= 0 || ct.IsCancellationRequested);
    }
    public static async Task ProcessWithLock(string lockFile, Func<Task> action, Func<Exception> onLockFailed, CancellationToken ct = default)
    {
        bool skipClearing = false;
        try
        {
            FileStream lockHandle = File.Open(lockFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            await action();
        }
        catch (IOException)
        {
            skipClearing = true;
            throw onLockFailed();
        }
        catch
        {
            throw;
        }
        finally
        {
            if (!skipClearing && File.Exists(lockFile))
                File.Delete(lockFile);
        }
    }

}
