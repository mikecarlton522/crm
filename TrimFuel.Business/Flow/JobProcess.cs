using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Flow
{
    public class JobProcess : BaseService, IDisposable
    {
        public Job Job { get; private set; }

        /// <summary>
        /// Use only with "using" statement
        /// </summary>
        /// <param name="key">String Key to identify Job Process </param>
        public JobProcess(string jobKey)
        {
            //Lock key or throw exception if key is already locked
            try
            {
                Job isBusy = CheckKey(jobKey);
                if (isBusy != null)
                {
                    throw new Exception(string.Format("The Job with JobKey({0}) is busy", jobKey));
                }
                if (string.IsNullOrEmpty(jobKey))
                {
                    throw new Exception(string.Format("The Job with JobKey({0}) is busy", jobKey));
                }
                Job = new Job();
                Job.JobKey = jobKey;
                Job.StartDT = DateTime.Now;
                Job.BackControlFlagStop = false;
                Job.Finished = false;
                Job.ProgressPercent = 0M;
                dao.Save(Job);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //throw exception if can't get key
                throw new Exception(string.Format("Can't lock JobKey({0})", jobKey), ex);
            }
        }

        public bool CheckAvailabilityAndUpdateProgress(decimal progressPercent, string processState)
        {
            bool res = CheckAvailability();
            try
            {
                Job.ProgressPercent = progressPercent;
                Job.CustomData = processState;
                UpdateLastState();
            }
            catch
            {
                res = false;
            }
            return res;
        }

        public bool CheckAvailability()
        {
            bool res = false;
            try
            {
                Job workingJob = CheckKey(Job.JobKey);
                RetrieveLastState();
                res = (
                    !Job.Finished.Value &&
                    !Job.BackControlFlagStop.Value &&
                    !(workingJob != null && workingJob.JobID.Value < Job.JobID.Value)
                    );

            }
            catch
            {
                res = false;
            }
            return res;
        }

        private void RetrieveLastState()
        {
            try 
	        {	        
		        Job = dao.Load<Job>(Job.JobID);
	        }
	        catch (Exception ex)
	        {
                logger.Error(ex);
                throw ex;
	        }
        }

        private void UpdateLastState()
        {
            try
            {
                dao.Save(Job);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        private Job CheckKey(string jobKey)
        {
            Job res = null;
            try
            {
                //First active job with this jobKey
                MySqlCommand q = new MySqlCommand(@"
                    select j.* from Job j
                    where j.JobKey = @jobKey and j.Finished = 0
                    order by j.JobID asc
                    limit 1
                ");
                q.Parameters.Add("@jobKey", MySqlDbType.VarChar).Value = jobKey;
                res = dao.Load<Job>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                //throw exception if can't get key
                throw new Exception(string.Format("Can't check JobKey({0})", jobKey), ex);
            }
            return res;
        }

        private void FinishJob()
        {
            try
            {
                RetrieveLastState();
                Job.Finished = true;
                Job.EndDT = DateTime.Now;
                UpdateLastState();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            try
            {
                //Unlock key
                if (Job != null && !Job.Finished.Value)
                {
                    FinishJob();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
    }
}
