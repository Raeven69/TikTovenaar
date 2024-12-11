using System;
using System.IO;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.WPF;

namespace TikTovenaar.Installer
{
    public class Program
    {
        static void Main()
        {
            var project = new ManagedProject("Installeer tovenaar",
                              new Dir(@"%ProgramFiles%\TikTovenaar",
                                  new WixSharp.File("Program.cs")));

            project.GUID = new Guid("ea03ac19-a5c9-4cad-ba64-f232063453bf");

            // project.ManagedUI = ManagedUI.DefaultWpf; // all stock UI dialogs

            //custom set of UI WPF dialogs
            project.ManagedUI = new ManagedUI();
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.Parent.FullName; //gets the default project files
            project.BackgroundImage = $"{projectDirectory}\\TikTovenaar\\Images\\logo.png";


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
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }
}