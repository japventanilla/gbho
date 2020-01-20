using GBHO_Business.Controllers;
using GBHO_Data.EntityFrameworks;
using GBHO_Web.Areas.MyAccount.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.WebPages.Html;

namespace GBHO_Web.Classes
{
    public static class MemberHelper
    {

        public static List<MemberViewModel> MyDirectReferrals(int memberId)
        {
            List<MemberViewModel> result = new List<MemberViewModel>();
            List<Member> geneology = IncomeManager.Instance.GetMyDirectReferrals(memberId);

            foreach (Member item in geneology)
            {
                result.Add(BuildMember(item));
            }

            return result;
        }

        public static string GeneologyCardStyle(MemberViewModel parent, MemberViewModel current, string role)
        {
            string style = "";

            //if (parent != null && parent.MemberType != "ForApproval")
            if (parent != null)
            {
                if (current == null)
                {
                    if (role != "Member")
                        style = "bg-dark clickable";
                    else
                        style = "bg-dark";
                }
                else if (current.MemberType == "ForApproval" || current.MemberType == "ForValidate")
                {
                    style = "bg-warning clickable";
                }
                else
                {
                    style = "bg-success clickable";
                }
            }
            else
            {
                style = "bg-dark";
            }

            return style;
        }

        public static string GeneologyRedirect(MemberViewModel parent, MemberViewModel current, int group, string role)
        {
            string url = "return false;";

            //if (parent != null && parent.MemberType != "ForApproval")
            if (parent != null)
            {
                if (current == null)
                {
                    if(role != "Member")
                        url = string.Format("window.location='/MyAccount/Member/Record?p={0}&g={1}';", parent.MemberId, group);
                    else
                        url = "return false;";
                }
                else
                {
                    url = string.Format("window.location='/MyAccount/Geneology?id={0}';", current.MemberId);
                }
            }

            return url;
        }

        public static List<MemberViewModel> MyGeneology(int memberId)
        {
            List<MemberViewModel> result = new List<MemberViewModel>();
            string hCode = Get(memberId).HierarchyCode;
            List<Member> geneology = IncomeManager.Instance.GetMyGeneology(hCode);


            //Level 1
            result.Insert(0, BuildGeneology(geneology,hCode));

            //Level 2
            result.Insert(1, BuildGeneology(geneology, hCode + "0"));
            result.Insert(2, BuildGeneology(geneology, hCode + "1"));

            //Level 3
            result.Insert(3, BuildGeneology(geneology, hCode + "00"));
            result.Insert(4, BuildGeneology(geneology, hCode + "01"));
            result.Insert(5, BuildGeneology(geneology, hCode + "10"));
            result.Insert(6, BuildGeneology(geneology, hCode + "11"));

            //Level 4
            result.Insert(7, BuildGeneology(geneology, hCode + "000"));
            result.Insert(8, BuildGeneology(geneology, hCode + "001"));
            result.Insert(9, BuildGeneology(geneology, hCode + "010"));
            result.Insert(10, BuildGeneology(geneology, hCode + "011"));
            result.Insert(11, BuildGeneology(geneology, hCode + "100"));
            result.Insert(12, BuildGeneology(geneology, hCode + "101"));
            result.Insert(13, BuildGeneology(geneology, hCode + "110"));
            result.Insert(14, BuildGeneology(geneology, hCode + "111"));

            return result;
        }

        public static List<MemberViewModel> MyGeneologyLinear(int memberId)
        {
            List<MemberViewModel> result = new List<MemberViewModel>();
            string hCode = Get(memberId).HierarchyCode;
            List<Member> geneology = IncomeManager.Instance.GetMyGeneology(hCode);

            foreach (Member item in geneology)
            {
                result.Add(BuildGeneologyItem(item, hCode));
            }

            return result;
        }

        private static MemberViewModel BuildGeneology(List<Member> geneology, string hCode)
        {
            MemberViewModel result = null;

            Member item = geneology.Where(x => x.HierarchyCode == hCode).FirstOrDefault();
            if (item != null)
            {
                result = BuildGeneologyItem(item, hCode);
            }

            return result;
        }

        private static MemberViewModel BuildGeneologyItem(Member item, string hCode)
        {
            MemberViewModel result = new MemberViewModel();

            result.MemberId = item.MemberId;
            result.Username = item.Username;
            result.LName = item.LastName;
            result.FName = item.FirstName;
            result.MName = item.MiddleName;

            if (item.HierarchyCode == hCode)
                result.Level = "1";
            else
                result.Level = (item.HierarchyCode.Replace(hCode, "").Length + 1).ToString();

            if (item.HierarchyCode != "$1")
                result.HierarchyCode = item.HierarchyCode.Substring(item.HierarchyCode.Length - 1, 1) == "0" ? PlacementGroup.Left.ToString() : PlacementGroup.Right.ToString();

            result.DateJoined = item.DateJoined.Value;
            result.MemberType = item.Type;
            result.PlacementId = item.ParentId;
            result.Placement = MemberManager.Instance.GetFullName(item.ParentId);
            result.Sponsor = MemberManager.Instance.GetFullName(item.SponsorId);

            if (item.Picture != null)
                result.Picture = ConfigurationManager.AppSettings["UploadPath"].ToString() + item.Picture;
            else
                result.Picture = ConfigurationManager.AppSettings["NoProfileImage"].ToString();

            return result;
        }

