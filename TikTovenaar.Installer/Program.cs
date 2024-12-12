using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.WPF;

namespace TikTovenaar.Installer
{
    public class Program
    {
        static void Main()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.Parent.FullName; //gets the default project files
            if (workingDirectory.EndsWith("TikTovenaar"))
            {
                Console.WriteLine(workingDirectory);
                projectDirectory = workingDirectory;
                Console.WriteLine(projectDirectory.Length);
            }
            else
            {
                Console.WriteLine(projectDirectory + " HOI");
            }
            string targetDirectory = Path.Combine(projectDirectory, "TikTovenaar", "bin", "Release", "net8.0-windows", "publish", "win-x86");
            Console.WriteLine(targetDirectory);
            var project = new ManagedProject("Installeer tovenaar",
                              new Dir(@"%ProgramFiles%\TikTovenaar",
                                  new Files(Path.Combine(targetDirectory, "*.*"), f => !f.EndsWith(".pdb") && !f.EndsWith("."))))
            {
                GUID = new Guid("ea03ac19-a5c9-4cad-ba64-f232063453bf"),


                ManagedUI = new ManagedUI(),
                BackgroundImage = $"{projectDirectory}\\TikTovenaar\\Images\\logo.png"
            };
            project.ControlPanelInfo.Manufacturer = "TikTovenaar BV";
            project.UIInitialized += e =>
            {
                // Since the default MSI localization data has no entry for 'CustomDlgTitle' (and other custom labels) we
                // need to add this new content dynamically. Alternatively, you can use WiX localization files (wxl).

                MsiRuntime runtime = e.ManagedUI.Shell.MsiRuntime();
                runtime.UIText["WixUINext"] = "Volgende";
                runtime.UIText["WixUICancel"] = "Annuleren";
            };

            Console.WriteLine(projectDirectory);

            project.ManagedUI.InstallDialogs.Add<TikTovenaar.Installer.WelcomeDialog>()
                                            .Add<TikTovenaar.Installer.LicenceDialog>()
                                            .Add<TikTovenaar.Installer.FeaturesDialog>()
                                            .Add<TikTovenaar.Installer.InstallDirDialog>()
                                            .Add<TikTovenaar.Installer.ProgressDialog>()
                                            .Add<TikTovenaar.Installer.ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<TikTovenaar.Installer.MaintenanceTypeDialog>()
                                           .Add<TikTovenaar.Installer.FeaturesDialog>()
                                           .Add<TikTovenaar.Installer.ProgressDialog>()
                                           .Add<TikTovenaar.Installer.ExitDialog>();

            //project.SourceBaseDir = "<input dir path>";
            project.OutDir = projectDirectory;

            project.BuildMsi();
        }
    }
}