using Launcher.UI;
// using Microsoft.Build.Construction;

namespace Launcher.Services
{
    public interface IProjectInitializationService
    {
        void Initialize(Project project);
    }

    public class ProjectInitializationService : IProjectInitializationService
    {
        /// <inheritdoc />
        public void Initialize(Project project)
        {
            string nugetPackage = "SharpEngine.Core";
            string programContent = "Replace with window initialization content";

            CreateSolution(project.Path, project.Name, project.Name, nugetPackage, programContent);
        }

        private void CreateSolution(string solutionPath, string solutionName, string projectName, string nugetPackage, string programContent)
        {
            string fullSolutionPath = Path.Combine(solutionPath, solutionName);
            string projectPath = Path.Combine(fullSolutionPath, projectName);

            // Ensure directories exist
            Directory.CreateDirectory(fullSolutionPath);
            Directory.CreateDirectory(projectPath);

            // Create a new solution file
            string solutionFilePath = Path.Combine(fullSolutionPath, $"{solutionName}.sln");
            // SolutionFile solution = SolutionFile.Create(solutionFilePath);
            // solution.Save(solutionFilePath);

            // Create a new .csproj file
            string projectFilePath = Path.Combine(projectPath, $"{projectName}.csproj");
            // var project = ProjectRootElement.Create();
            // project.ToolsVersion = "Current";

            // Define project type as a console application
            // var propertyGroup = project.AddPropertyGroup();
            // propertyGroup.AddProperty("OutputType", "Exe");
            // propertyGroup.AddProperty("TargetFramework", "net8.0");
            // 
            // // Add NuGet package reference
            // var itemGroup = project.AddItemGroup();
            // itemGroup.AddItem("PackageReference", nugetPackage);
            // 
            // // Save the .csproj file
            // project.Save(projectFilePath);

            // Add project to the solution
            // solution.AddProject(projectFilePath, projectName);
            // solution.Save(solutionFilePath);

            // Write predefined content to Program.cs
            string programFilePath = Path.Combine(projectPath, "Program.cs");
            File.WriteAllText(programFilePath, programContent);
        }
    }
}
