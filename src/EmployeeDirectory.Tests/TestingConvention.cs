namespace EmployeeDirectory.Tests
{
    using System.Diagnostics;
    using System.IO;
    using EmployeeDirectory.Infrastructure;
    using Fixie;
    using Infrastructure;
    using static Testing;

    public class TestingConvention : Discovery, Execution
    {
        public TestingConvention()
        {
            Methods
                .Where(x => x.Name != "SetUp");
        }

        public void Execute(TestClass testClass)
        {
            testClass.RunCases(@case =>
            {
                ResetStubLoginService();

                var instance = testClass.Construct();
                SetUp(instance);
                @case.Execute(instance);

                instance.Dispose();

                var methodWasExplicitlyRequested = testClass.TargetMethod != null;

                if (methodWasExplicitlyRequested && @case.Exception is MatchException exception)
                    LaunchDiffTool(exception);
            });
        }

        private static void ResetStubLoginService()
        {
            Scoped<ILoginService>(loginService => ((StubLoginService)loginService).Reset());
        }

        private static void SetUp(object instance)
        {
            instance.GetType().GetMethod("SetUp")?.Execute(instance);
        }

        private static void LaunchDiffTool(MatchException exception)
        {
            var diffTool = Configuration["Tests:DiffTool"];

            if (!File.Exists(diffTool)) return;

            var tempPath = Path.GetTempPath();
            var expectedPath = Path.Combine(tempPath, "expected.txt");
            var actualPath = Path.Combine(tempPath, "actual.txt");

            File.WriteAllText(expectedPath, Json(exception.Expected));
            File.WriteAllText(actualPath, Json(exception.Actual));

            using (Process.Start(diffTool, $"{expectedPath} {actualPath}")) { }
        }
    }
}
