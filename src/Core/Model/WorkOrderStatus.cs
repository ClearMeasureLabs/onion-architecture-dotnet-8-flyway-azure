using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Core.Model
{
	public class WorkOrderStatus
	{
		private static readonly ILogger _logger = NullLogger<WorkOrderStatus>.Instance;
		
		public static readonly WorkOrderStatus None = new WorkOrderStatus("", "", " ", 0);
		public static readonly WorkOrderStatus Draft = new WorkOrderStatus("DFT", "Draft", "Draft", 1);
		public static readonly WorkOrderStatus Assigned = new WorkOrderStatus("ASD", "Assigned", "Assigned", 2);
		public static readonly WorkOrderStatus InProgress = new WorkOrderStatus("IPG", "InProgress", "In Progress", 3);
		public static readonly WorkOrderStatus Complete = new WorkOrderStatus("CMP", "Complete", "Complete", 4);
        public static readonly WorkOrderStatus Cancelled = new WorkOrderStatus("CNL", "Cancelled", "Cancelled", 5);

	    private string _code;
		private string _key;

	    protected WorkOrderStatus()
		{
		}

		protected WorkOrderStatus(string code, string key, string friendlyName, byte sortBy)
		{
			_code = code;
			_key = key;
			FriendlyName = friendlyName;
			SortBy = sortBy;
		}

		public static WorkOrderStatus[] GetAllItems()
		{
			return new []
				{
					Draft,
					Assigned,
					InProgress,
					Complete,
                    Cancelled
				};
		}

		public string Code
		{
			get { return _code; }
		}

		public string Key
		{
			get { return _key; }
		}

	    public string FriendlyName { get; set; }

	    public byte SortBy { get; set; }

	    public override bool Equals(object obj)
		{
			var code = obj as WorkOrderStatus;
			if (code == null) return false;

			if (!GetType().Equals(obj.GetType())) return false;

			return _code.Equals(code.Code);
		}

		public override string ToString()
		{
			return FriendlyName;
		}

		public override int GetHashCode()
		{
			return _code.GetHashCode();
		}

		public bool IsEmpty()
		{
			return Code == "";
		}

		public static WorkOrderStatus FromCode(string code)
		{
			WorkOrderStatus[] items = GetAllItems();
			WorkOrderStatus match =
				Array.Find(items, delegate(WorkOrderStatus instance) { return instance.Code == code; });

			if (match == null)
			{
				_logger.LogWarning("WorkOrderStatusCode code {0} out of range.", code);
				match = None;
			}

			return match;
		}

		public static WorkOrderStatus FromKey(string key) 
		{
			if (key == null)
			{
				throw new NotSupportedException("Finding a WorkOrderStatusCode for a null key is not supported");
			}

			WorkOrderStatus[] items = GetAllItems();
			WorkOrderStatus match = Array.Find(items, delegate(WorkOrderStatus instance) { return (instance.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); });

			if (match == null)
			{
				throw new ArgumentOutOfRangeException(string.Format("Key '{0}' is not a valid key for {1}", key, typeof(WorkOrderStatus).Name));
			}

			return match;
		}

		public static WorkOrderStatus Parse(string name)
		{
			return FromKey(name);
		}
	}
}