namespace ConsoleApp
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helps launch a file with the default program
    /// </summary>
    public class Launcher
    {
        /// <summary>
        /// Gets the associated program that can open the given <paramref name="file"/>
        /// </summary>
        /// <remarks>
        /// https://github.com/dotnet/corefx/issues/10361#issuecomment-235502080
        /// </remarks>
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void Launch(FileInfo file)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {file.FullName}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", file.FullName);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", file.FullName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