        public static List<MemberViewModel> GetAllForValidations()
        {
            List<MemberViewModel> result = new List<MemberViewModel>();

            var list = MemberManager.Instance.GetAllForValidation();
            foreach (Member item in list)
            {
                result.Add(BuildMember(item));
            }

            return result;
        }

        public static List<MemberViewModel> GetAllForApprovals()
        {
            List<MemberViewModel> result = new List<MemberViewModel>();

            var list = MemberManager.Instance.GetAllForApproval();
            foreach (Member item in list)
            {
                result.Add(BuildMember(item));
            }

            return result;
        }

        public static List<MemberViewModel> GetAllAdmin()
        {
            List<MemberViewModel> result = new List<MemberViewModel>();

            var list = MemberManager.Instance.GetAllAdmin();
            foreach (Member item in list)
            {
                result.Add(BuildMember(item));
            }

            return result;
        }

        public static List<MemberViewModel> GetAll()
        {
            List<MemberViewModel> result = new List<MemberViewModel>();

            var list = MemberManager.Instance.GetAll();
            foreach (Member item in list)
            {
                result.Add(BuildMember(item));
            }

            return result;
        }

        public static List<MemberViewModel> GetChilds(string hCode)
        {
            List<MemberViewModel> result = new List<MemberViewModel>();

            var list = MemberManager.Instance.GetChilds(hCode);
            foreach (Member item in list)
            {
                result.Add(BuildMember(item));
            }

            return result;
        }

        public static MemberViewModel Get(int memberId)
        {
            var item = MemberManager.Instance.Get(memberId);
            MemberViewModel model = item != null? BuildMember(item) : null;

            return model;
        }

        private static MemberViewModel BuildMember(Member item)
        {
            MemberViewModel model = new MemberViewModel();
            model.MemberId = item.MemberId;
            model.MemberCode = item.MemberCode;
            model.Username = item.Username;
            model.FName = item.FirstName;
            model.MName = item.MiddleName;
            model.LName = item.LastName;
            model.Email = item.Email;
            model.MobileNo = item.MobileNo;
            model.TelNo = item.TelephoneNo;
            model.Occupation = item.Occupation;
            model.Birthday = item.Birthday.Value;
            model.BirthPlace = item.BirthPlace;
            model.Citizenship = item.Citizenship;
            model.Gender = item.Gender;
            model.MaritalStatus = item.MaritalStatus;
            model.PresentAddress = item.PresentAddress;
            model.ProvincialAddress = item.ProvincialAddress;
            model.MemberType = item.Type;
            model.HierarchyCode = item.HierarchyCode;
            model.RoleId = item.RoleId;
            model.IsFreeAccount = item.IsFreeAccount.ToString().ToLower();
            model.PaymentType = item.PaymentType == null ? string.Empty : item.PaymentType;
            model.CashNote = item.CashNote;
            model.BankBranch = item.BankBranch;
            model.DepositNo = item.DepositNo;
            model.DateDeposited = item.DateDeposited;

            if (item.Picture != null)
                model.Picture = ConfigurationManager.AppSettings["UploadPath"].ToString() + item.Picture;

            model.PlacementId = item.ParentId;
            if (item.ParentId > 0)
            {
                var member = GetMemberBase(item.ParentId);
                model.Placement = member.FullName;
                model.PlacementUsername = member.Username;
            }

            model.SponsorId = item.SponsorId;
            if (item.SponsorId > 0)
            {
                var member = GetMemberBase(item.SponsorId);
                model.Sponsor = member.FullName;
                model.SponsorUsername = member.Username;
            }

            MemberViewModel processedby = null;
            if (item.ModifiedBy != null)
            {
                processedby = GetMemberBase(item.ModifiedBy);
                model.ProcessedDate = item.ModifiedDate;
            }
            else
            {
                processedby = GetMemberBase(item.CreatedBy);
                model.ProcessedDate = item.CreatedDate;
            }

            if(processedby != null)
            {
                model.ProcessedById = processedby.MemberId;
                model.ProcessedBy = processedby.FullName;
                model.ProcessedByUsername = processedby.Username;
            }            

            if (item.DateJoined != null)
                model.DateJoined = item.DateJoined.Value;

            return model;
        }

        private static MemberViewModel GetMemberBase(int id)
        {
            MemberViewModel result = null;

            var item = MemberManager.Instance.Get(id);
            if (item != null)
            {
                result = new MemberViewModel();
                result.MemberId = item.MemberId;
                result.MemberCode = item.MemberCode;
                result.Username = item.Username;
                result.FName = item.FirstName;
                result.MName = item.MiddleName;
                result.LName = item.LastName;
                result.Email = item.Email;
            }

            return result;
        }

        private static MemberViewModel GetMemberBase(string username)
        {
            MemberViewModel result = null;

            var item = MemberManager.Instance.Get(username);
            if (item != null)
            {
                result = new MemberViewModel();
                result.MemberId = item.MemberId;
                result.MemberCode = item.MemberCode;
                result.Username = item.Username;
                result.FName = item.FirstName;
                result.MName = item.MiddleName;
                result.LName = item.LastName;
                result.Email = item.Email;
            }

            return result;
        }
    }
}