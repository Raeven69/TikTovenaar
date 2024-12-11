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
            string targetDirectory = Path.Combine(projectDirectory, "TikTovenaar", "bin", "Release", "net8.0-windows", "publish", "win-x86");
            var targetFiles = Directory.EnumerateFiles(targetDirectory, "*").Where(file => !file.EndsWith("pdb"));

            var project = new ManagedProject("Installeer tovenaar",
                              new Dir(@"%ProgramFiles%\TikTovenaar",
                                  new Files(Path.Combine(targetDirectory, "*.*"), f => !f.EndsWith(".pdb") && !f.EndsWith("."))))
            {
                GUID = new Guid("ea03ac19-a5c9-4cad-ba64-f232063453bf"),

                
                ManagedUI = new ManagedUI(),

                BackgroundImage = $"{projectDirectory}\\TikTovenaar\\Images\\logo.png"
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
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }
}