using ASOFT.A00.Entities;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;

namespace ASOFT.A00.API.BaseQuartz
{
    public class QuartzServiceUtility
    {

        public static void StartJob<TJob>(IScheduler scheduler, IConfiguration configuration) where TJob : IJob
        {
            string status = "0";
            string typeTime = string.Empty;
            string timeScan = string.Empty;

            string test = configuration["AutomationSchedule:Status"];
            if (ASOFTEnvironment.Automation.TryGetValue("status", out status)
                && ASOFTEnvironment.Automation.TryGetValue("timeScan", out timeScan)
                && ASOFTEnvironment.Automation.TryGetValue("typeOfTime", out typeTime))
            {
                var jobName = typeof(TJob).FullName;

                var job = JobBuilder.Create<TJob>().WithIdentity(jobName).Build();

                // Trường hợp thiết lập CÓ sử dụng chuông thông báo ở common.Configuration.json 
                if (status.Equals("1"))
                {
                    // Quét Automation:
                    // + Thực thi ngay lập tức khi đăng ký.
                    // + Lặp lại mỗi {n} phút => Vĩnh viễn lặp lại
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity($"{jobName}.trigger")
                        .StartNow()
                        .WithSimpleSchedule(s => s.WithIntervalInMinutes(TimeScanToMinute(Int32.Parse(typeTime), Int32.Parse(timeScan))).RepeatForever())
                        .Build();

                    scheduler.ScheduleJob(job, trigger);
                }
            }
        }

        /// <summary>
        ///     Tính thời gian quét Automation => Minute
        /// </summary>
        /// <param name="typeTime"></param>
        /// <param name="timeScan"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành] Created [18/11/2020]
        /// </history>
        private static int TimeScanToMinute(int typeTime, int timeScan)
        {
            // typeTime: 0 - Min / 1 - Hour
            switch (typeTime)
            {

                case 0:
                    break;
                case 1:
                    timeScan = timeScan * 60;
                    break;
            }
            return timeScan;
        }
    }
}
