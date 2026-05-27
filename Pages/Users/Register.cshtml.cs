using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnSet.Domain.Enums;
using OnSet.Features.Users.Register;
using OnSet.Infrastructure.Results;
using System.ComponentModel.DataAnnotations;

namespace OnSet.Pages.Users
{
    /// <summary>Razor Page handler (documented in OpenAPI under Razor Pages).</summary>
    [AllowAnonymous]
        public class RegisterModel : PageModel
    {
        private readonly IMediator _mediator;

        public RegisterModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        /// <summary>Razor Page handler for this route (see OpenAPI *Razor Pages* section).</summary>

        public class InputModel
        {
            [Required]
            [Display(Name = "Account Type")]
            public AccountType AccountType { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Main Occupation Role")]
            public ProjectRoles? MainOccupationRole { get; set; }

            [StringLength(500)]
            public string? Bio { get; set; }

            [Url]
            [Display(Name = "Profile Image URL")]
            public string? AvatarUrl { get; set; }

            [Display(Name = "Street")]
            public string? Street { get; set; }

            [Display(Name = "City")]
            public string? City { get; set; }

            [Display(Name = "Province/State")]
            public string? Province { get; set; }

            [Display(Name = "Zip Code")]
            public string? ZipCode { get; set; }

            [Display(Name = "Country")]
            public string? Country { get; set; }

            [Display(Name = "Spoken Languages")]
            public List<Languages>? SpokenLanguages { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime? DateOfBirth { get; set; }

            [Display(Name = "Place of Birth")]
            public string? PlaceOfBirth { get; set; }

            [Display(Name = "Nationality")]
            public string? Nationality { get; set; }

            [Display(Name = "National Registration Number")]
            public string? NationalRegistrationNumber { get; set; }

            [Display(Name = "Marital Status")]
            public MaritalStatus? MaritalStatus { get; set; }

            [Display(Name = "Dietary Preference")]
            public DietaryPreference? DietaryPreference { get; set; }

            [Display(Name = "Emergency Contact Name")]
            public string? EmergencyContactName { get; set; }

            [Display(Name = "Emergency Contact Phone")]
            public string? EmergencyContactPhone { get; set; }
        }

        public void OnGet(string? returnUrl = null, AccountType? accountType = null)
        {
            ReturnUrl = returnUrl;
            if (accountType.HasValue)
            {
                Input.AccountType = accountType.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var command = new Command
            {
                AccountType = Input.AccountType,
                Email = Input.Email,
                Password = Input.Password,
                ConfirmPassword = Input.ConfirmPassword,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                MainOccupationRole = Input.MainOccupationRole,
                Bio = Input.Bio,
                AvatarUrl = Input.AvatarUrl,
                Street = Input.Street,
                City = Input.City,
                Province = Input.Province,
                ZipCode = Input.ZipCode,
                Country = Input.Country,
                SpokenLanguages = Input.SpokenLanguages,
                DateOfBirth = Input.DateOfBirth,
                PlaceOfBirth = Input.PlaceOfBirth,
                Nationality = Input.Nationality,
                NationalRegistrationNumber = Input.NationalRegistrationNumber,
                MaritalStatus = Input.MaritalStatus,
                DietaryPreference = Input.DietaryPreference,
                EmergencyContactName = Input.EmergencyContactName,
                EmergencyContactPhone = Input.EmergencyContactPhone
            };

            Result result = await _mediator.Send(command);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return Page();
            }

            return LocalRedirect(ReturnUrl);
        }
    }
}
