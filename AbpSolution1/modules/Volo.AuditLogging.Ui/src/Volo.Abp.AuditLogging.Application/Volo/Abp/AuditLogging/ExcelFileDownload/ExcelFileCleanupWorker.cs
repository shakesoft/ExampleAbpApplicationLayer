using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Volo.Abp.AuditLogging.ExcelFileDownload;
/// <summary>
/// Background worker that cleans up old files periodically
/// </summary>
public class ExcelFileCleanupWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ExcelFileCleanupWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<AuditLogExcelFileOptions> options)
        : base(timer, serviceScopeFactory)
    {
        timer.Period = options.Value.ExcelFileCleanupOptions.Period;
        CronExpression = options.Value.ExcelFileCleanupOptions.CronExpression;
    }

    protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        Logger.LogInformation("File cleanup worker started");

        await workerContext
            .ServiceProvider
            .GetRequiredService<IExcelFileDownloadService>()
            .CleanupOldFilesAsync();

        Logger.LogInformation("File cleanup worker finished");
    }
}
