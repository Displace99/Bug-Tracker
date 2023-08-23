using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Web.Models.Task
{
    public class EditTask
    {
        /// <summary>
        /// Id of the Task
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Bug Id that the task belongs to
        /// </summary>
        public int BugId { get; set; }

        /// <summary>
        /// Description of the Task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// User Id of the person who created the Task
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// User Id of the last person who updated task
        /// </summary>
        public int LastUpdatedBy { get; set; }

        /// <summary>
        /// User Id of the person assiged to the task
        /// </summary>
        public int? AssignedTo { get; set; }

        /// <summary>
        /// Date that Task is planned to start on
        /// </summary>
        public DateTime? PlannedStartDate { get; set; }

        /// <summary>
        /// Date that the task actually started on
        /// </summary>
        public DateTime? ActualStartDate { get; set; }

        /// <summary>
        /// Date that Task is planned to end on
        /// </summary>
        public DateTime? PlannedEndDate { get; set; }

        /// <summary>
        /// Date that Task actually ended on
        /// </summary>
        public DateTime? ActualEndDate { get;set; }

        /// <summary>
        /// Measurement units for time
        /// </summary>
        public string DurationUnits { get; set; }

        /// <summary>
        /// How long the Task is planned to take
        /// </summary>
        public decimal? PlannedDuration { get; set; }

        /// <summary>
        /// How long the task actually took
        /// </summary>
        public decimal? ActualDuration { get; set; }

        /// <summary>
        /// Percentage of the Task that is complete
        /// </summary>
        public int? PercentComplete { get; set; }
        
        /// <summary>
        /// Status of the Task
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Number used to sort the Tasks in a specific order
        /// </summary>
        public int? SortSequence { get; set; }
        
    }
}