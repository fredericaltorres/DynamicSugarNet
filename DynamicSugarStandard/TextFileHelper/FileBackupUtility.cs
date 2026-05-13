using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Provides static utility methods for file backup and restoration.
///
/// Backup naming convention:
///   Original : report.txt
///   1st copy : report_backup1.txt
///   2nd copy : report_backup2.txt
///   nth copy : report_backupN.txt
/// </summary>
public static class FileBackupUtility
{
    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a numbered backup copy of <paramref name="filePath"/>.
    /// The counter is determined by scanning the file's directory for existing
    /// backup copies so that the new copy always receives the next available number.
    /// </summary>
    /// <param name="filePath">
    ///   Full or relative path to the file that should be backed up.
    /// </param>
    /// <returns>The full path of the newly created backup file.</returns>
    /// <exception cref="FileNotFoundException">
    ///   Thrown when <paramref name="filePath"/> does not exist.
    /// </exception>
    public static string BackupFile(string filePath)
    {
        filePath = Path.GetFullPath(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Source file not found: {filePath}");

        int nextNumber = GetHighestBackupNumber(filePath) + 1;
        string backupPath = BuildBackupPath(filePath, nextNumber);

        File.Copy(filePath, backupPath, overwrite: false);

        return backupPath;
    }

    /// <summary>
    /// Restores the highest-numbered backup of <paramref name="filePath"/> by
    /// overwriting the original file with it, then deletes that backup copy.
    /// </summary>
    /// <param name="filePath">
    ///   Full or relative path to the original file (the restore target).
    /// </param>
    /// <returns>The backup number that was restored.</returns>
    /// <exception cref="InvalidOperationException">
    ///   Thrown when no backup copies exist for the given file.
    /// </exception>
    public static int RestoreCopy(string filePath)
    {
        filePath = Path.GetFullPath(filePath);

        int highest = GetHighestBackupNumber(filePath);

        if (highest == 0)
            throw new InvalidOperationException(
                $"No backup copies found for: {filePath}");

        string backupPath = BuildBackupPath(filePath, highest);

        // Overwrite the original (create it if it was deleted).
        File.Copy(backupPath, filePath, overwrite: true);
        Console.WriteLine($"[RestoreCopy] Restored backup #{highest} → {filePath}");

        // Remove the consumed backup copy.
        File.Delete(backupPath);
        Console.WriteLine($"[RestoreCopy] Deleted backup copy: {backupPath}");

        return highest;
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Scans the directory of <paramref name="filePath"/> for files that match
    /// the backup naming pattern and returns the highest counter found (0 if none).
    /// </summary>
    private static int GetHighestBackupNumber(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath) ?? ".";
        string baseName = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);          // includes the dot

        // Pattern: <baseName>_backup<digits><extension>
        // Example: report_backup3.txt
        var pattern = new Regex(
            $@"^{Regex.Escape(baseName)}_backup\.(\d+)\.{Regex.Escape(extension)}bu$",
            RegexOptions.IgnoreCase);

        var files = Directory.EnumerateFiles(dir, $"{baseName}_backup*{extension}bu").ToList();
        var files2 = files.Select(f => pattern.Match(Path.GetFileName(f))).Where(m => m.Success);
        var files3 = files2.Select(m => int.Parse(m.Groups[1].Value));
        var versions = files3.DefaultIfEmpty(0).ToList();
        var max = versions.Max();
        return max;

    }

    /// <summary>
    /// Constructs the full path for backup copy number <paramref name="number"/>.
    /// </summary>
    private static string BuildBackupPath(string filePath, int number)
    {
        string dir = Path.GetDirectoryName(filePath) ?? ".";
        string baseName = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        string backupName = $"{baseName}_backup.{number}.{extension}bu";
        return Path.Combine(dir, backupName);
    }
}