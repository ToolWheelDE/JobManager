using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel
{
    [TestFixture]
    public class DefaultJobStart
    {
        private IServiceProvider CreateServiceProvider(Action<IJobManagerConfigurationBuilder>? configure = null)
        {
            var services = new ServiceCollection();

            services.AddJobManager(configure);

            return services.BuildServiceProvider();
        }

        [Test]
        public void Test_Execute_Job()
        {
            var completed = false;

            var provider = CreateServiceProvider(configure => configure
                .ConfigureJobs(jobs => jobs
                    .AddMethod(() => completed = true, m => m.Id("1"))
                ));

            var jobService = provider.GetRequiredService<IJobService>();
            var jobTaskService = provider.GetRequiredService<IJobTaskService>();
            var job = jobService.Find("1");

            Assert.That(job, Is.Not.Null);

            var jobTask = jobTaskService.Execute(job);

            Thread.Sleep(500);

            Assert.That(completed, Is.True);
            Assert.That(jobTask, Is.Not.Null);
            Assert.That(jobTask.Status, Is.EqualTo(JobTaskStatus.Success));
        }
    }
}
