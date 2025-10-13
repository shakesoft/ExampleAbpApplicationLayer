using System;
using System.IO;
using System.Threading.Tasks;

namespace Volo.Abp.AuditLogging.ExcelFileDownload;

/// <summary>
/// Interface for file download service
/// </summary>
public interface IExcelFileDownloadService
{
    /// <summary>
    /// Creates a download link for the specified file
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <returns>Download link as string</returns>
    Task<string> CreateDownloadLinkAsync(string fileName);
    
    /// <summary>
    /// Gets the file content from blob storage
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <returns>File content as stream</returns>
    Task<Stream> GetFileContentAsync(string fileName);
    
    /// <summary>
    /// Generates a unique file name
    /// </summary>
    /// <param name="fileNameWithoutExtension">File name without extension</param>
    /// <param name="extension">File extension</param>
    /// <returns>Unique file name</returns>
    string GenerateUniqueFileName(string fileNameWithoutExtension, string extension);
    
    /// <summary>
    /// Saves file content to blob storage
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="fileContent">Content of the file</param>
    /// <param name="createdBy">User who created the file</param>
    /// <returns>Task</returns>
    Task SaveFileAsync(string fileName, Stream fileContent, Guid? createdBy);
    
    /// <summary>
    /// Cleans up old files
    /// </summary>
    /// <returns>Task</returns>
    Task CleanupOldFilesAsync();
} 