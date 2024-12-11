using System;
using System.Windows.Forms;
using WixSharp.UI.WPF;

namespace WixSharp
{
    public class Program
    {
        static void Main()
        {
            CreateInstaller();
        }

        public static void CreateInstaller()
        {
            var project = new ManagedProject("Installeer Tovenaar",
                  new Dir(@"%ProgramFiles%\TikTovenaar",
                      new File("Program.cs")));

            project.GUID = new Guid("5de0d7a9-8e9d-4ef5-bfef-83c2f2c6e429");

            // project.ManagedUI = ManagedUI.DefaultWpf; // all stock UI dialogs

            //custom set of UI WPF dialogs
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add<WixSharp_Setup1.WelcomeDialog>()
                                            .Add<WixSharp_Setup1.LicenceDialog>()
                                            .Add<WixSharp_Setup1.FeaturesDialog>()
                                            .Add<WixSharp_Setup1.InstallDirDialog>()
                                            .Add<WixSharp_Setup1.ProgressDialog>()
                                            .Add<WixSharp_Setup1.ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<WixSharp_Setup1.MaintenanceTypeDialog>()
                                           .Add<WixSharp_Setup1.FeaturesDialog>()
                                           .Add<WixSharp_Setup1.ProgressDialog>()
                                           .Add<WixSharp_Setup1.ExitDialog>();

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }
}