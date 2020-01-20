using GBHO_Business.Objects;
using GBHO_Common.Controllers;
using GBHO_Data.EntityFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBHO_Business.Controllers
{
    public class MemberManager
    {

        private static MemberManager instance;
        public static MemberManager Instance
        {
            get
            {
                if (instance == null) instance = new MemberManager();
                return instance;
            }
        }

        private MemberManager()
        {

        }

        public List<Member> GetAllWithhDevAdmin()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A"
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ToList();
            }
        }

        public List<Member> GetAll()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & x.Type != "System"
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x=>x.FirstName).ToList();
            }
        }

        public List<Member> GetChilds(string hCode)
        {

            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & x.Type != "System" & x.HierarchyCode.Contains(hCode)
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ToList();
            }
        }

        public Member Get(int id)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.MemberId == id
                        select x).FirstOrDefault();
            }
        }

        public List<Member> GetAllForValidation()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & x.Type == "ForValidate"
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ToList();
            }
        }

        public List<Member> GetAllForApproval()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & x.Type == "ForApproval"
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ToList();
            }
        }

        public List<Member> GetAllAdmin()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.RecState == "A" & (x.Role.RoleCode == "Admin" | x.Role.RoleCode == "SuperAdmin")
                        select x).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ToList();
            }
        }

        public DashboardSummary GetDashboardSummary(string hCode)
        {
            DashboardSummary result = new DashboardSummary();
            using (GBHODBEntities db = new GBHODBEntities())
            {
                result.AllMembers = db.Members.Count(x => x.RecState == "A" && x.Type != "System");
                result.MyMembers = db.Members.Count(x => x.RecState == "A" && x.HierarchyCode.StartsWith(hCode + "0") || x.HierarchyCode.StartsWith(hCode + "1"));
                result.MyLeft = db.Members.Count(x => x.RecState == "A" && x.HierarchyCode.StartsWith(hCode + "0"));
                result.MyRight = db.Members.Count(x => x.RecState == "A" && x.HierarchyCode.StartsWith(hCode + "1"));
            }
            return result;
        }

        public string GetFullName(int id)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.MemberId == id
                        select x.FirstName + " " + x.LastName).FirstOrDefault();
            }
        }

        public string GetHierarchyCode(int id)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return (from x in db.Members
                        where x.MemberId == id
                        select x.HierarchyCode).FirstOrDefault();
            }
        }

        public Member Get(string username, string password)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                string pwdEncript = EncryptionManager.encrypt(password);
                Member result = null;

                Member item = (from x in db.Members
                               where (x.Username == username || x.Email == username)
                               && x.Password == pwdEncript && x.RecState == "A" && x.Type != "ForApproval"
                               select x).FirstOrDefault();


                if (item != null)
                {
                    result = new Member();
                    result.MemberId = item.MemberId;
                    result.MemberCode = item.MemberCode;
                    result.Username = item.Username;
                    result.LastName = item.LastName;
                    result.FirstName = item.FirstName;
                    result.MiddleName = item.MiddleName;
                    result.Email = item.Email;
                    result.ParentId = item.ParentId;
                    result.SponsorId = item.SponsorId;
                    result.Type = item.Type;
                    result.RoleId = item.RoleId;
                    result.HierarchyCode = item.HierarchyCode;
                    result.RecState = item.RecState;
                    result.Picture = item.Picture;
                    result.IsFreeAccount = item.IsFreeAccount;
                }
                

                return result;

            }
        }

        public Member Get(string username)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Member item = (from x in db.Members
                               where x.Username == username
                               select x).FirstOrDefault();

                return item;

            }
        }

        public int GetId(string username)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {

                Member item = (from x in db.Members
                               where x.Username == username 
                               select x).FirstOrDefault();


                if (item != null)
                {
                    return item.MemberId;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int SaveMember(Member member, string currUser)
        {
            int memberId = member.MemberId;

            using (GBHODBEntities db = new GBHODBEntities())
            {

                #region validate member code (hidden)
                ////validate member code
                //bool isCodeExists = db.Members.Count(x => x.MemberId != member.MemberId && x.MemberCode == member.MemberCode && x.RecState == "A") > 0;
                //if (isCodeExists)
                //{
                //    throw new Exception("Member ID already exists. Please enter a different member ID.");
                //}
                //else
                //{

                //}
                #endregion
                
                //validate username
                bool isUsernameExists = db.Members.Count(x => x.MemberId != member.MemberId && x.Username == member.Username && x.RecState == "A") > 0;
                if (isUsernameExists)
                {
                    throw new Exception("User name ID already exists. Please enter a different user name");
                }
                else
                {
                    // validate placement
                    bool isPlaceExists = false;
                    string hCode = string.Empty;
                    if (member.ParentId > 0)
                    {
                        string parentHCode = db.Members.SingleOrDefault(x => x.MemberId == member.ParentId).HierarchyCode;
                        hCode = parentHCode + member.HierarchyCode.Substring(member.HierarchyCode.Length - 1, 1);
                        isPlaceExists = db.Members.Count(x => x.MemberId != member.MemberId && x.HierarchyCode == hCode && x.RecState == "A") > 0;
                    }
                    if (isPlaceExists)
                    {
                        throw new Exception("Selected placement/group is already occupied. Please enter a different placement/group.");
                    }
                    else
                    {
                        int lastId = db.Members.OrderByDescending(x=>x.MemberId).First().MemberId;
                        Member item = db.Members.SingleOrDefault(x => x.MemberId == member.MemberId);

                        if (item == null)
                            item = new Member();
                                                
                        item.Username = member.Username;
                        item.FirstName = member.FirstName;
                        item.MiddleName = member.MiddleName;
                        item.LastName = member.LastName;
                        item.Email = member.Email;
                        item.MobileNo = member.MobileNo;
                        item.TelephoneNo = member.TelephoneNo;
                        item.Occupation = member.Occupation;
                        item.Birthday = member.Birthday;
                        item.BirthPlace = member.BirthPlace;
                        item.Citizenship = member.Citizenship;
                        item.Gender = member.Gender;
                        item.MaritalStatus = member.MaritalStatus;
                        item.PresentAddress = member.PresentAddress;
                        item.ProvincialAddress = member.ProvincialAddress;
                        item.ParentId = member.ParentId;
                        item.SponsorId = member.SponsorId;
                        item.PaymentType = member.PaymentType;
                        item.CashNote = member.CashNote;
                        item.BankBranch = member.BankBranch;
                        item.DepositNo = member.DepositNo;
                        item.DateDeposited = member.DateDeposited;

                        if (!string.IsNullOrEmpty(hCode))
                            item.HierarchyCode = hCode;

                        item.RoleId = member.RoleId;
                        item.IsFreeAccount = member.IsFreeAccount;

                        if (member.Picture != null)
                            item.Picture = member.Picture;

                        if (item.MemberId == 0)
                        {
                            item.MemberCode = String.Format("M-{0:D8}", lastId + 1);
                            item.Password = member.Password;
                            item.Type = MemberTypeEnum.ForValidate.ToString();
                            item.CreatedDate = DateHelper.DateTimeNow;
                            item.CreatedBy = currUser;
                            item.DateJoined = DateHelper.DateTimeNow;
                            item.RecState = "A";
                            db.Members.Add(item);
                        }
                        else
                        {
                            item.ModifiedBy = currUser;
                            item.ModifiedDate = DateHelper.DateTimeNow;
                        }

                        db.SaveChanges();
                        memberId = item.MemberId;
                    }
                }
            }

            return memberId;
        }

        public string ChangePassword(int memberId, string newPWD, string confirmPWD)
        {

            string e_newPWD = EncryptionManager.encrypt(newPWD);
            string e_confirmPWD = EncryptionManager.encrypt(confirmPWD);

            using (GBHODBEntities db = new GBHODBEntities())
            {
                //validate new and confirm password
                if (e_newPWD == e_confirmPWD)
                {
                    //validate old password
                    int checkOld = db.Members.Count(x => x.MemberId == memberId);
                    if (checkOld > 0)
                    {
                        //validate new password is unique
                        int checkUnique = db.Members.Count(x => x.MemberId == memberId && x.Password != e_newPWD);
                        if (checkUnique > 0)
                        {
                            Member item = db.Members.SingleOrDefault(x => x.MemberId == memberId);
                            item.Password = e_newPWD;
                            db.SaveChanges();
                            return string.Empty;
                        }
                        else
                        {
                            return "PasswordErrorMsg2";
                        }
                    }
                    else
                    {
                        return "PasswordErrorMsg1";
                    }
                }
                else
                {
                    return "PasswordErrorMsg1";
                }
            }
        }

        public void ValidateMemberType(int memberId, string status, string user)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                if (status != "validate")
                {
                    Member approver = db.Members.First(x => x.MemberId == memberId);
                    bool hasChildPending = db.Members.Count(x => x.RecState == "A" && x.HierarchyCode.StartsWith(approver.HierarchyCode) && x.MemberId != memberId) > 0;
                    if (hasChildPending)
                        throw new Exception("Unable to cancel, this member has already approved/pending downline.");
                }

                Member item = db.Members.SingleOrDefault(x => x.MemberId == memberId);
                if (item != null)
                {
                    item.Type = status == "validate" ? "ForApproval" : "Cancelled";
                    item.RecState = status == "validate" ? "A" : "D";
                    item.ApprovedBy = user;
                    item.ApprovedDate = DateHelper.DateTimeNow;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateMemberType(int memberId, string status, string user)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                if (status != "approve")
                {
                    Member approver = db.Members.First(x=>x.MemberId == memberId);
                    bool hasChildPending = db.Members.Count(x => x.RecState == "A" && x.HierarchyCode.StartsWith(approver.HierarchyCode) && x.MemberId != memberId) > 0;
                    if (hasChildPending)
                        throw new Exception("Unable to cancel approval, this member has already approved/pending downline.");
                }

                Member item = db.Members.SingleOrDefault(x => x.MemberId == memberId);
                if (item != null)
                {
                    item.Type = status == "approve" ? "Active" : "Cancelled";
                    item.RecState = status == "approve" ? "A" : "D";
                    item.ApprovedBy = user;
                    item.ApprovedDate = DateHelper.DateTimeNow;
                    db.SaveChanges();
                }
            }
        }

        public void SaveMemberLastLoggedOn(int id)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Member item = db.Members.SingleOrDefault(x => x.MemberId == id);
                if (item != null)
                {
                    item.LastLoggedIn = DateHelper.DateTimeNow;
                    db.SaveChanges();
                }
            }
        }

        public List<Role> GetAllRoles()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return db.Roles.Where(x=>x.RoleCode != "DevAdmin").ToList();
            }
        }

        public int GetDefaultRole()
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                return db.Roles.Where(x => x.RoleCode == "Member").First().RoleId;
            }
        }

        public string[] GetRoles(string userName)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                int memberId = int.Parse(userName);
                Member user = db.Members.FirstOrDefault(u => u.MemberId == memberId && u.RecState == "A");
                if (user != null)
                    return new string[] { user.Role.RoleCode };
                else
                    return new string[] { };
            }
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Member user = db.Members.FirstOrDefault(u => u.Username.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && u.RecState == "A" && u.Role.RoleCode == roleName);
                if (user != null)
                    return true;
                else
                    return false;
            }
        }

        public int NetGuardCheck(int memberId)
        {
            using (GBHODBEntities db = new GBHODBEntities())
            {
                Member user = db.Members.FirstOrDefault(u => u.MemberId == memberId);
                Member parent = user.ParentId > 0 ? db.Members.FirstOrDefault(u => u.MemberId == user.ParentId) : new Member();
                Member grandParent = user.ParentId > 0 ? db.Members.FirstOrDefault(u => u.MemberId == parent.ParentId) : new Member();

                if (user.LastName.ToLower().Trim() == parent.LastName.ToLower().Trim() && 
                    parent.LastName.ToLower().Trim() == grandParent.LastName.ToLower().Trim() &&
                    user.HierarchyCode.Substring(user.HierarchyCode.Length - 1) == parent.HierarchyCode.Substring(parent.HierarchyCode.Length - 1) &&
                    parent.HierarchyCode.Substring(parent.HierarchyCode.Length - 1) == grandParent.HierarchyCode.Substring(grandParent.HierarchyCode.Length - 1))
                {
                    return grandParent.MemberId;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
