using GBHO_Business.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace GBHO_Web.Areas.MyAccount.Models
{
    public class MemberViewModel
    {
        public int MemberId { get; set; }
        public string MemberCode { get; set; }

        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        [Required(ErrorMessage = "Username field is required")]
        [StringLength(20)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(30)]
        public string FName { get; set; }

        [Required(ErrorMessage = "Middle Name is required")]
        [StringLength(30)]
        public string MName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(30)]
        public string LName { get; set; }

        public string FullName
        {
            get
            {
                return LName + ", " + FName + " " + MName;
            }
        }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [StringLength(20)]
        public string MobileNo { get; set; }

        [StringLength(100)]
        public string TelNo { get; set; }

        [StringLength(100)]
        public string Occupation { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        public string BirthdayString
        {
            get
            {
                if (this.Birthday != null)
                {
                    return this.Birthday.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [Required(ErrorMessage = "Birth Place is required")]
        [StringLength(250)]
        public string BirthPlace { get; set; }

        [Required(ErrorMessage = "Citizenship is required")]
        [StringLength(50)]
        public string Citizenship { get; set; }

        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string MemberType { get; set; }

        [Required(ErrorMessage = "Present Address is required")]
        [StringLength(500)]
        public string PresentAddress { get; set; }

        [StringLength(500)]
        public string ProvincialAddress { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Placement is required")]
        public int PlacementId { get; set; }
        public string Placement { get; set; }
        public string PlacementUsername { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Sponsor is required")]
        public int SponsorId { get; set; }
        public string Sponsor { get; set; }
        public string SponsorUsername { get; set; }

        public string HierarchyCode { get; set; }
        public string HierarchyCodeDisplay
        {
            get
            {
                if (this.HierarchyCode != null)
                {
                    return this.HierarchyCode.Substring(this.HierarchyCode.Length - 1, 1) == "0" ? PlacementGroup.Left.ToString() : PlacementGroup.Right.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public DateTime DateJoined { get; set; }
        public string DateJoinedString
        {
            get
            {
                if (this.DateJoined != null)
                {
                    return this.DateJoined.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string Picture { get; set; }
        public string Level { get; set; }

        public int RoleId { get; set; }
        public string IsFreeAccount { get; set; }

        public string PaymentType { get; set; }
        public string CashNote { get; set; }
        public string BankBranch { get; set; }
        public string DepositNo { get; set; }

        public DateTime? DateDeposited { get; set; }
        public string DateDepositedString
        {
            get
            {
                if (this.DateDeposited != null)
                {
                    return this.DateDeposited.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int ProcessedById { get; set; }
        public string ProcessedBy { get; set; }
        public string ProcessedByUsername { get; set; }


        public DateTime? ProcessedDate { get; set; }
        public string ProcessedDateString
        {
            get
            {
                if (this.ProcessedDate != null)
                {
                    return this.ProcessedDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }


    public enum Gender
    {
        Male,
        Female
    }
    
    public enum MaritalStatus
    {
        Single,
        Married
    }

    public enum PlacementGroup
    {
        Left,
        Right
    }
}