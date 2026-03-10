using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnSet.Domain.Enums;
using OnSet.Features.Users.Details;
using Command = OnSet.Features.Users.Edit.Command;

namespace OnSet.Pages.Users
{
    [Authorize(Policy = "Authenticated")]
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public SelectList RoleOptions { get; set; }
        public SelectList LanguageOptions { get; set; }

        private void PrepareLookupData()
        {
            RoleOptions = new SelectList(Enum.GetNames(typeof(ProjectRoles)));
            LanguageOptions = new SelectList(Enum.GetNames(typeof(Languages)));
        }

        public DetailsModel(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        public Model? UserDetails { get; private set; }

        [BindProperty]
        public Command EditCommand { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!_currentUser.IsAuthenticated)
            {
                return Challenge();
            }

            string userIdToFetch = _currentUser.UserId;
            PrepareLookupData();

            UserDetails = await _mediator.Send(new Query
            {
                Id = userIdToFetch
            });

            if (UserDetails == null)
            {
                return NotFound();
            }

            EditCommand = new Command
            {
                UserId = UserDetails.Id,
                FirstName = UserDetails.FirstName,
                LastName = UserDetails.LastName,
                Bio = UserDetails.Bio,
                AvatarUrl = UserDetails.AvatarUrl,
                MainOccupationRole = UserDetails.MainOccupationRole?.ToString(),
                YearsExperience = UserDetails.YearsExperience,
                SpokenLanguages = UserDetails.SpokenLanguages.Select(l => l.ToString()).ToList(),
                IsAvailableForBooking = UserDetails.IsAvailableForBooking,
                NextAvailableDate = UserDetails.NextAvailableDate,
                Street = UserDetails.Street,
                City = UserDetails.City,
                Province = UserDetails.Province,
                ZipCode = UserDetails.ZipCode,
                Country = UserDetails.Country,
                EmergencyContactName = UserDetails.EmergencyContactName,
                EmergencyContactPhone = UserDetails.EmergencyContactPhone
            };

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!_currentUser.IsAuthenticated)
                return Challenge();

            try
            {
                EditCommand.UserId = _currentUser.UserId;
                PrepareLookupData();

                await _mediator.Send(EditCommand);

                TempData["SuccessMessage"] = "Profile updated!";
                return RedirectToPage("./Details");
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError($"EditCommand.{error.PropertyName}", error.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await LoadPageData();
            return Page();
        }

        private async Task LoadPageData()
        {
            UserDetails = await _mediator.Send(new Query
            {
                Id = _currentUser.UserId
            });
        }
    }

}
