namespace Lemoo_pos.Common.Enums
{
    public enum StaffStatus
    {
        /// <summary>
        /// Employee is currently working.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// Employee is currently on leave.
        /// </summary>
        ON_LEAVE,

        /// <summary>
        /// Employee has resigned from the company.
        /// </summary>
        RESIGNED,

        /// <summary>
        /// Employee has retired from the company.
        /// </summary>
        RETIRED,

        /// <summary>
        /// Employee is temporarily suspended from work.
        /// </summary>
        SUSPENDED,

        /// <summary>
        /// Employee is currently undergoing training.
        /// </summary>
        TRAINING,

        /// <summary>
        /// Employee is no longer working with the company.
        /// </summary>
        TERMINATED,

        /// <summary>
        /// Employee has not yet started work but is hired.
        /// </summary>
        HIRE_NOT_STARTED,


        /// <summary>
        /// An invitation has been sent to the employee, awaiting their response.
        /// </summary>
        PENDING_INVITATION,
        

    }
}
