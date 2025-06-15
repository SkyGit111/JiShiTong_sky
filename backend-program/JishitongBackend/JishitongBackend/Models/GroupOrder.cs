namespace JishitongBackend.Models
{
    public class GroupOrder
    {
        private int requestId;
        private int? primeUserId;
        private string? requestType;
        private string? requestStates;
        private DateTime? startTime;
        private DateTime? endTime;
        private string? title;
        private DateTime? initiationTime;
        private int? personNum;
        private string? description;
        private string? extraRequirement;
        private string? priceDistribution;
        private string? participants;  // JSON格式，通常会以字符串形式存储
        private List<Object>? participantsDetails;// 新增字段，非数据库属性，用来保存参与人详细信息

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public int? PrimeUserId
        {
            get { return primeUserId; }
            set { primeUserId = value; }
        }

        public string? RequestType
        {
            get { return requestType; }
            set { requestType = value; }
        }

        public string? RequestStates
        {
            get { return requestStates; }
            set { requestStates = value; }
        }

        public DateTime? StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public string? Title
        {
            get { return title; }
            set { title = value; }
        }

        public DateTime? InitiationTime
        {
            get { return initiationTime; }
            set { initiationTime = value; }
        }

        public int? PersonNum
        {
            get { return personNum; }
            set { personNum = value; }
        }

        public string? Description
        {
            get { return description; }
            set { description = value; }
        }

        public string? ExtraRequirement
        {
            get { return extraRequirement; }
            set { extraRequirement = value; }
        }

        public string? PriceDistribution
        {
            get { return priceDistribution; }
            set { priceDistribution = value; }
        }

        public string? Participants
        {
            get { return participants; }
            set { participants = value; }
        }

        public List<Object>? ParticipantsDetails
        {

            get { return participantsDetails; }
            set { participantsDetails = value; }
        }
        
    }

    public class GroupProject
    {
        private int projectId;
        private string? projectName;
        private int? totalPrice;
        private int? requestId;
        private string? requestType;
        // 新增字段，用于存储具体信息
        public object? specificInfo;

        public int ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        public string? ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        public int? TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }

        public int? RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public string? RequestType
        {
            get { return requestType; }
            set { requestType = value; }
        }

        public object? SpecificInfo
        {
            get { return specificInfo; }
            set { specificInfo = value; }
        }
        

    }

    public class TrafficGroup
    {
        private int projectId;
        private string? originPlace;
        private string? destinationPlace;
        private DateTime? trafficTime;

        public int ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        public string? OriginPlace
        {
            get { return originPlace; }
            set { originPlace = value; }
        }

        public string? DestinationPlace
        {
            get { return destinationPlace; }
            set { destinationPlace = value; }
        }

        public DateTime? TrafficTime
        {
            get { return trafficTime; }
            set { trafficTime = value; }
        }
    }

    public class ActivityGroup
    {
        private int projectId;
        private string? location;
        private DateTime? activityTime;

        public int ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        public string? Location
        {
            get { return location; }
            set { location = value; }
        }

        public DateTime? ActivityTime
        {
            get { return activityTime; }
            set { activityTime = value; }
        }
    }

    public class ProductGroup
    {
        private int projectId;
        private string? platform;
        private string? address;

        public int ProjectId
        {
            get { return projectId; }
            set { projectId = value; }
        }

        public string? Platform
        {
            get { return platform; }
            set { platform = value; }
        }

        public string? Address
        {
            get { return address; }
            set { address = value; }
        }
    }
}
