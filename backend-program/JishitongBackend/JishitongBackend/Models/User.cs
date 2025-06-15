namespace JishitongBackend.Models
{
    public class User
    {

        private int userId;
        private string? userName;
        private string? userRole;
        private string? userPassword;
        private string? userContact;
        private string? icon;
        private string? userIntroduction;
        private int? userAge;
        private int? userOrganizationId;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string? UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string? UserRole
            {
            get { return userRole; }
            set { userRole = value; }
        }   
        public string? UserPassword 
            {
            get { return userPassword; }
            set { userPassword = value; }
        }

        public string?  UserContact
            {
            get { return userContact; }
            set { userContact = value; }
        }
        public string? UserIntroduction
            {
            get { return userIntroduction; }
            set { userIntroduction = value; }
        }
        public string? Icon
            {
            get { return icon; }
            set { icon = value; }
        }
        public int? UserAge
            {
            get { return userAge; }
            set { userAge = value; }
        }
        public int? UserOrganizationId
            {
            get { return userOrganizationId; }
            set { userOrganizationId = value; }
        }

    }
}
