using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WixSharp;
using WixSharp.UI.WPF;

namespace TikTovenaar.Installer
{
    public class Program
    {
        static void Main()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = workingDirectory;
            System.IO.DirectoryInfo parentDir = Directory.GetParent(workingDirectory);
            for (int i = 0; i < 4 && parentDir != null; i++)
            {
                parentDir = parentDir.Parent;
            }
            if (parentDir != null)
            {
                projectDirectory = parentDir.FullName;
            } //gets the default project files


            if (workingDirectory.EndsWith("TikTovenaar"))
            {
                projectDirectory = workingDirectory;
            }

            string targetDirectory = Path.Combine(projectDirectory, "TikTovenaar", "bin", "Release", "net8.0-windows", "publish", "win-x86");
            Console.WriteLine(targetDirectory);
            
            ManagedProject project = new ManagedProject("TikTovenaar",
                            //actual install directory
                            new InstallDir(@"%ProgramFiles%\TikTovenaar",
                             new Files(Path.Combine(targetDirectory, "*.*"), f => !f.EndsWith(".pdb") && !f.EndsWith(".")),
                            //adds shortcut to desktop 
                             new Dir(@"%Desktop%",
                               new ExeFileShortcut("TikTovenaar", "[INSTALLDIR]TikTovenaar.exe", arguments: ""),
                               //adds shortcut to program menu to allow it to be searched
                               new Dir(@"%ProgramMenu%\Tiktovenaar",
                                 new ExeFileShortcut("TikTovenaar", "[INSTALLDIR]Tiktovenaar.exe", arguments: "")))))
            {
                GUID = new Guid("ea03ac19-a5c9-4cad-ba64-f232063453bf"),
                ManagedUI = new ManagedUI(),
                BackgroundImage = $"{projectDirectory}\\TikTovenaar\\Images\\background.png",
                BannerImage = $"{projectDirectory}\\TikTovenaar\\Images\\banner.png",
            };
            project.ControlPanelInfo.Manufacturer = "TikTovenaar BV";

            project.UIInitialized += e =>
            {
                //used to change the dialog buttons
                //gets started whenever the ui is initialised (event)
                MsiRuntime runtime = e.ManagedUI.Shell.MsiRuntime();
                runtime.UIText["WixUINext"] = "Volgende";
                runtime.UIText["WixUICancel"] = "Annuleren";
                runtime.UIText["WixUIBack"] = "Vorige";
                runtime.UIText["InstallDirDlgChange"] = "Wijzigen...";
                runtime.UIText["WixUIFinish"] = "Afronden";
                runtime.UIText["MaintenanceTypeDlgChangeButton"] = "Wijzigen...";
                runtime.UIText["MaintenanceTypeDlgRepairButton"] = "Repareren";
                runtime.UIText["MaintenanceTypeDlgVerwijderenButton"] = "Verwijderen";
            };
            
            project.OutDir = Path.Combine(projectDirectory, "TikTovenaar.Installer");
            Console.WriteLine(projectDirectory);

            project.ManagedUI.InstallDialogs.Add<TikTovenaar.Installer.WelcomeDialog>()
                                            .Add<TikTovenaar.Installer.LicenceDialog>()
                                            .Add<TikTovenaar.Installer.InstallDirDialog>()
                                            .Add<TikTovenaar.Installer.ProgressDialog>()
                                            .Add<TikTovenaar.Installer.ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<TikTovenaar.Installer.MaintenanceTypeDialog>()
                               .Add<TikTovenaar.Installer.ProgressDialog>()
                               .Add<TikTovenaar.Installer.ExitDialog>();
            project.BuildMsi();
        }
    }
}